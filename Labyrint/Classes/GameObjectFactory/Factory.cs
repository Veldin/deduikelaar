using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameObjectFactory
{
    public class Factory
    {
        //The pool contains the Objects that are not in use.
        private Pool pool;

        //The builder is used to change gameObjects to the onces that are requested
        private Builder builder;

        public Factory()
        {
            pool = new Pool();
            builder = new Builder();
        }

        /// <summary>
        /// Gets the gameObject from the pool and transform it with the builder.
        /// </summary>
        /// <param name="wantToGet">The argument for the builder</param>
        /// <param name="fromLeft">the location fromLeft</param>
        /// <param name="fromTop">the location fromTop</param>
        /// <param name="value">extra value parameter for the builder</param>
        /// <returns></returns>
        public GameObject GetGameObject(string wantToGet, float fromLeft, float fromTop, float fromBehind, object value = null)
        {
            //GameObject target = pool.GetGameObject();
            GameObject target = new GameObject();
            builder.TransformGameObject(target, wantToGet, fromLeft, fromTop, fromBehind, value);

            return target;
        }

        /// <summary>
        /// Returns a GameObject to the pool
        /// </summary>
        /// <param name="target">The gameObject to return</param>
        public void ReturnGameObject(GameObject target)
        {
            target.Reset();
            pool.ReturnGameObject(target);
        }

        /// <summary>
        /// Counts the available gameObjects in the pool
        /// </summary>
        /// <returns></returns>
        public int CountAvailable()
        {
            return pool.countAvailable();
        }
    }
}
