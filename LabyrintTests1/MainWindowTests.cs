using Microsoft.VisualStudio.TestTools.UnitTesting;
using Labyrint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;

namespace GameObjectFactory.Tests
{
    [TestClass()]
    public class MainWindowTests
    {
        [TestMethod()]
        public void PickupItemsTest()
        {
            //GameObjectFactoryFacade gameObjectFactoryFacade = new GameObjectFactoryFacade();
            GameObjectFactoryFacade.innit();

            GameObject player = GameObjectFactoryFacade.GetGameObject("player", 300, 300);
            GameObject pickup = GameObjectFactoryFacade.GetGameObject("pickup", 300, 300);

            Assert.IsTrue(player.IsColliding(pickup));
        }

    }
}