using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

using ShapeFileControl.UiComponent;
using ShapeFileControl.Event;
using System.Threading;

namespace ShapeFileControl
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private String path_ = "";

        public MainWindow()
        {
            EventControl.Instance().Init();

            InitializeComponent();

            GuiInit();

            Main.Instance().Init();
            Main.Instance().Finished += new EventHandler(WorkFinishEvent);


            //Thread t = new Thread(new ThreadStart(ShapeRead));
            //t.Start();
            
            //ShapeReader dl = new ShapeReader();
            //List<Polygon> p = dl.ReadShapeFile("D:\\00_Work\\96_SUPPORT\\Data\\161013\\AL_43_D010_20161001");

            //DbfReader r = new DbfReader();
            //r.SetPath("D:\\00_Work\\96_SUPPORT\\Data\\161013\\AL_43_D010_20161001");
            //r.SetFile("a");
            //DataTable data = r.GetData();
            //DataTable new_data = data.Clone();
            //List<Polygon> p_p = new List<Polygon>();

            //int increase_index = 0;
            //DbWrite db = new DbWrite();
            //db.OpenDb();

            //foreach (DataRow dr in data.Rows)
            //{
            //    object[] item = dr.ItemArray;

            //    if (item[8] != DBNull.Value)
            //    {
            //        String s = (item[8] as String);

            //        int code = 0;
            //        if (int.TryParse(s, out code))
            //        {
            //            code = Convert.ToInt32(item[8]);
            //            code = code / 1000;

            //            if (code == 1 || code == 2 || code == 21)
            //            {
            //                db.WriteToDb(code, p[increase_index].center.lat, p[increase_index].center.lon);
            //                new_data.Rows.Add(new_data.NewRow().ItemArray = (object[])item.Clone());
            //                p_p.Add(p[increase_index]);
            //            }
            //        }

            //    }
            //    increase_index++;
            //}

            //db.CloseDb();

            //Console.WriteLine("{0}", new_data.Rows.Count);

            //ShapeWriter sw = new ShapeWriter();
            //sw.SetShapeData(p_p, 1);
            //sw.OpenFile(@"D:\temp\shapetest\test");
            //sw.MakeStart();
            //sw.CloseFile();
        }


        private void ShapeRead()
        {
            ShapeReader dl = new ShapeReader();
            List<Polygon> p = dl.ReadShapeFile("D:\\00_Work\\96_SUPPORT\\Data\\161013\\AL_43_D010_20161001");
        }

        private void GuiInit()
        {
            BTN_Start.SetText(true, "Start");
            BTN_Start.SetText(false, "Stop");
            BTN_Start.SetState(true);

            TB_DBIP.SetWaterMarkText("Database IP Address..");
            TB_DBID.SetWaterMarkText("Database Access ID..");
            TB_DBPW.SetWaterMarkText("Database Access PW..");
            TB_InputPath.SetWaterMarkText("Input Sph, Shx, Dbf File Path..");
            TB_OutputPath.SetWaterMarkText("Output Sph, Shx, Dbf File Path..");

            LB_PROG.AddEventListener();
            PB_OPER.AddEventListener();

            TB_InputPath.FolderSelected += new EventHandler(FolderSelectedEvent);
        }

        private void FolderSelectedEvent(Object sender, EventArgs e)
        {
            path_ = (sender as TextBox).Text;
            DirectoryInfo di = new DirectoryInfo(path_);

            if (di.Exists)
            {   
                FileInfo[] files = di.GetFiles();

                foreach (FileInfo f in files)
                    if (f.Extension == ".shp")
                        List_Files.Items.Add(f.Name.Replace(".shp",""));
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EventControl.Instance().Stop();
        }

        private void BTN_Start_Click(object sender, RoutedEventArgs e)
        {
            StartStopButton btn = (sender as StartStopButton);

            if (btn.GetState())
            {
                List<String> file_name = new List<string>();
                foreach (String s in List_Files.Items)
                    file_name.Add(s);

                Main.Instance().SetShape(TB_InputPath.Text, TB_OutputPath.Text, file_name);
                Main.Instance().SetDB(TB_DBIP.Text, TB_DBID.Text, TB_DBPW.Text);

                Main.Instance().Start();
            }
            else
                Main.Instance().Stop();
        }

        private void WorkFinishEvent(object sender, EventArgs e)
        {
            BTN_Start.SetState(true);
        }
        
    }
}
