using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SatelliteDataPolling.SettingWindow
{
    /// <summary>
    /// SettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();

            #region 그룹박스 헤더 명 설정
            switch (SatelliteDataPolling.SettingWindow.SettingClass.Instance.Mode)
            {
                case SatelliteDataPolling.SettingWindow.SettingClass.ProcessMode.Snow:     //폭설
                    this.ModeGroupDB.Header = "폭설분석 결과저장 DB";
                    break;

                case SatelliteDataPolling.SettingWindow.SettingClass.ProcessMode.Drought1:     //가뭄(MODIS)
                case SatelliteDataPolling.SettingWindow.SettingClass.ProcessMode.Drought2:     //가뭄(LANDSAT)
                    this.ModeGroupDB.Header = "가뭄분석 결과저장 DB";
                    break;

//                case SatelliteDataPolling.SettingWindow.SettingClass.ProcessMode.Ground:     //지반침하
//                    this.ModeGroupDB.Header = "지반침하분석 결과저장 DB";
//                    break;
            }
            #endregion

            #region AWS자료수집 DB 설정
            tb_AwsIP.Text = SettingClass.Instance.DataBaseAwsIP;
            tb_AwsPort.Text = SettingClass.Instance.DataBaseAwsPort.ToString();
            tb_AwsName.Text = SettingClass.Instance.DataBaseAwsName;
            tb_AwsID.Text = SettingClass.Instance.DataBaseAwsID;
            pb_AwsPW.Password = SettingClass.Instance.DataBaseAwsPW;
            #endregion

            #region 분석 결과 저장 DB 설정
            tb_IP.Text = SettingClass.Instance.DataBaseIP;
            tb_Port.Text = SettingClass.Instance.DataBasePort.ToString();
            tb_Name.Text = SettingClass.Instance.DataBaseName;
            tb_ID.Text = SettingClass.Instance.DataBaseID;
            pb_PW.Password = SettingClass.Instance.DataBasePW;
            #endregion

            #region 신적설 DB 설정
            tb_Sinjeokseol_IP.Text = SettingClass.Instance.DataBaseNewSnowIP;
            tb_Sinjeokseol_Port.Text = SettingClass.Instance.DataBaseNewSnowPort.ToString();
            tb_Sinjeokseol_Name.Text = SettingClass.Instance.DataBaseNewSnowName;
            tb_Sinjeokseol_ID.Text = SettingClass.Instance.DataBaseNewSnowID;
            pb_Sinjeokseol_PW.Password = SettingClass.Instance.DataBaseNewSnowPW;
            #endregion

            #region 모드에 따른 UI
            switch (SettingClass.Instance.Mode)
            {
                case SettingClass.ProcessMode.Snow:     //폭설
                    GroupBox_DataSet.Visibility = System.Windows.Visibility.Visible;
                    GroupBox_AwsDataDB.Visibility = System.Windows.Visibility.Visible;
                    SinjeokseolGroupDB.Visibility = System.Windows.Visibility.Visible;
                    Sp_IDL.Visibility = System.Windows.Visibility.Visible;
                    break;

                case SettingClass.ProcessMode.Drought1:     //가뭄(LANDSAT)
                    GroupBox_DataSet.Visibility = System.Windows.Visibility.Collapsed;
                    GroupBox_AwsDataDB.Visibility = System.Windows.Visibility.Collapsed;
                    SinjeokseolGroupDB.Visibility = System.Windows.Visibility.Collapsed;
                    Sp_IDL.Visibility = System.Windows.Visibility.Collapsed;
                    this.Height = 350;
                    break;
                case SettingClass.ProcessMode.Drought2:     //가뭄(MODIS)
                    GroupBox_DataSet.Visibility = System.Windows.Visibility.Collapsed;
                    GroupBox_AwsDataDB.Visibility = System.Windows.Visibility.Visible;
                    SinjeokseolGroupDB.Visibility = System.Windows.Visibility.Collapsed;
                    Sp_IDL.Visibility = System.Windows.Visibility.Collapsed;
                    this.Height = 530;
                    break;

//                case SettingClass.ProcessMode.Ground:     //지반침하
//                    GroupBox_DataSet.Visibility = System.Windows.Visibility.Collapsed;
//                    GroupBox_AwsDataDB.Visibility = System.Windows.Visibility.Collapsed;
//                    SinjeokseolGroupDB.Visibility = System.Windows.Visibility.Collapsed;
//                    Sp_IDL.Visibility = System.Windows.Visibility.Collapsed;
//                    this.Height = 305;
//                    break;
            }
            #endregion

            if (SettingClass.Instance.Mode == SettingClass.ProcessMode.Drought1)
                tb_RootDir.Text = SettingClass.Instance.LandSatRootDir;
            else if (SettingClass.Instance.Mode == SettingClass.ProcessMode.Drought2)
                tb_RootDir.Text = SettingClass.Instance.ModisRootDir;
            else if (SettingClass.Instance.Mode == SettingClass.ProcessMode.Snow)
            {
                if (SettingClass.Instance.SnowModeSet == SettingClass.SnowMode.FSC)
                    tb_RootDir.Text = SettingClass.Instance.FCSRootDir;
                else
                    tb_RootDir.Text = SettingClass.Instance.ModisRootDir;
            }

            
            tb_ReferenceData.Text = SettingClass.Instance.ReferenceDataPath;
            tb_GisDir.Text = SettingClass.Instance.GisFilePath;
            tb_IDLDir.Text = SettingClass.Instance.AwsDir;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            #region AWS자료수집 DB 설정
            SettingClass.Instance.DataBaseAwsIP = tb_AwsIP.Text;
            SettingClass.Instance.DataBaseAwsPort = Int32.Parse(tb_AwsPort.Text);
            SettingClass.Instance.DataBaseAwsName = tb_AwsName.Text;
            SettingClass.Instance.DataBaseAwsID = tb_AwsID.Text;
            SettingClass.Instance.DataBaseAwsPW = pb_AwsPW.Password;
            #endregion

            #region 분석 결과 저장 DB 설정
            SettingClass.Instance.DataBaseIP = tb_IP.Text;
            SettingClass.Instance.DataBasePort = Int32.Parse(tb_Port.Text);
            SettingClass.Instance.DataBaseName = tb_Name.Text;
            SettingClass.Instance.DataBaseID = tb_ID.Text;
            SettingClass.Instance.DataBasePW = pb_PW.Password;
            #endregion

            #region 신적설 DB 설정
            SettingClass.Instance.DataBaseNewSnowIP = tb_Sinjeokseol_IP.Text;
            SettingClass.Instance.DataBaseNewSnowPort = Int32.Parse(tb_Sinjeokseol_Port.Text);
            SettingClass.Instance.DataBaseNewSnowName = tb_Sinjeokseol_Name.Text;
            SettingClass.Instance.DataBaseNewSnowID = tb_Sinjeokseol_ID.Text;
            SettingClass.Instance.DataBaseNewSnowPW = pb_Sinjeokseol_PW.Password;
            #endregion

            if (SettingClass.Instance.Mode == SettingClass.ProcessMode.Drought1)
                SettingClass.Instance.LandSatRootDir = tb_RootDir.Text;
            else if (SettingClass.Instance.Mode == SettingClass.ProcessMode.Drought2)
                SettingClass.Instance.ModisRootDir = tb_RootDir.Text;
            else if (SettingClass.Instance.Mode == SettingClass.ProcessMode.Snow)
            {
                if (SettingClass.Instance.SnowModeSet == SettingClass.SnowMode.FSC)
                    SettingClass.Instance.FCSRootDir = tb_RootDir.Text;
                else
                    SettingClass.Instance.ModisRootDir = tb_RootDir.Text;
            }
            
            SettingClass.Instance.ReferenceDataPath = tb_ReferenceData.Text;
            SettingClass.Instance.GisFilePath = tb_GisDir.Text;
            SettingClass.Instance.AwsDir = tb_IDLDir.Text;

            SettingClass.Instance.DBSaveConfig();

            this.Close();
        }

        private void RootDirButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            tb_RootDir.Text = dlg.SelectedPath;

            if (SettingClass.Instance.Mode == SettingClass.ProcessMode.Drought1 || SettingClass.Instance.Mode == SettingClass.ProcessMode.Drought2)
                SettingClass.Instance.LandSatRootDir = tb_RootDir.Text;

            else if (SettingClass.Instance.Mode == SettingClass.ProcessMode.Snow)
                SettingClass.Instance.ModisRootDir = tb_RootDir.Text;
        }

        private void ReferenceDataButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            tb_ReferenceData.Text = dlg.SelectedPath;
        }

        private void GisDirButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            tb_GisDir.Text = dlg.SelectedPath;
        }

        private void IDLDirButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            tb_IDLDir.Text = dlg.SelectedPath;
        }
    }
}
