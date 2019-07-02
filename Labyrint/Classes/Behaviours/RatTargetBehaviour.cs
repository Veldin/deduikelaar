using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;

namespace Labyrint
{
    class RatTargetBehaviour : IBehaviour
    {
        private GameObject anchor;
        private Random random;

        public RatTargetBehaviour()
        {
            random = new Random();
        }

        public bool OnTick(GameObject gameobject, ref GameObjects gameObjects, HashSet<string> pressedKeys, float delta)
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
            float diffFromLeft = anchor.FromLeft - gameobject.FromLeft;
            float diffFromLeftAbs = Math.Abs(diffFromLeft);

            float diffFromTop = anchor.FromTop - gameobject.FromTop;
            float diffFromTopAbs = Math.Abs(diffFromTop);

            float diffFromTotal = diffFromLeftAbs + diffFromTopAbs;

            float percentageFromLeft = diffFromTotal - diffFromLeftAbs;
            percentageFromLeft = percentageFromLeft / diffFromTotal * 100;

            float percentageFromTop = diffFromTotal - diffFromTopAbs;
            percentageFromTop = percentageFromTop / diffFromTotal * 100;

            if (gameobject.Target is null) {
                gameobject.Target = new Target(gameobject, false);
            }

            //If the absolute difference is high (when its out of the screen) place the object closer.
            if (diffFromTopAbs < 100 || diffFromLeftAbs < 100)
            {

                gameobject.Target.AddFromLeft(random.Next(-1000,1000));
                gameobject.Target.AddFromTop(random.Next(-1000, 1000));

            }
            else
            {
                gameobject.Target = null;
            }

            return true;
        }

        public bool OnTick(ref GameObjects gameObjects, float delta)
        {
            throw new NotImplementedException();
        }
    }
}
