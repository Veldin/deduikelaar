using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;

namespace Labyrint
{
    class SetToTargetBehaviour : IBehaviour
    {
        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, float delta)
        {
            // Check if target if null, if so return false
            if (gameobject.Target is null)
            {
                return false;
            }

            //Set the fromLeft and fromTop to the targets fromLeft and fromTop
            gameobject.FromLeft = gameobject.Target.FromLeft();
            gameobject.FromTop = gameobject.Target.FromTop();


            return true;
        }
    }
}
