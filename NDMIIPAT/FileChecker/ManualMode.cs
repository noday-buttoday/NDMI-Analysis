using FileChecker.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace FileChecker
{
    class ManualMode
    {
        private List<FileInfo> input_files_ = new List<FileInfo>();
        private String output_path_;

        public void AddInputFile(FileInfo input_file)
        {
            input_files_.Add(input_file);
        }

        public void SetOutputPath(String output_path)
        {
            output_path_ = output_path;
        }

        public void Run()
        {
            List<FileInfo> input_file_info = new List<FileInfo>();

            FileProcessEventArgs event_args = new FileProcessEventArgs(this, "FILE_COMPLETE");

            foreach (FileInfo f in input_files_)
            {
                event_args.input_files.Add(f);
                event_args.msg.Add("계산시작");
            }

            String output_path = StartEnvi(input_file_info);

            event_args.output_path = output_path;

            EventControl.Instance().SendEvent(event_args);
        }

        private void GetAwsData()
        {
        }

        private String StartEnvi(List<FileInfo> files)
        {
            string output_path = "";
            try
            {
                GetAwsData();

                ProcessStartInfo startInfo = new ProcessStartInfo("CMD.exe");
                startInfo.WindowStyle = ProcessWindowStyle.Maximized;

                DateTime nowdt = DateTime.Now;
                output_path = string.Format("{0}\\{1}", output_path_, nowdt.ToString("yyyyMMdd_HHmm"));
                System.IO.Directory.CreateDirectory(output_path);

                String cmd_command = "idl" + " -args " + files[0].FullName + " " + files[1].FullName + " " + output_path;

                startInfo.UseShellExecute = false;
                startInfo.WorkingDirectory = @"c:\SnowModule\";//scdlCdto.ScPath;
                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;

                Process p = new Process();
                p = Process.Start(startInfo);
                p.PriorityClass = ProcessPriorityClass.High;
                string pro = "snow100_ndsi_batch_mod_gui_auto";
                p.StandardInput.WriteLine(cmd_command);
                p.StandardInput.WriteLine(pro);
                p.StandardInput.WriteLine(@"exit");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return output_path;
        }
    }
}
