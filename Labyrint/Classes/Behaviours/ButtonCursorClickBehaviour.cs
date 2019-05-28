using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;
using LogSystem;

namespace Labyrint
{
    class ButtonCursorClickBehaviour : IBehaviour
    {
        GameObject cursor;

        public ButtonCursorClickBehaviour()
        {

        }

        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, HashSet<String> pressedKeys, float delta)
        {
            //If there is no cursor, try to find it.
            if (cursor is null)
            {
                foreach (GameObject needle in gameObjects)
                {

                    if (needle.BuilderType == "cursor")
                    {
                        cursor = needle;
                        break;
                    }
                }
            }

            if (cursor is null)
            {
                return false;
            }


            if (gameobject.IsColliding(cursor) && pressedKeys.Contains("LeftMouse"))
            { 
                gameobject.destroyed = true;
            }

            return true;
        }

        public bool OnTick(List<GameObject> gameObjects, float delta)
        {
            throw new NotImplementedException();
        }
    }
}
