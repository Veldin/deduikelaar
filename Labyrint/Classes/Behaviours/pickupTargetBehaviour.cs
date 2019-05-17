using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;
using LogSystem;

namespace Labyrint
{
    public class pickupTargetBehaviour : IBehaviour
    {
        private float destinationFromLeft;
        private float destinationFromTop;

        //The pickup floats around an anchor.
        private GameObject anchor;

        /// <summary>
        /// Initializes a new instance of the pickupTargetBehaviour.
        /// </summary>
        /// <param name="destinationFromLeft">The destination of the pickup from left.</param>
        /// <param name="destinationFromTop">The destination of the pickup from right.</param>
        public pickupTargetBehaviour(float destinationFromLeft, float destinationFromTop)
        {
            this.destinationFromLeft = destinationFromLeft;
            this.destinationFromTop = destinationFromTop;
        }

        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, float delta)
        {
            //If the anchor is null search an anchor.
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

            //If the anchor is still null return false (don't do anything).
            if (anchor == null)
            {
                return false;
            }

            //Calculations for difference.
            float diffFromLeft = anchor.FromLeft - destinationFromLeft;
            float diffFromLeftAbs = Math.Abs(diffFromLeft);

            float diffFromTop = anchor.FromTop - destinationFromTop;
            float diffFromTopAbs = Math.Abs(diffFromTop);

            float diffFromTotal = diffFromLeftAbs + diffFromTopAbs;

            float percentageFromLeft = diffFromTotal - diffFromLeftAbs;
            percentageFromLeft = percentageFromLeft / diffFromTotal * 100;

            float percentageFromTop = diffFromTotal - diffFromTopAbs;
            percentageFromTop = percentageFromTop / diffFromTotal * 100;

            //If the absolute difference is high (when its out of the screen) place the object closer.
            if (diffFromTopAbs > 300 || diffFromLeftAbs > 550)
            {
                gameobject.Target.SetFromLeft(destinationFromLeft + (diffFromLeft));
                //Place it to the left or right of the player depending on what side the item is.
                if (diffFromLeft > 0)
                {
                    gameobject.Target.AddFromLeft(percentageFromTop * -1);
                }
                else
                {
                    gameobject.Target.AddFromLeft(percentageFromTop);
                }

                gameobject.Target.SetFromTop(destinationFromTop + (diffFromTop));
                //Place it to the left or right of the player depending on what side the item is.
                if (diffFromTop > 0)
                {
                    gameobject.Target.AddFromTop(percentageFromLeft * -1);
                }
                else
                {
                    gameobject.Target.AddFromTop(percentageFromLeft);
                }
            }
            else
            {
                //If the object is close anough set its target to its destination
                gameobject.Target.SetFromLeft(destinationFromLeft);
                gameobject.Target.SetFromTop(destinationFromTop);
            }

            return true;
        }
    }
}
