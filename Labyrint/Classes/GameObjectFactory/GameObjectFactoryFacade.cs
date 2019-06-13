using LogSystem;
using System;
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

        /// <summary>
        /// Requests a gameObject.
        /// </summary>
        /// <param name="wantToGet">The buildertype of the gameobject.</param>
        /// <param name="fromLeft">The fromLeft location given to the gameobject.</param>
        /// <param name="fromTop">The fromTop location given to the gameobject.</param>
        /// <param name="value">Other variables given to the builder required to build an object. The actual contents differs from object to object.</param>
        /// <returns></returns>
        public static GameObject GetGameObject(string wantToGet, float fromLeft = 0, float fromTop = 0, object value = null)
        {
            return factory.GetGameObject(wantToGet, fromLeft, fromTop, value);
        }

        /// <summary>
        /// Give an object back to the pool.
        /// </summary>
        /// <param name="target">The object you are giving back.</param>
        public static void ReturnGameObject(GameObject target)
        {
            factory.ReturnGameObject(target);
        }

        /// <summary>
        /// Counts the avaibale gameObjects in the pool.
        /// </summary>
        /// <returns>The amount of objects available.</returns>
        public static int CountAvailable()
        {
            return factory.countAvailable();
        }
    }
}
