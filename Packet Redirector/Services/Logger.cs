using System;
using System.IO;

namespace PacketRedirector.Services
{
    public static class Logger
    {
        private static readonly string LogDir = Path.Combine(AppContext.BaseDirectory, "logs");
        private static readonly string EventLogPath = Path.Combine(LogDir, "events.log");
        private static readonly string ErrorLogPath = Path.Combine(LogDir, "errors.log");

        static Logger()
        {
            Directory.CreateDirectory(LogDir);
        }

        public static void LogEvent(string message)
        {
            var line = $"[{DateTime.Now:HH:mm:ss}] {message}";
            File.AppendAllText(EventLogPath, line + Environment.NewLine);
        }

        public static void LogError(Exception ex)
        {
            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex.Message}{Environment.NewLine}{ex.StackTrace}";
            File.AppendAllText(ErrorLogPath, line + Environment.NewLine + Environment.NewLine);
        }
    }
}