using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.IO;

namespace FileChecker.AWS
{
    public class AwsFileList : Dictionary<string, AwsFile>, INotifyPropertyChanged
    {
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

        public AwsFileList()
        {
        }

        public Dictionary<int, AwsAverageModel1> AwsAverage1()
        {
            if (this.Count <= 0)
                return null;

            Dictionary<int, AwsAverageModel1> Aws = new Dictionary<int, AwsAverageModel1>();

            string[] aflKeys = this.Keys.ToArray<string>();

            lock (this)
            {
                for (int i = 0; i < aflKeys.Length; i++)
                {
                    AwsFile af = this[aflKeys[i]];
                    foreach (AwsModel1 am in af)
                    {
                        try
                        {
                            AwsAverageModel1 aam = Aws[am.STN_ID];

                            if (am.WD != -999)
                            {
                                aam.WD += am.WD;
                                aam.WdCount++;
                            }

                            if (am.WS != -999)
                            {
                                aam.WS += am.WS;
                                aam.WsCount++;
                            }

                            if (am.TA != -999)
                            {
                                aam.TA += am.TA;
                                aam.TaCount++;
                            }

                            if (am.HM != -999)
                            {
                                aam.HM += am.HM;
                                aam.HmCount++;
                            }

                            if (am.PA != -999)
                            {
                                aam.PA += am.PA;
                                aam.PaCount++;
                            }

                            if (am.PS != -999)
                            {
                                aam.PS += am.PS;
                                aam.PsCount++;
                            }

                            //aam.RN_YN += am.RN_YN;
                            if (am.RN_1HR != -999)
                                aam.RN_1HR = am.RN_1HR;

                            if(am.RN_DAY != -999)
                                aam.RN_DAY = am.RN_DAY;
                            //aam.RN_15M += am.RN_15M;
                            //aam.RN_60M += am.RN_60M;
                            //aam.WD_INS += am.WD_INS;
                            //aam.WS_INS += am.WS_INS;

                            Aws[am.STN_ID] = aam;
                        }
                        catch (System.Collections.Generic.KeyNotFoundException e)
                        {
                            AwsAverageModel1 aam = new AwsAverageModel1();
                            aam.STN_ID = am.STN_ID;
                            aam.LON = am.LON;
                            aam.LAT = am.LAT;
                            aam.HT = am.HT;

                            if (am.WD != -999)
                            {
                                aam.WD = am.WD;
                                aam.WdCount++;
                            }

                            if (am.WS != -999)
                            {
                                aam.WS = am.WS;
                                aam.WsCount++;
                            }

                            if (am.TA != -999)
                            {
                                aam.TA = am.TA;
                                aam.TaCount++;
                            }

                            if (am.HM != -999)
                            {
                                aam.HM = am.HM;
                                aam.HmCount++;
                            }

                            if (am.PA != -999)
                            {
                                aam.PA = am.PA;
                                aam.PaCount++;
                            }

                            if (am.PS != -999)
                            {
                                aam.PS = am.PS;
                                aam.PsCount++;
                            }

                            //aam.RN_YN = am.RN_YN;
                            if (am.RN_1HR != -999)
                                aam.RN_1HR = am.RN_1HR;

                            if (am.RN_DAY != -999)
                                aam.RN_DAY = am.RN_DAY;
                            //aam.RN_15M = am.RN_15M;
                            //aam.RN_60M = am.RN_60M;
                            //aam.WD_INS = am.WD_INS;
                            //aam.WS_INS = am.WS_INS;
                            Aws.Add(aam.STN_ID, aam);
                        }
                    }
                }
            }

            int[] awsKeys = Aws.Keys.ToArray<int>();

            for (int i = 0; i < awsKeys.Length; i++)
            {
                AwsAverageModel1 aam = Aws[awsKeys[i]];
                aam.WD = (aam.WD / 10) / aam.WdCount;
                aam.WS = (aam.WS / 10) / aam.WsCount;
                aam.TA = (aam.TA / 10) / aam.TaCount;
                aam.HM = (aam.HM / 10) / aam.HmCount;
                aam.PA = (aam.PA / 10) / aam.PaCount;
                aam.PS = (aam.PS / 10) / aam.PsCount;
                Aws[awsKeys[i]] = aam;
            }

            return Aws;
        }

        public void AwsAverage1ToFile(string fullFilePath)
        {

            Dictionary<int, AwsAverageModel1> Aws = new Dictionary<int, AwsAverageModel1>();

            string[] aflKeys = this.Keys.ToArray<string>();

            lock (this)
            {
                for (int i = 0; i < aflKeys.Length; i++)
                {
                    AwsFile af = this[aflKeys[i]];
                    foreach (AwsModel1 am in af)
                    {
                        try
                        {
                            AwsAverageModel1 aam = Aws[am.STN_ID];

                            if (am.WD != -999)
                            {
                                aam.WD += am.WD;
                                aam.WdCount++;
                            }

                            if (am.WS != -999)
                            {
                                aam.WS += am.WS;
                                aam.WsCount++;
                            }

                            if (am.TA != -999)
                            {
                                aam.TA += am.TA;
                                aam.TaCount++;
                            }

                            if (am.HM != -999)
                            {
                                aam.HM += am.HM;
                                aam.HmCount++;
                            }

                            if (am.PA != -999)
                            {
                                aam.PA += am.PA;
                                aam.PaCount++;
                            }

                            if (am.PS != -999)
                            {
                                aam.PS += am.PS;
                                aam.PsCount++;
                            }

                            //aam.RN_YN += am.RN_YN;
                            if (am.RN_1HR != -999)
                                aam.RN_1HR = am.RN_1HR;

                            if (am.RN_DAY != -999)
                                aam.RN_DAY = am.RN_DAY;
                            //aam.RN_15M += am.RN_15M;
                            //aam.RN_60M += am.RN_60M;
                            //aam.WD_INS += am.WD_INS;
                            //aam.WS_INS += am.WS_INS;

                            Aws[am.STN_ID] = aam;
                        }
                        catch (System.Collections.Generic.KeyNotFoundException e)
                        {
                            AwsAverageModel1 aam = new AwsAverageModel1();
                            aam.STN_ID = am.STN_ID;
                            aam.LON = am.LON;
                            aam.LAT = am.LAT;
                            aam.HT = am.HT;

                            if (am.WD != -999)
                            {
                                aam.WD = am.WD;
                                aam.WdCount++;
                            }

                            if (am.WS != -999)
                            {
                                aam.WS = am.WS;
                                aam.WsCount++;
                            }

                            if (am.TA != -999)
                            {
                                aam.TA = am.TA;
                                aam.TaCount++;
                            }

                            if (am.HM != -999)
                            {
                                aam.HM = am.HM;
                                aam.HmCount++;
                            }

                            if (am.PA != -999)
                            {
                                aam.PA = am.PA;
                                aam.PaCount++;
                            }

                            if (am.PS != -999)
                            {
                                aam.PS = am.PS;
                                aam.PsCount++;
                            }

                            //aam.RN_YN = am.RN_YN;
                            if (am.RN_1HR != -999)
                                aam.RN_1HR = am.RN_1HR;

                            if (am.RN_DAY != -999)
                                aam.RN_DAY = am.RN_DAY;
                            //aam.RN_15M = am.RN_15M;
                            //aam.RN_60M = am.RN_60M;
                            //aam.WD_INS = am.WD_INS;
                            //aam.WS_INS = am.WS_INS;
                            Aws.Add(aam.STN_ID, aam);
                        }
                    }
                }
            }

            int[] awsKeys = Aws.Keys.ToArray<int>();

            FileStream saveFile = new FileStream(fullFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(saveFile);

            for (int i = 0; i < awsKeys.Length; i++)
            {
                AwsAverageModel1 aam = Aws[awsKeys[i]];
                aam.WD = (aam.WD / 10) / aam.WdCount;
                aam.WS = (aam.WS / 10) / aam.WsCount;
                aam.TA = (aam.TA / 10) / aam.TaCount;
                aam.HM = (aam.HM / 10) / aam.HmCount;
                aam.PA = (aam.PA / 10) / aam.PaCount;
                aam.PS = (aam.PS / 10) / aam.PsCount;
                Aws[awsKeys[i]] = aam;

                string writeStr = string.Format("{0}#{1}#{2}#{3}#{4}#{5}#{6}#{7}#{8}#{9}#{10}#{11}#{12}#{13}#{14}#{15}#{16}#{17}#=\n",
                    aam.STN_ID, "000000000000", aam.LAT, aam.LON, aam.HT, aam.WD, aam.WS, aam.TA, aam.HM, aam.PA, aam.PS, aam.RN_YN, aam.RN_1HR, aam.RN_DAY, aam.RN_15M, aam.RN_60M, aam.WD_INS, aam.WS_INS);
                sw.Write(writeStr);
            }

            sw.Flush();
            sw.Close();
            saveFile.Close();            
        }
    }

    public class AwsFile : List<AwsModel1>
    {
        public string FileName { get; set; }
        public DateTime FileTime { get; set; }


        public AwsFile()
        {
            FileName = string.Empty;
            FileTime = new DateTime(1, 1, 1);
        }
    }
}
