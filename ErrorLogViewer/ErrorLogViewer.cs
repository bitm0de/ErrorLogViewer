using System;
using System.Linq;
using ErrorLogViewerLibrary.Logs;

/* --------------------------------------------  GENERIC SIMPL+ TYPE HELPER ALIASES  -------------------------------------------- */
using STRING = System.String;                          // string = STRING
using SSTRING = Crestron.SimplSharp.SimplSharpString;  // SimplSharpString = STRING (used to interface with SIMPL+)
using INTEGER = System.UInt16;                         // ushort = INTEGER (unsigned)
using SIGNED_INTEGER = System.Int16;                   // short = SIGNED_INTEGER
using SIGNED_LONG_INTEGER = System.Int32;              // int = SIGNED_LONG_INTEGER
using LONG_INTEGER = System.UInt32;                    // uint = LONG_INTEGER (unsigned)
/* ------------------------------------------------------------------------------------------------------------------------------ */

namespace ErrorLogViewerLibrary
{
    public delegate void LogEntryArrayDelegate(LogEntryArray data);

    public sealed class ErrorLogViewer
    {
        private LogEntryArray _cachedLogEntries;
        private readonly ErrorLogHandler _errorLogHandler = new ErrorLogHandler();

        public LogEntryArrayDelegate LogsUpdated { get; set; }

        public ErrorLogViewer()
        {
            _errorLogHandler.ErrorLogDisplayChanged += (s, e) =>
            {
                if (_cachedLogEntries != null)
                    OnLogsUpdated(_cachedLogEntries);
            };
        }

        public void SetMaxLogs(INTEGER maxLogs)
        {
            _errorLogHandler.SetMaxLogs(maxLogs);
        }

        public void GetAllLogs(INTEGER appNumber)
        {
            var data = new LogEntryArray(_errorLogHandler.GetFilteredProgramLogs(appNumber, entry => true).ToArray());
            OnLogsUpdated(data);
            _cachedLogEntries = data;
        }

        public void GetLogsByTypeFilter(INTEGER appNumber, INTEGER typeMaskFilter)
        {
            LogEntryType entryTypeMask = (LogEntryType)typeMaskFilter;
            var data = new LogEntryArray(_errorLogHandler.GetFilteredProgramLogs(appNumber, entry => entryTypeMask.HasFlag(entry.Type)).ToArray());

            OnLogsUpdated(data);
            _cachedLogEntries = data;
        }

        public void GetLogsByProgramFilter(INTEGER appNumber, STRING programNameFilter)
        {
            programNameFilter = programNameFilter.Trim();
            if (programNameFilter.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                programNameFilter = programNameFilter.Remove(programNameFilter.LastIndexOf('.'), 4);

            if (_cachedLogEntries == null)
                return;

            var data = new LogEntryArray(_cachedLogEntries.Array.Where(
                entry => entry.Program.StartsWith(programNameFilter, StringComparison.OrdinalIgnoreCase)).ToArray());

            OnLogsUpdated(data);
        }

        public void GetLogsByMessageFilter(INTEGER appNumber, STRING messageFilter)
        {
            messageFilter = messageFilter.Trim();

            if (_cachedLogEntries == null)
                return;

            var data = new LogEntryArray(_cachedLogEntries.Array.Where(
                entry => entry.Message.IndexOf(messageFilter, StringComparison.OrdinalIgnoreCase) != -1).ToArray());

            OnLogsUpdated(data);
        }

        public void EnableShowLogType(INTEGER state) { _errorLogHandler.EnableShowLogType(state != 0); }
        public void EnableShowLogProgram(INTEGER state) { _errorLogHandler.EnableShowLogProgram(state != 0); }
        public void EnableShowLogAppNumber(INTEGER state) { _errorLogHandler.EnableShowLogAppNumber(state != 0); }
        public void EnableShowLogTimestamp(INTEGER state) { _errorLogHandler.EnableShowLogTimestamp(state != 0); }

        private void OnLogsUpdated(LogEntryArray data)
        {
            var handler = LogsUpdated;
            if (handler != null) handler.Invoke(data);
        }
    }
}