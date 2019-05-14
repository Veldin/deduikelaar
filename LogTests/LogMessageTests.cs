using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogSystem;

namespace LogSystem.Tests
{
    [TestClass()]
    public class LogMessageTests
    {
        [TestMethod()]
        public void LogMessageTest()
        {
            LogMessage testMessage = new LogMessage("Hallo", ELoglevel.Debug, "14/05/2019 11:00:00,236", "LogMessageTest.cs", "GetMessageTest", 12);

            // Test Getters
            Assert.AreEqual(testMessage.GetMessage(), "Hallo");
            Assert.AreEqual(testMessage.GetLogLevel(), ELoglevel.Debug);
            Assert.AreEqual(testMessage.GetCreationTime(), "14/05/2019 11:00:00,236");
            Assert.AreEqual(testMessage.GetFilePath(), "LogMessageTest.cs");
            Assert.AreEqual(testMessage.GetMemberName(), "GetMessageTest");
            Assert.AreEqual(testMessage.GetLineNumber(), 12);
        }
    }
}