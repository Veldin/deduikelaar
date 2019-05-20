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

        public Pool(int poolsize = 50)
        {
            Populate(poolsize);
        }

        public GameObject GetGameObject()
        {
            //Console.WriteLine("getting");
            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i] != null)
                {
                    GameObject needle = pool[i];
                    pool[i] = null;

                    //Console.WriteLine("getting " + needle.id);

                    //needle.Test();

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

        public void ReturnGameObject(GameObject target)
        {
            //Console.WriteLine("return" + target.id);
            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i] == null)
                {
                    ResetGameObject(target);
                    pool[i] = target;
                    return;
                }
            }
        }

        private void ResetGameObject(GameObject target)
        {
            target.Reset();
        }

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
