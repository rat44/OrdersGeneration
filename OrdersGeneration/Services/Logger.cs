using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersGeneration.Services
{
    public class Logger
    {
        public static void GenLog(string message, bool consoleVIew = true, string path = "")
        {
            var time = DateTime.Now.ToShortTimeString();
            var log = time + " Log message: " + message;
            if (consoleVIew)
                Console.WriteLine(log);
            else
            {
                string logFilePath = path + "\\Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
                FileInfo logFileInfo = new FileInfo(logFilePath);
                DirectoryInfo logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
                if (!logDirInfo.Exists) logDirInfo.Create();
                using (FileStream fileStream = new FileStream(logFilePath, FileMode.Append))
                {
                    using (StreamWriter swlog = new StreamWriter(fileStream))
                    {
                        swlog.WriteLine(log);
                    }
                }
            }
        }
    }
}
