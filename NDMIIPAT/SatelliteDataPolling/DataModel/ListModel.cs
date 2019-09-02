using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SatelliteDataPolling
{
    public enum ListStatus
    {
        Standby = 0,
        Recving,
        Received,
        Processing,
        Complete
    }


    public class ListModel : INotifyPropertyChanged
    {
        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                this.isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }

        private int index;
        public int Index
        {
            get
            {
                return this.index;
            }

            set
            {
                this.index = value;
                NotifyPropertyChanged("Index");
            }
        }

        private string rootpath;
        public string RootPath
        {
            get
            {
                return this.rootpath;
            }

            set
            {
                this.rootpath = value;
            }
        }

        private string dirpath;
        public string DirPath
        {
            get
            {
                return this.dirpath;
            }

            set
            {
                this.dirpath = value;
                NotifyPropertyChanged("DirPath");
            }
        }

        private string filename;
        public string FileName
        {
            get
            {
                return this.filename;
            }

            set
            {
                this.filename = value;
                NotifyPropertyChanged("FileName");
            }
        }

        private string status;
        public string Status
        {
            get
            {
                return this.status;
            }

            set
            {
                this.status = value;
                NotifyPropertyChanged("Status");
            }
        }

        private int size;
        public int Size
        {
            get
            {
                return this.size;
            }

            set
            {
                this.size = value;
                NotifyPropertyChanged("Size");
            }
        }

        public ListStatus StatusType
        {
            get
            {
                return StatusType;
            }

            set
            {
                ListStatus v = value;
                switch (v)
                {
                    case ListStatus.Standby:
                        this.Status = "대기중";
                        break;

                    case ListStatus.Recving:
                        this.Status = "수신중";
                        break;

                    case ListStatus.Received:
                        this.Status = "수신완료";
                        break;

                    case ListStatus.Processing:
                        this.Status = "처리중";
                        break;

                    case ListStatus.Complete:
                        this.Status = "완료";
                        break;
                }
            }
        }

        public ListModel(string rootDir)
        {
            this.RootPath = rootDir;
            this.Index = -1;
            this.DirPath = "NULL";
            this.FileName = "NULL";
            this.StatusType = ListStatus.Standby;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
