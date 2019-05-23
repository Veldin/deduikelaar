using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;
using LogSystem;

namespace Labyrint
{
    class PlayerSpriteBehaviour : IBehaviour
    {
        private float lastPositionFromLeft;
        private float lastPositionFromTop;

        private float animationDelta;
        private float animationDeltaMax;

        private int frame;

        public PlayerSpriteBehaviour()
        {
            lastPositionFromLeft = 0;
            lastPositionFromTop = 0;
            animationDeltaMax = 1000;
            animationDelta = animationDeltaMax;
        }

        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, float delta)
        {
            bool moved = false;
            if (Math.Abs(lastPositionFromLeft - gameobject.FromLeft) > 0.1f)
            {
                moved = true;
            }
            if (Math.Abs(lastPositionFromTop - gameobject.FromTop) > 0.1f)
            {
                moved = true;
            }
            lastPositionFromLeft = gameobject.FromLeft;
            lastPositionFromTop = gameobject.FromTop;

            if (moved)
            {
                animationDelta -= delta;
                if (animationDelta < 0)
                {
                    frame++;
                    if (frame > 3)
                    {
                        frame = 1;
                    }
                    Log.Debug(frame);
                }

                gameobject.setActiveBitmap("Assets/Sprites/right"+ frame + "_145_200_32.gif");
            }

            return false;
        }
    }
}
