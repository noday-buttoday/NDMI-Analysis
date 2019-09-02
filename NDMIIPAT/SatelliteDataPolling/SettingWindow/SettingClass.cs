using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using Soletop.DataModel;

namespace SatelliteDataPolling.SettingWindow
{
    public class SettingClass : PropertyModel
    {
        #region Static Fields

        /// <summary>The instance.</summary>
        private static readonly SettingClass instance = new SettingClass();

        #endregion

        #region Constructors
        public SettingClass()
        {
            LoadConfig();
        }
        #endregion

        #region Properties
        /// <summary>Gets the instance.</summary>
        /// <value>The instance.</value>
        public static SettingClass Instance
        {
            get
            {
                return instance;
            }
        }

        public bool IsFileCreated
        {
            get { return (bool)this["IsFileCreated"]; }
            set { this["IsFileCreated"] = value; }
        }

        /// <summary>
        /// 작동 모드
        /// Mode: 0:폭설 1:가뭄(MODIS) 2:가뭄(LANDSAT)
        /// </summary>
        public ProcessMode Mode
        {
            get { return (ProcessMode)this["Mode"]; }
            set { this["Mode"] = value; }
        }

        /// <summary>자동/수동 설정</summary>
        public bool AutoFlag
        {
            get { return (bool)this["AutoFlag"]; }
            set { this["AutoFlag"] = value; }
        }


        public string FCSRootDir
        {
            get { return (string)this["FCSRootDir"]; }
            set { this["FCSRootDir"] = value; }
        }
        public string ModisRootDir
        {
            get { return (string)this["ModisRootDir"]; }
            set { this["ModisRootDir"] = value; }
        }
        public string LandSatRootDir
        {
            get { return (string)this["LandSatRootDir"]; }
            set { this["LandSatRootDir"] = value; }
        }
        
        /// <summary>GIS 파일 경로</summary>
        public string GisFilePath
        {
            get { return (string)this["GisFilePath"]; }
            set { this["GisFilePath"] = value; }
        }

        /// <summary>IDL 파일 경로</summary>
        public string IdlFilePath
        {
            get { return (string)this["IdlFilePath"]; }
            set { this["IdlFilePath"] = value; }
        }
        public string AwsDir
        {
            get { return (string)this["AwsDir"]; }
            set { this["AwsDir"] = value; }
        }

        #region DB
        /// <summary>DataBase IP</summary>
        public string DataBaseIP
        {
            get { return (string)this["DataBaseIP"]; }
            set { this["DataBaseIP"] = value; }
        }

        /// <summary>DataBase Port</summary>
        public int DataBasePort
        {
            get { return (int)this["DataBasePort"]; }
            set { this["DataBasePort"] = value; }
        }

        /// <summary>DataBase Name</summary>
        public string DataBaseName
        {
            get { return (string)this["DataBaseName"]; }
            set { this["DataBaseName"] = value; }
        }

        /// <summary>DataBase ID</summary>
        public string DataBaseID
        {
            get { return (string)this["DataBaseID"]; }
            set { this["DataBaseID"] = value; }
        }

        /// <summary>DataBase Password</summary>
        public string DataBasePW
        {
            get { return (string)this["DataBasePW"]; }
            set { this["DataBasePW"] = value; }
        }

        /// <summary>Aws DataBase IP</summary>
        public string DataBaseAwsIP
        {
            get { return (string)this["DataBaseAwsIP"]; }
            set { this["DataBaseAwsIP"] = value; }
        }

        /// <summary>Aws DataBase Port</summary>
        public int DataBaseAwsPort
        {
            get { return (int)this["DataBaseAwsPort"]; }
            set { this["DataBaseAwsPort"] = value; }
        }

        /// <summary>Aws DataBase Name</summary>
        public string DataBaseAwsName
        {
            get { return (string)this["DataBaseAwsName"]; }
            set { this["DataBaseAwsName"] = value; }
        }

        /// <summary>Aws DataBase ID</summary>
        public string DataBaseAwsID
        {
            get { return (string)this["DataBaseAwsID"]; }
            set { this["DataBaseAwsID"] = value; }
        }

        /// <summary>Aws DataBase Password</summary>
        public string DataBaseAwsPW
        {
            get { return (string)this["DataBaseAwsPW"]; }
            set { this["DataBaseAwsPW"] = value; }
        }

        /// <summary>New-snow DataBase IP</summary>
        public string DataBaseNewSnowIP
        {
            get { return (string)this["DataBaseNewSnowIP"]; }
            set { this["DataBaseNewSnowIP"] = value; }
        }

        /// <summary>New-snow DataBase Port</summary>
        public int DataBaseNewSnowPort
        {
            get { return (int)this["DataBaseNewSnowPort"]; }
            set { this["DataBaseNewSnowPort"] = value; }
        }

        /// <summary>New-snow DataBase Name</summary>
        public string DataBaseNewSnowName
        {
            get { return (string)this["DataBaseNewSnowName"]; }
            set { this["DataBaseNewSnowName"] = value; }
        }

        /// <summary>New-snow DataBase ID</summary>
        public string DataBaseNewSnowID
        {
            get { return (string)this["DataBaseNewSnowID"]; }
            set { this["DataBaseNewSnowID"] = value; }
        }

        /// <summary>New-snow DataBase Password</summary>
        public string DataBaseNewSnowPW
        {
            get { return (string)this["DataBaseNewSnowPW"]; }
            set { this["DataBaseNewSnowPW"] = value; }
        }
        #endregion

        #region 폭설
        /// <summary>
        /// 폭설 모드
        /// SnowMode: 0:MDSI 1:FSC
        /// </summary>
        public SnowMode SnowModeSet
        {
            get { return (SnowMode)this["SnowModeSet"]; }
            set { this["SnowModeSet"] = value; }
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

        /// <summary>폭설분석 참조자료 경로</summary>
        public string ReferenceDataPath
        {
            get { return (string)this["ReferenceDataPath"]; }
            set { this["ReferenceDataPath"] = value; }
        }

        /// <summary>최소 건설(Min Temperature)</summary>
        public int MinTemperature
        {
            get { return (int)this["MinTemperature"]; }
            set { this["MinTemperature"] = value; }
        }

        /// <summary>최대 건설(Max Temperature)</summary>
        public int MaxTemperature
        {
            get { return (int)this["MaxTemperature"]; }
            set { this["MaxTemperature"] = value; }
        }

        /// <summary>습설(Humidity)</summary>
        public int Humidity
        {
            get { return (int)this["Humidity"]; }
            set { this["Humidity"] = value; }
        }

        /// <summary>GIS 매핑 기준값 - 시설재배지(건설)</summary>
        public int Building_Temperature
        {
            get { return (int)this["Building_Temperature"]; }
            set { this["Building_Temperature"] = value; }
        }

        /// <summary>GIS 매핑 기준값 - 시설재배지(습설)</summary>
        public int Building_Humidity
        {
            get { return (int)this["Building_Humidity"]; }
            set { this["Building_Humidity"] = value; }
        }

        /// <summary>GIS 매핑 기준값 - 동식물재배지(건설)</summary>
        public int AnimalPlant_Temperature
        {
            get { return (int)this["AnimalPlant_Temperature"]; }
            set { this["AnimalPlant_Temperature"] = value; }
        }

        /// <summary>GIS 매핑 기준값 - 동식물재배지(습설)</summary>
        public int AnimalPlant_Humidity
        {
            get { return (int)this["AnimalPlant_Humidity"]; }
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

        
        /// <summary>가뭄 MATLAB_Path 경로</summary>
        public string MATLAB_Path
        {
            get { return (string)this["MATLAB_Path"]; }
            set { this["MATLAB_Path"] = value; }
        }
        /// <summary>가뭄 PYTHON_Path 경로</summary>
//        public string PYTHON_Path
//        {
//            get { return (string)this["PYTHON_Path"]; }
//            set { this["PYTHON_Path"] = value; }
//        }

        /// <summary>가뭄 LandSatResultPath 경로</summary>
        public string LandSatResultPath
        {
            get { return (string)this["LandSatResultPath"]; }
            set { this["LandSatResultPath"] = value; }
        }
        #endregion

        #region 가뭄 MODIS
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

        #region Methods
        public void LoadConfig()
        {
            #region Config Load
            ExeConfigurationFileMap fileMap = null;
            fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename =
                System.IO.Directory.GetCurrentDirectory()
                + "\\SatelliteDataPolling.exe.config"
            };

            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            var var = new NameValueCollection();
            foreach (KeyValueConfigurationElement appSetting in config.AppSettings.Settings)
            {
                var.Add(appSetting.Key, appSetting.Value);
            }
            #endregion

            IsFileCreated = bool.Parse(var["IsFileCreated"]);
            GisFilePath = var["GisFilePath"];
            Mode = (ProcessMode)Int32.Parse(var["Mode"]);
            AutoFlag = bool.Parse(var["AutoFlag"]);
            FCSRootDir = var["FCSRootDir"];
            ModisRootDir = var["ModisRootDir"];
            LandSatRootDir = var["LandSatRootDir"];
            #region DB
            DataBaseIP = var["DataBaseIP"];
            DataBasePort = Int32.Parse(var["DataBasePort"]);
            DataBaseName = var["DataBaseName"];
            DataBaseID = var["DataBaseID"];
            DataBasePW = var["DataBasePW"];
            DataBaseAwsIP = var["DataBaseAwsIP"];
            DataBaseAwsPort = Int32.Parse(var["DataBaseAwsPort"]);
            DataBaseAwsName = var["DataBaseAwsName"];
            DataBaseAwsID = var["DataBaseAwsID"];
            DataBaseAwsPW = var["DataBaseAwsPW"];
            DataBaseNewSnowIP = var["DataBaseNewSnowIP"];
            DataBaseNewSnowPort = Int32.Parse(var["DataBaseNewSnowPort"]);
            DataBaseNewSnowName = var["DataBaseNewSnowName"];
            DataBaseNewSnowID = var["DataBaseNewSnowID"];
            DataBaseNewSnowPW = var["DataBaseNewSnowPW"];
            #endregion

            #region 폭설
            IdlFilePath = var["IdlFilePath"];
            AwsDir = var["AwsDir"];
            SnowModeSet = (SnowMode)Int32.Parse(var["SnowMode"]);
            InPutFilePathMOD03 = var["InPutFilePathMOD03"];
            InPutFilePathMOD21 = var["InPutFilePathMOD21"];
            ResultPath = var["ResultPath"];
            ReferenceDataPath = var["ReferenceDataPath"];
            MinTemperature = Int32.Parse(var["MinTemperature"]);
            MaxTemperature = Int32.Parse(var["MaxTemperature"]);
            Humidity = Int32.Parse(var["Humidity"]);
            Building_Temperature = Int32.Parse(var["Building_Temperature"]);
            Building_Humidity = Int32.Parse(var["Building_Humidity"]);
            AnimalPlant_Temperature = Int32.Parse(var["AnimalPlant_Temperature"]);
            AnimalPlant_Humidity = Int32.Parse(var["AnimalPlant_Humidity"]);
            #endregion

            #region 가뭄 LANDSAT
            LandSatSatelliteImagePath = var["LandSatSatelliteImagePath"];
            LandSatMaskImagePath = var["LandSatMaskImagePath"];
            DamShapeDataPath = var["DamShapeDataPath"];
            
            MATLAB_Path = var["MATLAB_Path"];
//            PYTHON_Path = var["PYTHON_Path"];
            LandSatResultPath = var["LandSatResultPath"];

            #endregion

            #region 가뭄 MODIS
            ModisLandAnalysisResultPath = var["ModisLandAnalysisResultPath"];
            MOD11A2InputPath = var["MOD11A2InputPath"];
            MOD13A2InputPath = var["MOD13A2InputPath"];
            ModisAlgorithmPath = var["ModisAlgorithmPath"];
            ManualGpmPath = var["ManualGpmPath"];
            #endregion
        }

        public void SaveConfig()
        {
            #region Config Load
            ExeConfigurationFileMap fileMap = null;
            fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename =
                System.IO.Directory.GetCurrentDirectory()
                + "\\SatelliteDataPolling.exe.config"
            };

            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            #endregion

            config.AppSettings.Settings["IsFileCreated"].Value = IsFileCreated.ToString();
            config.AppSettings.Settings["Mode"].Value = ((int)Mode).ToString();

            config.AppSettings.Settings["FCSRootDir"].Value = ModisRootDir;
            config.AppSettings.Settings["ModisRootDir"].Value = ModisRootDir;
            config.AppSettings.Settings["LandSatRootDir"].Value = LandSatRootDir;
            config.AppSettings.Settings["AutoFlag"].Value = AutoFlag.ToString();
            config.AppSettings.Settings["GisFilePath"].Value = GisFilePath;

            #region DB
            config.AppSettings.Settings["DataBaseIP"].Value = DataBaseIP;
            config.AppSettings.Settings["DataBasePort"].Value = DataBasePort.ToString();
            config.AppSettings.Settings["DataBaseName"].Value = DataBaseName;
            config.AppSettings.Settings["DataBaseID"].Value = DataBaseID;
            config.AppSettings.Settings["DataBasePW"].Value = DataBasePW;
            config.AppSettings.Settings["DataBaseAwsIP"].Value = DataBaseAwsIP;
            config.AppSettings.Settings["DataBaseAwsPort"].Value = DataBaseAwsPort.ToString();
            config.AppSettings.Settings["DataBaseAwsName"].Value = DataBaseAwsName;
            config.AppSettings.Settings["DataBaseAwsID"].Value = DataBaseAwsID;
            config.AppSettings.Settings["DataBaseAwsPW"].Value = DataBaseAwsPW;
            config.AppSettings.Settings["DataBaseNewSnowIP"].Value = DataBaseNewSnowIP;
            config.AppSettings.Settings["DataBaseNewSnowPort"].Value = DataBaseNewSnowPort.ToString();
            config.AppSettings.Settings["DataBaseNewSnowName"].Value = DataBaseNewSnowName;
            config.AppSettings.Settings["DataBaseNewSnowID"].Value = DataBaseNewSnowID;
            config.AppSettings.Settings["DataBaseNewSnowPW"].Value = DataBaseNewSnowPW;
            #endregion

            #region 폭설
            config.AppSettings.Settings["AwsDir"].Value = AwsDir;
            config.AppSettings.Settings["IdlFilePath"].Value = IdlFilePath;
            config.AppSettings.Settings["SnowMode"].Value = ((int)SnowModeSet).ToString();
            config.AppSettings.Settings["InPutFilePathMOD03"].Value = InPutFilePathMOD03;
            config.AppSettings.Settings["InPutFilePathMOD21"].Value = InPutFilePathMOD21;
            config.AppSettings.Settings["ResultPath"].Value = ResultPath;
            config.AppSettings.Settings["ReferenceDataPath"].Value = ReferenceDataPath;
            config.AppSettings.Settings["MinTemperature"].Value = MinTemperature.ToString();
            config.AppSettings.Settings["MaxTemperature"].Value = MaxTemperature.ToString();
            config.AppSettings.Settings["Humidity"].Value = Humidity.ToString();
            config.AppSettings.Settings["Building_Temperature"].Value = Building_Temperature.ToString();
            config.AppSettings.Settings["Building_Humidity"].Value = Building_Humidity.ToString();
            config.AppSettings.Settings["AnimalPlant_Temperature"].Value = AnimalPlant_Temperature.ToString();
            config.AppSettings.Settings["AnimalPlant_Humidity"].Value = AnimalPlant_Humidity.ToString();
            #endregion

            #region 가뭄 LANDSAT
            config.AppSettings.Settings["LandSatSatelliteImagePath"].Value = LandSatSatelliteImagePath;
            config.AppSettings.Settings["LandSatMaskImagePath"].Value = LandSatMaskImagePath;
            config.AppSettings.Settings["DamShapeDataPath"].Value = DamShapeDataPath;
            config.AppSettings.Settings["LandSatResultPath"].Value = LandSatResultPath;
            #endregion

            #region 가뭄 MODIS
            config.AppSettings.Settings["ModisLandAnalysisResultPath"].Value = ModisLandAnalysisResultPath;
            config.AppSettings.Settings["MOD11A2InputPath"].Value = MOD11A2InputPath;
            config.AppSettings.Settings["MOD13A2InputPath"].Value = MOD13A2InputPath;
            config.AppSettings.Settings["ModisAlgorithmPath"].Value = ModisAlgorithmPath;
            config.AppSettings.Settings["ManualGpmPath"].Value = ManualGpmPath;
            #endregion

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            if (SettingClassSaveEvent != null)
            {
                SettingClassSaveEvent(null, EventArgs.Empty);
            }
        }

        public void DBSaveConfig()
        {
            #region Config Load
            ExeConfigurationFileMap fileMap = null;
            fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename =
                System.IO.Directory.GetCurrentDirectory()
                + "\\SatelliteDataPolling.exe.config"
            };

            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            #endregion
            config.AppSettings.Settings["FCSRootDir"].Value = ModisRootDir;
            config.AppSettings.Settings["ModisRootDir"].Value = ModisRootDir;
            config.AppSettings.Settings["LandSatRootDir"].Value = LandSatRootDir;
            
            config.AppSettings.Settings["AwsDir"].Value = AwsDir;
            config.AppSettings.Settings["ReferenceDataPath"].Value = ReferenceDataPath;
            config.AppSettings.Settings["GisFilePath"].Value = GisFilePath;
            config.AppSettings.Settings["IdlFilePath"].Value = IdlFilePath;

            config.AppSettings.Settings["DataBaseIP"].Value = DataBaseIP;
            config.AppSettings.Settings["DataBasePort"].Value = DataBasePort.ToString();
            config.AppSettings.Settings["DataBaseName"].Value = DataBaseName;
            config.AppSettings.Settings["DataBaseID"].Value = DataBaseID;
            config.AppSettings.Settings["DataBasePW"].Value = DataBasePW;
            config.AppSettings.Settings["DataBaseAwsIP"].Value = DataBaseAwsIP;
            config.AppSettings.Settings["DataBaseAwsPort"].Value = DataBaseAwsPort.ToString();
            config.AppSettings.Settings["DataBaseAwsName"].Value = DataBaseAwsName;
            config.AppSettings.Settings["DataBaseAwsID"].Value = DataBaseAwsID;
            config.AppSettings.Settings["DataBaseAwsPW"].Value = DataBaseAwsPW;
            config.AppSettings.Settings["DataBaseNewSnowIP"].Value = DataBaseNewSnowIP;
            config.AppSettings.Settings["DataBaseNewSnowPort"].Value = DataBaseNewSnowPort.ToString();
            config.AppSettings.Settings["DataBaseNewSnowName"].Value = DataBaseNewSnowName;
            config.AppSettings.Settings["DataBaseNewSnowID"].Value = DataBaseNewSnowID;
            config.AppSettings.Settings["DataBaseNewSnowPW"].Value = DataBaseNewSnowPW;

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            if (SettingClassSaveEvent != null)
            {
                SettingClassSaveEvent(null, EventArgs.Empty);
            }
        }
        #endregion

        #region Events
        public event EventHandler SettingClassSaveEvent;
        #endregion

        #region Enum
        public enum ProcessMode
        {
            Snow = 0,
            Drought1,
            Drought2
        }

        public enum SnowMode
        {
            NDSI = 0,
            FSC,
        }
        #endregion
    }
}
