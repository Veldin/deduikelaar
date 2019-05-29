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
using BrowserUIControl;
using Settings;

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
        SolidColorBrush backgroundBrush;

        public Random random;

        private GameObject player;

        private GameObject cursor;

        private List<GameObject> gameObjects;
        private List<GameObject> backgroundObjects;

        //Strategies
        public List<IBehaviour> onTickList;

        string assemblyName;

        private int renderDistance;

        private Rectangle rectangle;

        private GameObject controllerAnchor;
        private GameObject controllerCursor;

        private Command command;

        public MainWindow()
        {
            InitializeComponent();

            // Add the OnMouseDown and the OnMouseUp as event handler
            AddHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), true);
            AddHandler(FrameworkElement.MouseUpEvent, new MouseButtonEventHandler(OnMouseUp), true);

            //Bind the KeyUp and KeyDown methods.
            Window.GetWindow(this).KeyUp += KeyUp;
            Window.GetWindow(this).KeyDown += KeyDown;
            Window.GetWindow(this).SizeChanged += SizeChanged;

            FileReaderWriterFacade.Init();
            SettingsFacade.Init();
            GameObjectFactoryFacade.innit();
            MazeFacade.Init();
            ApiParserFacade.Init();

            //Log.Debug(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Labyrint\\test.txt");
            //FileReaderWriterFacade.WriteText(new string[] { "bla" }, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Labyrint\\test.txt", false);

            // Create the camera
            camera = new Camera(gameCanvas, mainWindow);

            command = new Command(CommandBar, CommandResponse);

            random = new Random();

            browser.Navigate(new Uri(FileReaderWriterFacade.GetAppDataPath()));      //Inits a new navigate call


            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                rectangle = new Rectangle();
                rectangle.Stroke = new SolidColorBrush(Color.FromRgb(0, 111, 111));
            }));

            //Set the windowsstyle (,the bar above the application).
            switch (SettingsFacade.Get("WindowsStyle", "ToolWindow"))
            {
                case "None":
                    WindowStyle = WindowStyle.None;
                    break;
                default:
                    WindowStyle = WindowStyle.ToolWindow;
                    break;
            }

            //Set the windowState.
            switch (SettingsFacade.Get("WindowState", "Normal"))
            {
                case "Maximized":
                    WindowState = WindowState.Maximized;
                    break;
                case "Minimized":
                    WindowState = WindowState.Minimized;
                    break;
                default:
                    WindowState = WindowState.Normal;
                    break;
            }

            width = 1280;
            height = 720;

            assemblyName = Assembly.GetEntryAssembly().GetName().Name;

            pressedKeys = new HashSet<String>();

            //player = new GameObject(24 * 2.5f, 42 * 2.5f, 300, 300);
            player = GameObjectFactoryFacade.GetGameObject("player", MazeFacade.tileSize, MazeFacade.tileSize);

            this.Cursor = Cursors.None;
            cursor = GameObjectFactoryFacade.GetGameObject("cursor", 300, 300);

            //player.Target = new Target(500,500);
            //player.Target = new Target(player);
            //Debug.WriteLine(player.Target.FromLeft());
            //player.Target.AddFromLeft(20000);
            //player.Target.AddFromTop(20000);

            //Debug.WriteLine(player.Target.FromLeft());

            gameObjects = new List<GameObject>();

            gameObjects.Add(player);
            gameObjects.Add(cursor);


            //gameObjects.Add(GameObjectFactoryFacade.GetGameObject("pickup", 300, 300));


            for (int fromLeft = 0; fromLeft < MazeFacade.GetMazeWidth(); fromLeft++)
            {
                for (int fromTop = 0; fromTop < MazeFacade.GetMazeHeight(); fromTop++)
                {
                    if (!MazeFacade.IsWall(fromLeft, fromTop))
                    {
                        //gameObjects.Add(GameObjectFactoryFacade.GetGameObject("pickup", MazeFacade.tileSize * fromLeft, MazeFacade.tileSize * fromTop));
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                DropNewPickup();
            }

            TestBrowser();

            //PopulateButtonObject();

            backgroundObjects = new List<GameObject>();
            populateBackgroundObject();


            onTickList = new List<IBehaviour>();
            onTickList.Add(new SpaceButtonsHorisontallyBehaviour());

            //backgroundBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 112, 192, 160));
            backgroundBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 110, 155, 178));

            renderDistance = SettingsFacade.Get("RenderDistance", 1200);//Desired max fps.

            TestBrowser();
            
            fps = SettingsFacade.Get("fps", 999);//Desired max fps.
            interval = 1000 / fps;

            SettingsFacade.Save();

            then = Stopwatch.GetTimestamp();
            Run();
        }


        private void TestBrowser()
        {
            //browser.Refresh();
            var str = "<html><head></head><body>sdf</body></html>";
            //browser.NavigateToString(str);

            //browser = new System.Windows.Controls.WebBrowser();
            //browser.Visibility = Visibility.Visible;
            browser.Navigate(new Uri(FileReaderWriterFacade.GetAppDataPath())); //Inits a new navigate call

            browser.NavigateToString("<HTML><H2><B>This page comes using String</B><P></P></H2>");

            //browser.NavigateToString(str);
        }

        public void Run()
        {
            now = Stopwatch.GetTimestamp();
            delta = (now - then) / 1000; //Defide by 1000 to get the delta in MS

            if (delta > interval)
            {
                then = now; //Remember when this frame was.
                Logic(delta); //Run the logic of the simulation.
                Draw();
            }
            else
            {
                Thread.Sleep(1); //Sleep the thread so time is passed
            }

            Task.Yield();  //Force this task to complete asynchronously (This way the main thread is not blocked by this task calling itself.
            Task.Run(() => Run());  //Schedule new Run() task
        }

        private void Logic(long delta)
        {
            //Create a new arraylist used to hold the gameobjects for this loop.
            //The copy is made so it does the ontick methods on all the objects even the onces destroyed in the proces.
            ArrayList loopList;
            lock (gameObjects) //lock the gameobjects for duplication
            {
                try
                {
                    //Try to duplicate the arraylist.
                    loopList = new ArrayList(gameObjects);
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

            //For every gameobject in the room
            foreach (GameObject gameObject in loopList)
            {
                //OnTick every gameObject
                if (IsKeyPressed("Space"))
                {
                    gameObject.OnTick(gameObjects, pressedKeys, delta * 7f);
                }
                else
                {
                    gameObject.OnTick(gameObjects, pressedKeys, delta);
                }

                //If the key is down 
                if (IsKeyPressed("LeftMouse"))
                {

                }
            }

            //OnTick all the engine behaiviors
            if (onTickList.Count > 0)
            {
                foreach (IBehaviour behaivior in onTickList)
                {
                    behaivior.OnTick(gameObjects, delta);
                }
            }

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

            //Destory old objects
            foreach (GameObject gameObject in loopList)
            {
                if (gameObject.destroyed)
                {
                    //Do the deathrattle effect
                    //gameObject.OnDeath(GameObjects, pressedKeys);

                    //If a gameObject is marked to be destroyed remove it from the list and remove them from the canvas
                    gameObjects.Remove(gameObject);

                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        gameCanvas.Children.Remove(gameObject.rectangle);
                    });

                    GameObjectFactoryFacade.ReturnGameObject(gameObject);
                }
            }

            MovePlayer();

        }


        private void Draw()
        {
            //Create a new arraylist used to hold the gameobjects for this loop.
            //The copy is made so it does the ontick methods on all the objects even the onces destroyed in the proces.
            ArrayList loopList;
            lock (gameObjects) lock (backgroundObjects) //lock the gameobjects for duplication
                    {
                try
                {
                    //Try to duplicate the arraylist.
                    loopList = new ArrayList(backgroundObjects);
                    loopList.AddRange(gameObjects);

                    loopList.Add(cursor); 
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
                gameCanvas.Background = backgroundBrush;

                Rectangle bgRect = rectangle;

                bgRect.Width = 1280;
                bgRect.Height = 720;

                Canvas.SetLeft(bgRect, 0);
                Canvas.SetTop(bgRect, 0);

                //If the canvas does not contain the BGRect, add it.
                if (!gameCanvas.Children.Contains(bgRect))
                    gameCanvas.Children.Insert(0, bgRect);

                foreach (GameObject gameObject in loopList)
                {
                    //check if a gameObject is in the render distance.
                    if (player.distanceBetween(gameObject) > renderDistance)
                    {
                        gameCanvas.Children.Remove(gameObject.rectangle);
                        if (!(gameObject.textBlock is null))
                        {
                            gameCanvas.Children.Remove(gameObject.textBlock);
                        }
                    }
                    else
                    {
                        //It is in range so draw it.
                        Rectangle rect = gameObject.rectangle;

                        rect.Width = gameObject.Width + gameObject.RightDrawOffset + gameObject.LeftDrawOffset;
                        rect.Height = gameObject.Height + gameObject.TopDrawOffset + gameObject.BottomDrawOffset;

                        // Set up the position in the window.
                        Canvas.SetLeft(rect, gameObject.FromLeft - gameObject.LeftDrawOffset - camera.GetFromLeft());
                        Canvas.SetTop(rect, gameObject.FromTop - gameObject.TopDrawOffset - camera.GetFromTop());

                        if (!gameCanvas.Children.Contains(rect))
                        {
                            //If the gameobject is important to be seen add it to the end of the array
                            if (gameObject.highVisibility)
                            {
                                gameCanvas.Children.Add(rect);
                            }
                            else
                            {
                                gameCanvas.Children.Insert(0,rect);
                            }
                        }

                        //Draw the textblock assosiated with the GameObject
                        if (!(gameObject.textBlock is null))
                        {
                            TextBlock textBlock = gameObject.textBlock;

                            Canvas.SetLeft(textBlock, gameObject.FromLeft - gameObject.LeftDrawOffset - camera.GetFromLeft());
                            Canvas.SetTop(textBlock, gameObject.FromTop - gameObject.TopDrawOffset - camera.GetFromTop());

                            if (!gameCanvas.Children.Contains(textBlock))
                            {
                                //If the gameobject is important to be seen add it to the end of the array
                                if (gameObject.highVisibility)
                                {
                                    gameCanvas.Children.Add(textBlock);
                                }
                                else
                                {
                                    gameCanvas.Children.Insert(0, textBlock);
                                }
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

        /* IsKeyPressed */
        /* 
         * Returns wheater the given key exists within the pressedKeys collection.
         * The argument is the given key represented as a string.
         */
        public Boolean IsKeyPressed(String virtualKey)
        {
            return pressedKeys.Contains(virtualKey);
        }

        /// <summary>
        /// This is the EventHandler of the MouseDownEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMouseDown(object sender, MouseButtonEventArgs args)
        {
            // Set IsMouseDown on true
            pressedKeys.Add("LeftMouse");

            // Create the controller GameObjects
            controllerAnchor = GameObjectFactoryFacade.GetGameObject("ControllerAncher", cursor.FromLeft, cursor.FromTop);
            gameObjects.Add(controllerAnchor);
            controllerCursor = GameObjectFactoryFacade.GetGameObject("ControllerCursor", cursor.FromLeft , cursor.FromTop );
            gameObjects.Add(controllerCursor);
        }

        /// <summary>
        /// This is the EventHandler of the MouseUpEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMouseUp(object sender, MouseButtonEventArgs args)
        {
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

        /// <summary>
        /// Loops trough the maze and adds background objects where walls are.
        /// </summary>
        private void populateBackgroundObject()
        {
            for (int fromLeft = 0; fromLeft < MazeFacade.GetMazeWidth(); fromLeft++)
            {
                for (int fromTop = 0; fromTop < MazeFacade.GetMazeHeight(); fromTop++)
                {
                    if (MazeFacade.IsWall(fromLeft, fromTop))
                    {
                        backgroundObjects.Add(GameObjectFactoryFacade.GetGameObject("tile", MazeFacade.tileSize * fromLeft, MazeFacade.tileSize * fromTop));
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void PopulateButtonObject()
        {
            int max = 4;
            for (int i = 0; i < max; i++)
            {
                gameObjects.Add(GameObjectFactoryFacade.GetGameObject(
                    "button",
                    i * 100 - (100),
                    i,
                    camera
                ));
            }
        }

        /// <summary>
        /// Creates a new pickup somewere in the maze.
        /// </summary>
        private void DropNewPickup()
        {
            GameObject newPickup; //holds the new pickup
            do
            {
                int randomFromTop, randomFromLeft;
                do
                {
                    //Get a random wall position
                    randomFromTop = random.Next(MazeFacade.GetMazeHeight());
                    randomFromLeft = random.Next(MazeFacade.GetMazeWidth());
                }while (MazeFacade.IsWall(randomFromTop, randomFromLeft)); //If its a wall pick a new location
                
                // create the pickup
                newPickup = GameObjectFactoryFacade.GetGameObject(
                    "pickup", 
                    randomFromLeft * (MazeFacade.tileSize) + MazeFacade.tileSize / 2 , 
                    randomFromTop * (MazeFacade.tileSize) + MazeFacade.tileSize / 2,
                    new object[2] { browser, camera }
                );
            } while (newPickup.distanceBetween(player) < 300); //if its to close to the player pick a new location

            gameObjects.Add(newPickup);
        }

        public void SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //camera.
            if (e.WidthChanged || e.HeightChanged)
            {
                camera.GenerateHeightAndWidth();
            }
            Log.Debug("changed" + camera.GetWidth());
        }

        /// <summary>
        /// CloseApp gets called from the mainWindow_Closing event.
        /// </summary>
        public async void CloseApp()
        {
            await ApiParserFacade.InformApiAsync();
            SettingsFacade.Save();
            this.Close();
        }

        /// <summary>
        /// MainWindow_Closing Event from the XAML
        /// </summary>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseApp();
        }
    }
}
