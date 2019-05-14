using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;

namespace Labyrint
{
    class MoveToTargetBehaviour : IBehaviour
    {
        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, float delta)
        {
            if (gameobject.Target is null)
            {
                return false;
            }

            float differenceLeftAbs;
            if (gameobject.Target.FromLeft() - (gameobject.FromLeft + (gameobject.Width / 2)) == 0)
            {
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

            if (!(totalDifferenceAbs < -1 || totalDifferenceAbs > 1))
            {
                return true;
            }

            float originalmoveSpeed = gameobject.MovementSpeed;


            float differenceTopPercent = differenceTopAbs / (totalDifferenceAbs / 100);
            float differenceLeftPercent = differenceLeftAbs / (totalDifferenceAbs / 100);


            float moveTopDistance = gameobject.MovementSpeed * (differenceTopPercent / 100);
            float moveLeftDistance = gameobject.MovementSpeed * (differenceLeftPercent / 100);

            if (gameobject.Target.FromLeft() > gameobject.FromLeft)
            {
                gameobject.AddFromLeft((moveLeftDistance * delta) / 10000);
            }
            else
            {
                gameobject.AddFromLeft(((moveLeftDistance * delta) / 10000) * -1);
            }

            if (gameobject.Target.FromTop() > gameobject.FromTop)
            {
                gameobject.AddFromTop((moveTopDistance * delta) / 10000);
            }
            else
            {
                gameobject.AddFromTop(((moveTopDistance * delta) / 10000) * -1);
            }

            return true;
        }
    }
}
