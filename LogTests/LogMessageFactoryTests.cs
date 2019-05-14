using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogSystem.Tests
{
    [TestClass()]
    public class LogMessageFactoryTests
    {
        [TestMethod()]
        public void CreateMessageTest()
        {
            LogMessageFactory factory = new LogMessageFactory();
            LogMessage testMessage = factory.CreateMessage("TestMessage", ELoglevel.Info, "LogMessageFactoryTests.cs", "CreateMessageTest", 19);

            Assert.IsNotNull(testMessage);
            
            // Test Getters
            Assert.AreEqual(testMessage.GetMessage(), "TestMessage");
            Assert.AreEqual(testMessage.GetLogLevel(), ELoglevel.Info);
            Assert.IsNotNull(testMessage.GetCreationTime());
            Assert.AreEqual(testMessage.GetFilePath(), "LogMessageFactoryTests.cs");
            Assert.AreEqual(testMessage.GetMemberName(), "CreateMessageTest");
            Assert.AreEqual(testMessage.GetLineNumber(), 19);
        }
    }
}