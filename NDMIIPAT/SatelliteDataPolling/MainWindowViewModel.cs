using System;
using System.Windows.Input;
using Soletop.DataModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using Module.Event;
using Module.Integration;
using Module.PassingParameter;

namespace SatelliteDataPolling
{
    public class MainWindowViewModel : PropertyModel
    {
        #region Properties
        /// <summary>ListView의 ItemSource.</summary>
        /// <value>ItemSource for ListView.</value>
        public ObservableCollection<ListModel> List
        {
            get
            {
                return (ObservableCollection<ListModel>)this["List"];
            }

            set
            {
                this["List"] = value;
            }
        }

        /// <summary>윈도우 타이틀 텍스트</summary>
        /// <value>Title Text</value>
        public string TitleText
        {
            get { return (string)this["TitleText"]; }
            set { this["TitleText"] = value; }
        }

        /// <summary>List의 현재 선택된 인덱스</summary>
        /// <value>Selected Index</value>
        public int SelectedIndex
        {
            get { return (int)this["SelectedIndex"]; }
            set { this["SelectedIndex"] = value; }
        }

        /// <summary>버튼 텍스트</summary>
        /// <value>Button Text</value>
        public string TextPollingBtn
        {
            get { return (string)this["TextPollingBtn"]; }
            set { this["TextPollingBtn"] = value; }
        }

        /// <summary>Root 경로</summary>
        /// <value>Root Path.</value>
        public string RootPath
        {
            get { return (string)this["RootPath"]; }
            set { this["RootPath"] = value; }
        }

        /// <summary>Setting 버튼 Enable</summary>
        /// <value>Set Btn Enable.</value>
        public bool SetBtnEnable
        {
            get { return (bool)this["SetBtnEnable"]; }
            set { this["SetBtnEnable"] = value; }
        }

        /// <summary>컨텍스트메뉴 Enable</summary>
        /// <value>Context Menu Enable.</value>
        public bool ContextMenuEnable
        {
            get { return (bool)this["ContextMenuEnable"]; }
            set { this["ContextMenuEnable"] = value; }
        }

        /// <summary>자동 모드 체크</summary>
        /// <value>Auto Mode Cheked.</value>
        public bool AutoModeCheked
        {
            get { return (bool)this["AutoModeCheked"]; }
            set { this["AutoModeCheked"] = value; }
        }

        /// <summary>수동 모드 체크</summary>
        /// <value>Manual Mode Cheked.</value>
        public bool ManualModeCheked
        {
            get { return (bool)this["ManualModeCheked"]; }
            set { this["ManualModeCheked"] = value; }
        }

        /// <summary>셋팅 뷰</summary>
        /// <value>Setting View.</value>
        public SettingWindow.SettingView SettingView
        {
            get { return (SettingWindow.SettingView)this["SettingView"]; }
            set { this["SettingView"] = value; }
        }

        /// <summary>버튼 이미지 소스</summary>
        /// <value>Button Image Source.</value>
        public System.Windows.Media.Imaging.BitmapImage ImageSource
        {
            get { return (System.Windows.Media.Imaging.BitmapImage)this["ImageSource"]; }
            set { this["ImageSource"] = value; }
        }
        #endregion

        #region Command
        /// <summary>버튼 커맨드</summary>
        /// <value>Button Command.</value>
        public ICommand BtnPollingCommand { get; set; }

        /// <summary>버튼 커맨드</summary>
        /// <value>Button Command.</value>
        public ICommand BtnSettingCommand { get; set; }

        /// <summary>Ctrl+C 키보드 커맨드</summary>
        /// <value>Copy Key Command.</value>
        public ICommand CopyKeyCommand { get; set; }

        /// <summary>알고리즘 커맨드</summary>
        /// <value>Algorithmus Command.</value>
        public ICommand AlgorithmusCommand { get; set; }

        /// <summary>자동/수동모드 라디오 버튼 커맨드</summary>
        /// <value>Algorithmus Command.</value>
        public ICommand RadioCommand { get; set; }
        #endregion

        #region Fields
        /// <summary>폴링 플래그</summary>
        /// <value>Polling Flag.</value>
        private bool PollingFlag;

        private System.Windows.Media.Imaging.BitmapImage PlayImage;
        private System.Windows.Media.Imaging.BitmapImage StopImage;

        private Snow snow_module;
        private Drought drought_Landsat_module;
        #endregion

        #region Constructors
        /// <summary>MainWindowViewModel의 생성자</summary>
        public MainWindowViewModel()
        {
            Init();   
        }
        #endregion

        #region Methods
        /// <summary>초기화 함수</summary>
        private void Init()
        {

            if (SatelliteDataPolling.SettingWindow.SettingClass.Instance.Mode == SettingWindow.SettingClass.ProcessMode.Snow)
            {
                if (SettingWindow.SettingClass.Instance.SnowModeSet == SettingWindow.SettingClass.SnowMode.FSC)
                    this.RootPath = SatelliteDataPolling.SettingWindow.SettingClass.Instance.FCSRootDir;
                else
                    this.RootPath = SatelliteDataPolling.SettingWindow.SettingClass.Instance.ModisRootDir;
            }
            else if (SatelliteDataPolling.SettingWindow.SettingClass.Instance.Mode == SettingWindow.SettingClass.ProcessMode.Drought1 || SatelliteDataPolling.SettingWindow.SettingClass.Instance.Mode == SettingWindow.SettingClass.ProcessMode.Drought2)
                this.RootPath = SatelliteDataPolling.SettingWindow.SettingClass.Instance.LandSatRootDir;

            this.List = new ObservableCollection<ListModel>();
            this.SelectedIndex = -1;
            this.SettingView = new SettingWindow.SettingView();

            this.PollingFlag = false;
            this.SetBtnEnable = true;
            this.AutoModeCheked = SettingWindow.SettingClass.Instance.AutoFlag ? true : false;
            this.ManualModeCheked = SettingWindow.SettingClass.Instance.AutoFlag ? false : true;
            this.ContextMenuEnable = SettingWindow.SettingClass.Instance.AutoFlag;  //현재 모드에 따른 컨텍스트 메뉴 설정
            SettingWindow.SettingClass.Instance.SettingClassSaveEvent += Instance_SettingClassSaveEvent; //설정값 저장 완료 이벤트

            if (AutoModeCheked)
            {
                //자동모드라면 버튼 이름 감시 시작
                this.TextPollingBtn = "감시 시작";
            }
            else
            {
                //자동모드아니라면 버튼 이름 분석 시작
                this.TextPollingBtn = "분석 시작";
            }

            #region 버튼 이미지 설정
            this.PlayImage = new System.Windows.Media.Imaging.BitmapImage();
            PlayImage.BeginInit();
            PlayImage.UriSource = new Uri("pack://application:,,,/SatelliteDataPolling;component/Image/1479834932_Play01.png");
            PlayImage.EndInit();

            this.StopImage = new System.Windows.Media.Imaging.BitmapImage();
            StopImage.BeginInit();
            StopImage.UriSource = new Uri("pack://application:,,,/SatelliteDataPolling;component/Image/1479834941_black_stop.png");
            StopImage.EndInit();

            this.ImageSource = this.PlayImage;
            #endregion

            #region 윈도우 타이틀 명 설정
            switch (SatelliteDataPolling.SettingWindow.SettingClass.Instance.Mode)
            {
                case SatelliteDataPolling.SettingWindow.SettingClass.ProcessMode.Snow:     //폭설
                    this.TitleText = "폭설분석 시스템";
                    break;

                case SatelliteDataPolling.SettingWindow.SettingClass.ProcessMode.Drought2:     //가뭄(MODIS)
                    this.TitleText = "MODIS 가뭄분석 시스템";
                    break;

                case SatelliteDataPolling.SettingWindow.SettingClass.ProcessMode.Drought1:     //가뭄(LANDSAT)
                    this.TitleText = "LANDSAT 가뭄분석 시스템";
                    break;
            }
            #endregion

            InitCommand();
        }

        /// <summary>설정이 저장 완료되면 호출되는 콜백 함수</summary>
        void Instance_SettingClassSaveEvent(object sender, EventArgs e)
        {
            //현재 모드에 따른 컨텍스트 메뉴 설정
            this.ContextMenuEnable = SettingWindow.SettingClass.Instance.AutoFlag;

            //현재 RootPath 설정
            if(SettingWindow.SettingClass.Instance.Mode == SettingWindow.SettingClass.ProcessMode.Drought1 || SettingWindow.SettingClass.Instance.Mode == SettingWindow.SettingClass.ProcessMode.Drought2)
                this.RootPath = SettingWindow.SettingClass.Instance.LandSatRootDir;
            else if (SettingWindow.SettingClass.Instance.Mode == SettingWindow.SettingClass.ProcessMode.Snow)
            {
                if (SettingWindow.SettingClass.Instance.SnowModeSet == SettingWindow.SettingClass.SnowMode.FSC)
                    this.RootPath = SatelliteDataPolling.SettingWindow.SettingClass.Instance.FCSRootDir;
                else
                    this.RootPath = SatelliteDataPolling.SettingWindow.SettingClass.Instance.ModisRootDir;
            } 
        }

        private void InitCommand()
        {
            #region BtnPollingCommand
            this.BtnPollingCommand = new RelayCommand(
                delegate
                {
                    if (!this.PollingFlag)
                    {
                        //if (checkFinishedData()) return;

                        //폴링 시작할때 현재 셋팅값 저장
                        SettingWindow.SettingViewModel svm = SettingView.DataContext as SettingWindow.SettingViewModel;
                        List.Clear();
                        if (svm != null)
                            svm.SaveSettingData();
                        if (SettingWindow.SettingClass.Instance.Mode == SettingWindow.SettingClass.ProcessMode.Snow)
                            snow_module = MakeSnowModule();
                        else if (SettingWindow.SettingClass.Instance.Mode == SettingWindow.SettingClass.ProcessMode.Drought1 || SettingWindow.SettingClass.Instance.Mode == SettingWindow.SettingClass.ProcessMode.Drought2)
                            drought_Landsat_module = MakeDroughtModule();

                        if (AutoModeCheked)
                        {
                            //자동모드라면 버튼 이름 감시 정지
                            this.TextPollingBtn = "감시 정지";
                        }
                        else
                        {
                            //자동모드아니라면 버튼 이름 분석 정지
                            this.TextPollingBtn = "분석 정지";
                        }
                        this.PollingFlag = true;
                        this.SetBtnEnable = false;

                        this.ImageSource = this.StopImage;
                    }
                    else
                    {
                        if (SettingWindow.SettingClass.Instance.Mode == SettingWindow.SettingClass.ProcessMode.Snow)
                        {
                            if (snow_module != null)
                                snow_module.Stop();
                        }
                        else if (SettingWindow.SettingClass.Instance.Mode == SettingWindow.SettingClass.ProcessMode.Drought1 || SettingWindow.SettingClass.Instance.Mode == SettingWindow.SettingClass.ProcessMode.Drought2)
                        {
                            if (drought_Landsat_module != null)
                                drought_Landsat_module.Stop();
                        }

                        if (AutoModeCheked)
                        {
                            //자동모드라면 버튼 이름 감시 시작
                            this.TextPollingBtn = "감시 시작";
                        }
                        else
                        {
                            //자동모드아니라면 버튼 이름 분석 시작
                            this.TextPollingBtn = "분석 시작";
                        }
                        this.PollingFlag = false;
                        this.SetBtnEnable = true;

                        this.ImageSource = this.PlayImage;
                    }
                });
            #endregion

            #region BtnSettingCommand
            this.BtnSettingCommand = new RelayCommand(
                delegate
                {
                    SettingWindow.SettingWindow setting = new SettingWindow.SettingWindow();

                    setting.ShowDialog();
                });
            #endregion

            #region CopyKeyCommand
            this.CopyKeyCommand = new RelayCommand(
                delegate
                {
                    if (SelectedIndex > -1)
                    {
                        try
                        {
                            ListModel lm = List[SelectedIndex];
                            string str = string.Format("{0}\t{1}\t{2}\t{3}", lm.Index, lm.DirPath, lm.FileName, lm.Status);
                            System.Windows.Clipboard.SetText(str);
                        }
                        catch (Exception e)
                        {
                        }
                    }
                });
            #endregion

            #region AlgorithmusCommand - 수동 시작 컨텍스트메뉴 클릭
            this.AlgorithmusCommand = new RelayCommand(
                delegate
                {
                    ObservableCollection<ListModel> SeletedItem = new ObservableCollection<ListModel>();
                    foreach (ListModel lm in List)
                    {                        
                        if (lm.IsSelected)
                        {
                            SeletedItem.Add(lm);
                        }
                    }
                    
                });
            #endregion

            #region RadioCommand
            RadioCommand = new RelayCommand(
                delegate
                {
                    if (this.AutoModeCheked)
                    {
                        SettingWindow.SettingClass.Instance.AutoFlag = this.AutoModeCheked;

                        if (AutoModeCheked)
                        {
                            //자동모드라면 버튼 이름 감시 시작
                            this.TextPollingBtn = "감시 시작";
                        }
                        else
                        {
                            //자동모드아니라면 버튼 이름 분석 시작
                            this.TextPollingBtn = "분석 시작";
                        }
                    }
                    else
                    {
                        SettingWindow.SettingClass.Instance.AutoFlag = this.AutoModeCheked;

                        if (AutoModeCheked)
                        {
                            //자동모드라면 버튼 이름 감시 시작
                            this.TextPollingBtn = "감시 시작";
                        }
                        else
                        {
                            //자동모드아니라면 버튼 이름 분석 시작
                            this.TextPollingBtn = "분석 시작";
                        }
                    }
                });
            #endregion
        }

        //private bool checkFinishedData()
        //{
        //    bool result = false;

        //    if (drought_Landsat_module != null)
        //    {
        //        LandsatDroughtParameter param = (drought_Landsat_module.module_parameter as LandsatDroughtParameter);

        //        if (param != null)
        //        {
        //            foreach (ListModel lModel in List)
        //            {

        //            }
        //        }

        //        result = true;
        //    }
        //    return result;
        //}

        public void ModuleFinished()
        {
            if (AutoModeCheked)
            {
                //자동모드라면 버튼 이름 감시 시작
                this.TextPollingBtn = "감시 시작";
            }
            else
            {
                //자동모드아니라면 버튼 이름 분석 시작
                this.TextPollingBtn = "분석 시작";
            }
            this.PollingFlag = false;
            this.SetBtnEnable = true;

            this.ImageSource = this.PlayImage;
        }

        public void Close()
        {
            if (snow_module != null)
                snow_module.Stop();

            ModuleFinished();
        }

        #endregion

        #region 폭설모듈 인스턴스 생성
        private Snow MakeSnowModule()
        {
            IParameter parameter;
            Snow snow_module = new Snow();
            if (SettingWindow.SettingClass.Instance.SnowModeSet == SettingWindow.SettingClass.SnowMode.NDSI)
            {
                if (SettingWindow.SettingClass.Instance.AutoFlag)
                    parameter = GetAutoNdsiParameter(snow_module);
                else
                    parameter = GetManualNdsiParameter(snow_module);
            }
            else if (SettingWindow.SettingClass.Instance.SnowModeSet == SettingWindow.SettingClass.SnowMode.FSC)
            {
                if (SettingWindow.SettingClass.Instance.AutoFlag)
                    parameter = GetAutoFscParameter(snow_module);
                else
                    parameter = GetManualFscParameter(snow_module);
            }
            else
            {
                parameter = null;
            }
            snow_module.SnowModuleStart(parameter);
            return snow_module;
        }
        #endregion

        #region 폭설모듈 파라미터 설정(NDSI, FSC 자동-수동 모듈)
        private IParameter GetAutoNdsiParameter(Snow snow_module)
        {
            NdsiSnowParameter parameter = new NdsiSnowParameter();
            parameter.SetParameterType(IParameter.PARAMETER_TYPE.TYPE_SNOW_NDSI_AUTO);

            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_AWS_KRIGING_PATH)] = SettingWindow.SettingClass.Instance.AwsDir;
            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_ENVI_SCRIPT)] = "snow100_ndsi_batch_mod_gui_auto";
            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_GIS_FILE_PATH)] = SettingWindow.SettingClass.Instance.GisFilePath;
            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_INPUT_FILE_PATH)] = SettingWindow.SettingClass.Instance.ModisRootDir;
            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_AUTO_INPUT_FILE_PATH)] = SettingWindow.SettingClass.Instance.ModisRootDir;
            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_OUTPUT_FILE_PATH)] = SettingWindow.SettingClass.Instance.ResultPath;
            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_REFERENCE_PATH)] = SettingWindow.SettingClass.Instance.ReferenceDataPath;
            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_WORKING_PATH)] = SettingWindow.SettingClass.Instance.IdlFilePath;
            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_TEMP_CREATE)] = (SettingWindow.SettingClass.Instance.IsFileCreated == true) ? "삭제" : "유지";

            parameter.GisThreshold[Convert.ToInt32(NdsiSnowParameter.THRESHOLD_GIS.GIS_BUILDING_HUMI)] = SettingWindow.SettingClass.Instance.Building_Humidity;
            parameter.GisThreshold[Convert.ToInt32(NdsiSnowParameter.THRESHOLD_GIS.GIS_BUILDING_TEMP)] = SettingWindow.SettingClass.Instance.Building_Temperature;
            parameter.GisThreshold[Convert.ToInt32(NdsiSnowParameter.THRESHOLD_GIS.GIS_HOUSE_HUMI)] = SettingWindow.SettingClass.Instance.AnimalPlant_Humidity;
            parameter.GisThreshold[Convert.ToInt32(NdsiSnowParameter.THRESHOLD_GIS.GIS_HOUSE_TEMP)] = SettingWindow.SettingClass.Instance.AnimalPlant_Temperature;

            parameter.SnowThreshold[Convert.ToInt32(NdsiSnowParameter.THRESHOLD_SNOW.TYPE_DRY_SNOW_MIN)] = SettingWindow.SettingClass.Instance.MinTemperature;
            parameter.SnowThreshold[Convert.ToInt32(NdsiSnowParameter.THRESHOLD_SNOW.TYPE_DRY_SNOW_MAX)] = SettingWindow.SettingClass.Instance.MaxTemperature;
            parameter.SnowThreshold[Convert.ToInt32(NdsiSnowParameter.THRESHOLD_SNOW.TYPE_WET_SNOW)] = SettingWindow.SettingClass.Instance.Humidity;

            parameter.SatelliteFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_SATELLITE_FILE.FILE_MOD03)] = new SnowFileParameter("MOD03", ".hdf");
            parameter.SatelliteFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_SATELLITE_FILE.FILE_MOD021)] = new SnowFileParameter("MOD021", ".hdf");

            parameter.OutputFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_OUTPUT_FILE.FILE_DRY_HDR)] = new SnowFileParameter("ndsi_dry_snow", ".hdr", true);
            parameter.OutputFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_OUTPUT_FILE.FILE_DRY_IMG)] = new SnowFileParameter("ndsi_dry_snow", ".img", true);
            parameter.OutputFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_OUTPUT_FILE.FILE_WET_HDR)] = new SnowFileParameter("ndsi_wet_snow", ".hdr", true);
            parameter.OutputFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_OUTPUT_FILE.FILE_WET_IMG)] = new SnowFileParameter("ndsi_wet_snow", ".img", true);
            parameter.OutputFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_OUTPUT_FILE.FILE_TH_HDR)] = new SnowFileParameter("ndsi_th", ".hdr", true);
            parameter.OutputFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_OUTPUT_FILE.FILE_TH_IMG)] = new SnowFileParameter("ndsi_th", ".img", true);
            /*
            parameter.OutputFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_OUTPUT_FILE.FILE_HDR)] = new SnowFileParameter("ndsi", ".hdr", true);
            parameter.OutputFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_OUTPUT_FILE.FILE_IMG)] = new SnowFileParameter("ndsi", ".img", true);
             */

            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.AWS_DB_ADDR)] = SettingWindow.SettingClass.Instance.DataBaseAwsIP;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.AWS_DB_ID)] = SettingWindow.SettingClass.Instance.DataBaseAwsID;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.AWS_DB_PW)] = SettingWindow.SettingClass.Instance.DataBaseAwsPW;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.AWS_DB_NAME)] = SettingWindow.SettingClass.Instance.DataBaseAwsName;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.AWS_DB_PORT)] = SettingWindow.SettingClass.Instance.DataBaseAwsPort.ToString();

            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.SNOW_DB_ADDR)] = SettingWindow.SettingClass.Instance.DataBaseNewSnowIP;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.SNOW_DB_ID)] = SettingWindow.SettingClass.Instance.DataBaseNewSnowID;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.SNOW_DB_PW)] = SettingWindow.SettingClass.Instance.DataBaseNewSnowPW;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.SNOW_DB_NAME)] = SettingWindow.SettingClass.Instance.DataBaseNewSnowName;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.SNOW_DB_PORT)] = SettingWindow.SettingClass.Instance.DataBaseNewSnowPort.ToString();

            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.REPORT_DB_ADDR)] = SettingWindow.SettingClass.Instance.DataBaseIP;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.REPORT_DB_ID)] = SettingWindow.SettingClass.Instance.DataBaseID;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.REPORT_DB_PW)] = SettingWindow.SettingClass.Instance.DataBasePW;

            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.REPORT_DB_NAME)] = SettingWindow.SettingClass.Instance.DataBaseName;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.REPORT_DB_PORT)] = SettingWindow.SettingClass.Instance.DataBasePort.ToString();
            
            SettingWindow.SettingViewModel svm = SettingView.DataContext as SettingWindow.SettingViewModel;

            DateTime date = svm.ManualSelectedDate; //수동 모드일 경우 선택 날짜 정보
            TimeSpan time = svm.ManualSelectedTime; //수동 모드일 경우 선택 시간 정보

            parameter.AwsStartTime = date + time;

            parameter.ModuleFail = false;
            parameter.AutoMode = true;
            parameter.EnviFail = false;

            parameter.SnowModule = snow_module;

            return parameter;
        }
        private IParameter GetManualNdsiParameter(Snow snow_module)
        {
            NdsiSnowParameter parameter = new NdsiSnowParameter();
            parameter.SetParameterType(IParameter.PARAMETER_TYPE.TYPE_SNOW_NDSI_MANUAL);

            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_AWS_KRIGING_PATH)] = SettingWindow.SettingClass.Instance.AwsDir;
            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_ENVI_SCRIPT)] = "snow100_ndsi_batch_mod_gui_auto";
            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_GIS_FILE_PATH)] = SettingWindow.SettingClass.Instance.GisFilePath;
            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_INPUT_FILE_PATH)] = SettingWindow.SettingClass.Instance.ModisRootDir;
            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_AUTO_INPUT_FILE_PATH)] = SettingWindow.SettingClass.Instance.ModisRootDir;
            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_OUTPUT_FILE_PATH)] = SettingWindow.SettingClass.Instance.ResultPath;
            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_REFERENCE_PATH)] = SettingWindow.SettingClass.Instance.ReferenceDataPath;
            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_WORKING_PATH)] = SettingWindow.SettingClass.Instance.IdlFilePath;
            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_TEMP_CREATE)] = (SettingWindow.SettingClass.Instance.IsFileCreated == true) ? "삭제" : "유지";

            parameter.GisThreshold[Convert.ToInt32(NdsiSnowParameter.THRESHOLD_GIS.GIS_BUILDING_HUMI)] = SettingWindow.SettingClass.Instance.Building_Humidity;
            parameter.GisThreshold[Convert.ToInt32(NdsiSnowParameter.THRESHOLD_GIS.GIS_BUILDING_TEMP)] = SettingWindow.SettingClass.Instance.Building_Temperature;
            parameter.GisThreshold[Convert.ToInt32(NdsiSnowParameter.THRESHOLD_GIS.GIS_HOUSE_HUMI)] = SettingWindow.SettingClass.Instance.AnimalPlant_Humidity;
            parameter.GisThreshold[Convert.ToInt32(NdsiSnowParameter.THRESHOLD_GIS.GIS_HOUSE_TEMP)] = SettingWindow.SettingClass.Instance.AnimalPlant_Temperature;

            parameter.SnowThreshold[Convert.ToInt32(NdsiSnowParameter.THRESHOLD_SNOW.TYPE_DRY_SNOW_MIN)] = SettingWindow.SettingClass.Instance.MinTemperature;
            parameter.SnowThreshold[Convert.ToInt32(NdsiSnowParameter.THRESHOLD_SNOW.TYPE_DRY_SNOW_MAX)] = SettingWindow.SettingClass.Instance.MaxTemperature;
            parameter.SnowThreshold[Convert.ToInt32(NdsiSnowParameter.THRESHOLD_SNOW.TYPE_WET_SNOW)] = SettingWindow.SettingClass.Instance.Humidity;

            parameter.SatelliteFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_SATELLITE_FILE.FILE_MOD03)] = new SnowFileParameter("MOD03", ".hdf");
            parameter.SatelliteFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_SATELLITE_FILE.FILE_MOD021)] = new SnowFileParameter("MOD021", ".hdf");

            if(SettingWindow.SettingClass.Instance.InPutFilePathMOD03 != "")
                parameter.SatelliteFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_SATELLITE_FILE.FILE_MOD03)].file_info = new FileInfo(SettingWindow.SettingClass.Instance.InPutFilePathMOD03);
            if (SettingWindow.SettingClass.Instance.InPutFilePathMOD21 != "")
                parameter.SatelliteFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_SATELLITE_FILE.FILE_MOD021)].file_info = new FileInfo(SettingWindow.SettingClass.Instance.InPutFilePathMOD21);

            parameter.OutputFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_OUTPUT_FILE.FILE_DRY_HDR)] = new SnowFileParameter("ndsi_dry_snow", ".hdr", true);
            parameter.OutputFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_OUTPUT_FILE.FILE_DRY_IMG)] = new SnowFileParameter("ndsi_dry_snow", ".img", true);
            parameter.OutputFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_OUTPUT_FILE.FILE_WET_HDR)] = new SnowFileParameter("ndsi_wet_snow", ".hdr", true);
            parameter.OutputFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_OUTPUT_FILE.FILE_WET_IMG)] = new SnowFileParameter("ndsi_wet_snow", ".img", true);
            parameter.OutputFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_OUTPUT_FILE.FILE_TH_HDR)] = new SnowFileParameter("ndsi_th", ".hdr", true);
            parameter.OutputFiles[Convert.ToInt32(NdsiSnowParameter.NDSI_OUTPUT_FILE.FILE_TH_IMG)] = new SnowFileParameter("ndsi_th", ".img", true);

            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.AWS_DB_ADDR)] = SettingWindow.SettingClass.Instance.DataBaseAwsIP;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.AWS_DB_ID)] = SettingWindow.SettingClass.Instance.DataBaseAwsID;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.AWS_DB_PW)] = SettingWindow.SettingClass.Instance.DataBaseAwsPW;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.AWS_DB_NAME)] = SettingWindow.SettingClass.Instance.DataBaseAwsName;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.AWS_DB_PORT)] = SettingWindow.SettingClass.Instance.DataBaseAwsPort.ToString();

            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.SNOW_DB_ADDR)] = SettingWindow.SettingClass.Instance.DataBaseNewSnowIP;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.SNOW_DB_ID)] = SettingWindow.SettingClass.Instance.DataBaseNewSnowID;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.SNOW_DB_PW)] = SettingWindow.SettingClass.Instance.DataBaseNewSnowPW;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.SNOW_DB_NAME)] = SettingWindow.SettingClass.Instance.DataBaseNewSnowName;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.SNOW_DB_PORT)] = SettingWindow.SettingClass.Instance.DataBaseNewSnowPort.ToString();

            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.REPORT_DB_ADDR)] = SettingWindow.SettingClass.Instance.DataBaseIP;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.REPORT_DB_ID)] = SettingWindow.SettingClass.Instance.DataBaseID;
            parameter.DbInfo[Convert.ToInt32(NdsiSnowParameter.DB_CONNECTION_INFO.REPORT_DB_PW)] = SettingWindow.SettingClass.Instance.DataBasePW;

            SettingWindow.SettingViewModel svm = SettingView.DataContext as SettingWindow.SettingViewModel;

            DateTime date = svm.ManualSelectedDate; //수동 모드일 경우 선택 날짜 정보
            TimeSpan time = svm.ManualSelectedTime; //수동 모드일 경우 선택 시간 정보

            parameter.AwsStartTime = date + time;
            parameter.AutoMode = false;
            parameter.ModuleFail = false;
            parameter.EnviFail = false;
            

            parameter.SnowModule = snow_module;

            return parameter;
        }
        private IParameter GetAutoFscParameter(Snow snow_module)
        {
            FscSnowParameter parameter = new FscSnowParameter();
            parameter.SetParameterType(IParameter.PARAMETER_TYPE.TYPE_SNOW_FSC_AUTO);
            
            parameter.InputParameter[Convert.ToInt32(FscSnowParameter.FSC_INPUT_PARAMETER.PARAMETER_ENVI_SCRIPT)] = "snow200_fsc_batch_mod_gui_auto";            
            parameter.InputParameter[Convert.ToInt32(FscSnowParameter.FSC_INPUT_PARAMETER.PARAMETER_INPUT_FILE_PATH)] = SettingWindow.SettingClass.Instance.FCSRootDir;
            parameter.InputParameter[Convert.ToInt32(FscSnowParameter.FSC_INPUT_PARAMETER.PARAMETER_OUTPUT_FILE_PATH)] = SettingWindow.SettingClass.Instance.ResultPath;
            parameter.InputParameter[Convert.ToInt32(FscSnowParameter.FSC_INPUT_PARAMETER.PARAMETER_REFERENCE_PATH)] = SettingWindow.SettingClass.Instance.ReferenceDataPath;
            parameter.InputParameter[Convert.ToInt32(FscSnowParameter.FSC_INPUT_PARAMETER.PARAMETER_WORKING_PATH)] = SettingWindow.SettingClass.Instance.IdlFilePath;
            parameter.InputParameter[Convert.ToInt32(NdsiSnowParameter.NDSI_INPUT_PARAMETER.PARAMETER_AUTO_INPUT_FILE_PATH)] = SettingWindow.SettingClass.Instance.FCSRootDir;

            parameter.SatelliteFiles[Convert.ToInt32(FscSnowParameter.FSC_SATELLITE_FILE.FILE_MOD03)] = new SnowFileParameter("MOD03", ".hdf");
            parameter.SatelliteFiles[Convert.ToInt32(FscSnowParameter.FSC_SATELLITE_FILE.FILE_MOD021)] = new SnowFileParameter("MOD021", ".hdf");

            parameter.OutputFiles[Convert.ToInt32(FscSnowParameter.FSC_OUTPUT_FILE.FILE_UNMIXING_HDR)] = new SnowFileParameter("unmixing_hp", ".hdr", true);
            parameter.OutputFiles[Convert.ToInt32(FscSnowParameter.FSC_OUTPUT_FILE.FILE_UNMIXING_IMG)] = new SnowFileParameter("unmixing_hp", ".img", true);

            parameter.DbInfo[Convert.ToInt32(FscSnowParameter.DB_CONNECTION_INFO.REPORT_DB_ADDR)] = SettingWindow.SettingClass.Instance.DataBaseAwsIP;
            parameter.DbInfo[Convert.ToInt32(FscSnowParameter.DB_CONNECTION_INFO.REPORT_DB_ID)] = SettingWindow.SettingClass.Instance.DataBaseAwsID;
            parameter.DbInfo[Convert.ToInt32(FscSnowParameter.DB_CONNECTION_INFO.REPORT_DB_PW)] = SettingWindow.SettingClass.Instance.DataBaseAwsPW;
            parameter.DbInfo[Convert.ToInt32(FscSnowParameter.DB_CONNECTION_INFO.REPORT_DB_NAME)] = SettingWindow.SettingClass.Instance.DataBaseAwsName;
            parameter.DbInfo[Convert.ToInt32(FscSnowParameter.DB_CONNECTION_INFO.REPORT_DB_PORT)] = SettingWindow.SettingClass.Instance.DataBaseAwsPort.ToString();

            

            parameter.SnowModule = snow_module;
            //수동/자동 모드 저장 및 사용자 시간 지정 시간 저장 20160110 박정일
            SettingWindow.SettingViewModel svm = SettingView.DataContext as SettingWindow.SettingViewModel;

            DateTime date = svm.ManualSelectedDate; //수동 모드일 경우 선택 날짜 정보
            TimeSpan time = svm.ManualSelectedTime; //수동 모드일 경우 선택 시간 정보

            parameter.AwsStartTime = date + time;

            parameter.ModuleFail = false;
            parameter.AutoMode = true;

            return parameter;
        }
        private IParameter GetManualFscParameter(Snow snow_module)
        {
            FscSnowParameter parameter = new FscSnowParameter();
            parameter.SetParameterType(IParameter.PARAMETER_TYPE.TYPE_SNOW_FSC_MANUAL);

            parameter.InputParameter[Convert.ToInt32(FscSnowParameter.FSC_INPUT_PARAMETER.PARAMETER_ENVI_SCRIPT)] = "snow200_fsc_batch_mod_gui_auto";
            parameter.InputParameter[Convert.ToInt32(FscSnowParameter.FSC_INPUT_PARAMETER.PARAMETER_INPUT_FILE_PATH)] = SettingWindow.SettingClass.Instance.FCSRootDir;
            parameter.InputParameter[Convert.ToInt32(FscSnowParameter.FSC_INPUT_PARAMETER.PARAMETER_OUTPUT_FILE_PATH)] = SettingWindow.SettingClass.Instance.ResultPath;
            parameter.InputParameter[Convert.ToInt32(FscSnowParameter.FSC_INPUT_PARAMETER.PARAMETER_REFERENCE_PATH)] = SettingWindow.SettingClass.Instance.ReferenceDataPath;
            parameter.InputParameter[Convert.ToInt32(FscSnowParameter.FSC_INPUT_PARAMETER.PARAMETER_WORKING_PATH)] = SettingWindow.SettingClass.Instance.IdlFilePath;

            parameter.SatelliteFiles[Convert.ToInt32(FscSnowParameter.FSC_SATELLITE_FILE.FILE_MOD03)] = new SnowFileParameter("MOD03", ".hdf");
            parameter.SatelliteFiles[Convert.ToInt32(FscSnowParameter.FSC_SATELLITE_FILE.FILE_MOD021)] = new SnowFileParameter("MOD021", ".hdf");

            if (SettingWindow.SettingClass.Instance.InPutFilePathMOD03 != "")
                parameter.SatelliteFiles[Convert.ToInt32(FscSnowParameter.FSC_SATELLITE_FILE.FILE_MOD03)].file_info = new FileInfo(SettingWindow.SettingClass.Instance.InPutFilePathMOD03);
            if (SettingWindow.SettingClass.Instance.InPutFilePathMOD21 != "")
                parameter.SatelliteFiles[Convert.ToInt32(FscSnowParameter.FSC_SATELLITE_FILE.FILE_MOD021)].file_info = new FileInfo(SettingWindow.SettingClass.Instance.InPutFilePathMOD21);

            parameter.OutputFiles[Convert.ToInt32(FscSnowParameter.FSC_OUTPUT_FILE.FILE_UNMIXING_HDR)] = new SnowFileParameter("unmixing_hp", ".hdr", true);
            parameter.OutputFiles[Convert.ToInt32(FscSnowParameter.FSC_OUTPUT_FILE.FILE_UNMIXING_IMG)] = new SnowFileParameter("unmixing_hp", ".img", true);

            parameter.DbInfo[Convert.ToInt32(FscSnowParameter.DB_CONNECTION_INFO.REPORT_DB_ADDR)] = SettingWindow.SettingClass.Instance.DataBaseAwsIP;
            parameter.DbInfo[Convert.ToInt32(FscSnowParameter.DB_CONNECTION_INFO.REPORT_DB_ID)] = SettingWindow.SettingClass.Instance.DataBaseAwsID;
            parameter.DbInfo[Convert.ToInt32(FscSnowParameter.DB_CONNECTION_INFO.REPORT_DB_PW)] = SettingWindow.SettingClass.Instance.DataBaseAwsPW;
            parameter.DbInfo[Convert.ToInt32(FscSnowParameter.DB_CONNECTION_INFO.REPORT_DB_NAME)] = SettingWindow.SettingClass.Instance.DataBaseAwsName;
            parameter.DbInfo[Convert.ToInt32(FscSnowParameter.DB_CONNECTION_INFO.REPORT_DB_PORT)] = SettingWindow.SettingClass.Instance.DataBaseAwsPort.ToString();

            parameter.SnowModule = snow_module;

            //수동/자동 모드 저장 및 사용자 시간 지정 시간 저장 20160110 박정일
            SettingWindow.SettingViewModel svm = SettingView.DataContext as SettingWindow.SettingViewModel;

            DateTime date = svm.ManualSelectedDate; //수동 모드일 경우 선택 날짜 정보
            TimeSpan time = svm.ManualSelectedTime; //수동 모드일 경우 선택 시간 정보

            parameter.AwsStartTime = date + time;

            parameter.ModuleFail = false;
            parameter.AutoMode = false;
            return parameter;
        }
        #endregion

        #region 가뭄모듈 인스턴스 생성
        private Drought MakeDroughtModule()
        {
            //<!--Mode: 0:폭설 1:가뭄(LANDSAT) 2:가뭄(MODIS)-->
            IParameter parameter;
            Drought drought_module = new Drought();
            if (SettingWindow.SettingClass.Instance.Mode == SettingWindow.SettingClass.ProcessMode.Drought1)
            {
                if (SettingWindow.SettingClass.Instance.AutoFlag)
                    parameter = GetAutoLandSatParameter(drought_module);
                else
                    parameter = GetManualLandSatParameter(drought_module);
            }
            else if (SettingWindow.SettingClass.Instance.Mode == SettingWindow.SettingClass.ProcessMode.Drought2)
            {
                if (SettingWindow.SettingClass.Instance.AutoFlag)
                    parameter = GetAutoModisParameter(drought_module);
                else
                    parameter = GetManualModisParameter(drought_module);
            }
            else
            {
                parameter = null;
            }
            drought_module.DroughtModuleStart(parameter);
            return drought_module;
        }
        #endregion

        #region 가뭄모듈 파라미터 설정
        private IParameter GetAutoModisParameter(Drought drought_module)
        {
            ModisDroughtParameter parameter = new ModisDroughtParameter();
            parameter.SetParameterType(IParameter.PARAMETER_TYPE.TYPE_DROUGHT_MODIS_AUTO);
            parameter.InputParameter[Convert.ToInt32(ModisDroughtParameter.MODIS_INPUT_PARAMETER.PARAMETER_GPM_PATH)] = SettingWindow.SettingClass.Instance.ModisRootDir;
            parameter.InputParameter[Convert.ToInt32(ModisDroughtParameter.MODIS_INPUT_PARAMETER.PARAMETER_MOD11_PATH)] = SettingWindow.SettingClass.Instance.MOD11A2InputPath;
            parameter.InputParameter[Convert.ToInt32(ModisDroughtParameter.MODIS_INPUT_PARAMETER.PARAMETER_MOD13_PATH)] = SettingWindow.SettingClass.Instance.MOD13A2InputPath;
            parameter.InputParameter[Convert.ToInt32(ModisDroughtParameter.MODIS_INPUT_PARAMETER.PARAMETER_ALGORITHM_PATH)] = SettingWindow.SettingClass.Instance.ModisAlgorithmPath;
            parameter.InputParameter[Convert.ToInt32(ModisDroughtParameter.MODIS_INPUT_PARAMETER.PARAMETER_OUTPUT_FILE_PATH)] = SettingWindow.SettingClass.Instance.ModisLandAnalysisResultPath;


            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_ADDR)] = SettingWindow.SettingClass.Instance.DataBaseIP;
            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_ID)] = SettingWindow.SettingClass.Instance.DataBaseID;
            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_NAME)] = SettingWindow.SettingClass.Instance.DataBaseName;
            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_PORT)] = SettingWindow.SettingClass.Instance.DataBasePort.ToString();
            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_PW)] = SettingWindow.SettingClass.Instance.DataBasePW;

            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.AWS_DB_ADDR)] = SettingWindow.SettingClass.Instance.DataBaseAwsIP;
            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.AWS_DB_ID)] = SettingWindow.SettingClass.Instance.DataBaseAwsID;
            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.AWS_DB_NAME)] = SettingWindow.SettingClass.Instance.DataBaseAwsName;
            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.AWS_DB_PORT)] = SettingWindow.SettingClass.Instance.DataBaseAwsPort.ToString();
            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.AWS_DB_PW)] = SettingWindow.SettingClass.Instance.DataBaseAwsPW;

            parameter.AwsStartTime = (SettingView.DataContext as SettingWindow.SettingViewModel).SelectedDate;
            parameter.AutoMode = true;
            parameter.drought = drought_module;

            return parameter;
        }
        private IParameter GetManualModisParameter(Drought drought_module)
        {
            ModisDroughtParameter parameter = new ModisDroughtParameter();
            parameter.SetParameterType(IParameter.PARAMETER_TYPE.TYPE_DROUGHT_MODIS_MANUAL);
            parameter.InputParameter[Convert.ToInt32(ModisDroughtParameter.MODIS_INPUT_PARAMETER.PARAMETER_GPM_PATH)] = SettingWindow.SettingClass.Instance.ManualGpmPath;
            parameter.InputParameter[Convert.ToInt32(ModisDroughtParameter.MODIS_INPUT_PARAMETER.PARAMETER_MOD11_PATH)] = SettingWindow.SettingClass.Instance.MOD11A2InputPath;
            parameter.InputParameter[Convert.ToInt32(ModisDroughtParameter.MODIS_INPUT_PARAMETER.PARAMETER_MOD13_PATH)] = SettingWindow.SettingClass.Instance.MOD13A2InputPath;
            parameter.InputParameter[Convert.ToInt32(ModisDroughtParameter.MODIS_INPUT_PARAMETER.PARAMETER_ALGORITHM_PATH)] = SettingWindow.SettingClass.Instance.ModisAlgorithmPath;
            parameter.InputParameter[Convert.ToInt32(ModisDroughtParameter.MODIS_INPUT_PARAMETER.PARAMETER_OUTPUT_FILE_PATH)] = SettingWindow.SettingClass.Instance.ModisLandAnalysisResultPath;

            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_ADDR)] = SettingWindow.SettingClass.Instance.DataBaseIP;
            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_ID)] = SettingWindow.SettingClass.Instance.DataBaseID;
            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_NAME)] = SettingWindow.SettingClass.Instance.DataBaseName;
            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_PORT)] = SettingWindow.SettingClass.Instance.DataBasePort.ToString();
            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_PW)] = SettingWindow.SettingClass.Instance.DataBasePW;

            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.AWS_DB_ADDR)] = SettingWindow.SettingClass.Instance.DataBaseAwsIP;
            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.AWS_DB_ID)] = SettingWindow.SettingClass.Instance.DataBaseAwsID;
            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.AWS_DB_NAME)] = SettingWindow.SettingClass.Instance.DataBaseAwsName;
            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.AWS_DB_PORT)] = SettingWindow.SettingClass.Instance.DataBaseAwsPort.ToString();
            parameter.DbInfo[Convert.ToInt32(ModisDroughtParameter.DB_CONNECTION_INFO.AWS_DB_PW)] = SettingWindow.SettingClass.Instance.DataBaseAwsPW;

            parameter.AwsStartTime = (SettingView.DataContext as SettingWindow.SettingViewModel).SelectedDate;
            parameter.AutoMode = false;
            parameter.drought = drought_module;

            return parameter;
        }
        private IParameter GetAutoLandSatParameter(Drought drought_module)
        {
            LandsatDroughtParameter parameter = new LandsatDroughtParameter();
            parameter.SetParameterType(IParameter.PARAMETER_TYPE.TYPE_DROUGHT_LANDSAT_AUTO);
            SettingWindow.SettingViewModel svm = SettingView.DataContext as SettingWindow.SettingViewModel;

            parameter.InputParameter[Convert.ToInt32(LandsatDroughtParameter.LANDSAT_INPUT_PARAMETER.PARAMETER_INPUT_FILE_PATH)] = SettingWindow.SettingClass.Instance.LandSatRootDir;
            parameter.InputParameter[Convert.ToInt32(LandsatDroughtParameter.LANDSAT_INPUT_PARAMETER.PARAMETER_MATLAB_PATH)] = svm.DamShapeDataPath;
            parameter.InputParameter[Convert.ToInt32(LandsatDroughtParameter.LANDSAT_INPUT_PARAMETER.PARAMETER_OUTPUT_FILE_PATH)] = SettingWindow.SettingClass.Instance.LandSatResultPath;
//            parameter.InputParameter[Convert.ToInt32(LandsatDroughtParameter.LANDSAT_INPUT_PARAMETER.PARAMETER_PYTHON_PATH)] = svm.DamShapeDataPath;
            parameter.InputParameter[Convert.ToInt32(LandsatDroughtParameter.LANDSAT_INPUT_PARAMETER.PARAMETER_SHP_FILE_PATH)] = svm.DamShapeDataPath;
            parameter.InputParameter[Convert.ToInt32(LandsatDroughtParameter.LANDSAT_INPUT_PARAMETER.PARAMETER_CURRENT_OUTPUT_PATH)] = SettingWindow.SettingClass.Instance.LandSatResultPath;
            parameter.InputParameter[Convert.ToInt32(LandsatDroughtParameter.LANDSAT_INPUT_PARAMETER.PARAMETER_AUTO_INPUT_FILE_PATH)] = SettingWindow.SettingClass.Instance.LandSatRootDir;
            parameter.ModuleFail = false;

            parameter.DbInfo[Convert.ToInt32(LandsatDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_ADDR)] = SettingWindow.SettingClass.Instance.DataBaseIP;
            parameter.DbInfo[Convert.ToInt32(LandsatDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_ID)] = SettingWindow.SettingClass.Instance.DataBaseID;
            parameter.DbInfo[Convert.ToInt32(LandsatDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_NAME)] = SettingWindow.SettingClass.Instance.DataBaseName;
            parameter.DbInfo[Convert.ToInt32(LandsatDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_PORT)] = SettingWindow.SettingClass.Instance.DataBasePort.ToString();
            parameter.DbInfo[Convert.ToInt32(LandsatDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_PW)] = SettingWindow.SettingClass.Instance.DataBasePW;

            parameter.drought_module = drought_module;

            parameter.ModuleFail = false;
            //수동/자동 모드 저장 및 사용자 시간 지정 시간 저장 20160110 박정일


            DateTime date = svm.ManualSelectedDate; //수동 모드일 경우 선택 날짜 정보
            TimeSpan time = svm.ManualSelectedTime; //수동 모드일 경우 선택 시간 정보

            parameter.AwsStartTime = date + time;

            parameter.ModuleFail = false;
            parameter.AutoMode = true;

            return parameter;
        }
        private IParameter GetManualLandSatParameter(Drought drought_module)
        {
            LandsatDroughtParameter parameter = new LandsatDroughtParameter();
            parameter.SetParameterType(IParameter.PARAMETER_TYPE.TYPE_DROUGHT_LANDSAT_MANUAL);
            SettingWindow.SettingViewModel svm = SettingView.DataContext as SettingWindow.SettingViewModel;

            parameter.InputParameter[Convert.ToInt32(LandsatDroughtParameter.LANDSAT_INPUT_PARAMETER.PARAMETER_INPUT_FILE_PATH)] = SettingWindow.SettingClass.Instance.LandSatSatelliteImagePath;
            parameter.InputParameter[Convert.ToInt32(LandsatDroughtParameter.LANDSAT_INPUT_PARAMETER.PARAMETER_MATLAB_PATH)] = svm.DamShapeDataPath;
            parameter.InputParameter[Convert.ToInt32(LandsatDroughtParameter.LANDSAT_INPUT_PARAMETER.PARAMETER_OUTPUT_FILE_PATH)] = SettingWindow.SettingClass.Instance.LandSatResultPath;
//            parameter.InputParameter[Convert.ToInt32(LandsatDroughtParameter.LANDSAT_INPUT_PARAMETER.PARAMETER_PYTHON_PATH)] = svm.DamShapeDataPath;
            parameter.InputParameter[Convert.ToInt32(LandsatDroughtParameter.LANDSAT_INPUT_PARAMETER.PARAMETER_SHP_FILE_PATH)] = svm.DamShapeDataPath;
            parameter.InputParameter[Convert.ToInt32(LandsatDroughtParameter.LANDSAT_INPUT_PARAMETER.PARAMETER_CURRENT_OUTPUT_PATH)] = SettingWindow.SettingClass.Instance.LandSatResultPath;
            
            
            parameter.ModuleFail = false;

            parameter.DbInfo[Convert.ToInt32(LandsatDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_ADDR)] = SettingWindow.SettingClass.Instance.DataBaseIP;
            parameter.DbInfo[Convert.ToInt32(LandsatDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_ID)] = SettingWindow.SettingClass.Instance.DataBaseID;
            parameter.DbInfo[Convert.ToInt32(LandsatDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_NAME)] = SettingWindow.SettingClass.Instance.DataBaseName;
            parameter.DbInfo[Convert.ToInt32(LandsatDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_PORT)] = SettingWindow.SettingClass.Instance.DataBasePort.ToString();
            parameter.DbInfo[Convert.ToInt32(LandsatDroughtParameter.DB_CONNECTION_INFO.REPORT_DB_PW)] = SettingWindow.SettingClass.Instance.DataBasePW;
            
            parameter.drought_module = drought_module;

            parameter.ModuleFail = false;
            //수동/자동 모드 저장 및 사용자 시간 지정 시간 저장 20160110 박정일
            
            
            DateTime date = svm.ManualSelectedDate; //수동 모드일 경우 선택 날짜 정보
            TimeSpan time = svm.ManualSelectedTime; //수동 모드일 경우 선택 시간 정보

            parameter.AwsStartTime = date + time;

            parameter.ModuleFail = false;
            parameter.AutoMode = false;

            return parameter;
        }
        #endregion

        #region GUI 업데이트
        public void UpdateList(FileInfo file_, String msg_)
        {
            ListModel list_model_;

            int list_index_ = 0;
            for (; list_index_ < List.Count; ++list_index_)
            {
                list_model_ = List[list_index_];
                if (list_model_.FileName == file_.Name)
                    break;
            }

            if (list_index_ < List.Count)
            {
                List[list_index_].Status = msg_;
                List[list_index_].Size = Convert.ToInt32(file_.Length);
            }
            else
            {
                list_model_ = new ListModel(Path.GetDirectoryName(file_.FullName));
                list_model_.Index = list_index_;
                list_model_.Status = msg_;
                list_model_.FileName = file_.Name;
                list_model_.DirPath = Path.GetDirectoryName(file_.FullName);
                list_model_.Size = Convert.ToInt32(file_.Length);
                List.Add(list_model_);
            }
        }
        #endregion
    }
}

