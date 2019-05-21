using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LogSystem
{
    public static class Log
    {
        private static List<ILogObserver> observers = new List<ILogObserver>();
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
                UpdateObservers(logMessage);
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
                UpdateObservers(logMessage);
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
                UpdateObservers(logMessage);
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
                UpdateObservers(logMessage);
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
                UpdateObservers(logMessage);
            }
        }

        /// <summary>
        /// Subscribe a class with the ILogObserver interface to the Log class so it recieves all incoming LogMessages
        /// </summary>
        /// <param name="observer">Class with the ILogObserver</param>
        public static void Subscribe(ILogObserver observer)
        {
            // Check if the observer is already subscribed
            if (!observers.Contains(observer))
            {
                // Subscribe the observer to the log by adding it to the list
                observers.Add(observer);
            }
        }

        /// <summary>
        /// Unsubscribe a class with the ILogObserver interface from the Log class so it does nolonger recieve all incoming LogMessages
        /// </summary>
        /// <param name="observer">Class with the ILogObserver</param>
        public static void Unsubscribe(ILogObserver observer)
        {
            // Check if the observer is already subscribed
            if (observers.Contains(observer))
            {
                // Unsubscribe the observer to the log by removing it to the list
                observers.Remove(observer);
            }
        }

        /// <summary>
        /// Check if the class is subscribed to the Log
        /// </summary>
        /// <param name="observer">The class that needs to be checked</param>
        /// <returns>Returns whether the class is subscribed to the Log </returns>
        public static bool IsSubscribed(ILogObserver observer)
        {
            // Check if the observer is subscribed
            if (observers.Contains(observer))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Pass the LogMessage to all subscribed observers
        /// </summary>
        /// <param name="logMessage">The message that will be sent to all subscribed observers</param>
        private static void UpdateObservers(LogMessage logMessage)
        {
            foreach (ILogObserver observer in observers)
            {
                observer.LogUpdate(logMessage);
            }
        }
    }
}
