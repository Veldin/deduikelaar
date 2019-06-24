using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ApiParser;
using CameraSystem;
using GameObjectFactory;
using LogSystem;
using Settings;

namespace Labyrint
{
    public class PickupCollisionBehavior : IBehaviour
    {
        private List<GameObject> loopList;
        private WebBrowser browser;
        private Camera camera;
        private MainWindow engine;

        public PickupCollisionBehavior(object value){
            loopList = new List<GameObject>();

            object[] values = value as object[];

            this.browser = values[0] as WebBrowser;
            this.camera = values[1] as Camera;
            this.engine = values[2] as MainWindow;
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

                    // Increment the pickud up pickups
                    int currentPickupCount = SettingsFacade.Get("CountPickups", 0) + 1;
                    SettingsFacade.SetSetting("CountPickups", currentPickupCount.ToString());


                    int closestBorder = engine.GetLastClickClosestBorder();
                    int degrees = 0;
                    string compas = "Down";
                    switch (closestBorder)
                    {
                        case 0:
                            degrees = 180;
                            compas = "Up";

                            // Invoke the Ui thread
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                Thickness margin = browser.Margin;
                                margin.Left = 60;
                                margin.Top = 60;
                                margin.Right = 30;
                                margin.Bottom = 60;

                                browser.Margin = margin;
                            }));

                            break;
                        case 1:
                            degrees = 270;
                            compas = "Right";

                            // Invoke the Ui thread
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                // Needs to be initialized
                                Thickness margin = browser.Margin;
                                browser.Margin = margin;
                            }));
                            break;
                        case 2:
                            degrees = 0;
                            compas = "Down";

                            // Invoke the Ui thread
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                Thickness margin = browser.Margin;
                                margin.Left = 30;
                                margin.Top = 60;
                                margin.Right = 30;
                                margin.Bottom = 60;
                                browser.Margin = margin;
                            }));

                            break;
                        case 3:
                            degrees = 90;
                            compas = "Left";

                            // Invoke the Ui thread
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                // Needs to be initialized
                                Thickness margin = browser.Margin;
                                browser.Margin = margin;
                            }));

                            break;
                    }


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

                                // Split the new html again to apply the right rotation
                                string[] htmlArray2 = html.Split(new string[] { "--rotatingAngle--" }, StringSplitOptions.None);

                                // Create the right string to insert in the css
                                string addCss = degrees.ToString();

                                // Combine everything back together
                                string html2 = htmlArray2[0] + addCss + htmlArray2[1];

                                // Create the letter (thing behind the browser and buttons)
                                GameObject letter = GameObjectFactoryFacade.GetGameObject("letter", 0, 0, new object[] { camera, true, compas });

                                // Invoke the Ui thread
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    // Set put the html of the story in the browser and make it visible
                                    browser.NavigateToString(html2);
                                    browser.Visibility = Visibility.Visible;

                                    // Rotate the browser object
                                    RotateTransform rotateTransform1 = new RotateTransform(degrees)
                                    {
                                        CenterX = 75,
                                        CenterY = 60
                                    };
                                    browser.RenderTransform = rotateTransform1;

                                    // Make the lettet a bit bigger than the browser
                                    letter.AddHeight((float)(browser.Height + 250));
                                    letter.AddWidth((float)(browser.Width + 250));
                                }));

                                // Position the letter right
                                letter.FromLeft = camera.GetFromLeft() + (camera.GetWidth() - letter.Width) / 2;
                                letter.FromTop = camera.GetFromTop() + (camera.GetHeight() - letter.Height) / 1.5f;
                                letter.Target = new Target(camera.GetFromLeft() + (camera.GetWidth() - letter.Width) / 2, camera.GetFromTop() + (camera.GetHeight() - letter.Height) / 2);

                                // Add the letter to the gameObjects list
                                gameObjects.Add(letter);

                                object[] arguments = new object[3];
                                //val[0] should contain a Camera
                                //val[1] should contain the orientation (True is horizontal, false is vertical)
                                //val[2] Compas Direction - north / east / south / west
                                arguments[0] = camera;

                                lock (gameObjects)
                                {
                                    GameObject toAdd = null;


                                    // Loop through all answers of the question to create buttons for the answers
                                    for (int i = 0; i < question.anwsers.Count; i++)
                                    {
                                        // Create the variables for the position of the buttons
                                        float fromLeftPosition = 0;
                                        float fromTopPosition = 0;

                                        // Set the right positions of the buttons depending on the rotation
                                        switch (degrees)
                                        {
                                            case 0:     // Down
                                                //fromLeftPosition = (50 - ((question.anwsers.Count * 10) / 2)) + (i * 10);
                                                fromLeftPosition = 50 - (82.35f / camera.GetWidth() * 100 * (i+1) /2 )  - (i) * 82.35f / camera.GetWidth() * 100 + 82.35f / camera.GetWidth() * 100 * (question.anwsers.Count -1); 
                                                fromTopPosition = 80f;
                                                break;
                                            case 90:    // Left
                                                fromLeftPosition = 32f;
                                                //fromTopPosition = camera.GetFromLeft() +camera
                                                fromTopPosition = 50 - (82.35f / camera.GetHeight() * 100 * (i + 1) / 2) - (i) * 82.35f / camera.GetHeight() * 100 + 82.35f / camera.GetHeight() * 100 * (question.anwsers.Count - 1);
                                                break;
                                            case 180:   // Up
                                                fromLeftPosition = 50 - (82.35f / camera.GetWidth() * 100 * (i + 1) / 2) - (i) * 82.35f / camera.GetWidth() * 100 + 82.35f / camera.GetWidth() * 100 * (question.anwsers.Count - 1);
                                                fromTopPosition = 23f;
                                                break;
                                            case 270:   // Right
                                                fromLeftPosition = 68f;
                                                fromTopPosition = 50 - (82.35f / camera.GetHeight() * 100 * (i + 1) / 2) - (i) * 82.35f / camera.GetHeight() * 100 + 82.35f / camera.GetHeight() * 100 * (question.anwsers.Count - 1);
                                                break;
                                        }

                                        //int a = (i * 10 + 50) * question.anwsers.Count / 2;

                                        //int fromLeftPosition = (50 - ((question.anwsers.Count * 10) / 2)) + (i * 10);

                                        toAdd = null;
                                        toAdd = GameObjectFactoryFacade.GetGameObject("button", fromLeftPosition, fromTopPosition, new object[] { camera, storyBehaviour.GetStoryId(), question.anwsers[i].answerId, browser });

                                        switch (question.anwsers[i].response)
                                        {
                                            case "\\u1F603": // smile head
                                                toAdd.setActiveBitmap("Assets/Sprites/Answers/happyStamp.png");
                                                break;
                                            case "\\u1F620": // angry head
                                                toAdd.setActiveBitmap("Assets/Sprites/Answers/angryStamp.png");
                                                break;
                                            case "\\u1F622": // sad head
                                                toAdd.setActiveBitmap("Assets/Sprites/Answers/sadStamp.png");
                                                break;
                                            default:
                                                toAdd.setActiveBitmap("Assets/Sprites/Answers/poststamp.png");
                                                toAdd.SetText(question.anwsers[i].response);

                                                switch (degrees)
                                                {
                                                    case 0:
                                                        // Invoke the Ui thread
                                                        Application.Current.Dispatcher.Invoke(new Action(() =>
                                                        {
                                                            // Set the width of the textBlock
                                                            toAdd.textBlock.Width = toAdd.Width - 15;

                                                            // Set the fontSize of the textBlock
                                                            toAdd.textBlock.FontSize = 9;

                                                            // Add some margins to the textBlock
                                                            Thickness textblockMargin = toAdd.textBlock.Margin;
                                                            textblockMargin.Top = 15;
                                                            textblockMargin.Left = 15;
                                                            toAdd.textBlock.Margin = textblockMargin;

                                                            // Make text /n if it cant fit in the textblock
                                                            toAdd.textBlock.TextWrapping = TextWrapping.Wrap;
                                                        }));
                                                        break;
                                                    case 90:
                                                        // Invoke the Ui thread
                                                        Application.Current.Dispatcher.Invoke(new Action(() =>
                                                        {
                                                            // Set the width and height of the textBlock
                                                            toAdd.textBlock.Width = toAdd.Width - 15;
                                                            toAdd.textBlock.Height = toAdd.Height - 15;

                                                            // Set the fontSize of the textBlock
                                                            toAdd.textBlock.FontSize = 9;

                                                            // Add some margins to the textBlock
                                                            Thickness textblockMargin = toAdd.textBlock.Margin;
                                                            textblockMargin.Top = 10;
                                                            textblockMargin.Right = 25;
                                                            textblockMargin.Left = -10;
                                                            toAdd.textBlock.Margin = textblockMargin;

                                                            // Make text /n if it cant fit in the textblock
                                                            toAdd.textBlock.TextWrapping = TextWrapping.Wrap;
                                                        }));
                                                        break;
                                                    case 180:
                                                        // Invoke the Ui thread
                                                        Application.Current.Dispatcher.Invoke(new Action(() =>
                                                        {
                                                            // Set the width of the textBlock
                                                            toAdd.textBlock.Width = toAdd.Width - 15;
                                                            toAdd.textBlock.Height = toAdd.Height - 15;

                                                            // Set the fontSize of the textBlock
                                                            toAdd.textBlock.FontSize = 9;

                                                            // Add some margins to the textBlock
                                                            Thickness textblockMargin = toAdd.textBlock.Margin;
                                                            textblockMargin.Top = -15;
                                                            textblockMargin.Left = -15;
                                                            toAdd.textBlock.Margin = textblockMargin;

                                                            // Make text /n if it cant fit in the textblock
                                                            toAdd.textBlock.TextWrapping = TextWrapping.Wrap;
                                                        }));

                                                        break;
                                                    case 270:
                                                        // Invoke the Ui thread
                                                        Application.Current.Dispatcher.Invoke(new Action(() =>
                                                        {
                                                            // Set the width and height of the textBlock
                                                            toAdd.textBlock.Width = toAdd.Width - 15;
                                                            toAdd.textBlock.Height = toAdd.Height - 15;

                                                            // Set the fontSize of the textBlock
                                                            toAdd.textBlock.FontSize = 9;

                                                            // Add some margins to the textBlock
                                                            Thickness textblockMargin = toAdd.textBlock.Margin;
                                                            textblockMargin.Top = -15;
                                                            textblockMargin.Right = -25;
                                                            textblockMargin.Left = 10;
                                                            toAdd.textBlock.Margin = textblockMargin;

                                                            // Make text /n if it cant fit in the textblock
                                                            toAdd.textBlock.TextWrapping = TextWrapping.Wrap;
                                                        }));
                                                        break;
                                                }
                                                break;
                                        }

                                        // Invoke the Ui thread
                                        Application.Current.Dispatcher.Invoke(new Action(() =>
                                        {
                                            // Create a RotateTransform object
                                            RotateTransform rotateTransform1 = new RotateTransform(degrees)
                                            {
                                                CenterX = toAdd.Width / 2,
                                                CenterY = toAdd.Height / 2
                                            };

                                            // If the rectangle is not null apply the rotation on the rectangle
                                            if (toAdd.rectangle != null)
                                            {
                                                toAdd.rectangle.RenderTransform = rotateTransform1;
                                            }

                                            // If the textBlock is not null apply the rotation on the textblock
                                            if (toAdd.textBlock != null)
                                            {
                                                toAdd.textBlock.RenderTransform = rotateTransform1;
                                            }
                                        }));

                                        // Adde the button to the list of gameObjects
                                        lock (gameObjects)
                                        {
                                            Log.Debug("button added");
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
