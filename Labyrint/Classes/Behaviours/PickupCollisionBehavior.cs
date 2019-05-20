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
        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, float delta)
        {
            foreach(GameObject needle in gameObjects)
            {
                if (needle.BuilderType == "player" && gameobject.IsColliding(needle))
                {
                    gameobject.destroyed = true;
                }
            }

            return true;
        }
    }
}
