using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Soletop.DataModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace SatelliteDataPolling.SettingWindow
{
    public class SettingViewModel : PropertyModel
    {
        #region Commands
        /// <summary>경로설정 버튼 커맨드</summary>
        /// <value>Path Button Command.</value>
        public ICommand PathSettingCommand { get; set; }
        #endregion

        #region Properties
        /// <summary>수동 날짜 선택</summary>
        public DateTime ManualSelectedDate
        {
            get { return (DateTime)this["ManualSelectedDate"]; }
            set
            {
                this["ManualSelectedDate"] = value;
            }
        }

        public TimeSpan ManualSelectedTime
        {
            get { return (TimeSpan)this["ManualSelectedTime"]; }
            set
            {
                this["ManualSelectedTime"] = value;
            }
        }

        public bool IsFileCreated
        {
            get { return (bool)this["IsFileCreated"]; }
            set { this["IsFileCreated"] = value; }
        }

        public System.Windows.Visibility AutoVisibility
        {
            get { return (System.Windows.Visibility)this["AutoVisibility"]; }
            set { this["AutoVisibility"] = value; }
        }

        /// <summary>자동 모드</summary>
        public bool AutoMode
        {
            get { return (bool)this["AutoMode"]; }
            set { this["AutoMode"] = value; }
        }

        /// <summary>수동 모드</summary>
        public bool ManualMode
        {
            get { return (bool)this["ManualMode"]; }
            set { this["ManualMode"] = value; }
        }

        /// <summary>폭설 설정 컨트롤 Visibility</summary>
        public System.Windows.Visibility SnowVisibility
        {
            get { return (System.Windows.Visibility)this["SnowVisibility"]; }
            set { this["SnowVisibility"] = value; }
        }

        /// <summary>가뭄 LANDSAT 설정 컨트롤 Visibility</summary>
        public System.Windows.Visibility Drought1Visibility
        {
            get { return (System.Windows.Visibility)this["Drought1Visibility"]; }
            set { this["Drought1Visibility"] = value; }
        }

        /// <summary>가뭄 MODIS 설정 컨트롤 Visibility</summary>
        public System.Windows.Visibility Drought2Visibility
        {
            get { return (System.Windows.Visibility)this["Drought2Visibility"]; }
            set { this["Drought2Visibility"] = value; }
        }

        #region 폭설 분석
        /// <summary>폭설모드</summary>
        public int SnowMode
        {
            get { return (int)this["SnowMode"]; }
            set { this["SnowMode"] = value; }
        }

        /// <summary>분석유형</summary>
        public string SnowType
        {
            get { return (string)this["SnowType"]; }
            set { this["SnowType"] = value;}
        }

        /// <summary>입력영상 파일 경로(분석대상 영상1번의 파일 경로)</summary>
        public string InPutFilePathMOD03
        {
            get { return (string)this["InPutFilePathMOD03"]; }
            set { this["InPutFilePathMOD03"] = value; }
        }

        /// <summary>입력영상 파일 경로(분석대상 영상2번의 파일 경로)</summary>
        public string InPutFilePathMOD21
        {
            get { return (string)this["InPutFilePathMOD21"]; }
            set { this["InPutFilePathMOD21"] = value; }
        }

        /// <summary>분석결과 저장 경로로</summary>
        public string ResultPath
        {
            get { return (string)this["ResultPath"]; }
            set { this["ResultPath"] = value; }
        }

        /// <summary>최소 건설(Min Temperature)</summary>
        public string MinTemperature
        {
            get { return (string)this["MinTemperature"]; }
            set { this["MinTemperature"] = value; }
        }

        /// <summary>최대 건설(Max Temperature)</summary>
        public string MaxTemperature
        {
            get { return (string)this["MaxTemperature"]; }
            set { this["MaxTemperature"] = value; }
        }

        /// <summary>습설(Humidity)</summary>
        public string Humidity
        {
            get { return (string)this["Humidity"]; }
            set { this["Humidity"] = value; }
        }

        /// <summary>GIS 매핑 기준값 - 시설재배지(건설)</summary>
        public string Building_Temperature
        {
            get { return (string)this["Building_Temperature"]; }
            set { this["Building_Temperature"] = value; }
        }

        /// <summary>GIS 매핑 기준값 - 시설재배지(습설)</summary>
        public string Building_Humidity
        {
            get { return (string)this["Building_Humidity"]; }
            set { this["Building_Humidity"] = value; }
        }

        /// <summary>GIS 매핑 기준값 - 동식물재배지(건설)</summary>
        public string AnimalPlant_Temperature
        {
            get { return (string)this["AnimalPlant_Temperature"]; }
            set { this["AnimalPlant_Temperature"] = value; }
        }

        /// <summary>GIS 매핑 기준값 - 동식물재배지(습설)</summary>
        public string AnimalPlant_Humidity
        {
            get { return (string)this["AnimalPlant_Humidity"]; }
            set { this["AnimalPlant_Humidity"] = value; }
        }        
        #endregion

        #region 가뭄 LANDSAT
        /// <summary>LANDSAT 위성영상 경로</summary>
        public string LandSatSatelliteImagePath
        {
            get { return (string)this["LandSatSatelliteImagePath"]; }
            set { this["LandSatSatelliteImagePath"] = value; }
        }

        /// <summary>LANDSAT Mask영상 경로</summary>
        public string LandSatMaskImagePath
        {
            get { return (string)this["LandSatMaskImagePath"]; }
            set { this["LandSatMaskImagePath"] = value; }
        }

        /// <summary>댐유역 Shape 자료 경로</summary>
        public string DamShapeDataPath
        {
            get { return (string)this["DamShapeDataPath"]; }
            set { this["DamShapeDataPath"] = value; }
        }

        /// <summary>가뭄분석 결과 경로</summary>
        public string LandSatResultPath
        {
            get { return (string)this["LandSatResultPath"]; }
            set { this["LandSatResultPath"] = value; }
        }
        #endregion

        #region 가뭄 MODIS
        /// <summary>Day of Year</summary>
        public string DOY
        {
            get { return (string)this["DOY"]; }
            set { this["DOY"] = value; }
        }

        /// <summary>SelectedDate</summary>
        public DateTime SelectedDate
        {
            get { return (DateTime)this["SelectedDate"]; }
            set
            {
                DateTime time = (DateTime)value;
                DOY = time.DayOfYear.ToString();
                this["SelectedDate"] = value; 
            }
        }

        /// <summary>가뭄(MODIS)분석 결과 저장 경로</summary>
        public string ModisLandAnalysisResultPath
        {
            get { return (string)this["ModisLandAnalysisResultPath"]; }
            set { this["ModisLandAnalysisResultPath"] = value; }
        }

        /// <summary>MOD11A2 입력파일 경로</summary>
        public string MOD11A2InputPath
        {
            get { return (string)this["MOD11A2InputPath"]; }
            set { this["MOD11A2InputPath"] = value; }
        }

        /// <summary>MOD13A2 입력파일 경로</summary>
        public string MOD13A2InputPath
        {
            get { return (string)this["MOD13A2InputPath"]; }
            set { this["MOD13A2InputPath"] = value; }
        }

        public string ModisAlgorithmPath
        {
            get { return (string)this["ModisAlgorithmPath"]; }
            set { this["ModisAlgorithmPath"] = value; }
        }

        public string ManualGpmPath
        {
            get { return (string)this["ManualGpmPath"]; }
            set { this["ManualGpmPath"] = value; }
        }

        #endregion
        #endregion

        public SettingViewModel()
        {
            SelectedDate = DateTime.Now;
            ManualSelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            ManualSelectedTime = DateTime.Now.TimeOfDay;
            LoadSettingData();
            InitCommand();

            #region AutoVisibility 설정
            if (SettingClass.Instance.AutoFlag)
            {
                AutoVisibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                AutoVisibility = System.Windows.Visibility.Visible;
            }

            SettingClass.Instance.Bind(
                delegate
                {
                    if (SettingClass.Instance.AutoFlag)
                    {
                        AutoVisibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        AutoVisibility = System.Windows.Visibility.Visible;
                    }
                    
                }, "AutoFlag");
            #endregion

            #region 모드에 따른 UI
            switch (SettingClass.Instance.Mode)
            {
                case SettingClass.ProcessMode.Snow:     //폭설
                    this.SnowVisibility = System.Windows.Visibility.Visible;
                    this.Drought1Visibility = System.Windows.Visibility.Collapsed;
                    this.Drought2Visibility = System.Windows.Visibility.Collapsed;
                    break;

                case SettingClass.ProcessMode.Drought1:     //가뭄(MODIS)
                    this.SnowVisibility = System.Windows.Visibility.Collapsed;
                    this.Drought1Visibility = System.Windows.Visibility.Visible;
                    this.Drought2Visibility = System.Windows.Visibility.Collapsed;
                    break;

                case SettingClass.ProcessMode.Drought2:     //가뭄(LANDSAT)
                    this.SnowVisibility = System.Windows.Visibility.Collapsed;
                    this.Drought1Visibility = System.Windows.Visibility.Collapsed;
                    this.Drought2Visibility = System.Windows.Visibility.Visible;
                    break;
            }
            #endregion
        }

        private void InitCommand()
        {
            this.PathSettingCommand = new ParameterRelayCommand(CmdPathSettingCallback);
        }

        public void LoadSettingData()
        {
            if (SettingClass.Instance.AutoFlag)
            {
                AutoMode = true;
                ManualMode = false;
            }
            else
            {
                AutoMode = false;
                ManualMode = true;
            }

            IsFileCreated = SettingClass.Instance.IsFileCreated;

            #region 폭설
            this.SnowMode = (int)SettingClass.Instance.SnowModeSet;
            this.InPutFilePathMOD03 = SettingClass.Instance.InPutFilePathMOD03;
            this.InPutFilePathMOD21 = SettingClass.Instance.InPutFilePathMOD21;
            this.ResultPath = SettingClass.Instance.ResultPath;
            this.MinTemperature = SettingClass.Instance.MinTemperature.ToString();
            this.MaxTemperature = SettingClass.Instance.MaxTemperature.ToString();
            this.Humidity = SettingClass.Instance.Humidity.ToString();
            this.Building_Temperature = SettingClass.Instance.Building_Temperature.ToString();
            this.Building_Humidity = SettingClass.Instance.Building_Humidity.ToString();
            this.AnimalPlant_Temperature = SettingClass.Instance.AnimalPlant_Temperature.ToString();
            this.AnimalPlant_Humidity = SettingClass.Instance.AnimalPlant_Humidity.ToString();
            #endregion

            #region 가뭄 LANDSAT
            this.LandSatSatelliteImagePath = SettingClass.Instance.LandSatSatelliteImagePath;
            this.LandSatMaskImagePath = SettingClass.Instance.LandSatMaskImagePath;
            this.DamShapeDataPath = SettingClass.Instance.DamShapeDataPath;
            this.LandSatResultPath = SettingClass.Instance.LandSatResultPath;
            #endregion

            #region 가뭄 MODIS
            this.ModisLandAnalysisResultPath = SettingClass.Instance.ModisLandAnalysisResultPath;
            this.MOD11A2InputPath = SettingClass.Instance.MOD11A2InputPath;
            this.MOD13A2InputPath = SettingClass.Instance.MOD13A2InputPath;
            this.ModisAlgorithmPath = SettingClass.Instance.ModisAlgorithmPath;
            this.ManualGpmPath = SettingClass.Instance.ManualGpmPath;
            #endregion
        }

        public void SaveSettingData()
        {
            SettingClass.Instance.IsFileCreated = IsFileCreated;

            #region 폭설
            SettingClass.Instance.SnowModeSet = (SettingClass.SnowMode)this.SnowMode;
            SettingClass.Instance.InPutFilePathMOD03 = this.InPutFilePathMOD03;
            SettingClass.Instance.InPutFilePathMOD21 = this.InPutFilePathMOD21;
            SettingClass.Instance.ResultPath = this.ResultPath;
            SettingClass.Instance.MinTemperature = int.Parse(this.MinTemperature);
            SettingClass.Instance.MaxTemperature = int.Parse(this.MaxTemperature);
            SettingClass.Instance.Humidity = int.Parse(this.Humidity);
            SettingClass.Instance.Building_Temperature = int.Parse(this.Building_Temperature);
            SettingClass.Instance.Building_Humidity = int.Parse(this.Building_Humidity);
            SettingClass.Instance.AnimalPlant_Temperature = int.Parse(this.AnimalPlant_Temperature);
            SettingClass.Instance.AnimalPlant_Humidity = int.Parse(this.AnimalPlant_Humidity);
            #endregion

            #region 가뭄 LANDSAT
            SettingClass.Instance.LandSatSatelliteImagePath = this.LandSatSatelliteImagePath;
            SettingClass.Instance.LandSatMaskImagePath = this.LandSatMaskImagePath;
            SettingClass.Instance.DamShapeDataPath = this.DamShapeDataPath;
            SettingClass.Instance.LandSatResultPath = this.LandSatResultPath;
            #endregion

            #region 가뭄 MODIS
            SettingClass.Instance.ModisLandAnalysisResultPath = this.ModisLandAnalysisResultPath;
            SettingClass.Instance.MOD11A2InputPath = this.MOD11A2InputPath;
            SettingClass.Instance.MOD13A2InputPath = this.MOD13A2InputPath;
            SettingClass.Instance.ModisAlgorithmPath = this.ModisAlgorithmPath;
            SettingClass.Instance.ManualGpmPath = this.ManualGpmPath;
            #endregion

            SettingClass.Instance.SaveConfig();
        }        

        /// <summary>경로 설정 커맨드.</summary>
        /// <param name="obj">The obj.</param>
        private void CmdPathSettingCallback(object obj)
        {
            int num = Convert.ToInt16(obj);
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            //Directory 초기화 삭제 20160110 박정일
            //ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            ofd.Filter = "HDF fire (*.hdf)|*.hdf";
            
            if (num == 0 || num == 1)
            {
                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;
            }
            else
            {
                if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;
            }

            switch (num)
            {
                    //폭설
                case 0:
                    this.InPutFilePathMOD03 = ofd.FileName;
                    SettingClass.Instance.InPutFilePathMOD03 = ofd.FileName;
                    break;

                case 1:
                    this.InPutFilePathMOD21 = ofd.FileName;
                    SettingClass.Instance.InPutFilePathMOD21 = ofd.FileName;
                    break;

                case 2:
                    this.ResultPath = dlg.SelectedPath;
                    break;

                    //가뭄(LANDSAT)
                case 10:
                    this.LandSatSatelliteImagePath = dlg.SelectedPath; 
                    break;

                case 11:
                    this.LandSatMaskImagePath = dlg.SelectedPath; 
                    break;

                case 12:
                    this.DamShapeDataPath = dlg.SelectedPath; 
                    break;

                case 13:
                    this.LandSatResultPath = dlg.SelectedPath; 
                    break;

                    //가뭄(MODIS)

                case 20:
                    this.ModisLandAnalysisResultPath = dlg.SelectedPath;
                    break;

                case 21:
                    this.ModisAlgorithmPath = dlg.SelectedPath;
                    break;

                case 22:
                    this.MOD11A2InputPath = dlg.SelectedPath;
                    break;
                    
                case 23:
                    this.MOD13A2InputPath = dlg.SelectedPath;
                    break;
                case 24:
                    this.ManualGpmPath = dlg.SelectedPath;
                    break;
                default:
                    break;
            }
        }
    }
}
