using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameObjectFactory
{
    public class Factory
    {
        private Pool pool;
        private Builder builder;

        public Factory()
        {
            pool = new Pool();
            builder = new Builder();
        }

        public GameObject GetGameObject(string wantToGet)
        {
            GameObject target = pool.GetGameObject();
            builder.TransformGameObject(target, wantToGet);

            return target;
        }

        public void ReturnGameObject(GameObject target)
        {
            pool.ReturnGameObject(target);
        }

        public int countAvailable()
        {
            return pool.countAvailable();
        }
    }
}
