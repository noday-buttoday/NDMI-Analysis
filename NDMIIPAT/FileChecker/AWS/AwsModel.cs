using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace FileChecker.AWS
{
    public class AwsModel1 : INotifyPropertyChanged
    {
        const int INVALID_VALUE = -999;
        public AwsModel1()
        {
            InitProperties();           
        }

        #region Properties
        /// <summary>AWS ID</summary>
        private int stn_id;
        public int STN_ID
        {
            get
            {
                return this.stn_id;
            }

            set
            {
                this.stn_id = value;
                NotifyPropertyChanged("STN_ID");
            }
        }

        /// <summary>관측시간 (년월일시분)</summary>
        private string tm;
        public string TM
        {
            get
            {
                return this.tm;
            }

            set
            {
                this.tm = value;
                NotifyPropertyChanged("TM");
            }
        }

        /// <summary>
        /// 위도 (deg)
        /// min:0 max:90
        /// </summary>
        private double lat;
        public double LAT
        {
            get
            {
                return this.lat;
            }

            set
            {
                this.lat = value;
                NotifyPropertyChanged("LAT");
            }
        }

        /// <summary>
        /// 경도 (deg)
        /// min:0 max:360
        /// </summary>
        private double lon;
        public double LON
        {
            get
            {
                return this.lon;
            }

            set
            {
                this.lon = value;
                NotifyPropertyChanged("LON");
            }
        }

        /// <summary>
        /// 고도 (m)
        /// min:0
        /// </summary>
        private double ht;
        public double HT
        {
            get
            {
                return this.ht;
            }

            set
            {
                this.ht = value;
                NotifyPropertyChanged("HT");
            }
        }

        /// <summary>
        /// 1분 평균 풍향 (0.1 deg)
        /// min:0 max:3600
        /// </summary>
        private int wd;
        public int WD
        {
            get
            {
                return this.wd;
            }

            set
            {
                int num = value;

                if (num > 0 && num <= 3600)
                {
                    this.wd = num;
                    NotifyPropertyChanged("WD");
                }
            }
        }

        /// <summary>
        /// 1분 평균 풍속 (0.1 m/s)
        /// min:0
        /// </summary>
        private int ws;
        public int WS
        {
            get
            {
                return this.ws;
            }

            set
            {
                int num = value;

                if (num > 0)
                {
                    this.ws = num;
                    NotifyPropertyChanged("WS");
                }
            }
        }

        /// <summary>
        /// 1분 평균 기온 (0.1 C)
        /// 실제론 min, max 값은 없지만 -1000 이하 이거나
        /// 1000 이상이면 오류 값으로 판단
        /// min:-1000 max:1000
        /// </summary>
        private int ta;
        public int TA
        {
            get
            {
                return this.ta;
            }

            set
            {
                int num = value;

                if (num > -1000 && num < 1000)
                {
                    this.ta = num;
                    NotifyPropertyChanged("TA");
                }
            }
        }

        /// <summary>
        /// 1분 평균 습도 (0.1 %)
        /// min:0 max:1000
        /// </summary>
        private int hm;
        public int HM
        {
            get
            {
                return this.hm;
            }

            set
            {
                int num = value;

                if (num > 0 && num <= 1000)
                {
                    this.hm = num;
                    NotifyPropertyChanged("HM");
                }
            }
        }

        /// <summary>
        /// 1분 평균 현지기압 (0.1 hPa)
        /// min:0 max:10132
        /// </summary>
        private int pa;
        public int PA
        {
            get
            {
                return this.pa;
            }

            set
            {
                int num = value;

                if (num > 0 && num < 10132)
                {
                    this.pa = num;
                    NotifyPropertyChanged("PA");
                }
            }
        }

        /// <summary>
        /// 1분 평균 해면기압 (0.1 hPa)
        /// min:0 max:10132
        /// </summary>
        private int ps;
        public int PS
        {
            get
            {
                return this.ps;
            }

            set
            {
                int num = value;

                if (num > 0 && num < 10132)
                {
                    this.ps = num;
                    NotifyPropertyChanged("PS");
                }
            }
        }

        /// <summary>
        /// 강수 감지 (0: 무강수)
        /// min:0
        /// </summary>
        private int rn_yn;
        public int RN_YN
        {
            get
            {
                return this.rn_yn;
            }

            set
            {
                int num = value;

                if (num > 0)
                {
                    this.rn_yn = num;
                    NotifyPropertyChanged("RN_YN");
                }
            }
        }

        /// <summary>
        /// 시간 누적 강수량 (0.1 mm)
        /// min:0
        /// </summary>
        private int rn_1hr;
        public int RN_1HR
        {
            get
            {
                return this.rn_1hr;
            }

            set
            {
                int num = value;

                if (num > 0)
                {
                    this.rn_1hr = num;
                    NotifyPropertyChanged("RN_1HR");
                }
            }
        }

        /// <summary>
        /// 일 누적 강수량 (0.1 mm)
        /// min:0
        /// </summary>
        private int rn_day;
        public int RN_DAY
        {
            get
            {
                return this.rn_day;
            }

            set
            {
                int num = value;

                if (num > 0)
                {
                    this.rn_day = num;
                    NotifyPropertyChanged("RN_DAY");
                }
            }
        }

        /// <summary>
        /// 15분 이동 누적 강수량 (0.1 mm)
        /// min:0
        /// </summary>
        private int rn_15m;
        public int RN_15M
        {
            get
            {
                return this.rn_15m;
            }

            set
            {
                int num = value;

                if (num > 0)
                {
                    this.rn_15m = num;
                    NotifyPropertyChanged("RN_15M");
                }
            }
        }

        /// <summary>
        /// 60분 이동 누적 강수량 (0.1 mm)
        /// min:0
        /// </summary>
        private int rn_60m;
        public int RN_60M
        {
            get
            {
                return this.rn_60m;
            }

            set
            {
                int num = value;

                if (num > 0)
                {
                    this.rn_60m = num;
                    NotifyPropertyChanged("RN_60M");
                }
            }
        }

        /// <summary>
        /// 일 순간최대 풍향 (0.1 deg)
        /// min:0 max:3600
        /// </summary>
        private int wd_ins;
        public int WD_INS
        {
            get
            {
                return this.wd_ins;
            }

            set
            {
                int num = value;

                if (num > 0 && num < 3600)
                {
                    this.wd_ins = num;
                    NotifyPropertyChanged("WD_INS");
                }
            }
        }

        /// <summary>
        /// 일 순간최대 풍속(0.1 m/s)
        /// min:0
        /// </summary>
        private int ws_ins;
        public int WS_INS
        {
            get
            {
                return this.ws_ins;
            }

            set
            {
                int num = value;

                if (num > 0)
                {
                    this.ws_ins = num;
                    NotifyPropertyChanged("WS_INS");
                }
            }
        }        
        #endregion

        #region PropertyChangeEvent
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        #region Init()
        /// <summary>속성 초기화</summary>
        public void InitProperties()
        {
            this.STN_ID = INVALID_VALUE;
            this.TM = string.Empty;
            this.LAT = INVALID_VALUE;
            this.LON = INVALID_VALUE;
            this.HT = INVALID_VALUE;
            this.WD = INVALID_VALUE;
            this.WS = INVALID_VALUE;
            this.TA = INVALID_VALUE;
            this.HM = INVALID_VALUE;
            this.PA = INVALID_VALUE;
            this.PS = INVALID_VALUE;
            this.RN_YN = INVALID_VALUE;
            this.RN_1HR = INVALID_VALUE;
            this.RN_DAY = INVALID_VALUE;
            this.RN_15M = INVALID_VALUE;
            this.RN_60M = INVALID_VALUE;
            this.WD_INS = INVALID_VALUE;
            this.WS_INS = INVALID_VALUE;
        }        
        #endregion
    }

    public class AwsAverageModel1 : INotifyPropertyChanged
    {
        const int INVALID_VALUE = -999;
        public AwsAverageModel1()
        {
            InitProperties();
            InitCount();
        }

        #region Properties
        /// <summary>AWS ID</summary>
        private int stn_id;
        public int STN_ID
        {
            get
            {
                return this.stn_id;
            }

            set
            {
                this.stn_id = value;
                NotifyPropertyChanged("STN_ID");
            }
        }

        /// <summary>위도 (deg)</summary>
        private double lat;
        public double LAT
        {
            get
            {
                return this.lat;
            }

            set
            {
                this.lat = value;
                NotifyPropertyChanged("LAT");
            }
        }

        /// <summary>경도 (deg)</summary>
        private double lon;
        public double LON
        {
            get
            {
                return this.lon;
            }

            set
            {
                this.lon = value;
                NotifyPropertyChanged("LON");
            }
        }

        /// <summary>고도 (m)</summary>
        private double ht;
        public double HT
        {
            get
            {
                return this.ht;
            }

            set
            {
                this.ht = value;
                NotifyPropertyChanged("HT");
            }
        }

        /// <summary>1분 평균 풍향 (0.1 deg)</summary>
        private double wd;
        public double WD
        {
            get
            {
                return this.wd;
            }

            set
            {
                this.wd = value;
                NotifyPropertyChanged("WD");
            }
        }

        /// <summary>1분 평균 풍속 (0.1 m/s)</summary>
        private double ws;
        public double WS
        {
            get
            {
                return this.ws;
            }

            set
            {
                this.ws = value;
                NotifyPropertyChanged("WS");
            }
        }

        /// <summary>1분 평균 기온 (0.1 C)</summary>
        private double ta;
        public double TA
        {
            get
            {
                return this.ta;
            }

            set
            {
                this.ta = value;
                NotifyPropertyChanged("TA");
            }
        }

        /// <summary>1분 평균 습도 (0.1 %)</summary>
        private double hm;
        public double HM
        {
            get
            {
                return this.hm;
            }

            set
            {
                this.hm = value;
                NotifyPropertyChanged("HM");
            }
        }

        /// <summary>1분 평균 현지기압 (0.1 hPa)</summary>
        private double pa;
        public double PA
        {
            get
            {
                return this.pa;
            }

            set
            {
                this.pa = value;
                NotifyPropertyChanged("PA");
            }
        }

        /// <summary>1분 평균 해면기압 (0.1 hPa)</summary>
        private double ps;
        public double PS
        {
            get
            {
                return this.ps;
            }

            set
            {
                this.ps = value;
                NotifyPropertyChanged("PS");
            }
        }

        /// <summary>강수 감지 (0: 무강수)</summary>
        private int rn_yn;
        public int RN_YN
        {
            get
            {
                return this.rn_yn;
            }

            set
            {
                this.rn_yn = value;
                NotifyPropertyChanged("RN_YN");
            }
        }

        /// <summary>시간 누적 강수량 (0.1 mm)</summary>
        private double rn_1hr;
        public double RN_1HR
        {
            get
            {
                return this.rn_1hr;
            }

            set
            {
                this.rn_1hr = value;
                NotifyPropertyChanged("RN_1HR");
            }
        }

        /// <summary>일 누적 강수량 (0.1 mm)</summary>
        private double rn_day;
        public double RN_DAY
        {
            get
            {
                return this.rn_day;
            }

            set
            {
                this.rn_day = value;
                NotifyPropertyChanged("RN_DAY");
            }
        }

        /// <summary>15분 이동 누적 강수량 (0.1 mm)</summary>
        private double rn_15m;
        public double RN_15M
        {
            get
            {
                return this.rn_15m;
            }

            set
            {
                this.rn_15m = value;
                NotifyPropertyChanged("RN_15M");
            }
        }

        /// <summary>60분 이동 누적 강수량 (0.1 mm)</summary>
        private double rn_60m;
        public double RN_60M
        {
            get
            {
                return this.rn_60m;
            }

            set
            {
                this.rn_60m = value;
                NotifyPropertyChanged("RN_60M");
            }
        }

        /// <summary>일 순간최대 풍향 (0.1 deg)</summary>
        private double wd_ins;
        public double WD_INS
        {
            get
            {
                return this.wd_ins;
            }

            set
            {
                this.wd_ins = value;
                NotifyPropertyChanged("WD_INS");
            }
        }

        /// <summary>일 순간최대 풍속(0.1 m/s)</summary>
        private double ws_ins;
        public double WS_INS
        {
            get
            {
                return this.ws_ins;
            }

            set
            {
                this.ws_ins = value;
                NotifyPropertyChanged("WS_INS");
            }
        }
        #endregion

        #region Count Properties
        private int wdcount;
        public int WdCount
        {
            get
            {
                return this.wdcount;
            }

            set
            {
                this.wdcount = value;
                NotifyPropertyChanged("WdCount");
            }
        }

        private int wscount;
        public int WsCount
        {
            get
            {
                return this.wscount;
            }

            set
            {
                this.wscount = value;
                NotifyPropertyChanged("WsCount");
            }
        }

        private int tacount;
        public int TaCount
        {
            get
            {
                return this.tacount;
            }

            set
            {
                this.tacount = value;
                NotifyPropertyChanged("TaCount");
            }
        }

        private int hmcount;
        public int HmCount
        {
            get
            {
                return this.hmcount;
            }

            set
            {
                this.hmcount = value;
                NotifyPropertyChanged("HmCount");
            }
        }

        private int pacount;
        public int PaCount
        {
            get
            {
                return this.pacount;
            }

            set
            {
                this.pacount = value;
                NotifyPropertyChanged("PaCount");
            }
        }

        private int pscount;
        public int PsCount
        {
            get
            {
                return this.pscount;
            }

            set
            {
                this.pscount = value;
                NotifyPropertyChanged("PsCount");
            }
        }

        private int rnynCount;
        public int RnYnCount
        {
            get
            {
                return this.rnynCount;
            }

            set
            {
                this.rnynCount = value;
                NotifyPropertyChanged("RnYnCount");
            }
        }

        private int rn1hrCount;
        public int Rn1HrCount
        {
            get
            {
                return this.rn1hrCount;
            }

            set
            {
                this.rn1hrCount = value;
                NotifyPropertyChanged("Rn1HrCount");
            }
        }

        private int rndayCount;
        public int RnDayCount
        {
            get
            {
                return this.rndayCount;
            }

            set
            {
                this.rndayCount = value;
                NotifyPropertyChanged("RnDayCount");
            }
        }

        private int rn15mCount;
        public int Rn15MCount
        {
            get
            {
                return this.rn15mCount;
            }

            set
            {
                this.rn15mCount = value;
                NotifyPropertyChanged("Rn15MCount");
            }
        }

        private int rn60mCount;
        public int Rn60MCount
        {
            get
            {
                return this.rn60mCount;
            }

            set
            {
                this.rn60mCount = value;
                NotifyPropertyChanged("Rn60MCount");
            }
        }

        private int wdinsCount;
        public int WdInsCount
        {
            get
            {
                return this.wdinsCount;
            }

            set
            {
                this.wdinsCount = value;
                NotifyPropertyChanged("WdInsCount");
            }
        }

        private int wsinsCount;
        public int WsInsCount
        {
            get
            {
                return this.wsinsCount;
            }

            set
            {
                this.wsinsCount = value;
                NotifyPropertyChanged("WsInsCount");
            }
        }
        #endregion

        #region PropertyChangeEvent
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        #region Init()
        /// <summary>속성 초기화</summary>
        public void InitProperties()
        {
            this.STN_ID = INVALID_VALUE;
            this.LAT = INVALID_VALUE;
            this.LON = INVALID_VALUE;
            this.HT = INVALID_VALUE;
            this.WD = INVALID_VALUE;
            this.WS = INVALID_VALUE;
            this.TA = INVALID_VALUE;
            this.HM = INVALID_VALUE;
            this.PA = INVALID_VALUE;
            this.PS = INVALID_VALUE;
            this.RN_YN = INVALID_VALUE;
            this.RN_1HR = INVALID_VALUE;
            this.RN_DAY = INVALID_VALUE;
            this.RN_15M = INVALID_VALUE;
            this.RN_60M = INVALID_VALUE;
            this.WD_INS = INVALID_VALUE;
            this.WS_INS = INVALID_VALUE;
        }

        /// <summary>속성 카운트 초기화</summary>
        public void InitCount()
        {
            this.WdCount = 0;
            this.WsCount = 0;
            this.TaCount = 0;
            this.HmCount = 0;
            this.PaCount = 0;
            this.PsCount = 0;
            this.RnYnCount = 0;
            this.Rn1HrCount = 0;
            this.RnDayCount = 0;
            this.Rn15MCount = 0;
            this.Rn60MCount = 0;
            this.WdInsCount = 0;
            this.WsInsCount = 0;
        }
        #endregion
    }
}
