using FileChecker.AWS;
using FileChecker.Checker;
using FileChecker.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace FileChecker
{
    public class FileControl
    {
        protected List<NewFile> file_new_checker_ = new List<NewFile>();
        protected List<FileSize> file_size_checker_ = new List<FileSize>();
        protected System.Timers.Timer timer_;

        protected String base_path_;
        protected String prefix_;
        protected String extension_;

        virtual public void AddModule(String path, String prefix, String extension, bool same_name = false, bool event_use = true)
        {
            NewFile new_file = new NewFile();
            new_file.SetEnvironment(path, prefix, extension, same_name, event_use);
            new_file.InitializeFiles();
            file_new_checker_.Add(new_file);

            FileSize file_size = new FileSize(event_use);
            file_size.FileSizeCheckCompleted += new EventHandler(SizeCheckCompleted);
            file_size_checker_.Add(file_size);
            file_size.Start();
        }

        public virtual void Close()
        {
            foreach (FileSize f in file_size_checker_)
                f.Close();
            timer_.Stop();
            timer_.Close();
        }

        virtual public void Start(int interval = 10)
        {
            timer_ = new System.Timers.Timer();
            timer_.Interval = interval * 1000;
            timer_.Elapsed += new System.Timers.ElapsedEventHandler(TimerEvent);
            timer_.Start();
        }

        virtual protected void TimerEvent(Object sender, EventArgs e)
        {
            for (int i = 0; i < file_new_checker_.Count; i++)
            {
                List<FileInfo> files = file_new_checker_[i].GetNewFiles();
                if (files.Count > 0)
                    foreach (FileInfo f in files)
                        file_size_checker_[i].SetFile(f);
            }
        }

        virtual protected void SizeCheckCompleted(object sender, EventArgs e)
        {
        }

        virtual public void StartAwsMaker(String aws_input_path, String aws_output_path, int period = 6)
        {}
    }

    public class SnowInputFileControl : FileControl
    {
        private AwsMaker aws_maker_;
        private String output_path_ = "";
        private String last_date_ = "";
        private String aws_output_path_ = "";
        private int check_count = 0;
        private int module_type_ = 0;

        public SnowInputFileControl()
        {
            last_date_ = DateTime.Now.ToShortDateString();
        }

        public void SetOutputPath(String output_path)
        {
            output_path_ = output_path;
        }

        public void SetModuleType(int module_type)
        {
            module_type_ = module_type;
        }

        public override void AddModule(string path, string prefix, string extension, bool same_name = false, bool event_use = true)
        {
            base_path_ = path;
            prefix_ = prefix;
            extension_ = extension;

            if (last_date_ != DateTime.Now.ToShortDateString())
                last_date_ = DateTime.Now.ToShortDateString();

            path = path + "\\" + last_date_;

            base.AddModule(path, prefix, extension, same_name, event_use);
        }

        public override void Close()
        {
            base.Close();
            if(aws_maker_ != null)
                aws_maker_.Stop();
        }

        public override void StartAwsMaker(String aws_input_path, String aws_output_path, int period)
        {
            aws_maker_ = new AwsMaker(aws_input_path, period);
            aws_output_path_ = aws_output_path;            
        }

        protected override void TimerEvent(object sender, EventArgs e)
        {
            if (last_date_ != DateTime.Now.ToShortDateString())
            {
                last_date_ = DateTime.Now.ToShortDateString();

                String path = base_path_ + "\\" + last_date_;

                foreach (NewFile n in file_new_checker_)
                    n.ChangePath(path);
            }

            base.TimerEvent(sender, e);
        }

        protected override void SizeCheckCompleted(object sender, EventArgs e)
        {
            FileProcessEventArgs event_args = new FileProcessEventArgs(this, "FILE_STATUS");

            FileSize ch = (sender as FileSize);            
            ch.Pause();

            event_args.input_files.Add(ch.current_file);
            event_args.msg.Add("수신완료");

            EventControl.Instance().SendEvent(event_args);

            check_count++;

            if (check_count == file_size_checker_.Count)
            {
                FileProcessEventArgs new_event_args = new FileProcessEventArgs(this, "FILE_COMPLETE");
                List<FileInfo> input_file = new List<FileInfo>();
                foreach (FileSize f in file_size_checker_)
                {
                    input_file.Add(f.current_file);
                    new_event_args.input_files.Add(f.current_file);
                    new_event_args.msg.Add("계산시작");
                    f.Resume();
                }

                String output_path = "";

                if (module_type_ == 0)
                {
                    output_path = (new AlgorithmControl()).StartNDSIModule(input_file, output_path_);
                }
                else
                {

                }

                check_count = 0;

                new_event_args.output_path = output_path;

                EventControl.Instance().SendEvent(new_event_args);
            }
        }

        private void GetAwsData(String file_name)
        {
            String full_path = aws_output_path_ + "\\" + file_name;
            aws_maker_.MakeFile(aws_output_path_);
        }

    }

    public class OutputFileControl : FileControl
    {
        private Queue<FileProcessEventArgs> output_path_ = new Queue<FileProcessEventArgs>();
        private List<FileInfo> input_files = new List<FileInfo>();
        private bool process_flag_ = false;
        private String output_ = "";
        private int check_count = 0;

        public OutputFileControl()
        {
            EventControl.Instance().CheckEvent += new EventHandler(OutputLogicStartEvent);
        }

        private void OutputLogicStartEvent(Object sender, EventArgs e)
        {
            CustomEventArgs custom = (e as CustomEventArgs);
            if (custom.ChkDestination("FILE_COMPLETE"))
            {
                FileProcessEventArgs args = (custom as FileProcessEventArgs);
                output_path_.Enqueue(args);
            }
        }

        public void ChangePath(String path)
        {
            output_ = path;
            foreach (NewFile n in file_new_checker_)
            {
                n.ChangePath(path);
                n.InitializeFiles();
            }
        }

        protected override void TimerEvent(object sender, EventArgs e)
        {
            if (output_path_.Count > 0 && process_flag_ == false)
            {
                process_flag_ = true;
                FileProcessEventArgs args = output_path_.Dequeue();

                ChangePath(args.output_path);

                input_files.Clear();

                foreach (FileInfo f in args.input_files)
                    input_files.Add(f);
            }

            base.TimerEvent(sender, e);
        }

        protected override void SizeCheckCompleted(object sender, EventArgs e)
        {
            FileSize ch = (sender as FileSize);
            ch.Pause();

            check_count++;

            if (check_count == file_size_checker_.Count)
            {
                FileProcessEventArgs event_args = new FileProcessEventArgs(this, "PROCESS_COMPLETE");

                foreach (FileInfo f in input_files)
                    event_args.input_files.Add(f);

                foreach (FileSize f in file_size_checker_)
                {   
                    event_args.output_files.Add(f.current_file);
                    event_args.msg.Add("계산완료");
                    f.Resume();
                }

                check_count = 0;
                event_args.output_path = output_;
                EventControl.Instance().SendEvent(event_args);

                process_flag_ = false;
            }
        }
    }
}
