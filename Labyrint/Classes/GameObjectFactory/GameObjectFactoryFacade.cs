﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameObjectFactory
{
    public static class GameObjectFactoryFacade
    {
        private static Factory factory;

        public static bool innit()
        {
            factory = new Factory();

            return true;
        }

        public static GameObject GetGameObject(string wantToGet, float fromLeft, float fromTop)
        {
            return factory.GetGameObject(wantToGet, fromLeft, fromTop);
        }

        public static void ReturnGameObject(GameObject target)
        {
            factory.ReturnGameObject(target);
        }

        public static int countAvailable()
        {
            return factory.countAvailable();
        }
    }
}
