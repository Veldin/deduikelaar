using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogSystem;
using System.Collections.Generic;
using System.Linq;


namespace LogSystem.Tests
{
    [TestClass()]
    public class LogTests
    {
        [TestMethod()]
        public void GetLogMessagesTest()
        {
            List<LogMessage> logMessages = Log.GetLogMessages();
            Assert.IsTrue(logMessages.Count == 0);
            Log.Debug("Test");
            logMessages = Log.GetLogMessages();
            Assert.IsFalse(logMessages.Count == 0);
        }

        [TestMethod()]
        public void MessageTest()
        {
            Log.Message("This is a test message", ELoglevel.Info);
            LogMessage testMessage = Log.GetLogMessages().Last();
            
            // Test Getters
            Assert.AreEqual(testMessage.GetMessage(), "This is a test message");
            Assert.AreEqual(testMessage.GetLogLevel(), ELoglevel.Info);
            Assert.IsNotNull(testMessage.GetCreationTime());
            Assert.IsNotNull(testMessage.GetFilePath());
            Assert.AreEqual(testMessage.GetMemberName(), "MessageTest");
            Assert.AreEqual(testMessage.GetLineNumber(), 25);
        }

        [TestMethod()]
        public void DebugTest()
        {
            Log.Debug("This is a test message");
            LogMessage testMessage = Log.GetLogMessages().Last();

            // Test Getters
            Assert.AreEqual(testMessage.GetMessage(), "This is a test message");
            Assert.AreEqual(testMessage.GetLogLevel(), ELoglevel.Debug);
            Assert.IsNotNull(testMessage.GetCreationTime());
            Assert.IsNotNull(testMessage.GetFilePath());
            Assert.AreEqual(testMessage.GetMemberName(), "DebugTest");
            Assert.AreEqual(testMessage.GetLineNumber(), 40);
        }

        [TestMethod()]
        public void InfoTest()
        {
            Log.Info("This is a test message");
            LogMessage testMessage = Log.GetLogMessages().Last();

            // Test Getters
            Assert.AreEqual(testMessage.GetMessage(), "This is a test message");
            Assert.AreEqual(testMessage.GetLogLevel(), ELoglevel.Info);
            Assert.IsNotNull(testMessage.GetCreationTime());
            Assert.IsNotNull(testMessage.GetFilePath());
            Assert.AreEqual(testMessage.GetMemberName(), "InfoTest");
            Assert.AreEqual(testMessage.GetLineNumber(), 55);
        }

        [TestMethod()]
        public void WarningTest()
        {
            Log.Warning("This is a test message");
            LogMessage testMessage = Log.GetLogMessages().Last();

            // Test Getters
            Assert.AreEqual(testMessage.GetMessage(), "This is a test message");
            Assert.AreEqual(testMessage.GetLogLevel(), ELoglevel.Warning);
            Assert.IsNotNull(testMessage.GetCreationTime());
            Assert.IsNotNull(testMessage.GetFilePath());
            Assert.AreEqual(testMessage.GetMemberName(), "WarningTest");
            Assert.AreEqual(testMessage.GetLineNumber(), 70);
        }

        [TestMethod()]
        public void ErrorTest()
        {
            Log.Error("This is a test message");
            LogMessage testMessage = Log.GetLogMessages().Last();

            // Test Getters
            Assert.AreEqual(testMessage.GetMessage(), "This is a test message");
            Assert.AreEqual(testMessage.GetLogLevel(), ELoglevel.Error);
            Assert.IsNotNull(testMessage.GetCreationTime());
            Assert.IsNotNull(testMessage.GetFilePath());
            Assert.AreEqual(testMessage.GetMemberName(), "ErrorTest");
            Assert.AreEqual(testMessage.GetLineNumber(), 85);
        }
    }
}