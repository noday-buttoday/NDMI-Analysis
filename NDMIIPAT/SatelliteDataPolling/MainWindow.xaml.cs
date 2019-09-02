using Module.Event;
using SatelliteDataPolling.Event;
using System;
using System.IO;
using System.Windows;

using System.Collections.Generic;

namespace SatelliteDataPolling
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EventProcess event_process = null;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            MakeEventProcessor();
            
            #region 출력테스트용
            //FileProcessEventArgs event_args = new FileProcessEventArgs(this, "PROCESS_COMPLETE");
            //event_args.output_files.Add(new FileInfo(@"D:\00_Work\96_SUPPORT\Data\폭설_알고리즘_출력파일_20141214\ndsi_th.img"));
            //event_args.msg.Add("계산완료");
            //event_args.output_path = @"D:\00_Work\96_SUPPORT\Data\폭설_알고리즘_출력파일_20141214\shapetest";
            //EventControl.Instance().SendEvent(event_args);
            #endregion

            #region 진행상태창
            //ProgressWindow.ProgressWindow pw = new ProgressWindow.ProgressWindow();
            //pw.TextBlock.Text = "asd";
            //pw.Show();
            //pw.Exit();
            #endregion

            //FileInfo fInfo = new FileInfo(@"Z:\LANDSAT_8\OLI_TIRS\L1\L1GT\Y2018\M07\D25\LC81140352016014LGN00\LC81140352016014LGN00_B3.TIF");
            //String ext = fInfo.Extension;

            //Directory.CreateDirectory(@"Z:\reportdata\Y2018\M07\D01");

            //String strRoot = Path.GetPathRoot(@"Z:\COMS");

            //String strPath = "D:/SDCI/Landsat/exe/output/Y2018/M07/D19/mask/hwangju/LC81170332016035LGN00_MTL.txt";
            //String strResult = Path.GetDirectoryName(strPath);

            //IMG_Converter.IMG_Converter.Instance.ImgToGeoTiff(@"C:\Temp\ndsi_th.img", @"C:\Temp\ndsi_th.tif");
            //IMG_Converter.IMG_Converter.Instance.ImgToJpg(@"C:\Temp\ndsi_th.img", @"C:\Temp\ndsi_th.jpg");
            //IMG_Converter.IMG_Converter.Instance.ImgToPng(@"C:\Temp\ndsi_th.img", @"C:\Temp\ndsi_th.png");
            //IMG_Converter.IMG_Converter.Instance.ImgToKml(@"C:\Temp\ndsi_th.img", @"C:\Temp\ndsi_th.kml");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EventControl.Instance().Stop();
            (this.DataContext as MainWindowViewModel).Close();
        }

        private void MakeEventProcessor()
        {
            event_process = new EventProcess(this);
            event_process.MakeEventProcessor();
        }

    }
}
