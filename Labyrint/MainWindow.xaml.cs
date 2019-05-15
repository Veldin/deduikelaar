﻿using System;
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

        string assemblyName;

        private int renderDistance;

        private Rectangle rectangle;

        private GameObject controllerAnchor;
        private GameObject controllerCursor;

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
            MazeFacade.Init();

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

            player.Target = new Target(player, true);
            //player.Target = new Target(player);
            //Debug.WriteLine(player.Target.FromLeft());
            //player.Target.AddFromLeft(20000);
            //player.Target.AddFromTop(20000);

            //Debug.WriteLine(player.Target.FromLeft());

            gameObjects = new List<GameObject>();

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
                        if (gameObject is TextBox)
                        {
                            //TextBox textblock = gameObject as TextBox;
                            //TestCanvas.Children.Remove(textblock.textblock);
                        }
                    });
                }
            }

           MovePlayer();
        }


        private void Draw()
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

        /// <summary>
        /// This method calculates the direction in which the player need to move and set the target.
        /// This method is depending on the controllerAnchor and the cursor.
        /// </summary>
        private void MovePlayer()
        {
            // Only move the player if the mouse is down
            if (IsMouseDown)
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

                // Create a new Target for the player in the right direction
                player.Target = new Target(differenceLeft * 20f, differenceTop * 20f);

                // Set the controllerCursor on the same position as the actual cursor
                controllerCursor.FromLeft = cursor.FromLeft;
                controllerCursor.FromTop = cursor.FromTop;
            }
        }


        /* KeyDown */
        /* 
        * Add the given key in the pressedKeys collection.
        * The argument is the given key represented as a string.
        */
        public void KeyDown(object sender, KeyEventArgs args)
        {
            Debug.WriteLine(args.Key.ToString());
        }

        /* KeyDown */
        /* 
         * Remove the given key in the pressedKeys collection.
         * The argument is the given key represented as a string.
         */
        public void KeyUp(object sender, KeyEventArgs args)
        {
            Debug.WriteLine(args.Key.ToString());
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
            IsMouseDown = true;

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
            IsMouseDown = false;

            // Set the target of the player to the current position to stop it from moving
            player.Target.SetFromLeft(player.FromLeft);
            player.Target.SetFromTop(player.FromTop);

            // Remove the anchor for the controller
            controllerAnchor.destroyed = true;
            controllerCursor.destroyed = true;
        }
    }
}
