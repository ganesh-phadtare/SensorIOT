using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorIOT.Helpers
{
    public class AppLogger
    {
        static string _FileName = "D:/IOTLogger.txt";
        public static void LogError(Exception ex)
        {
            string logMessageToLog = string.Concat(">> Error occured : ", ex.Message, Environment.NewLine, ex.StackTrace);
            File.AppendAllText(_FileName, logMessageToLog);
        }

        public static void LogTimer(Stopwatch watch)
        {
            string logMessageToLog = string.Concat("Time Taken : ", watch.ElapsedMilliseconds, " MilliSeconds");
            File.AppendAllText(_FileName, logMessageToLog);

        }
    }
}
