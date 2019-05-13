namespace LogSystem
{
    /// <summary>
    /// This enum describes the importance of a logMessage
    /// </summary>
    public enum ELoglevel
    {
        Unknown,        // Use this when the loglevel is null or unknown
        Debug,          // Use this to inform the progrommer
        Info,           // Use this to inform the user
        Warning,        // Use this if something went wrong but the Engine can keep running
        Error           // Use this if something is wrong and the Engine breaks
    };
}
