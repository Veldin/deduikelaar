using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;
using LogSystem;

namespace Labyrint
{
    class PickupCollisionBehavior : IBehaviour
    {
        List<GameObject> loopList;

        public PickupCollisionBehavior(){
            List<GameObject> loopList = new List<GameObject>();
        }

        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, float delta)
        {

            loopList.Clear();
            lock (gameObjects)
            {
                loopList.AddRange(gameObjects);
            }

            foreach (GameObject needle in loopList)
            {
                if (needle != null && needle.BuilderType == "player" && gameobject.IsColliding(needle))
                {
                    gameobject.destroyed = true;
                }
            }

            return true;
        }
    }
}
