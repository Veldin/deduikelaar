using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GameObjectFactory;
using LogSystem;
using Maze;
using CameraSystem;
using FileReaderWriterSystem;
using ApiParser;
using Settings;
using System.IO;

namespace Labyrint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //For keeping the sizes of the window (on the canvas, not IRL)
        private int width;
        private int height;

        //For timekeeping (we need to know when the last frame happend when the next frame happens and the delta between)
        private long delta;     //The lenght in time the last frame lasted (so we can use it to calculate speeds of things without slowing down due to low fps)
        private long now;       //This is the time of the frame. (To calculate the delta)
        private long then;      //This is the time of the previous frame. (To calculate the delta)

        //The max fps we want to run at
        private float fps;  //The set FPS limit
        private float interval; //Interfal that gets calculated based on the fps

        //Arraylist with all the gameObjects in the current game
        private HashSet<String> pressedKeys;

        //Camera
        private  Camera camera;

        //The brush used to fill in the background
        private SolidColorBrush backgroundBrush;
        private Rectangle rectangle;

        public Random random;

        private GameObject player;

        private GameObject cursor;

        private GameObjects gameObjects;

        //Strategies
        public List<IBehaviour> onTickList;

        string assemblyName;

        private int renderDistance;

        //TODO: controllers in list?
        private GameObject controllerAnchor;
        private GameObject controllerCursor;
        private string controlMode;

        private Command command;

        

        // Holds the last location of the touch input
        // 0 north, 1 east, 2 south, 3 west
        private int lastClickClosestBorder;

        public MainWindow()
        {
            //Initialize the Window
            InitializeComponent();

            SplashScreen splashScreen = new SplashScreen();
            splashScreen.Show();

            // Add the OnMouseDown and the OnMouseUp as event handler
            AddHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), true);
            AddHandler(FrameworkElement.MouseUpEvent, new MouseButtonEventHandler(OnMouseUp), true);

            //Bind the KeyUp and KeyDown methods.
            Window.GetWindow(this).KeyUp += KeyUp;
            Window.GetWindow(this).KeyDown += KeyDown;
            Window.GetWindow(this).SizeChanged += SizeChanged;

            //Innitialise all the Facades
            FileReaderWriterFacade.Init();
            splashScreen.AddProgress();

            SettingsFacade.Init();
            splashScreen.AddProgress();           

            ApiParserFacade.Init();
            splashScreen.AddProgress();

            GameObjectFactoryFacade.Init();
            splashScreen.AddProgress();

            MazeFacade.Init();
            splashScreen.AddProgress();

            // Create the camera
            camera = new Camera(gameCanvas, mainWindow);

            // Create the command bar
            command = new Command(this, CommandBar, CommandResponse);

            // Create an instance of the Random class
            random = new Random();

            // Init browser
            browser.Navigate(new Uri(FileReaderWriterFacade.GetAppDataPath()));      //Inits a new navigate call

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                rectangle = new Rectangle();
                rectangle.Stroke = new SolidColorBrush(Color.FromRgb(0, 111, 111));
            }));

            //Set the windowsstyle (,the bar above the application).
            switch (SettingsFacade.Get("WindowsStyle", "ToolWindow", "Dictates the window style [ToolWindow || None]"))
            {
                case "None":
                    WindowStyle = WindowStyle.None;
                    break;
                default:
                    WindowStyle = WindowStyle.ToolWindow;
                    break;
            }

            //Set the windowState.
            switch (SettingsFacade.Get("WindowState", "Maximized", "Dictates the window state [Maximized || Maximized || Normal]"))
            {
                case "Normal":
                    WindowState = WindowState.Normal;
                    break;
                case "Minimized":
                    WindowState = WindowState.Minimized;
                    break;
                default:
                    WindowState = WindowState.Maximized;
                    break;
            }

            // Set the controlmode.
            switch (SettingsFacade.Get("ControlMode", "Mouse", "Dictates which events will be used [Mouse || Touch || Both]"))
            {
                case "Touch":
                    controlMode = "Touch";
                    break;
                case "Minimized":
                    controlMode = "Both";
                    break;
                default:
                    controlMode = "Mouse";
                    break;
            }

            //todo: Get them from the Xaml
            width = 1280;
            height = 720;

            //Set the assembly name
            assemblyName = Assembly.GetEntryAssembly().GetName().Name;

            //Inits the pressed keys set
            pressedKeys = new HashSet<String>();

            //Create a new player, Use the mazefacade to set them in the maze
            player = GameObjectFactoryFacade.GetGameObject("player", MazeFacade.tileSize, MazeFacade.tileSize);

            this.Cursor = Cursors.None; //Hide the default windows cursor.
            cursor = GameObjectFactoryFacade.GetGameObject("cursor", 300, 300, 10);

            //Inits the GameObject list
            gameObjects = new GameObjects();
            gameObjects.Add(player);    
            gameObjects.Add(cursor);

            //Inits the background object list
            PopulateBackgroundObject();

            // Inits the onTickList and add behaviours.
            onTickList = new List<IBehaviour>();
            onTickList.Add(new SpaceButtonsHorisontallyBehaviour());
            onTickList.Add(new SpawnNewItemsBehaviour(browser, camera, player, this));

            // Set the background brush
            backgroundBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 110, 155, 178));

            // Get the render ditsance from the SettingsFacade or use the default
            renderDistance = SettingsFacade.Get("RenderDistance", 1200);

            // Calculate and sets the hight and with of the camera (internally).
            camera.GenerateHeightAndWidth();

            // Get the fps from the SettingsFacade or use the default
            fps = SettingsFacade.Get("fps", 999);//Desired max fps.
            interval = 1000 / fps;

            // Save the SettingsFacade. This creates the INI file.
            SettingsFacade.Save();

            // Check if all the necassary JSON files exist
            // If not create a popup and close the application
            if (!ApiParserFacade.ExistJsonFiles())
            {
                // Invoke the Ui thread
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    // Rescale the browser by adding margins
                    Thickness margin = browser.Margin;
                    margin.Left = 30;
                    margin.Top = 60;
                    margin.Right = 30;
                    margin.Bottom = 60;
                    browser.Margin = margin;
                }));

                //The file not found error in html.
                string fileNorAPIfoundHTML = "<!doctype html><html lang='{{ app()->getLocale() }}'> <head> <meta charset='utf-8'> <meta http-equiv='X-UA-Compatible' content='IE=edge'> <meta name='viewport' content='width=device-width, initial-scale=1'> <title>Error</title> <style> /**{*/ /*margin: 0;*/ /*padding: 0;*/ /*}*/ *{ max-width: 100%; } html, body{ position: absolute; width: 100%; height: 100%; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen-Sans, Ubuntu, Cantarell, 'Helvetica Neue', sans-serif; font-size: @if(isset($_GET['overview'])) 0.5rem @else 1rem @endif; background-color: #aa9479; margin: 0; padding: 0; overflow: auto; transform-origin: center center; } html{ overflow: hidden; } .title{ font-weight: 400; font-size: 1.8rem; margin-bottom: @if(isset($_GET['overview'])) 20px @else 60px @endif; width: 70%; } audio, video{ width: 100%; max-width: 100%; } img{ max-width: 100%; } .description,.feedback{ font-weight: normal; font-size: 1.3rem; }.feedback{display: block;width: 100%;text-align: center;} </style> </head> <body oncontextmenu='return false;'> <header> <h1 class='title'>Error</h1> </header> <div class='description'><p>Er kan geen verbinding met de server gemaakt worden. Tevens zijn er geen gecachte bestanden.</p><p>Zorg dat er informatiestukken op de server staan en maak vervolgens een verbinding met de server.</p> </div> </body></html>";

                // Navigate the browser to the html page with the error message and set it on visible
                browser.NavigateToString(fileNorAPIfoundHTML);
                browser.Visibility = Visibility.Visible;

                // Create a button for the popup and add it to the gameObjects list
                gameObjects.Add(GameObjectFactoryFacade.GetGameObject("popupButton", 49, 80, 4, new object[2] { camera, this }));
            }

            splashScreen.Close();

            // Set the time of the 'old' frame on the current time then start the app.
            then = Stopwatch.GetTimestamp();
            Run();
        }

        /***************************************************************************
         * GETTERS AND SETTERS
         * ************************************************************************/
        #region gettersSetters

        /// <summary>
        /// This method returns the closest border to the last click.
        /// 0 = top, 1 = right, 2 = bottom, 4 = left
        /// </summary>
        /// <returns>Returns the number of the closest border</returns>
        public int GetLastClickClosestBorder()
        {
            return lastClickClosestBorder;
        }

        public int GetFps()
        {
            return (1000 / (int)delta);
        }

        #endregion

        /***************************************************************************
         * PUBLIC METHODS
         * ************************************************************************/
        #region publicMethods

        /// <summary>
        ///  Starts the Simulation
        /// </summary>
        public void Run()
        {
            //Get the current time
            now = Stopwatch.GetTimestamp();

            //Get the difference between now and then
            delta = (now - then) / 1000; //Defide by 1000 to get the delta in MS

            //Check if it should draw a new frame.
            if (delta > interval)
            {
                then = now; //Remember when this frame was.
                Logic(delta *  (long) ((float) SettingsFacade.Get("DeltaModifier", 100) / 100f)); //Run the logic of the simulation.
                Draw();// Draw to the canvas
            }
            else
            {
                Thread.Sleep(1); //Sleep the thread so time is passed
            }

            Task.Yield();  //Force this task to complete asynchronously (This way the main thread is not blocked by this task calling itself.
            Task.Run(() => Run());  //Schedule new Run() task
        }

        /// <summary>
        /// CloseApp gets called from the mainWindow_Closing event.
        /// </summary>
        public async void CloseApp()
        {
            // Give the user feedback
            Log.Info("Shutting down...");

            // Sending the api the statsistics
            if (await ApiParserFacade.InformApiAsync())
            {
                // If the statistics are sent to the api delete the file to prevent double data insertion
                FileReaderWriterFacade.DeleteFile(new string[] { FileReaderWriterFacade.GetAppDataPath() + "Items\\Statistics.json" });
            }

            // Save the settings
            SettingsFacade.Save();

            // Close the application on the UI thread
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                System.Windows.Application.Current.Shutdown();
            }));
        }

        /// <summary>
        /// This method reset the controls by unpressing all keys and destroying all gameObjects that are needed for the controls
        /// (callable from the command bar)
        /// </summary>
        public void ResetControls()
        {
            // Unpress all keys
            pressedKeys.Clear();

            // Loop through all the gameObjects to find anchors
            foreach (GameObject gameObject in gameObjects)
            {
                // Destroy it if its a controllerAnchor or an ControllerCursor
                if (gameObject.BuilderType == "ControllerAncher" || gameObject.BuilderType == "ControllerCursor")
                {
                    gameObject.destroyed = true;
                }
            }

            // Give programmer feedback
            Log.Info("Keys resetted");
        }

        /// <summary>
        /// Search the player and turn his collision on if off and off if on.
        /// (usable from the command bar)
        /// </summary>
        public void ActivateCollision()
        {
            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.BuilderType == "player")
                {
                    gameObject.Collition = !gameObject.Collition;
                    Log.Info("The player collision is now: " + gameObject.Collition);
                }
            }
        }

        #endregion

        /***************************************************************************
         * PRIVATE METHODS
         * ************************************************************************/
        #region privateMethods

        /// <summary>
        /// Calculates the logic of this frame
        /// </summary>
        /// <param name="delta"> Time passed since the last frame </param>
        private void Logic(long delta)
        {
            //Create a new arraylist used to hold the gameobjects for this loop.
            //The copy is made so it does the ontick methods on all the objects even the onces destroyed in the proces.
            GameObjects loopList;
            lock (gameObjects) //lock the gameobjects for duplication
            {
                try
                {
                    //Try to duplicate the arraylist.
                    loopList = new GameObjects();
                    loopList.AddRange(gameObjects);
                }
                catch
                {
                    //if it failes for any reason skip this frame.
                    return;
                }
            }

            //set the camera on the player position
            camera.SetFromLeft(player.FromLeft - (width / 2) + player.Width / 2);
            camera.SetFromTop(player.FromTop - (height / 2) + player.Height / 2);

            //For every gameobject in the list
            foreach (GameObject gameObject in loopList)
            {
                //OnTick every gameObject
                if (IsKeyPressed("Space")) //Act like the game runs faster if space is pressed
                {
                    gameObject.OnTick(ref gameObjects, ref pressedKeys, delta * 7f);
                }
                else
                {
                    gameObject.OnTick(ref gameObjects, ref pressedKeys, delta);
                }
            }

            //OnTick all the engine behaiviors
            if (onTickList.Count > 0)
            {
                foreach (IBehaviour behaivior in onTickList)
                {
                    behaivior.OnTick(ref gameObjects, delta);
                }
            }


            // If the Controlmode is Mouse or Both set the cursor to the mouse position
            if (controlMode == "Mouse" || controlMode == "Both")
            {
                //Try catch due to us not knowing if the UI thead exists *it does not in unit tests*
                try
                {
                    //Set the new curser location
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        try
                        {
                            Point p = Mouse.GetPosition(gameCanvas);
                            cursor.FromLeft = (float)p.X - width / 2 + (player.FromLeft) + player.Width / 2;
                            cursor.FromTop = (float)p.Y - height / 2 + (player.FromTop) + player.Height / 2;
                        }
                        catch
                        {
                            Log.Warning("Could not find pointer location");
                        }
                    });
                }
                catch
                {
                    Log.Warning("Could not invoke thread");
                }
            }

            //Destory old objects by going trough each gameObject and checking if its destroyed.
            //If it is, remove it from the canvas.
            foreach (GameObject gameObject in loopList)
            {
                if (gameObject.destroyed)
                {

                    //If a gameObject is marked to be destroyed remove it from the list and remove them from the canvas
                    gameObjects.Remove(gameObject);

                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        gameCanvas.Children.Remove(gameObject.rectangle);
                        gameCanvas.Children.Remove(gameObject.textBlock);

                    });

                    // Give the gameObject back to the GameObjectFactoryFacade (to the pool)
                    GameObjectFactoryFacade.ReturnGameObject(gameObject);
                }
            }

            MovePlayer();
        }

        private void Draw()
        {
            //Create a new arraylist used to hold the gameobjects for this loop.
            //The copy is made so it does the ontick methods on all the objects even the onces destroyed in the proces.
            GameObjects loopList;
            lock (gameObjects)  //lock the list for duplication
                {
                    try
                    {
                        //Try to duplicate the list.
                        loopList = new GameObjects();
                        loopList.Add(cursor);
                        loopList.AddRange(gameObjects);
                        
                    }
                    catch
                    {
                        //if it failes for any reason skip this frame.
                        return;
                    }
                }

            //Run it in the UI thread
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                //gameCanvas.Children.Clear();

                //gameCanvas = new Canvas();

                //gameCanvas.Width = 1280;
                //gameCanvas.Height = 720;



                //Set the background to the backgroundbrush in the engine
                gameCanvas.Background = backgroundBrush;

                //Set the background rectangle on the rectangle of the engine.
                Rectangle bgRect = rectangle;

                //TODO: set the with automatic
                bgRect.Width = 1280;
                bgRect.Height = 720;

                //Set the bgRect to left upper corner
                Canvas.SetLeft(bgRect, 0);
                Canvas.SetTop(bgRect, 0);

                //If the canvas does not contain the BGRect, add it.
                if (!gameCanvas.Children.Contains(bgRect))
                    gameCanvas.Children.Insert(0, bgRect);

                //Go trough eatch object to see how to render them.
                for (int i = 0; i < loopList.Count; i++)
                {
                    GameObject gameObject = loopList[i];

                    //check if a gameObject is in the render distance.
                    if (player.distanceBetween(gameObject) > renderDistance)
                    {
                        //Remove the Rect and Textblock
                        gameCanvas.Children.Remove(gameObject.rectangle);
                        if (!(gameObject.textBlock is null))
                        {
                            gameCanvas.Children.Remove(gameObject.textBlock);
                        }
                    }
                    else
                    {
                        //It is in range so draw it.
                        Rectangle rect = gameObject.rectangle; //Set rect to the internal rectangle of a gameObject
              
                        //Set the width and hight of the shown rectangle.
                        rect.Width = gameObject.Width + gameObject.RightDrawOffset + gameObject.LeftDrawOffset;
                        rect.Height = gameObject.Height + gameObject.TopDrawOffset + gameObject.BottomDrawOffset;

                        // Set up the position in the window.
                        Canvas.SetLeft(rect, gameObject.FromLeft - gameObject.LeftDrawOffset - camera.GetFromLeft());
                        Canvas.SetTop(rect, gameObject.FromTop - gameObject.TopDrawOffset - camera.GetFromTop());
                        
                        // If the rect is not in the canvas yet
                        if (!gameCanvas.Children.Contains(rect))
                        {
                            Panel.SetZIndex(rect, (int)gameObject.FromBehind);
                            gameCanvas.Children.Add(rect); //Add at the end of the list
                        }

                        //Draw the textblock assosiated with the GameObject
                        if (!(gameObject.textBlock is null))
                        {
                            TextBlock textBlock = gameObject.textBlock; //Get the textBlock of the GameObject

                            Canvas.SetLeft(textBlock, gameObject.FromLeft - gameObject.LeftDrawOffset - camera.GetFromLeft());
                            Canvas.SetTop(textBlock, gameObject.FromTop - gameObject.TopDrawOffset - camera.GetFromTop());                    

                            // If the textBlock is not in the canvas yet
                            if (!gameCanvas.Children.Contains(textBlock))
                            {
                                Panel.SetZIndex(textBlock, (int)gameObject.FromBehind);
                                gameCanvas.Children.Add(textBlock);
                            }
                        }
                    }
                }           
            });    
        }

        /// <summary>
        /// This method calculates the direction in which the player need to move and set the target.
        /// This method is depending on the controllerAnchor and the cursor.
        /// </summary>
        private void MovePlayer()
        {
            // Only move the player if the mouse is down
            if (IsKeyPressed("LeftMouse"))
            {
                Target startTarget = new Target(controllerAnchor.FromLeft, controllerAnchor.FromTop);
                Target newTarget = new Target(cursor.FromLeft, cursor.FromTop);

                // Check the fromLeft difference between the target and the GameObject
                float differenceLeft;
                if (newTarget.FromLeft() - startTarget.FromLeft() == 0)
                {
                    // Set differenceLeft to very close to 0 to avoid errors with mathematics
                    differenceLeft = 0.000000001f;
                }
                else
                {
                    // Calc the difference between the two fromLefts of the Targets
                    differenceLeft = newTarget.FromLeft() - startTarget.FromLeft();
                }

                // Check the fromTop difference between the target and the GameObject
                float differenceTop;
                if (newTarget.FromTop() - startTarget.FromTop() == 0)
                {
                    // Set differenceTop to very close to 0 to avoid errors with mathematics
                    differenceTop = 0.0000000001f;
                }
                else
                {
                    // Calc the difference between the two fromTops of the Targets
                    differenceTop = newTarget.FromTop() - startTarget.FromTop();
                }

                // Move the controllerAnchor with the camera so it doesnt leave the screen
                controllerAnchor.FromLeft -= (camera.GetPrevFromLeft() - camera.GetFromLeft());
                controllerAnchor.FromTop -= (camera.GetPrevFromTop() - camera.GetFromTop());

                // Set the controllerCursor on the same position as the actual cursor
                controllerCursor.FromLeft = cursor.FromLeft;
                controllerCursor.FromTop = cursor.FromTop;

                // This adds  movementspeed to the player depending on the distance between the controllerAnchor and the controllerCursor. It's capped on 700!            
                player.MovementSpeed = 500 + controllerAnchor.distanceBetween(controllerCursor) > 700 ? 700 : 500 + controllerAnchor.distanceBetween(controllerCursor);

                // Set the target of the player to the current pos of the player to reset the target
                player.Target.SetFromLeft(player.FromLeft);
                player.Target.SetFromTop(player.FromTop);

                // Add the difference to the players target to move it in the right direction
                player.Target.AddFromLeft(differenceLeft * 20f);
                player.Target.AddFromTop(differenceTop * 20f);

            }
        }

        /* IsKeyPressed */
        /* 
         * Returns wheater the given key exists within the pressedKeys collection.
         * The argument is the given key represented as a string.
         */
        private Boolean IsKeyPressed(String virtualKey)
        {
            return pressedKeys.Contains(virtualKey);
        }

        /// <summary>
        /// Loops trough the maze and adds background objects where walls are.
        /// </summary>
        private void PopulateBackgroundObject()
        {
            //Loop trough the width
            for (int fromLeft = 0; fromLeft < MazeFacade.GetMazeWidth(); fromLeft++)
            {
                //Loop trough the height
                for (int fromTop = 0; fromTop < MazeFacade.GetMazeHeight(); fromTop++)
                {
                    //If its a wall draw an object
                    if (MazeFacade.IsWall(fromLeft, fromTop))
                    {
                        gameObjects.Add(GameObjectFactoryFacade.GetGameObject("tile", MazeFacade.tileSize * fromLeft, MazeFacade.tileSize * fromTop, -1));
                    }
                }
            }
        }

        #endregion

        /***************************************************************************
         * EVENTS
         * ************************************************************************/
        #region events

        /* KeyDown */
        /* 
        * Add the given key in the pressedKeys collection.
        * The argument is the given key represented as a string.
        */
        public void KeyDown(object sender, KeyEventArgs args)
        {
            pressedKeys.Add(args.Key.ToString());
            command.KeyPressed(args);           
        }


        /* KeyDown */
        /* 
         * Remove the given key in the pressedKeys collection.
         * The argument is the given key represented as a string.
         */
        public void KeyUp(object sender, KeyEventArgs args)
        {
            pressedKeys.Remove(args.Key.ToString());
        }

        /// <summary>
        /// This is the EventHandler of the MouseDownEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMouseDown(object sender, MouseButtonEventArgs args)
        {
            // If the Controlmode is not Mouse or Both return
            if (controlMode != "Mouse" && controlMode != "Both")
            {
                return;
            }

            // Set IsMouseDown on true
            pressedKeys.Add("LeftMouse");

            // Create the controller GameObjects (on the place of the mouse)
            controllerAnchor = GameObjectFactoryFacade.GetGameObject("ControllerAncher", cursor.FromLeft, cursor.FromTop, 4);
            gameObjects.Add(controllerAnchor);
            controllerCursor = GameObjectFactoryFacade.GetGameObject("ControllerCursor", cursor.FromLeft, cursor.FromTop, 5);
            gameObjects.Add(controllerCursor);

            // Calc the distance between the cursor to the border
            // This is to know where the player is standing
            float[] distances = new float[]
            {
                    // Cursor to top border
                    Math.Abs(camera.GetFromTop() - cursor.FromTop), 

                    // Cursor to right border
                    Math.Abs(camera.GetWidth()  +  camera.GetFromLeft() - cursor.FromLeft),

                    // Cursor to bottom border
                    Math.Abs(camera.GetHeight() +  camera.GetFromTop() - cursor.FromTop),

                    // Cursor to left border
                    Math.Abs(camera.GetFromLeft() - cursor.FromLeft)
            };

            // Check which distance is the lowest
            // 0 north, 1 east, 2 south, 3 west
            int lowestKey = 0;
            float lowestVal = cursor.FromTop;
            for (int i = 0; i < 4; i++)
            {
                if (distances[i] < lowestVal)
                {
                    lowestVal = distances[i];
                    lowestKey = i;
                }
            }
        
            // Save the value in an attibrute
            lastClickClosestBorder = lowestKey;
        }

        /// <summary>
        /// This is the EventHandler of the MouseUpEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMouseUp(object sender, MouseButtonEventArgs args)
        {
            // If the Controlmode is not Mouse or Both return
            if (controlMode != "Mouse" && controlMode != "Both")
            {
                return;
            }

            // Set IsMouseDown on false
            pressedKeys.Remove("LeftMouse");

            // Set the target of the player to the current position to stop it from moving
            player.Target.SetFromLeft(player.FromLeft);
            player.Target.SetFromTop(player.FromTop);

            // Remove the anchor for the controller
            if (!(controllerAnchor is null))
            {
                controllerAnchor.destroyed = true;
            }
            if (!(controllerCursor is null))
            {
                controllerCursor.destroyed = true;
            }
        }


        public void SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // if the width or height is changed call the camera to generate new width and height
            if (e.WidthChanged || e.HeightChanged)
            {
                camera.GenerateHeightAndWidth();
            }
        }

        /// <summary>
        /// MainWindow_Closing Event from the XAML
        /// </summary>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseApp();
        }

        /// <summary>
        /// Clears the pressed keys if you enter the mouse enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browser_MouseEnter(object sender, MouseEventArgs e)
        {
            pressedKeys.Clear();
        }

        private void ViewBox_TouchDown(object sender, TouchEventArgs e)
        {
            // If the Controlmode is not Mouse or Both return
            if (controlMode != "Touch" && controlMode != "Both")
            {
                return;
            }

            // Reset the controlls
            ResetControls();

            Point p = e.GetTouchPoint((IInputElement)gameCanvas).Position;
            cursor.FromLeft = (float)p.X + camera.GetFromLeft();
            cursor.FromTop = (float)p.Y + camera.GetFromTop();

            // Set IsMouseDown on true
            pressedKeys.Add("LeftMouse");

            // Create the controller GameObjects (on the place of the mouse)
            controllerAnchor = GameObjectFactoryFacade.GetGameObject("ControllerAncher", cursor.FromLeft, cursor.FromTop, 4);
            gameObjects.Add(controllerAnchor);
            controllerCursor = GameObjectFactoryFacade.GetGameObject("ControllerCursor", cursor.FromLeft, cursor.FromTop, 5);
            gameObjects.Add(controllerCursor);

            // Calc the distance between the cursor to the border
            // This is to know where the player is standing
            float[] distances = new float[]
            {
                    // Cursor to top border
                    Math.Abs(camera.GetFromTop() - cursor.FromTop), 

                    // Cursor to right border
                    Math.Abs(camera.GetWidth()  +  camera.GetFromLeft() - cursor.FromLeft),

                    // Cursor to bottom border
                    Math.Abs(camera.GetHeight() +  camera.GetFromTop() - cursor.FromTop),

                    // Cursor to left border
                    Math.Abs(camera.GetFromLeft() - cursor.FromLeft)
            };

            // Check which distance is the lowest
            // 0 north, 1 east, 2 south, 3 west
            int lowestKey = 0;
            float lowestVal = cursor.FromTop;
            for (int i = 0; i < 4; i++)
            {
                if (distances[i] < lowestVal)
                {
                    lowestVal = distances[i];
                    lowestKey = i;
                }
            }

            // Save the value in an attibrute
            lastClickClosestBorder = lowestKey;
        }

        private void ViewBox_TouchUp(object sender, TouchEventArgs e)
        {
            // If the Controlmode is not Mouse or Both return
            if (controlMode != "Touch" && controlMode != "Both")
            {
                return;
            }

            // Set IsMouseDown on false
            pressedKeys.Remove("LeftMouse");

            // Set the target of the player to the current position to stop it from moving
            player.Target.SetFromLeft(player.FromLeft);
            player.Target.SetFromTop(player.FromTop);

            // Remove the anchor for the controller
            if (!(controllerAnchor is null))
            {
                controllerAnchor.destroyed = true;
            }
            if (!(controllerCursor is null))
            {
                controllerCursor.destroyed = true;
            }
        }

        #endregion

        private void ViewBox_TouchMove(object sender, TouchEventArgs e)
        {
            // If the Controlmode is not Mouse or Both return
            if (controlMode != "Touch" && controlMode != "Both")
            {
                return;
            }

            Point p = e.GetTouchPoint((IInputElement)gameCanvas).Position;
            cursor.FromLeft = (float)p.X + camera.GetFromLeft();
            cursor.FromTop = (float)p.Y + camera.GetFromTop();
        }
    }
}
