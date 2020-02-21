using System;

namespace ErrorLogViewerLibrary.Logs
{
    [Flags]
    public enum LogEntryType
    {
        Notice  = 1,
        Ok      = 2,
        Info    = 4,
        Warning = 8,
        Error   = 16,
        Fatal   = 32
    }

}