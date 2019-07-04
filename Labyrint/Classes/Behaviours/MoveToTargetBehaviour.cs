using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;

namespace Labyrint
{
    /// <summary>
    /// Makes a GameObject move to the target using its movement speed.
    /// </summary>
    class MoveToTargetBehaviour : IBehaviour
    {
        //If the behaviour is set to be pauzed it returns false in the OnTick
        public Boolean pauzed;

        public MoveToTargetBehaviour()
        {
            pauzed = false;
        }

        public bool OnTick(GameObject gameobject, ref GameObjects gameObjects, HashSet<String> pressedKeys, float delta)
        {
            //This behaviour can be pauzed by the pauzed boolean.
            if (pauzed)
            {
                return false;
            }

            // Check if target if null, if so return false.
            if (gameobject.Target is null)
            {
                return false;
            }

            // Check the fromLeft difference between the target and the GameObject
            float differenceLeftAbs;
            if (gameobject.Target.FromLeft() - (gameobject.FromLeft + (gameobject.Width / 2)) == 0)
            {
                // Set differenceLeftAbs to very close to 0 to avoid errors with mathematics
                differenceLeftAbs = 0.000000001f;
            }
            else
            {
                differenceLeftAbs = Math.Abs(gameobject.Target.FromLeft() - (gameobject.FromLeft + (gameobject.Width / 2)));
            }

            float differenceTopAbs;
            if (gameobject.Target.FromTop() - (gameobject.FromTop + (gameobject.Height / 2)) == 0)
            {
                differenceTopAbs = 0.0000000001f;
            }
            else
            {
                differenceTopAbs = Math.Abs(gameobject.Target.FromTop() - (gameobject.FromTop + (gameobject.Height / 2)));
            }

            float totalDifferenceAbs = differenceLeftAbs + differenceTopAbs;


            float differenceTopPercent = differenceTopAbs / (float)(totalDifferenceAbs / 100);
            float differenceLeftPercent = differenceLeftAbs / (float)(totalDifferenceAbs / 100);

            float moveTopDistance = (gameobject.MovementSpeed * (float)(differenceTopPercent)) / 100;
            float moveLeftDistance = (gameobject.MovementSpeed * (float)(differenceLeftPercent)) / 100;

            float moveTopDistanceDelta = (moveTopDistance * delta) / 10000;
            float moveLeftDistanceDelta = (moveLeftDistance * delta) / 10000;


            //if the targets from left is further then the gameObject from left it needs to move right (by adding fromleft)
            if (gameobject.Target.FromLeft() > gameobject.FromLeft)
            {
                if (gameobject.FromLeft + gameobject.Width / 2 + moveLeftDistanceDelta < gameobject.Target.FromLeft())
                { 
                    gameobject.AddFromLeft(moveLeftDistanceDelta);
                }
            }
            else
            {
                if (gameobject.FromLeft + gameobject.Width / 2 - moveLeftDistanceDelta > gameobject.Target.FromLeft())
                {
                    gameobject.AddFromLeft(moveLeftDistanceDelta * -1);
                }
            }

            if (gameobject.Target.FromTop() > gameobject.FromTop)
            {
                if (gameobject.FromTop + gameobject.Height / 2 + moveTopDistanceDelta < gameobject.Target.FromTop())
                {
                    gameobject.AddFromTop(moveTopDistanceDelta);
                }
            }
            else
            {
                if (gameobject.FromTop + gameobject.Height / 2 - moveTopDistanceDelta > gameobject.Target.FromTop())
                {
                    gameobject.AddFromTop(moveTopDistanceDelta * -1);
                }
            }

            return true;
        }

        public bool OnTick(ref GameObjects gameObjects, float delta)
        {
            throw new NotImplementedException();
        }
    }
}
