using System;

namespace ErrorLogViewerLibrary.Logs
{
    public sealed class LogEntry
    {
        private readonly ILogDisplay _loggerDisplay;
        
        public LogEntryType Type { get; set; }
        public string Program { get; set; }
        public uint AppNumber { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }

        [Obsolete("Provided only for S+ compatibility", false)]
        public LogEntry() { }

        public LogEntry(ILogDisplay loggerDisplay)
        {
            _loggerDisplay = loggerDisplay;
        }

        public override string ToString()
        {
            return _loggerDisplay.Format(this);
        }
    }
}