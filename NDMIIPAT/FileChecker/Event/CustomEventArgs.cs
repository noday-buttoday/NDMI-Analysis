using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileChecker.Event
{
    public class CustomEventArgs : EventArgs
    {
        private String destination_;
        private Object instance_;

        public CustomEventArgs(Object instance, String destnation)
        {
            destination_ = destnation;
            instance_ = instance;
        }

        public bool ChkDestination(String name)
        {
            bool rtn = false;

            try
            {

                if (destination_.Equals(name))
                    rtn = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return rtn;
        }

        public Object GetSender()
        {
            return instance_;
        }

        public void SetDestination(String destination)
        {
            destination_ = destination;
        }

        public String GetDestination()
        {
            return destination_;
        }
    }

    public class FileProcessEventArgs : CustomEventArgs
    {

        public FileProcessEventArgs(Object instance, String destination)
            : base(instance, destination)
        {
        }

        public List<FileInfo> input_files = new List<FileInfo>();
        public List<FileInfo> output_files = new List<FileInfo>();
        public List<FileInfo> etc_files = new List<FileInfo>();

        public List<String> msg = new List<string>();
        public String output_path { get; set; }

        public FileProcessEventArgs Clone()
        {
            FileProcessEventArgs rtn = new FileProcessEventArgs(this.GetSender(), this.GetDestination());

            foreach (FileInfo f in this.etc_files)
                rtn.etc_files.Add(f);

            foreach (FileInfo f in this.input_files)
                rtn.input_files.Add(f);

            foreach (FileInfo f in this.output_files)
                rtn.output_files.Add(f);

            foreach (String s in this.msg)
                rtn.msg.Add(s);

            rtn.output_path = this.output_path;

            return rtn;
        }
    }
}
