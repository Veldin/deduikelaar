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
        public bool OnTick(GameObject gameobject, ref GameObjects gameObjects, HashSet<String> pressedKeys, float delta)
        {
            // Check if target if null, if so return false
            if (gameobject.Target is null)
            {
                gameobject.Target = new Target(0, 0);
            }else
            {
                //Set the fromLeft and fromTop to the targets fromLeft and fromTop
                gameobject.FromLeft = gameobject.Target.FromLeft() - gameobject.Width / 2;
                gameobject.FromTop = gameobject.Target.FromTop() - gameobject.Height / 2;
            }


            return true;
        }

        public bool OnTick(ref GameObjects gameObjects, float delta)
        {
            throw new NotImplementedException();
        }
    }
}
