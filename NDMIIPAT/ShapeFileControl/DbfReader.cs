using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeFileControl
{
    class DbfReader
    {
        private string path_ = "";
        private string file_ = "";

        public void SetPath(string path)
        {
            path_ = "Data Source=" + path + ";";
        }
        
        public void SetFile(string file)
        {
            if (file.Contains(".dbf"))
                file_ = file;
            else
                file_ = file + ".dbf;";
        }

        public DataTable GetData()
        {
            DataTable result = new DataTable();

            string cmd = "Provider=Microsoft.ACE.OLEDB.12.0;" + path_ + "Extended Properties=dBASE IV;User ID=Admin;Password=;";
            OleDbConnection conn = new OleDbConnection(cmd);

            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                string query = @"select * from " + file_;

                OleDbCommand q = new OleDbCommand(query, conn);
                OleDbDataAdapter da = new OleDbDataAdapter(q);
                
                da.Fill(result);
                conn.Close();
            }

            return result;
        }
    }
}
