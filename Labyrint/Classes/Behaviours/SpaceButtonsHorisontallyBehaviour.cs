using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;

namespace Labyrint
{
    class SpaceButtonsHorisontallyBehaviour : IBehaviour
    {
        public List<GameObject> loopList;

        public SpaceButtonsHorisontallyBehaviour()
        {
            loopList = new List<GameObject>();
        }

        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, float delta)
        {
            throw new NotImplementedException();
        }

        public bool OnTick(List<GameObject> gameObjects, float delta)
        {
            loopList.Clear();

            lock (gameObjects)
            {
                loopList.AddRange(gameObjects);
            }

            foreach (GameObject needle in loopList)
            {
                
            }

            return true;
        }
    }
}
