using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;

namespace Labyrint
{
    public class OnClickPopupBehaviour : IBehaviour
    {
        private MainWindow engine;                  // The engine
        private List<GameObject> loopList;          // This is a loopList where all gameObjects will be placed in

        public OnClickPopupBehaviour (MainWindow engine)
        {
            // Set variables
            this.engine = engine;
            loopList = new List<GameObject>();
        }

        public bool OnTick (GameObject gameobject, ref GameObjects gameObjects, HashSet<string> pressedKeys, float delta)
        {
            // Clear the loopList
            loopList.Clear();

            // Lock the gameObjects list to copy it in the loopList
            lock (gameObjects)
            {
                loopList.AddRange(gameObjects);
            }

            // Loop through the gameObjects to search for the cursor
            foreach (GameObject needle in loopList)
            {
                // Check if the gameObject is a cursor and if it is colliding with the gameObject
                if (needle != null && needle.BuilderType == "cursor" && gameobject.IsColliding(needle))
                {
                    engine.CloseApp();
                    return true;
                }
            }

            return false;
        }

        public bool OnTick (ref GameObjects gameObjects, float delta)
        {
            throw new NotImplementedException();
        }
    }
}
