using System;

namespace LogSystem
{
    public class LogMessageFactory
    {
        public LogMessageFactory()
        {
        }

        /// <summary>
        /// Create a LogMessage
        /// </summary>
        /// <param name="message">The actual message of the LogMessage</param>
        /// <param name="logLevel">This shows the level of importance of the message like debug, warning or error</param>
        /// <param name="filePath">The file path of the file where the message is called</param>
        /// <param name="memberName">The memberName or methodName of where the message is called</param>
        /// <param name="lineNumber">The lineNumber of where the message is called</param>
        /// <returns>The LogMessage Object</returns>
        public LogMessage CreateMessage(string message, ELoglevel logLevel, string filePath, string memberName, int lineNumber)
        {  
            // Get current time
            string creationTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff");

            // Write message into the console
            string[] split = filePath.Split('\\');
            string className = split[split.Length - 1];
            Console.WriteLine(className + " " + memberName + " Line: "+ lineNumber + " " +logLevel.ToString() + ": " + message);

            // Return the message
            return new LogMessage(message, logLevel, creationTime, filePath, memberName, lineNumber);
        }
    }
}
