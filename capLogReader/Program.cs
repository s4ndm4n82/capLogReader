using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace capLogReader
{
    class Program
    {
        static void Main(string[] args)
        {
            //Get the path for the settings.ini file.
            string workingFolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            string fileName = "settings.ini";
            string[] settingsFile =Directory.GetFiles(workingFolder, fileName);

            //Reading the settings.ini file to read the path to CAP log folder.
            string[] logPath = File.ReadAllLines(settingsFile[0]); //Read the enteri file line wise.

            //Global string for the clean path.
            string logSysPath = string.Empty;

            foreach (string line in logPath)
            {
                string stringLine = line.ToString();

                if (stringLine.Contains("logSystemPath")) //Mathing text in the ini file.
                {
                    //Replaces the unwanted cracters in the log file path rad from the ini file.
                    logSysPath = stringLine.Replace( "logSystemPath=", "").Replace("\\", "\\\\").Replace("\"", "");                    
                }
                
            }

            //Read the log file and waits for changes to diplay the next line
            var wh = new AutoResetEvent(false);
            var fsw = new FileSystemWatcher(".");
            fsw.Filter = logSysPath +"\\ServiceLog_CAPS2_20211026.txt";
            fsw.EnableRaisingEvents = true;
            fsw.Changed += (s, e) => wh.Set();

            var fs = new FileStream(logSysPath +"\\ServiceLog_CAPS2_20211026.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var sr = new StreamReader(fs))
            {
                var s = "";
                while (true)
                {
                    s = sr.ReadLine();
                    if (s != null)
                        Console.WriteLine(s);
                    else
                        wh.WaitOne(1000);
                }
            }

            wh.Close();
        }
    }
}
