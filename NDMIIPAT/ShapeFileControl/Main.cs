using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using ShapeFileControl.Event;

namespace ShapeFileControl
{
    class Main
    {
        private static Main instance_ = null;

        private String input_path_;
        private String output_path_;
        private String db_ip_;
        private String db_id_;
        private String db_pw_;

        private List<String> file_list_     = new List<string>();
        private List<String> shp_file_list_ = new List<string>();
        private List<String> shx_file_list_ = new List<string>();
        private List<String> dbf_file_list_ = new List<string>();

        private Thread proc_thread;

        public event EventHandler Finished;

        private Main()
        {

        }

        public static Main Instance()
        {
            if (instance_ == null)
                instance_ = new Main();

            return instance_;
        }

        public void Init()
        {

        }

        public void SetShape(String input_path, String output_path, List<String> file_list)
        {
            input_path_ = input_path;
            output_path_ = output_path;

            shp_file_list_.Clear();
            shx_file_list_.Clear();
            dbf_file_list_.Clear();

            foreach (String s in file_list)
            {
                file_list_.Add(s);
                shp_file_list_.Add(input_path + "\\" + s + ".shp");
                shx_file_list_.Add(input_path + "\\" + s + ".shx");
                dbf_file_list_.Add(s + ".dbf");
            }
        }

        public void Start()
        {
            proc_thread = new Thread(new ThreadStart(Go));
            proc_thread.Start();
        }

        public void Stop()
        {
            EventControl.Instance().SendEvent(new ProgressLabelEventArgs(null, "LB_PROG", "Cancelled" ));
            proc_thread.Abort();
        }

        private void Go()
        {
            ShapeReader sr = new ShapeReader();
            DbfReader dr = new DbfReader();
 
            dr.SetPath(input_path_);

            for (int i = 0; i < shp_file_list_.Count; ++i)
            {
                EventControl.Instance().SendEvent(new ProgressLabelEventArgs(null, "LB_PROG", "Reading Shape\n" + file_list_[i] + ".shp"));
                /*
                List<Polygon> polygon_list = sr.ReadShapeFile(shp_file_list_[i], shx_file_list_[i]);
                List<Point> point_list = GetPoint(polygon_list);
                 */
                 

                //List<Point> point_list = sr.ReadPointShapeFile(shp_file_list_[i], shx_file_list_[i]);

                List<Point> point_list = sr.ReadPointShapeFile(shp_file_list_[i], shx_file_list_[i]);
                //polygon_list.Clear();

                dr.SetFile(dbf_file_list_[i]);
                
                Dictionary<String, List<Point>> point_data = new Dictionary<string, List<Point>>();
                Dictionary<String, DataTable> dbf_data = new Dictionary<string, DataTable>();

                EventControl.Instance().SendEvent(new ProgressLabelEventArgs(null, "LB_PROG", "Read Dbf\n" + file_list_[i] + ".dbf"));
                DataTable build_info = dr.GetData();
                List<String> addr = GetPartialData(point_list, build_info, ref dbf_data, ref point_data);

                foreach (String file_name in addr)
                {
                    WriteDbf(dbf_data[file_name], file_name + ".dbf");
                    WriteShp(point_data[file_name], file_name);
                }

                //EventControl.Instance().SendEvent(new ProgressLabelEventArgs(null, "LB_PROG", "Read Dbf\n" + file_list_[i] + ".dbf"));
                //EventControl.Instance().SendEvent(new ProgressEventArgs(null, "PB_OPER", 0, 1));
                //DataTable build_info = dr.GetData();

                //DataTable extract_info = build_info.Clone();
                //List<Point> extract_point = new List<Point>();

                //ExtractInfo(ref extract_point, ref extract_info, point_list, build_info);
                //point_list.Clear();
                //build_info.Clear();

                //EventControl.Instance().SendEvent(new ProgressLabelEventArgs(null, "LB_PROG", "Write to DB"));

                //WriteDbf(extract_info, i);
                //WriteShp(extract_point, i);

                //WriteShp(point_list, i);

                //extract_info.Clear();
                //extract_point.Clear();

                EventControl.Instance().SendEvent(new ProgressLabelEventArgs(null, "LB_PROG", "Complete"));

                Finished(null, null);
            }
        }
        
        public void SetDB(String db_ip, String db_id, String db_pw)
        {
            db_ip_ = db_ip;
            db_id_ = db_id;
            db_pw_ = db_pw;
        }

        private void WriteToDb(DataTable extract_info, List<Point> point)
        {
            
            DbWrite db = new DbWrite();
            db.OpenDb(db_ip_, db_id_, db_pw_);
            int increase_index = 0;
            int max = extract_info.Rows.Count;


            foreach (DataRow dr in extract_info.Rows)
            {
                object[] item = dr.ItemArray;

                if ((increase_index % 100) == 0)
                    EventControl.Instance().SendEvent(new ProgressEventArgs(null, "PB_OPER", increase_index, max));

                if (item[8] != DBNull.Value)
                {
                    String s = (item[8] as String);

                    int code = 0;
                    if (int.TryParse(s, out code))
                    {
                        code = Convert.ToInt32(item[8]);
                        code = code / 1000;

                        if (code == 1 || code == 2 || code == 21)
                            db.WriteToDb(code, point[increase_index].x, point[increase_index].y);
                    }

                }
                increase_index++;
            }

            db.CloseDb();
        }

        private void WriteShp(List<Point> point, String file_name)
        {
            EventControl.Instance().SendEvent(new ProgressLabelEventArgs(null, "LB_PROG", "Writing Shp File"));
            ShapeWriter sw = new ShapeWriter();
            sw.SetShapeData(point);
            sw.OpenFile(output_path_ + "\\" + file_name);
            sw.MakeStart();
            sw.CloseFile();
        }

        private void WriteShp(List<Point> point, int file_index)
        {
            EventControl.Instance().SendEvent(new ProgressLabelEventArgs(null, "LB_PROG", "Writing Shp File"));
            ShapeWriter sw = new ShapeWriter();
            sw.SetShapeData(point);
            sw.OpenFile(output_path_ + "\\" + file_list_[file_index]);
            sw.MakeStart();
            sw.CloseFile();
        }

        private void WriteDbf(DataTable data, String file_name)
        {
            DbfWriter dw = new DbfWriter();

            EventControl.Instance().SendEvent(new ProgressLabelEventArgs(null, "LB_PROG", "Writing Dbf File"));
            dw.SetPath(output_path_);
            dw.SetFile(file_name);
            dw.SetData(data);
        }

        private void WriteDbf(DataTable data, int file_index)
        {
            DbfWriter dw = new DbfWriter();

            EventControl.Instance().SendEvent(new ProgressLabelEventArgs(null, "LB_PROG", "Writing Dbf File"));
            dw.SetPath(output_path_);
            dw.SetFile(dbf_file_list_[file_index]);
            dw.SetData(data);
        }

        private List<String> GetPartialData(List<Point> point, DataTable build_data, ref Dictionary<String, DataTable> dbf_data, ref Dictionary<String, List<Point>> point_data)
        {
            /*
            Dictionary<String, List<Point>> point_data = new Dictionary<string, List<Point>>();
            Dictionary<String, DataTable> dbf_data = new Dictionary<string, DataTable>();
            */

            List<String> addr_name = new List<string>();

            int data_count = 0;
            int record_number = build_data.Rows.Count;
            EventControl.Instance().SendEvent(new ProgressEventArgs(null, "PB_OPER", data_count, record_number));

            int index_count = 0;
            foreach (DataRow dr in build_data.Rows)
            {
                data_count++;
                if ((data_count % 100) == 0)
                    EventControl.Instance().SendEvent(new ProgressEventArgs(null, "PB_OPER", data_count, record_number));

                object[] item = dr.ItemArray;

                Encoding kr = Encoding.GetEncoding("EUC-KR");
                Encoding utf = Encoding.UTF8;
                byte[] utfb = utf.GetBytes(item[4].ToString());
                byte[] krb = Encoding.Convert(utf, kr, utfb);
                string msg = kr.GetString(krb);

                if (item[4] != DBNull.Value)
                {
                    String addr = item[4].ToString();
                    String[] partial = addr.Split(' ');

                    String addr_partial = "";

                    foreach (String s in partial)
                    {
                        if ((s.EndsWith("시") || s.EndsWith("군")))
                        {
                            addr_partial += s;

                            bool same_flag = false;

                            foreach (String adr in addr_name)
                            {
                                if (adr == addr_partial)
                                {
                                    same_flag = true;
                                    break;
                                }
                            }

                            if(!same_flag)
                                addr_name.Add(addr_partial);

                            if (!point_data.ContainsKey(addr_partial))
                            {
                                point_data.Add(addr_partial, new List<Point>());
                                dbf_data.Add(addr_partial, build_data.Clone());
                            }

                            dbf_data[addr_partial].Rows.Add(dbf_data[addr_partial].NewRow().ItemArray = (object[])item.Clone());
                            point_data[addr_partial].Add(point[index_count]);
                        }
                        else
                        {
                            addr_partial += s + "_";
                        }
                    }
                }
                index_count++;
            }

            return addr_name;
        }

        private void ExtractInfo(ref List<Point> point, ref DataTable build, List<Point> point_data, DataTable build_data)
        {
            int increase_index = 0;
            int max = point_data.Count;

            EventControl.Instance().SendEvent(new ProgressLabelEventArgs(null, "LB_PROG", "Extracting Infomation"));
            EventControl.Instance().SendEvent(new ProgressEventArgs(null, "PB_OPER", increase_index, max));
            foreach (DataRow dr in build_data.Rows)
            {
                object[] item = dr.ItemArray;

                if ((increase_index % 1000) == 0)
                    EventControl.Instance().SendEvent(new ProgressEventArgs(null, "PB_OPER", increase_index, max));

                /*
                if (item[8] != DBNull.Value)
                {
                    String s = (item[8] as String);

                    int code = 0;
                    if (int.TryParse(s, out code))
                    {
                        code = Convert.ToInt32(item[8]);
                        code = code / 1000;

                        if (code == 1 || code == 2 || code == 21)
                        {
                            build.Rows.Add(build.NewRow().ItemArray = (object[])item.Clone());
                            point.Add(point_data[increase_index]);
                        }
                    }

                }
                 */

                build.Rows.Add(build.NewRow().ItemArray = (object[])item.Clone());
                point.Add(point_data[increase_index]);

                increase_index++;
            }
            EventControl.Instance().SendEvent(new ProgressEventArgs(null, "PB_OPER", max, max));
        }

        private List<Point> GetPoint(List<Polygon> data)
        {
            List<Point> result_point = new List<Point>();

            EventControl.Instance().SendEvent(new ProgressLabelEventArgs(null, "LB_PROG", "Get Center Coordinate"));

            int point_count = 1;
            int point_max = data.Count;
            foreach (Polygon p in data)
            {
                if ((point_count % 1000) == 0)
                    EventControl.Instance().SendEvent(new ProgressEventArgs(null, "PB_OPER", point_count, point_max));
                result_point.Add(new Point(p.center.x, p.center.y));
                point_count++;
            }

            EventControl.Instance().SendEvent(new ProgressEventArgs(null, "PB_OPER", point_max, point_max));

            return result_point;
        }

    }
}
