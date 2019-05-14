namespace LogSystem
{
    public class LogMessage
    {
        private string message;                 // The actual message of the LogMessage
        private ELoglevel logLevel;             // This shows the level of importance of the message like debug, warning or error
        private string creationTime;            // This shows the time and date of when the message is created
        private string filePath;                // This shows the file path of the file where the message is called
        private string memberName;              // This shows the method name of the method where in the message is called
        private int lineNumber;                 // This shows the line number on which the message is called

        public LogMessage(string message, ELoglevel logLevel, string creationTime, string filePath, string memberName, int lineNumber)
        {
            // Initializing the class variables
            this.message        = message;
            this.logLevel       = logLevel;
            this.creationTime   = creationTime;
            this.filePath       = filePath;
            this.memberName     = memberName;
            this.lineNumber     = lineNumber;
        }

        /*********************************************************
         * Getters
         * ******************************************************/
        /// <summary>
        /// This returns the message
        /// </summary>
        /// <returns>The actual message of the LogMessage</returns>
        public string GetMessage()
        {
            return message;
        }

        /// <summary>
        /// This returns the LogLevel
        /// </summary>
        /// <returns>This shows the level of importance of the message like debug, warning or error</returns>
        public ELoglevel GetLogLevel()
        {
            return logLevel;
        }

        /// <summary>
        /// This returns the creationTime of the message
        /// </summary>
        /// <returns>This shows the time and date of when the message is created</returns>
        public string GetCreationTime()
        {
            return creationTime;
        }

        /// <summary>
        /// This returns the filePath
        /// </summary>
        /// <returns>This shows the file path of the file where the message is called</returns>
        public string GetFilePath()
        {
            return filePath;
        }

        /// <summary>
        /// This returns the MemberName
        /// </summary>
        /// <returns>This shows the method name of the method where in the message is called</returns>
        public string GetMemberName()
        {
            return memberName;
        }

        /// <summary>
        /// This returns the lineNumber
        /// </summary>
        /// <returns>This shows the line number on which the message is called</returns>
        public int GetLineNumber()
        {
            return lineNumber;
        }
    }
}