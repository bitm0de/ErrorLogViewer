using System.Text;

namespace ErrorLogViewerLibrary.Logs
{
    public sealed class ErrorLogDisplay : ILogDisplay
    {
        public bool EnableShowType { get; set; }
        public bool EnableShowProgram { get; set; }
        public bool EnableShowAppNumber { get; set; }
        public bool EnableShowTimestamp { get; set; }

        public string Format(LogEntry entry)
        {
            var sb = new StringBuilder(255);
            if (EnableShowType)
                sb.Append(entry.Type + ": ");
            if (EnableShowProgram)
                sb.Append(entry.Program + " ");
            if (EnableShowAppNumber)
                sb.Append("[App " + entry.AppNumber + "] ");
            if (EnableShowTimestamp)
                sb.Append("# " + entry.Timestamp.ToString("yyyy-MM-dd hh:mm:ss") + " ");

            sb.Append("# " + entry.Message);
            return sb.ToString().Trim(new [] { ' ', '#' });
        }
    }
}