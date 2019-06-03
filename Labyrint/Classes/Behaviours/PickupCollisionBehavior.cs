﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ApiParser;
using CameraSystem;
using GameObjectFactory;
using LogSystem;


namespace Labyrint
{
    public class PickupCollisionBehavior : IBehaviour
    {
        private List<GameObject> loopList;
        private WebBrowser browser;
        private Camera camera;

        public PickupCollisionBehavior(object value){
            loopList = new List<GameObject>();

            object[] values = value as object[];

            this.browser = values[0] as WebBrowser;
            this.camera = values[1] as Camera;
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
                    //Destory the UI anchors and pauze the movementbehaviour of other objects
                    //This makes the game apear to stop.
                    foreach (GameObject gameObject in gameObjects)
                    {
                        if (gameObject.BuilderType == "ControllerAncher" || gameObject.BuilderType == "ControllerCursor")
                        {
                            gameObject.destroyed = true;
                        }
                        else
                        {
                            foreach(IBehaviour behaviour in gameObject.onTickList)
                            {
                                if (behaviour is MoveToTargetBehaviour)
                                {
                                    MoveToTargetBehaviour moveToTargetBehaviour = behaviour as MoveToTargetBehaviour;
                                    moveToTargetBehaviour.pauzed = true;
                                }
                            }
                        }
                    }
                    // Unpress all keys
                    pressedKeys.Clear();

                    // Loop through the behaviours of the gameObject to find the HaveAStory behaviour
                    foreach (IBehaviour behaviour in gameobject.onTickList)
                    {
                        // Check if the behaviour is the HaveAStoryBehaviour
                        if (behaviour.GetType().ToString() == "Labyrint.HaveAStoryBehaviour")
                        {                  
                            // Convert the IBehaviour to a HaveAStoryBehaviour
                            HaveAStoryBehaviour storyBehaviour = behaviour as HaveAStoryBehaviour;

                            // Check if the behaviour holds a story
                            if (storyBehaviour.HasStory())
                            {
                                // Get the question object from the apiparserfacade
                                Question question = ApiParserFacade.GetQuestion(storyBehaviour.GetFeedbackId());

                                // Get the original hmtml from the story
                                string ogHtml = storyBehaviour.GetHtml();

                                // Split the html string at the end body tag
                                string[] htmlArray = ogHtml.Split(new string[] { "</body>" }, StringSplitOptions.None);
                                
                                // Create a new html string with the feedback question in it
                                string addHtml = " <div class='feedback' > <br/> <p> " + question.question + " </p> </div> </body>";
                                
                                // Insert the addhtml string in the og html
                                string html = htmlArray[0] + addHtml + htmlArray[1];

                                // Invoke the Ui thread
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    // Set put the html of the story in the browser and make it visible
                                    browser.NavigateToString(html);
                                    browser.Visibility = Visibility.Visible;
                                }));

                                gameObjects.Add(GameObjectFactoryFacade.GetGameObject("cover",0,0,camera));

                                lock (gameObjects)
                                {
                                    GameObject toAdd = null;


                                    //i * 10 - (10) + 50
                                    for (int i = 0; i < question.anwsers.Count; i++)
                                    {

                                        //int a = (i * 10 + 50) * question.anwsers.Count / 2;
                 
                                        int fromLeftPosition = (50 - ((question.anwsers.Count * 10) / 2)) + (i * 10);

                                        toAdd = null;
                                        toAdd = GameObjectFactoryFacade.GetGameObject("button", fromLeftPosition, 83.5f, new object[] { camera, storyBehaviour.GetStoryId(), question.anwsers[i].answerId, browser });

                                    Log.Debug(toAdd);
                                    switch (question.anwsers[i].response)
                                    {
                                        case "\\u1F603": // smile head
                                            toAdd.setActiveBitmap("Assets/Sprites/Answers/happy.gif");
                                            break;
                                        case "\\u1F620": // angry head
                                            toAdd.setActiveBitmap("Assets/Sprites/Answers/angry.gif");
                                            break;
                                        case "\\u1F622": // sad head
                                            toAdd.setActiveBitmap("Assets/Sprites/Answers/sad.gif");
                                            break;
                                        default:
                                            toAdd.SetText(question.anwsers[i].response);
                                            break;
                                    }
                                        lock (gameObjects)
                                        {
                                            Log.Debug("added");
                                            gameObjects.Add(toAdd);
                                        }
                                       
                                    }
                                }
                            }
                        }                        
                    }
                    // Destroy the pickup
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
