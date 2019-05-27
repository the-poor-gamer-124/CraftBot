using NLog;
using NLog.Layouts;
using System.Text;

namespace CraftBot.Base
{
    internal class BetterLayout : Layout
    {
        private int LoggerNameMax = 30;

        protected override string GetFormattedMessage(LogEventInfo logEvent)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"[{logEvent.TimeStamp.ToString()}] ");

            string loggerName = logEvent.LoggerName;
            if (loggerName.Length > LoggerNameMax)
            {
                loggerName = "..." + loggerName.Substring(loggerName.Length - LoggerNameMax + 3, LoggerNameMax - 3);
            }

            stringBuilder.Append(loggerName.PadRight(LoggerNameMax) + ": ");
            stringBuilder.Append(logEvent.Message);

            if (logEvent.Exception != null)
            {
                stringBuilder.Append("\n" + logEvent.Exception.ToString());
            }

            return stringBuilder.ToString();
        }
    }
}