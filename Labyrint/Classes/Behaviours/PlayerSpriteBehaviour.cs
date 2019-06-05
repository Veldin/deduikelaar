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
        private string position;

        public PlayerSpriteBehaviour()
        {
            lastPositionFromLeft = 0;
            lastPositionFromTop = 0;
            animationDeltaMax = 1000;
            animationDelta = animationDeltaMax;
            position = "right";
        }

        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, HashSet<String> pressedKeys, float delta)
        {
            bool moved = false;
            if (Math.Abs(lastPositionFromLeft - gameobject.FromLeft) > 0.01f)
            {
                moved = true;
            }
            if (Math.Abs(lastPositionFromTop - gameobject.FromTop) > 0.01f)
            {
                moved = true;
            }

            //Check if there was movement horisontal or vertical
            if (Math.Abs(lastPositionFromLeft - gameobject.FromLeft) > Math.Abs(lastPositionFromTop - gameobject.FromTop))
            {
                //Moving horisontal
                //Check if moving left or right
                if (lastPositionFromLeft < gameobject.FromLeft)
                {
                    //moving right
                    position = "right";
                }
                else
                {
                    //moving left
                    position = "left";
                }
            }
            else
            {
                //Moving vertical
                //Check if moving up or down
                if (lastPositionFromTop < gameobject.FromTop)
                {
                    //moving down
                    position = "front";
                }
                else
                {
                    //moving up
                    position = "back";
                }
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

                    animationDelta = animationDeltaMax;
                    //'assets/sprites/left2_145_200_32.gif'.'
                    gameobject.setActiveBitmap("Assets/Sprites/Player/"+ position + frame + "_145_200_32_v2.gif");
                }

            }

            return false;
        }

        public bool OnTick(List<GameObject> gameObjects, float delta)
        {
            throw new NotImplementedException();
        }
    }
}
