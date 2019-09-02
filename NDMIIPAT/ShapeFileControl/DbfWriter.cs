using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using ShapeFileControl.Event;

namespace ShapeFileControl
{
    class DbfWriter
    {
        private string path_ = "";
        private string file_ = "";
        private bool stop_flag = false;

        public void SetPath(string path)
        {
            path_ = "Data Source=" + path + ";";
        }
        
        // 확장자 없이 파일만
        public void SetFile(string file)
        {
            file_ = file;
        }

        public void SetStop(bool flag)
        {
            stop_flag = flag;
        }

        private string ReplaceEscape(string str)
        {
            str = str.Replace("'", "''");
            return str;
        }

        public void SetData(DataTable data)
        {
            List<string> list = new List<string>();

            string createSql = "create table " + file_ + " (";
            foreach (DataColumn dc in data.Columns)
            {
                string fieldName = dc.ColumnName;
                string type = dc.DataType.ToString();

                switch (type)
                {
                    case "System.String":
                        type = "varchar(100)";
                        break;

                    case "System.Boolean":
                        type = "varchar(10)";
                        break;

                    case "System.Int32":
                        type = "int";
                        break;

                    case "System.Double":
                        type = "Double";
                        break;

                    case "System.DateTime":
                        type = "TimeStamp";
                        break;
                }
                createSql = createSql + "[" + fieldName + "]" + " " + type + ",";

                list.Add(fieldName);
            }

            string path = "Provider=Microsoft.ACE.OLEDB.12.0;" + path_ + "Extended Properties=dBASE IV;User ID=Admin;Password=;";

            createSql = createSql.Substring(0, createSql.Length - 1) + ")";
            OleDbConnection con = new OleDbConnection(path);
            OleDbCommand cmd = new OleDbCommand();

            cmd.Connection = con;
            con.Open();
            cmd.CommandText = createSql;
            cmd.ExecuteNonQuery();

            int data_count = 0;
            int record_number = data.Rows.Count;
            EventControl.Instance().SendEvent(new ProgressEventArgs(null, "PB_OPER", data_count, record_number));
            foreach (DataRow row in data.Rows)
            {
                string insertSql = "insert into " + file_ + " values(";

                for (int i = 0; i < list.Count; i++)
                {
                    insertSql = insertSql + "'" + ReplaceEscape(row[list[i].ToString()].ToString()) + "',";
                }

                insertSql = insertSql.Substring(0, insertSql.Length - 1) + ")";
                cmd.CommandText = insertSql;
                cmd.ExecuteNonQuery();

                if (stop_flag == true)
                {
                    break;
                }

                if( (data_count % 100) == 0)
                    EventControl.Instance().SendEvent(new ProgressEventArgs(null, "PB_OPER", data_count, record_number));
                data_count++;
            }

            con.Close();

        }
    }
}
