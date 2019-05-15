﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LogSystem
{
     public static class Log
    {
        private static LogMessageFactory factory    = new LogMessageFactory();              // This is the LogMessageFactory which handles the creation of all the LogMessages
        private static List<LogMessage> logMessages = new List<LogMessage>();               // This is the list with all the LogMessages

        /// <summary>
        /// This method returns all the LogMessages
        /// </summary>
        /// <returns>All LogMessages</returns>
        public static List<LogMessage> GetLogMessages()
        {
            return logMessages;
        }

        /// <summary>
        /// Create a LogMessage. Do not fill in the filePath, memberName and lineNumber!
        /// </summary>
        /// <param name="message">The Message</param>
        /// <param name="logLevel">The LogLevel like Debug or Info</param>
        /// <param name="filePath">Do not fill this in. This will be filled in automatically.</param>
        /// <param name="memberName">Do not fill this in. This will be filled in automatically.</param>
        /// <param name="lineNumber">Do not fill this in. This will be filled in automatically.</param>
        public static void Message(object message, ELoglevel logLevel, [CallerFilePath] string filePath = null, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0)
        {
            // Create the logMessage
            LogMessage logMessage = factory.CreateMessage(message.ToString(), logLevel, filePath, memberName, lineNumber);

            // Add the logMessage to the logMessages if it isn't null
            if (logMessage != null)
            {
                logMessages.Add(logMessage);
            }
        }

        /// <summary>
        /// Create a LogMessage. Do not fill in the filePath, memberName and lineNumber!
        /// </summary>
        /// <param name="message">The Message</param>
        /// <param name="filePath">Do not fill this in. This will be filled in automatically.</param>
        /// <param name="memberName">Do not fill this in. This will be filled in automatically.</param>
        /// <param name="lineNumber">Do not fill this in. This will be filled in automatically.</param>
        public static void Debug(object message, [CallerFilePath] string filePath = null, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0)
        {
            // Create the logMessage
            LogMessage logMessage = factory.CreateMessage(message.ToString(), ELoglevel.Debug, filePath, memberName, lineNumber);

            // Add the logMessage to the logMessages if it isn't null
            if (logMessage != null)
            {
                logMessages.Add(logMessage);
            }
        }

        /// <summary>
        /// Create a LogMessage. Do not fill in the filePath, memberName and lineNumber!
        /// </summary>
        /// <param name="message">The Message</param>
        /// <param name="filePath">Do not fill this in. This will be filled in automatically.</param>
        /// <param name="memberName">Do not fill this in. This will be filled in automatically.</param>
        /// <param name="lineNumber">Do not fill this in. This will be filled in automatically.</param>
        public static void Info(object message, [CallerFilePath] string filePath = null, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0)
        {
            // Create the logMessage
            LogMessage logMessage = factory.CreateMessage(message.ToString(), ELoglevel.Info, filePath, memberName, lineNumber);

            // Add the logMessage to the logMessages if it isn't null
            if (logMessage != null)
            {
                logMessages.Add(logMessage);
            }
        }

        /// <summary>
        /// Create a LogMessage. Do not fill in the filePath, memberName and lineNumber!
        /// </summary>
        /// <param name="message">The Message</param>
        /// <param name="filePath">Do not fill this in. This will be filled in automatically.</param>
        /// <param name="memberName">Do not fill this in. This will be filled in automatically.</param>
        /// <param name="lineNumber">Do not fill this in. This will be filled in automatically.</param>
        public static void Warning(object message, [CallerFilePath] string filePath = null, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0)
        {
            // Create the logMessage
            LogMessage logMessage = factory.CreateMessage(message.ToString(), ELoglevel.Warning, filePath, memberName, lineNumber);

            // Add the logMessage to the logMessages if it isn't null
            if (logMessage != null)
            {
                logMessages.Add(logMessage);
            }
        }

        /// <summary>
        /// Create a LogMessage. Do not fill in the filePath, memberName and lineNumber!
        /// </summary>
        /// <param name="message">The Message</param>
        /// <param name="filePath">Do not fill this in. This will be filled in automatically.</param>
        /// <param name="memberName">Do not fill this in. This will be filled in automatically.</param>
        /// <param name="lineNumber">Do not fill this in. This will be filled in automatically.</param>
        public static void Error(object message, [CallerFilePath] string filePath = null, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0)
        {
            // Create the logMessage
            LogMessage logMessage = factory.CreateMessage(message.ToString(), ELoglevel.Error, filePath, memberName, lineNumber);

            // Add the logMessage to the logMessages if it isn't null
            if (logMessage != null)
            {
                logMessages.Add(logMessage);
            }
        }
    }
}
