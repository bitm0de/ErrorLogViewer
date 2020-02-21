namespace ErrorLogViewerLibrary.Logs
{
    public interface ILogDisplay
    {
        bool EnableShowType { get; set; }
        bool EnableShowProgram { get; set; }
        bool EnableShowAppNumber { get; set; }
        bool EnableShowTimestamp { get; set; }

        string Format(LogEntry entry);
    }
}