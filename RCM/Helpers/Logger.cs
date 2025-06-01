using System;
using System.IO;
using System.Text;

namespace RCM.Helpers
{
    public class Logger
    {
        private static readonly string filePath = "nalabs_error.log";

        public static void LogError(Exception ex, string context = null)
        {
            Log("ERROR", ex.Message, context, ex.StackTrace);
        }

        public static void LogWarning(string message, string context = null)
        {
            Log("WARNING", message, context);
        }

        public static void LogInfo(string message, string context = null)
        {
            Log("INFO", message, context);
        }

        private static void Log(string level, string message, string context, string stackTrace = null)
        {
            try
            {
                var content = new StringBuilder();
                content.AppendLine($"[{level}] [{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Message: {message}");
                if (!string.IsNullOrEmpty(context))
                   content.AppendLine($"  Context: {context}");

                if (!string.IsNullOrEmpty(stackTrace))
                    content.AppendLine($"  StackTrace: {stackTrace}");

                File.AppendAllText(filePath, content.ToString());
            }
            catch
            {
                // Intentionally suppress exceptions from logger
            }
        }
    }
}
