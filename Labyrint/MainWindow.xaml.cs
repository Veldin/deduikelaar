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
using LogSystem;
using GameObjectFactory;
using Maze;


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
        private bool IsMouseDown;

        //Camera
        private float cameraLeftOffset = 0;
        private float cameraTopOffset = 0;

        //The brush used to fill in the background
        SolidColorBrush backgroundBrush;

        public Random random;

        private GameObject player;

        private GameObject cursor;

        private List<GameObject> gameObjects;
        private List<GameObject> backgroundObjects;


        string assemblyName;

        private int renderDistance;

        private Rectangle rectangle;

        public MainWindow()
        {
            InitializeComponent();

            // Add the OnMouseDown and the OnMouseUp as event handler
            AddHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), true);
            AddHandler(FrameworkElement.MouseUpEvent, new MouseButtonEventHandler(OnMouseUp), true);

            //Bind the KeyUp and KeyDown methods.
            Window.GetWindow(this).KeyUp += KeyUp;
            Window.GetWindow(this).KeyDown += KeyDown;

            GameObjectFactoryFacade.innit();
            MazeFacade.init();

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                rectangle = new Rectangle();

                rectangle = new Rectangle();
                rectangle.Stroke = new SolidColorBrush(Color.FromRgb(0, 111, 111));
            }));

            //WindowStyle = WindowStyle.None;
            //Topmost = true;
            //WindowState = WindowState.Maximized;

            width = 1280;
            height = 720;

            assemblyName = Assembly.GetEntryAssembly().GetName().Name;

            pressedKeys = new HashSet<String>();

            //player = new GameObject(24 * 2.5f, 42 * 2.5f, 300, 300);
            player = GameObjectFactoryFacade.GetGameObject("player", 300, 300);

            this.Cursor = Cursors.None;
            cursor = GameObjectFactoryFacade.GetGameObject("cursor", 300, 300);

            //player.Target = new Target(500,500);
            //player.Target = new Target(player);
            //Debug.WriteLine(player.Target.FromLeft());
            //player.Target.AddFromLeft(20000);
            //player.Target.AddFromTop(20000);

            //Debug.WriteLine(player.Target.FromLeft());

            gameObjects = new List<GameObject>();

            backgroundObjects = new List<GameObject>();

            //create a cell on everfromTop place in the double arrafromTop
            for (int fromLeft = 0; fromLeft < MazeFacade.GetMazeWidth(); fromLeft++)
            {
                for (int fromTop = 0; fromTop < MazeFacade.GetMazeHeight(); fromTop++)
                {
                    if (MazeFacade.isWall(fromLeft, fromTop))
                    {
                        backgroundObjects.Add(GameObjectFactoryFacade.GetGameObject("tile", MazeFacade.tileSize * fromLeft, MazeFacade.tileSize * fromTop));
                    }
                    else
                    {
                        
                    }
                }
            }


            gameObjects.Add(player);

            //backgroundBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 112, 192, 160));
            backgroundBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 24, 40, 80));


            //gameObjects.Add(new TextBox(42, 36, 300, 300,0,0,0,0, "text loltext loltext lol"));

            renderDistance = 1200;

            InitializeComponent();

            Window.GetWindow(this).KeyUp += KeyUp;
            Window.GetWindow(this).KeyDown += KeyDown;

            fps = 999999999; //Desired max fps.
            interval = 1000 / fps;
            then = Stopwatch.GetTimestamp();
            Run();
        }

        public void Run()
        {
            if (random is null)
            {
                random = new Random();
            }

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
                    loopList.Add(cursor);
                }
                catch
                {
                    //if it failes for any reason skip this frame.
                    return;
                }
            }

            //set the camera on the player position
            cameraLeftOffset = player.FromLeft - (width / 2) + player.Width / 2;
            cameraTopOffset = player.FromTop - (height / 2) + player.Height / 2;


            player.Target.SetTarget(player, true);
            if (pressedKeys.Contains("W"))
            {
                player.Target.AddFromTop(-5000);
            }
            if (pressedKeys.Contains("A"))
            {
                player.Target.AddFromLeft(-5000);
            }
            if (pressedKeys.Contains("S"))
            {
                player.Target.AddFromTop(5000);
            }
            if (pressedKeys.Contains("D"))
            {
                player.Target.AddFromLeft(5000);
            }

            //For every gameobject in the room
            foreach (GameObject gameObject in loopList)
            {
                //OnTick every gameObject
                if (IsKeyPressed("Space"))
                {
                    gameObject.OnTick(gameObjects, pressedKeys, delta * 5f);
                }
                else
                {
                    gameObject.OnTick(gameObjects, pressedKeys, delta);
                }
            }

            //Set the new curser location
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                Point p = Mouse.GetPosition(TestCanvas);
                cursor.FromLeft = (float)p.X - width / 2 + (player.FromLeft) + player.Width / 2;
                cursor.FromTop = (float)p.Y - height / 2 + (player.FromTop) + player.Height / 2;
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
                        TestCanvas.Children.Remove(gameObject.rectangle);
                    });
                }
            }
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
                    loopList = new ArrayList(gameObjects);
                    loopList.AddRange(backgroundObjects);

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
                //TestCanvas.Children.Clear();    //Remove all recs from the canvas, start clean every loop

                TestCanvas.Background = backgroundBrush;

                Rectangle bgRect = rectangle;

                bgRect.Width = 1280;
                bgRect.Height = 720;

                Canvas.SetLeft(bgRect, 0);
                Canvas.SetTop(bgRect, 0);

                if (!TestCanvas.Children.Contains(bgRect))
                    TestCanvas.Children.Add(bgRect);

                foreach (GameObject gameObject in loopList)
                {

                    if (player.distanceBetween(gameObject) > renderDistance)
                    {
                        TestCanvas.Children.Remove(gameObject.rectangle);
                    }
                    else
                    {
                        if (!(gameObject is TextBox))
                        {
                            Rectangle rect = gameObject.rectangle;

                            rect.Width = gameObject.Width + gameObject.RightDrawOffset + gameObject.LeftDrawOffset;
                            rect.Height = gameObject.Height + gameObject.TopDrawOffset + gameObject.BottomDrawOffset;

                            // Set up the position in the window, at mouse coordonate
                            Canvas.SetLeft(rect, gameObject.FromLeft - gameObject.LeftDrawOffset - cameraLeftOffset);
                            Canvas.SetTop(rect, gameObject.FromTop - gameObject.TopDrawOffset - cameraTopOffset);

                            if (!TestCanvas.Children.Contains(rect))
                                TestCanvas.Children.Add(rect);
                        }

                    }
                }
            });
        }


        /* KeyDown */
        /* 
            * Add the given key in the pressedKeys collection.
            * The argument is the given key represented as a string.
            */
        public void KeyDown(object sender, KeyEventArgs args)
        {
            pressedKeys.Add(args.Key.ToString());
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
            Log.Debug("Mouse is down");
            // Set IsMouseDown on true
            IsMouseDown = true;
        }

        /// <summary>
        /// This is the EventHandler of the MouseUpEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMouseUp(object sender, MouseButtonEventArgs args)
        {
            Log.Debug("Mouse is up");
            // Set IsMouseDown on false
            IsMouseDown = false;

        }
    }
}
