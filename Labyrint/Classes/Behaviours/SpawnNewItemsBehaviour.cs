using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CameraSystem;
using GameObjectFactory;
using Maze;

namespace Labyrint
{
    class SpawnNewItemsBehaviour : IBehaviour
    {
        private List<GameObject> loopList;
        private Random random;
        private WebBrowser browser;
        private Camera camera;
        private GameObject player;

        /// <summary>
        /// Constructor for the behaviour responsable for spawing new items.
        /// </summary>
        /// <param name="browser">The browser given to the pickup so they know where to render if they are picked up.</param>
        /// <param name="camera">The camera so the pickups know where to be attached too.</param>
        /// <param name="player">The given gameobject where the pickup cannot spawn close to. Usualy this is the player.</param>
        public SpawnNewItemsBehaviour(WebBrowser browser, Camera camera, GameObject player)
        {
            loopList = new List<GameObject>();
            random = new Random();

            this.browser = browser;
            this.camera = camera;
            this.player = player;
        }

        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, HashSet<string> pressedKeys, float delta)
        {
            throw new NotImplementedException();
        }

        public bool OnTick(List<GameObject> gameObjects, float delta)
        {
            loopList.Clear();
            lock (gameObjects)
            {
                loopList.AddRange(gameObjects);
            }

            foreach (GameObject needle in loopList)
            {
                if (needle.BuilderType == "pickup")
                {
                    return false;
                }
            }
            ///

            for (int i = 0; i < 5; i++)
            {
                DropNewPickup(gameObjects);
            }

            ///

            return true;
        }

        private void DropNewPickup(List<GameObject> gameObjects)
        {
            GameObject testPickup; //holds the new pickup

            int randomFromTop, randomFromLeft;
            do
            {
                
                do
                {
                    //Get a random wall position
                    randomFromTop = random.Next(MazeFacade.GetMazeHeight());
                    randomFromLeft = random.Next(MazeFacade.GetMazeWidth() );
                } while (MazeFacade.IsWall(randomFromLeft, randomFromTop)); //If its a wall pick a new location

                // create the pickup
                testPickup = GameObjectFactoryFacade.GetGameObject(
                    "",
                    randomFromLeft * MazeFacade.tileSize + MazeFacade.tileSize / 2,
                    randomFromTop * MazeFacade.tileSize + MazeFacade.tileSize / 2
                );

            } while (testPickup.distanceBetween(player) < 200); //if its to close to the player pick a new location

            testPickup.destroyed = true;

            // create the pickup
            GameObject newPickup = GameObjectFactoryFacade.GetGameObject(
                "pickup",
                randomFromLeft * MazeFacade.tileSize + MazeFacade.tileSize / 2,
                randomFromTop * MazeFacade.tileSize + MazeFacade.tileSize / 2,
                new object[2] { browser, camera }
            );

            // create the pickup
            GameObject orb = GameObjectFactoryFacade.GetGameObject(
                "orb",
                randomFromLeft * MazeFacade.tileSize + MazeFacade.tileSize / 2,
                randomFromTop * MazeFacade.tileSize + MazeFacade.tileSize / 2,
                newPickup
            );

            gameObjects.Add(orb);
            gameObjects.Add(newPickup);

        }
    }
}
