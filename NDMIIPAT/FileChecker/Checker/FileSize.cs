using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using FileChecker.Event;

namespace FileChecker.Checker
{
    public class FileSize
    {
        private Queue<FileInfo> input_queue_ = new Queue<FileInfo>();
        private Thread size_check_thread_;
        private Semaphore sync_;
        private bool pause_;
        private bool operate_;
        private bool event_use_;

        public FileInfo current_file { get; set; }

        public event EventHandler FileSizeCheckCompleted;

        public FileSize(bool event_use = true)
        {
            sync_ = new Semaphore(1, 1);
            pause_ = false;
            operate_ = true;
            event_use_ = event_use;
        }

        public void SetFile(FileInfo file)
        {
            sync_.WaitOne();
            input_queue_.Enqueue(file);
            sync_.Release();
        }

        public void Start()
        {
            size_check_thread_ = new Thread(new ThreadStart(GetFile));
            size_check_thread_.Start();
        }

        public void Close()
        {
            operate_ = false;
            size_check_thread_.Join();
        }

        public void Pause()
        {
            pause_ = true;
        }

        public void Resume()
        {
            pause_ = false;
        }

        private void GetFile()
        {
            do
            {
                sync_.WaitOne();
                if (input_queue_.Count > 0 && pause_ == false)
                {
                    FileInfo file = input_queue_.Dequeue();
                    sync_.Release();
                    CheckFileSize(file);
                }
                else
                {   
                    sync_.Release();
                    Thread.Sleep(100);
                }

            } while (operate_);
        }

        private void CheckFileSize(FileInfo file)
        {
            long prev_size = 0;
            
            do
            {
                FileInfo check_file = new FileInfo(file.FullName);
                current_file = check_file;

                FileProcessEventArgs event_arg = new FileProcessEventArgs(this, "FILE_STATUS");

                if (prev_size == check_file.Length && check_file.Length > 0)
                {
                    event_arg.input_files.Add(check_file);
                    event_arg.msg.Add("수신완료");

                    if(event_use_)
                        EventControl.Instance().SendEvent(event_arg);

                    break;
                }

                event_arg.input_files.Add(check_file);
                event_arg.msg.Add("수신중");

                if (event_use_)
                    EventControl.Instance().SendEvent(event_arg);

                prev_size = check_file.Length;

                Thread.Sleep(10 * 1000);

            } while (operate_);

            if (FileSizeCheckCompleted != null && operate_ == true)
                FileSizeCheckCompleted(this, null);

        }

    }
}
