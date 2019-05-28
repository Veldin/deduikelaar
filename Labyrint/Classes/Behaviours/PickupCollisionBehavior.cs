using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GameObjectFactory;
using LogSystem;


namespace Labyrint
{
    class PickupCollisionBehavior : IBehaviour
    {
        public List<GameObject> loopList;
        private WebBrowser browser;

        public PickupCollisionBehavior(object browser){
            loopList = new List<GameObject>();
            this.browser = browser as WebBrowser;
        }

        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, HashSet<String> pressedKeys, float delta)
        {
 
            loopList.Clear();

            lock (gameObjects)
            {
                loopList.AddRange(gameObjects);
            }

            foreach (GameObject needle in loopList)
            {
                if (needle != null && needle.BuilderType == "player" && gameobject.IsColliding(needle))
                {
                    foreach (IBehaviour behaviour in gameobject.onTickList)
                    {
                        if (behaviour.GetType().ToString() == "Labyrint.HaveAStoryBehaviour")
                        {                  
                            HaveAStoryBehaviour storyBehaviour = behaviour as HaveAStoryBehaviour;

                            if (storyBehaviour.HasStory())
                            {
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    browser.NavigateToString(storyBehaviour.GetHtml());
                                    browser.Visibility = Visibility.Visible;
                                }));
                            }
                        }                        
                    }

                    gameobject.destroyed = true;
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
