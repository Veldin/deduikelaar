using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameObjectFactory
{
    public class Pool
    {
        public GameObject[] pool;

        /// <summary>
        /// Creates a pool.
        /// </summary>
        /// <param name="poolsize">The size of the pool created. Default value is 50.</param>
        public Pool(int poolsize = 50)
        {
            Populate(poolsize);
        }

        /// <summary>
        /// Get a gameObject from the pool. The place the gameObject was wil be emptyd.
        /// </summary>
        /// <returns>An gameobject from the pool.</returns>
        public GameObject GetGameObject()
        {
            //Console.WriteLine("getting");
            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i] != null)
                {
                    GameObject needle = pool[i];
                    pool[i] = null;

                    return needle;
                }
            }

            // There are no available gameObjects, make the pool bigger, and repopulate
            // double the size of the pool!

            int newPoolSize = pool.Length * 2;

            GameObject[] newpool = new GameObject[newPoolSize];

            //Copy the old array over in the new
            for (int i = 0; i < pool.Length; i++)
            {
                newpool[i] = pool[i];
            }

            //Populate the rest of the new pool
            for (int i = pool.Length - 1; i < newpool.Length; i++)
            {
                newpool[i] = new GameObject();
                ResetGameObject(newpool[i]);
            }

            //Overwrite the old pool
            this.pool = newpool;

            //Try to get an new object
            return this.GetGameObject();
        }

        /// <summary>
        /// Return a gameObject to the pool. The gameObject gets reset here too.
        /// </summary>
        /// <param name="target">The GameObject to return.</param>
        public void ReturnGameObject(GameObject target)
        {
            for (int i = 0; i < pool.Length; i++)
            {
                //If the space in te pool is available
                if (pool[i] == null)
                {
                    //reset the item and place it in the pool
                    ResetGameObject(target);
                    pool[i] = target;
                    return;
                }
            }
        }

        /// <summary>
        /// Calls a gameObject to reset itself.
        /// </summary>
        /// <param name="target">The gameObject reset is called on</param>
        private void ResetGameObject(GameObject target)
        {
            target.Reset();
        }

        /// <summary>
        /// Populates the pool.
        /// </summary>
        /// <param name="poolsize">The desired pool size.</param>
        private void Populate(int poolsize)
        {
            if (pool == null)
            {
                pool = new GameObject[poolsize];
            }

            for (int i = 0; i < pool.Length; i++)
            {
                pool[i] = new GameObject();
                ResetGameObject(pool[i]);
            }
        }

        public int countAvailable()
        {
            int count = 0;
            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i] != null)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
