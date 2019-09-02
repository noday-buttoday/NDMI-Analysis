using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace FileChecker
{
    public class AlgorithmControl
    {
        public String StartNDSIModule(List<FileInfo> files, String output_path)
        {
            string output_path_date = "";
            try
            {
                String aws_output_file_name = "Aws_Data_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".txt";

                ProcessStartInfo startInfo = new ProcessStartInfo("CMD.exe");
                startInfo.WindowStyle = ProcessWindowStyle.Maximized;

                DateTime nowdt = DateTime.Now;
                output_path_date = string.Format("{0}\\{1}", output_path, nowdt.ToString("yyyyMMdd_HHmm"));
                System.IO.Directory.CreateDirectory(output_path_date);

                String cmd_command = "idl" + " -args " + files[0].FullName + " " + files[1].FullName + " " + output_path_date;

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

            return output_path_date;
        }

        public String StartFSCModulie(List<FileInfo> files)
        {
            string output_path_date = "";

            return output_path_date;
        }
    }
}
