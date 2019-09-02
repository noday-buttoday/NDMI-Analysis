using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ShapeFileControl
{
    class DbWrite
    {
        MySqlConnection conn;
        private int sequence_;

        public void OpenDb(String db_addr, String db_id, String db_pw)
        {
            string conn_str = "Server=" + db_addr + ";Database=ndmi;Uid=" + db_id +";Pwd=" + db_pw +";";
            conn = new MySqlConnection(conn_str);
            conn.Open();
            sequence_ = 1;
        }

        public int WriteToDb(int type, double lat, double lon)
        {
            string query = "INSERT INTO tb_shp_cb VALUES (" + sequence_.ToString() + "," + type.ToString() + "," + lat.ToString() + "," + lon.ToString() + ")";
            MySqlCommand cmd = new MySqlCommand(query, conn);

            int rtn = 0;
            try
            {
                rtn = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            sequence_++;
            return rtn;
        }

        public void CloseDb()
        {
            conn.Close();
        }

        internal void OpenDb()
        {
            throw new NotImplementedException();
        }
    }
}
