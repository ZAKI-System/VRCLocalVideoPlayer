using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCLocalVideoPlayer
{
    internal class Logger
    {
        private static readonly List<KeyValuePair<string, Logger.LogLevel>> logs = new List<KeyValuePair<string, Logger.LogLevel>>();

        public enum LogLevel
        {
            Trace,
            Debug,
            Info,
            Warn,
            Error,
            Fatal
        }

        public static event Action<KeyValuePair<string, Logger.LogLevel>> LogAdded;

        public static List<KeyValuePair<string, Logger.LogLevel>> GetAllLogs()
        {
            return new List<KeyValuePair<string, Logger.LogLevel>>(Logger.logs);
        }

        public static void LogInfo(string message)
        {
            Logger.AddLog(message, Logger.LogLevel.Info);
        }

        public static void LogWarn(string message)
        {
            Logger.AddLog(message, Logger.LogLevel.Warn);
        }

        public static void LogError(string message)
        {
            Logger.AddLog(message, Logger.LogLevel.Error);
        }

        private static void AddLog(string message, Logger.LogLevel logLevel)
        {
            var log = new KeyValuePair<string, Logger.LogLevel>($"[{DateTime.Now.ToString("yyyy/MM/dd_HH:mm:ss")}]: {message}", logLevel);
            Logger.logs.Add(log);
            Logger.LogAdded?.Invoke(log);
        }
    }
}
