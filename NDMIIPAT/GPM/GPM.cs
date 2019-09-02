using System;
using System.Net;
using System.IO;

namespace GPM
{

    public class GPMDownload
    {
        //https://urs.earthdata.nasa.gov
        //ftp://gpm1.gesdisc.eosdis.nasa.gov/data/opendap/GPM_L3/GPM_3IMERGDL.03/2016/12/

        /// <summary>연결 할 기본 디렉토리.</summary>
        public string urs { get; set; }

        /// <summary>연결 유저 이름.</summary>
        public string username { get; set; }

        /// <summary>연결 비밀번호.</summary>
        public string password { get; set; }

        /// <summary>저장할 기본 디렉토리. 경로 마지막에 '\'으로 끝나야 함.</summary>
        /// <example></example>
        public string localSavePath { get; set; }        

        public event EventHandler DownloadSuccess = null;

        public GPMDownload()
        {
            urs = "ftp://gpm1.gesdisc.eosdis.nasa.gov/data/opendap/GPM_L3/GPM_3IMERGDL.03"; // 기본 Url 입력
            username = string.Empty; // 유저 이름(ID)
            password = string.Empty; // 유저 비밀번호(Password)
            localSavePath = AppDomain.CurrentDomain.BaseDirectory;
        }

        private bool FileExistanceCheck(String file_path)
        {
            FileInfo file = new FileInfo(file_path);
            return file.Exists;
        }

        /// <summary>현재 날자의 GPM데이터를 다운받는다.</summary>
        public void DownLoad()
        {
            if (!CreateDirectory(localSavePath))
                return;

            string nc4FileName = string.Format("3B-DAY-L.MS.MRG.3IMERG.{0}-S000000-E235959.V03.nc4", DateTime.Now.ToString("yyyyMMdd"));
            string xmlFileName = string.Format("3B-DAY-L.MS.MRG.3IMERG.{0}-S000000-E235959.V03.nc4.xml", DateTime.Now.ToString("yyyyMMdd"));

            string nc4DwnUrl = string.Format("{0}/{1}/{2}/{3}", urs, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), nc4FileName);
            string xmlDwnUrl = string.Format("{0}/{1}/{2}/{3}", urs, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), xmlFileName);

            #region Nc4 다운로드
            if (!FileExistanceCheck(localSavePath + nc4FileName))
            {
                using (WebClient nc4Request = new WebClient())
                {
                    nc4Request.Credentials = new NetworkCredential(username, password); // 접속 아이디, 비밀번호

                    nc4Request.DownloadFileCompleted +=
                        delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
                        {
                            if (e.Error != null)
                            {
                                File.Delete(localSavePath + nc4FileName);
                            }
                            else
                            {
                                if (DownloadSuccess != null)
                                    DownloadSuccess(this, null);
                            }
                        };

                    //nc4Request.DownloadProgressChanged +=
                    //        delegate(object sender, DownloadProgressChangedEventArgs e)
                    //        {
                    //            long BytesReceived = e.BytesReceived;
                    //        };

                    nc4Request.DownloadFileAsync(new Uri(nc4DwnUrl), localSavePath + nc4FileName); //다운로드 시작
                }
            }
            #endregion

            #region xml 다운로드
            if (!FileExistanceCheck(localSavePath + xmlFileName))
            {
                using (WebClient xmlRequest = new WebClient())
                {
                    xmlRequest.Credentials = new NetworkCredential(username, password); // 접속 아이디, 비밀번호

                    xmlRequest.DownloadFileCompleted +=
                        delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
                        {
                            if (e.Error != null)
                            {
                                File.Delete(localSavePath + nc4FileName);
                            }
                            else
                            {
                                if (DownloadSuccess != null)
                                    DownloadSuccess(this, null);
                            }
                        };

                    //nc4Request.DownloadProgressChanged +=
                    //        delegate(object sender, DownloadProgressChangedEventArgs e)
                    //        {
                    //            long BytesReceived = e.BytesReceived;
                    //        };

                    xmlRequest.DownloadFileAsync(new Uri(nc4DwnUrl), localSavePath + xmlFileName); //다운로드 시작
                }
            }
            #endregion
        }

        /// <summary>현재 년,월에서 입력받는 일자의 GPM데이터를 다운받는다.</summary>
        /// <param name="day">다운받을 일자를 입력한다. ex)01, 15, 20, 31 </param>
        public void DownLoad(string day)
        {
            if (!CreateDirectory(localSavePath))
                return;

            string nc4FileName = string.Format("3B-DAY-L.MS.MRG.3IMERG.{0}{1}-S000000-E235959.V03.nc4", DateTime.Now.ToString("yyyyMM"), day);
            string xmlFileName = string.Format("3B-DAY-L.MS.MRG.3IMERG.{0}{1}-S000000-E235959.V03.nc4.xml", DateTime.Now.ToString("yyyyMM"), day);

            string nc4DwnUrl = string.Format("{0}/{1}/{2}/{3}", urs, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), nc4FileName);
            string xmlDwnUrl = string.Format("{0}/{1}/{2}/{3}", urs, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), xmlFileName);

            #region Nc4 다운로드
            if (!FileExistanceCheck(localSavePath + nc4FileName))
            {
                using (WebClient nc4Request = new WebClient())
                {
                    nc4Request.Credentials = new NetworkCredential(username, password); // 접속 아이디, 비밀번호

                    nc4Request.DownloadFileCompleted +=
                        delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
                        {
                            if (e.Error != null)
                            {
                                File.Delete(localSavePath + nc4FileName);
                            }
                            else
                            {
                                if (DownloadSuccess != null)
                                    DownloadSuccess(this, null);
                            }
                        };

                    //nc4Request.DownloadProgressChanged +=
                    //        delegate(object sender, DownloadProgressChangedEventArgs e)
                    //        {
                    //            long BytesReceived = e.BytesReceived;
                    //        };
           
                    nc4Request.DownloadFileAsync(new Uri(nc4DwnUrl), localSavePath + nc4FileName); //다운로드 시작
                }
            }
            #endregion

            #region xml 다운로드
            if (!FileExistanceCheck(localSavePath + xmlFileName))
            {
                using (WebClient xmlRequest = new WebClient())
                {
                    xmlRequest.Credentials = new NetworkCredential(username, password); // 접속 아이디, 비밀번호

                    xmlRequest.DownloadFileCompleted +=
                        delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
                        {
                            if (e.Error != null)
                            {
                                File.Delete(localSavePath + nc4FileName);
                            }
                            else
                            {
                                if (DownloadSuccess != null)
                                    DownloadSuccess(this, null);
                            }
                        };

                    //nc4Request.DownloadProgressChanged +=
                    //        delegate(object sender, DownloadProgressChangedEventArgs e)
                    //        {
                    //            long BytesReceived = e.BytesReceived;
                    //        };

                    //xmlRequest.DownloadFileTaskAsync(xmlDwnUrl, localSavePath + xmlFileName); //다운로드 시작
                    xmlRequest.DownloadFileAsync(new Uri(xmlDwnUrl), localSavePath + xmlFileName); //다운로드 시작
                }
            }
            #endregion
        }

        /// <summary>현재 년에서 입력받는 월,일자의 GPM데이터를 다운받는다.</summary>
        /// <param name="month">다운받을 월을 입력한다. ex)01, 05, 10, 12 </param>
        /// <param name="day">다운받을 일자를 입력한다. ex)01, 15, 20, 31 </param>
        public void DownLoad(string month , string day)
        {
            if (!CreateDirectory(localSavePath))
                return;

            string nc4FileName = string.Format("3B-DAY-L.MS.MRG.3IMERG.{0}{1}{2}-S000000-E235959.V03.nc4", DateTime.Now.ToString("yyyy"), month, day);
            string xmlFileName = string.Format("3B-DAY-L.MS.MRG.3IMERG.{0}{1}{2}-S000000-E235959.V03.nc4.xml", DateTime.Now.ToString("yyyy"), month, day);

            string nc4DwnUrl = string.Format("{0}/{1}/{2}/{3}", urs, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), nc4FileName);
            string xmlDwnUrl = string.Format("{0}/{1}/{2}/{3}", urs, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), xmlFileName);

            #region Nc4 다운로드
            if (!FileExistanceCheck(localSavePath + nc4FileName))
            {
                using (WebClient nc4Request = new WebClient())
                {
                    nc4Request.Credentials = new NetworkCredential(username, password); // 접속 아이디, 비밀번호

                    nc4Request.DownloadFileCompleted +=
                        delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
                        {
                            if (e.Error != null)
                            {
                                File.Delete(localSavePath + nc4FileName);
                            }
                        };

                    //nc4Request.DownloadProgressChanged +=
                    //        delegate(object sender, DownloadProgressChangedEventArgs e)
                    //        {
                    //            long BytesReceived = e.BytesReceived;
                    //        };

                    nc4Request.DownloadFileAsync(new Uri(nc4DwnUrl), localSavePath + nc4FileName); //다운로드 시작
                }
            }
            #endregion

            #region xml 다운로드
            if (!FileExistanceCheck(localSavePath + xmlFileName))
            {
                using (WebClient xmlRequest = new WebClient())
                {
                    xmlRequest.Credentials = new NetworkCredential(username, password); // 접속 아이디, 비밀번호

                    xmlRequest.DownloadFileCompleted +=
                        delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
                        {
                            if (e.Error != null)
                            {
                                File.Delete(localSavePath + nc4FileName);
                            }
                            else
                            {
                                if (DownloadSuccess != null)
                                    DownloadSuccess(this, null);
                            }
                        };

                    //nc4Request.DownloadProgressChanged +=
                    //        delegate(object sender, DownloadProgressChangedEventArgs e)
                    //        {
                    //            long BytesReceived = e.BytesReceived;
                    //        };

                    xmlRequest.DownloadFileAsync(new Uri(xmlDwnUrl), localSavePath + xmlFileName); //다운로드 시작
                }
            }
            #endregion
        }

        /// <summary>입력받는 년,월,일자의 GPM데이터를 다운받는다.</summary>
        /// <param name="year">다운받을 년을 입력한다. ex)2000, 2016</param>
        /// <param name="month">다운받을 월을 입력한다. ex)01, 05, 10, 12 </param>
        /// <param name="day">다운받을 일자를 입력한다. ex)01, 15, 20, 31 </param>
        public void DownLoad(string year, string month, string day)
        {
            if (!CreateDirectory(localSavePath))
                return;

            string nc4FileName = string.Format("3B-DAY-L.MS.MRG.3IMERG.{0}{1}{2}-S000000-E235959.V03.nc4", year, month, day);
            string xmlFileName = string.Format("3B-DAY-L.MS.MRG.3IMERG.{0}{1}{2}-S000000-E235959.V03.nc4.xml", year, month, day);

            string nc4DwnUrl = string.Format("{0}/{1}/{2}/{3}", urs, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), nc4FileName);
            string xmlDwnUrl = string.Format("{0}/{1}/{2}/{3}", urs, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), xmlFileName);

            #region Nc4 다운로드
            if (!FileExistanceCheck(localSavePath + nc4FileName))
            {
                using (WebClient nc4Request = new WebClient())
                {
                    nc4Request.Credentials = new NetworkCredential(username, password); // 접속 아이디, 비밀번호

                    nc4Request.DownloadFileCompleted +=
                        delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
                        {
                            if (e.Error != null)
                            {
                                File.Delete(localSavePath + nc4FileName);
                            }
                            else
                            {
                                if (DownloadSuccess != null)
                                    DownloadSuccess(this, null);
                            }
                        };

                    //nc4Request.DownloadProgressChanged +=
                    //        delegate(object sender, DownloadProgressChangedEventArgs e)
                    //        {
                    //            long BytesReceived = e.BytesReceived;
                    //        };

                    nc4Request.DownloadFileAsync(new Uri(nc4DwnUrl), localSavePath + nc4FileName); //다운로드 시작
                }
            }
            #endregion

            #region xml 다운로드
            if (!FileExistanceCheck(localSavePath + xmlFileName))
            {
                using (WebClient xmlRequest = new WebClient())
                {
                    xmlRequest.Credentials = new NetworkCredential(username, password); // 접속 아이디, 비밀번호

                    xmlRequest.DownloadFileCompleted +=
                        delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
                        {
                            if (e.Error != null)
                            {
                                File.Delete(localSavePath + nc4FileName);
                            }
                            else
                            {
                                if (DownloadSuccess != null)
                                    DownloadSuccess(this, null);
                            }
                        };

                    //nc4Request.DownloadProgressChanged +=
                    //        delegate(object sender, DownloadProgressChangedEventArgs e)
                    //        {
                    //            long BytesReceived = e.BytesReceived;
                    //        };

                    xmlRequest.DownloadFileAsync(new Uri(xmlDwnUrl), localSavePath + xmlFileName); //다운로드 시작
                }
            }
            #endregion
        }
        
        /// <summary>로컬 경로를 체크하여 없으면 생성한다.</summary>
        /// <param name="path">생성 또는 체크할 경로를 입력한다.</param>
        /// <returns>이미 경로가 있거나, 생성이 완료되면 true, 실패하거나 오류가 난다면 false</returns>
        private bool CreateDirectory(string path)
        {
            bool ret = Directory.Exists(path);

            if (!ret)
            {
                try
                {
                    Directory.CreateDirectory(path);
                    ret = true;
                }
                catch(Exception e)
                {
                    System.Windows.MessageBox.Show("GPM CreateDirectory-" + e.Message, "오류", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
                }
            }

            return ret;
        }
    }
}
