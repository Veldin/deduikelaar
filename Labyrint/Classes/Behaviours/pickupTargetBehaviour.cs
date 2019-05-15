using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;

namespace Labyrint
{
    public class pickupTargetBehaviour : IBehaviour
    {
        private float destinationFromLeft;
        private float destinationFromTop;

        private GameObject anchor;

        public pickupTargetBehaviour(float destinationFromLeft, float destinationFromTop)
        {
            this.destinationFromLeft = destinationFromLeft;
            this.destinationFromTop = destinationFromTop;

        }

        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, float delta)
        {
            if (anchor == null)
            {
                foreach (GameObject needle in gameObjects)
                {
                    if (needle.BuilderType == "player")
                    {
                        anchor = needle;
                        break;
                    }
                }
            }

            if (anchor == null)
            {
                return false;
            }

            //Debug.WriteLine(anchor.DistanceBetween(destinationFromLeft, destinationFromTop));

            gameobject.Target.SetFromLeft(destinationFromLeft);
            gameobject.Target.SetFromTop(destinationFromTop);

            return true;
        }
    }
}
