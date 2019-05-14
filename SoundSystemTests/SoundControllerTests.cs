using LogSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoundSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundSystem.Tests
{
    [TestClass()]
    public class SoundControllerTests
    {
        [TestMethod()]
        public void AddSoundTest()
        {
            //Log.Debug("Last message");
            SoundController sc = new SoundController();
            sc.AddSound("test", "test");

            LogMessage message = Log.GetLogMessages().Last();

            Log.Debug("Last message: "+message.GetMessage());
            Assert.AreEqual("Sound not found (test): test", message.GetMessage());

            Assert.AreNotEqual("test", message.GetMessage());

        }
    }
}