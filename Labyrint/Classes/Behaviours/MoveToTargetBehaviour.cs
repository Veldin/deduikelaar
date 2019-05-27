using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;

namespace Labyrint
{
    class MoveToTargetBehaviour : IBehaviour
    {
        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, HashSet<String> pressedKeys, float delta)
        {
            // Check if target if null, if so return false
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

            //If they are close each other to each other don't move
            if (totalDifferenceAbs < 50)
            {
                return true;
            }

            float originalmoveSpeed = gameobject.MovementSpeed;

            float differenceTopPercent = differenceTopAbs / (totalDifferenceAbs / 100);
            float differenceLeftPercent = differenceLeftAbs / (totalDifferenceAbs / 100);

            float moveTopDistance = gameobject.MovementSpeed * (differenceTopPercent / 100);
            float moveLeftDistance = gameobject.MovementSpeed * (differenceLeftPercent / 100);

            float moveTopDistanceDelta = (moveTopDistance * delta) / 10000;
            float moveLeftDistanceDelta = (moveLeftDistance * delta) / 10000;

            if (gameobject.Target.FromLeft() > gameobject.FromLeft)
            {
                if (differenceLeftAbs < moveLeftDistanceDelta)
                {
                    gameobject.AddFromLeft(differenceLeftAbs);
                }
                else
                {
                    gameobject.AddFromLeft(moveLeftDistanceDelta);
                }
            }
            else
            {
                if (differenceLeftAbs < moveLeftDistanceDelta)
                {
                    gameobject.AddFromLeft(differenceLeftAbs * -1);
                }
                else
                {
                    gameobject.AddFromLeft(moveLeftDistanceDelta * -1);
                }
            }

            if (gameobject.Target.FromTop() > gameobject.FromTop)
            {
                if (differenceTopAbs < moveTopDistanceDelta)
                {
                    gameobject.AddFromTop(differenceTopAbs);
                }
                else
                {
                    gameobject.AddFromTop(moveTopDistanceDelta);
                }
            }
            else
            {
                if (differenceTopAbs < moveTopDistanceDelta)
                {
                    gameobject.AddFromTop(differenceTopAbs * -1);
                }
                else
                {
                    gameobject.AddFromTop(moveTopDistanceDelta * -1);
                }
            }

            return true;
        }

        public bool OnTick(List<GameObject> gameObjects, float delta)
        {
            throw new NotImplementedException();
        }
    }
}
