using System;
using ErrorLogViewerLibrary.SimplPlus;

namespace ErrorLogViewerLibrary.Logs
{
    /// <inheritdoc />
    /// <summary>
    /// Log entry array for SIMPL+ compatibility.
    /// </summary>
    public sealed class LogEntryArray : CrestronArray<LogEntry>
    {
        [Obsolete("Provided only for S+ compatibility", false)]
        public LogEntryArray() { }
        public LogEntryArray(LogEntry[] array) : base(array) { }
    }
}