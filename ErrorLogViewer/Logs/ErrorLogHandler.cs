using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Crestron.SimplSharp;

namespace ErrorLogViewerLibrary.Logs
{
    internal sealed class ErrorLogHandler
    {
        private int _maxLogs = 500;

        public event EventHandler ErrorLogDisplayChanged;
        private readonly ErrorLogDisplay _errorLogDisplay = new ErrorLogDisplay();

        // Note: console command prefixes numbers before each message but extracting from the logfile itself does not.
        private const string StderrJunkPattern = @"(?<!^)(?<=[^\d])[\s*^\n]\d+\. Error: (?:SimplSharpPro|splusmanagerapp)\.exe(?: \[App \d+?])? # \d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}  # ?";
        private readonly Regex _regexFix = new Regex(StderrJunkPattern, RegexOptions.Compiled | RegexOptions.Multiline);
        private readonly Regex _regexCurrentProgramLogs;

        public ErrorLogHandler()
        {
            _regexCurrentProgramLogs = new Regex(GetAppNumberRegexPattern(InitialParametersClass.ApplicationNumber),
                RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
        }

        /// <summary>
        /// Sets the max number of most recent logs to retrieve from the file.
        /// </summary>
        /// <param name="maxLogs">Maximum number of logs.</param>
        public void SetMaxLogs(int maxLogs)
        {
            _maxLogs = maxLogs;
        }

        /// <summary>
        /// Removes all of the junk standard error buffered data getting injected in the middle of lines
        /// in the logfile.
        /// </summary>
        /// <param name="content">Logfile contents.</param>
        /// <returns>Fixed logfile contents.</returns>
        private string FixLog(string content)
        {
            return _regexFix.Replace(content, string.Empty);
        }

        /// <summary>
        /// Regex pattern for the specified app number.
        /// </summary>
        /// <param name="appNumber">App number.</param>
        /// <returns>Regex pattern.</returns>
        private static string GetAppNumberRegexPattern(uint appNumber)
        {
            return @"^\s*\d+\. (?<type>Notice|Ok|Info|Warning|Error|Fatal): (?<program>[\w-]+\.exe) \[App " + appNumber +
                   @"\] # (?<timestamp>\d{4}(-\d{2}){2} \d{2}(:\d{2}){2})  #(?<message>.+\n*((\s+at.+\n)+)?)";
        }

        public IEnumerable<LogEntry> GetFilteredProgramLogs(uint appNumber, Predicate<LogEntry> filterPredicate)
        {
            // note: most recent logs are at the end...
            // return GetProgramErrorLog(appNumber).OrderByDescending(e => e.Timestamp).Take(_maxLogs).Where(e => filterPredicate(e));
            return GetProgramErrorLogs(appNumber).Reverse().Take(_maxLogs).Where(e => filterPredicate(e));
        }
        
        private IEnumerable<LogEntry> GetProgramErrorLogs(uint appNumber)
        {
            string logfileContents = null;
            StringBuilder sb = null;

            string consoleResponse = string.Empty;
            try
            {
                if (CrestronConsole.SendControlSystemCommand("ERR\r", ref consoleResponse))
                    sb = new StringBuilder(consoleResponse);

            }
            catch (Exception) { /* ignore */ }

            if (sb != null)
                logfileContents = FixLog(sb.ToString());

            return ParseLogEntries(appNumber, logfileContents);
        }
        
        private IEnumerable<LogEntry> ParseLogEntries(uint appNumber, string logfileContents)
        {
            if (!string.IsNullOrEmpty(logfileContents))
            {
                var matches = appNumber == InitialParametersClass.ApplicationNumber
                    ? _regexCurrentProgramLogs.Matches(logfileContents)
                    : Regex.Matches(logfileContents, GetAppNumberRegexPattern(appNumber), RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
                
                foreach (Match m in matches)
                {
                    LogEntry entry = null;
                    try
                    {
                        entry = new LogEntry(_errorLogDisplay)
                        {
                            Type = (LogEntryType)Enum.Parse(typeof(LogEntryType), m.Groups["type"].Value, true),
                            Program = m.Groups["program"].Value,
                            AppNumber = appNumber,
                            Timestamp = DateTime.Parse(m.Groups["timestamp"].Value),
                            Message = m.Groups["message"].Value
                        };
                    }
                    catch (Exception)
                    {
                        /* ignore */
                    }

                    if (entry != null)
                        yield return entry;
                }
            }
        }

        public void EnableShowLogType(bool state)
        {
            _errorLogDisplay.EnableShowType = state;
            OnErrorLogDisplayChanged();
        }

        public void EnableShowLogProgram(bool state)
        {
            _errorLogDisplay.EnableShowProgram = state;
            OnErrorLogDisplayChanged();
        }

        public void EnableShowLogAppNumber(bool state)
        {
            _errorLogDisplay.EnableShowAppNumber = state;
            OnErrorLogDisplayChanged();
        }

        public void EnableShowLogTimestamp(bool state)
        {
            _errorLogDisplay.EnableShowTimestamp = state;
            OnErrorLogDisplayChanged();
        }

        private void OnErrorLogDisplayChanged()
        {
            var handler = ErrorLogDisplayChanged;
            if (handler != null) handler.Invoke(this, EventArgs.Empty);
        }
    }
}