using System;
using System.IO;
using System.Collections.Generic;

using FileChecker.Event;

namespace FileChecker.Checker
{
    public class NewFile
    {
        private string path_;
        private string prefix_;
        private string extension_;
        private bool same_name_;
        private bool event_use_;

        private List<FileInfo> last_file_list_ = new List<FileInfo>();
        private List<FileInfo> new_file_list_ = new List<FileInfo>();

        public void SetEnvironment(String path, String prefix, String extension, bool same_name = false, bool event_use = true)
        {
            path_ = path;
            prefix_ = prefix;
            extension_ = extension;
            same_name_ = same_name;
            event_use_ = event_use;
        }

        public void ChangePath(String path)
        {
            path_ = path;
        }

        public void InitializeFiles()
        {
            last_file_list_.Clear();

            FileProcessEventArgs event_arg = new FileProcessEventArgs(this, "FILE_STATUS");

            foreach (FileInfo f in GetFiles(new DirectoryInfo(path_)))
            {
                last_file_list_.Add(f);
                event_arg.input_files.Add(f);
                event_arg.msg.Add("기존파일");
            }

            if(event_use_)
                EventControl.Instance().SendEvent(event_arg);
        }

        public List<FileInfo> GetNewFiles()
        {
            List<FileInfo> new_files = GetFiles(new DirectoryInfo(path_));

            foreach (FileInfo s in last_file_list_)
            {
                int file_index = new_files.FindIndex(x => x.FullName.Contains(s.FullName));

                if(file_index >= 0)
                    new_files.RemoveAt(file_index);
            }

            FileProcessEventArgs event_arg = new FileProcessEventArgs(this, "FILE_STATUS");

            foreach (FileInfo f in new_files)
            {
                last_file_list_.Add(f);
                event_arg.input_files.Add(f);
                event_arg.msg.Add("수신시작");
            }

            if (event_use_ && event_arg.input_files.Count > 0)
                EventControl.Instance().SendEvent(event_arg);

            return new_files;
        }

        private List<FileInfo> GetFiles(DirectoryInfo di)
        {
            List<FileInfo> rtn = new List<FileInfo>();
            try
            {
                FileInfo[] files = di.GetFiles();

                foreach (FileInfo f in files)
                {
                    if (same_name_ == false)
                    {
                        if (f.Name.Contains(prefix_) && f.Extension == extension_)
                            rtn.Add(f);
                    }
                    else
                    {
                        if (f.Name == (prefix_ + extension_))
                            rtn.Add(f);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return rtn;
        }
       
    }
}
