using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;
using LogSystem;

namespace Labyrint
{
    class AnimateOpacityBehaviour : IBehaviour
    {
        private float currentDelta;
        private float maxDelta;
        private float maxOpacity;


        public AnimateOpacityBehaviour(float maxOpacity)
        {
            maxDelta = 10;
            currentDelta = maxDelta;
            this.maxOpacity = maxOpacity;
        }

        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, HashSet<string> pressedKeys, float delta)
        {
            float Opacity = gameobject.GetOpacity();

            if (Opacity < 0.5)
            {
                currentDelta -= delta;
                if (currentDelta < 0)
                {
                    currentDelta = maxDelta;

                    gameobject.SetOpacity(Opacity + 0.005f);
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
