using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace FileChecker.AWS
{
    public class AwsMaker
    {
        AwsFileList AswFiles;
        Thread AwsThread;

        private String aws_input_path_;
        private int aws_period_;

        public AwsMaker(String aws_input_path, int period)
        {
            this.AswFiles = new AwsFileList();
            aws_input_path_ = aws_input_path;

            aws_period_ = period;

            this.AwsThread = new Thread(AwsHourRun);
            this.AwsThread.Start();
        }

        public void Stop()
        {
            this.AwsThread.Abort();
        }

        public void MakeFile(String file_full_path)
        {
            AswFiles.AwsAverage1ToFile(file_full_path);
        }
        
        private void AwsHourRun()
        {
            DateTime nowTime = DateTime.Now;
            DateTime starttime = (nowTime.AddHours( aws_period_ * -1 ));
            DateTime time = (nowTime.AddHours(aws_period_ * -1));

            string Path = aws_input_path_;

            #region 해당 폴더에 있는 AWS 로드
            for (int i = 0; (time = starttime.AddMinutes(i)) <= nowTime; i++)
            {
                string fileName = string.Format("AWS_MIN_{0}", time.ToString("yyyyMMddHHmm"));

                if (File.Exists(Path + fileName))
                {
                    FileInfo fileInfo = new FileInfo(Path + fileName);

                    StreamReader sr = File.OpenText(Path + fileName);
                    string[] awsInfos = sr.ReadToEnd().Split('=');

                    // 한개의 AWS 파일에 대한 정보
                    AwsFile af = new AwsFile();
                    af.FileName = fileInfo.Name;
                    af.FileTime = DateTime.ParseExact(fileInfo.Name.Replace("AWS_MIN_", ""), "yyyyMMddHHmm", null);

                    for (int j = 0; j < awsInfos.Length; j++)
                    {
                        string[] info = awsInfos[j].Split('#');

                        if (info.Length < 19)
                            continue;

                        // 한개의 AWS 파일 내부의 한개 정보
                        AwsModel1 ai = new AwsModel1();
                        ai.STN_ID = Int32.Parse(info[0]);
                        ai.TM = info[1];
                        ai.LAT = double.Parse(info[2]);
                        ai.LON = double.Parse(info[3]);
                        ai.HT = double.Parse(info[4]);
                        ai.WD = Int32.Parse(info[5]);
                        ai.WS = Int32.Parse(info[6]);
                        ai.TA = Int32.Parse(info[7]);
                        ai.HM = Int32.Parse(info[8]);
                        ai.PA = Int32.Parse(info[9]);
                        ai.PS = Int32.Parse(info[10]);
                        ai.RN_YN = Int32.Parse(info[11]);
                        ai.RN_1HR = Int32.Parse(info[12]);
                        ai.RN_DAY = Int32.Parse(info[13]);
                        ai.RN_15M = Int32.Parse(info[14]);
                        ai.RN_60M = Int32.Parse(info[15]);
                        ai.WD_INS = Int32.Parse(info[16]);
                        ai.WS_INS = Int32.Parse(info[17]);

                        af.Add(ai);
                    }

                    // 여러개의 AWS 파일에 대한 정보
                    if (af.Count > 0)
                        lock (AswFiles)
                            AswFiles.Add(af.FileName, af);
                }
            }
            #endregion

            while (true)
            {
                Thread.Sleep(30000);

                nowTime = DateTime.Now;
                starttime = (nowTime.AddHours(aws_period_ * -1));

                string nowAwsFileName = string.Format("AWS_MIN_{0}", nowTime.ToString("yyyyMMddHHmm"));
                string startAwsFileName = string.Format("AWS_MIN_{0}", starttime.ToString("yyyyMMddHHmm"));

                if (AswFiles.Count > 0)
                {
                    try
                    {
                        AwsFile deleteAwsFile = AswFiles[startAwsFileName];

                        AswFiles.Remove(startAwsFileName);

                        string fileName = string.Format("AWS_MIN_{0}", nowTime.ToString("yyyyMMddHHmm"));

                        FileInfo fileInfo = new FileInfo(Path + fileName);
                        if (File.Exists(Path + fileName))
                        {
                            StreamReader sr = File.OpenText(Path + fileName);
                            string[] awsInfos = sr.ReadToEnd().Split('=');

                            // 한개의 AWS 파일에 대한 정보
                            AwsFile af = new AwsFile();
                            af.FileName = fileInfo.Name;
                            af.FileTime = DateTime.ParseExact(fileInfo.Name.Replace("AWS_MIN_", ""), "yyyyMMddHHmm", null);

                            for (int j = 0; j < awsInfos.Length; j++)
                            {
                                string[] info = awsInfos[j].Replace("\n", "").Split('#');


                                if (info.Length < 19)
                                    continue;

                                // 한개의 AWS 파일 내부의 한개 정보
                                AwsModel1 ai = new AwsModel1();
                                ai.STN_ID = Int32.Parse(info[0]);
                                ai.TM = info[1];
                                ai.LAT = double.Parse(info[2]);
                                ai.LON = double.Parse(info[3]);
                                ai.HT = double.Parse(info[4]);
                                ai.WD = Int32.Parse(info[5]);
                                ai.WS = Int32.Parse(info[6]);
                                ai.TA = Int32.Parse(info[7]);
                                ai.HM = Int32.Parse(info[8]);
                                ai.PA = Int32.Parse(info[9]);
                                ai.PS = Int32.Parse(info[10]);
                                ai.RN_YN = Int32.Parse(info[11]);
                                ai.RN_1HR = Int32.Parse(info[12]);
                                ai.RN_DAY = Int32.Parse(info[13]);
                                ai.RN_15M = Int32.Parse(info[14]);
                                ai.RN_60M = Int32.Parse(info[15]);
                                ai.WD_INS = Int32.Parse(info[16]);
                                ai.WS_INS = Int32.Parse(info[17]);

                                af.Add(ai);
                            }

                            // 여러개의 AWS 파일에 대한 정보
                            if (af.Count > 0)
                                lock (AswFiles)
                                    AswFiles.Add(af.FileName, af);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
