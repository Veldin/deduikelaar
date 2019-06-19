using Labyrint;
using LogSystem;
using Maze;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameObjectFactory
{
    static class GameObjectStatic
    {
        public static Dictionary<string, BitmapImage> maps = new Dictionary<string, BitmapImage>();
    }

    public class GameObject
    {
        private Dictionary<string, BitmapImage> bitmaps;

        // The target this gameobject uses.
        // This gets used to know where the gameObject wants to go.
        protected Target target;

        //List with Strategies that get run onTick
        public List<IBehaviour> onTickList;

        // Location where this gameObject is within the game.
        // This is also used for the hitbox 
        private float width;
        private float height;
        private float fromLeft;
        private float fromTop;

        // Offset where to draw the gameObject in the game.
        // The Sprite can be bigger or smaller then the hitbox.
        // The sprite can be more to the left or right then the hitbox.
        private float leftDrawOffset;
        private float rightDrawOffset;
        private float topDrawOffset;
        private float bottomDrawOffset;

        // Rectangle and Textblock can get rendered in the canvas.
        // The rectangle always gets rendered and the textblock only if its not null
        public Rectangle rectangle;
        public TextBlock textBlock;

        // Holds the assemblyName to locate the sprites
        public string assemblyName;

        // Holds an instance of the random class
        public Random random;

        // If this boolean is set to true the engine will consider this to be destroyed.
        public Boolean destroyed;

        // This boolean dictates if the engine should treat this object as important to see.
        public Boolean highVisibility;

        // This is the movementSpeed in units that this gameObject can go
        private float movementSpeed;

        // GameObjects can be internaly grouped
        private int group;

        // If the collition boolean is false the move methods wont check for collition bevore moving the gameObject
        private Boolean collision;

        //Holds the string the builder used to make this object.
        private string builderType;

        //The sprite location and the CanvasBitmap are stored seperatly
        //This is so the location gets changed more times in a frame the canvasBitmap doesn't have to get loaded more then once a frame.
        //protected CanvasBitmap sprite;
        private string location;    

        public GameObject(float width = 0, float height = 0, float fromLeft = 0, float fromTop = 0)
        {
            //bitmaps = new Dictionary<string, BitmapImage>();
            bitmaps = GameObjectStatic.maps;
            random = new Random(GetHashCode() + (int)DateTime.UtcNow.Ticks);

            this.width = width;
            this.height = height;
            this.fromLeft = fromLeft;
            this.fromTop = fromTop;

            onTickList = new List<IBehaviour>();

            destroyed = false;

            //Dictates of this object is important to be visible
            highVisibility = true;

            movementSpeed = 0;
            group = 0;

            try{
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    rectangle = new Rectangle();
                }));
            }catch
            {
                Log.Warning("Can't invoke Application.Current.Dispatcher.");
            }

            assemblyName = "Labyrint";

            //Default location of the sprite.
            //setActiveBitmap("Assets/redrand.png");
            //location = "Assets/redrand.png";
            //setActiveBitmap(location);
        }

        public String Location
        {
            get { return location; }
            set { location = value; }
        }

        //Getters and setters for the fields that have to do with positioning of the GameObject.
        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        public float Height
        {
            get { return height; }
            set { height = value; }
        }

        public float FromLeft
        {
            get { return fromLeft; }
            set { fromLeft = value; }
        }

        public float FromTop
        {
            get { return fromTop; }
            set { fromTop = value; }
        }

        //Methods to add ammounts to fields that have to do with positioning of the GameOgject.
        //They also return the new number so we can use them to calculate with instandly.
        public float AddWidth(float width)
        {
            this.width += width; return this.width;
        }

        public float AddHeight(float height)
        {
            this.height += height; return this.height;
        }

        public float AddFromTop(float fromTop)
        {
            return AddFromTop(fromTop, collision);
        }

        public float AddFromTop(float fromTop, Boolean collition)
        {
            this.fromTop += fromTop;

            if (collision)
            {
                if(MazeFacade.IsWall((int)this.fromLeft / MazeFacade.tileSize, (int)this.fromTop / MazeFacade.tileSize))
                {
                    this.fromTop -= fromTop;
                }

                if (MazeFacade.IsWall((int)this.fromLeft / MazeFacade.tileSize, (int)(this.fromTop + this.Height) / MazeFacade.tileSize))
                {
                    this.fromTop -= fromTop;
                }

                if (MazeFacade.IsWall((int)(this.fromLeft + this.Width) / MazeFacade.tileSize, (int)this.fromTop / MazeFacade.tileSize))
                {
                    this.fromTop -= fromTop;
                }

                if (MazeFacade.IsWall((int)(this.fromLeft + this.Width) / MazeFacade.tileSize, (int)(this.fromTop + this.Height) / MazeFacade.tileSize))
                {
                    this.fromTop -= fromTop;
                }
            }

            return this.fromTop;
        }

        public float AddFromLeft(float fromTop)
        {
            return AddFromLeft(fromTop, collision);
        }

        public float AddFromLeft(float fromLeft, Boolean collision)
        {
            this.fromLeft += fromLeft;

            if (collision)
            {
                if (MazeFacade.IsWall((int)this.fromLeft / MazeFacade.tileSize, (int)this.fromTop / MazeFacade.tileSize))
                {
                    this.fromLeft -= fromLeft;
                }

                if (MazeFacade.IsWall((int)(this.fromLeft + this.Width) / MazeFacade.tileSize, (int)this.fromTop / MazeFacade.tileSize))
                {
                    this.fromLeft -= fromLeft;
                }

                if (MazeFacade.IsWall((int)this.fromLeft / MazeFacade.tileSize, (int)(this.fromTop + this.Height) / MazeFacade.tileSize))
                {
                    this.fromLeft -= fromLeft;
                }

                if (MazeFacade.IsWall((int)(this.fromLeft + this.Width) / MazeFacade.tileSize, (int)(this.fromTop + this.Height) / MazeFacade.tileSize))
                {
                    this.fromLeft -= fromLeft;
                }
            }

            return this.fromLeft;
        }

        //Getters and setters for the fields that have to do with positioning in the canvas.
        public float LeftDrawOffset
        {
            get { return leftDrawOffset; }
            set { leftDrawOffset = value; }
        }

        public float RightDrawOffset
        {
            get { return rightDrawOffset; }
            set { rightDrawOffset = value; }
        }

        public float BottomDrawOffset
        {
            get { return bottomDrawOffset; }
            set { bottomDrawOffset = value; }
        }

        public float TopDrawOffset
        {
            get { return topDrawOffset; }
            set { topDrawOffset = value; }
        }

        public Target Target
        {
            get { return target; }
            set { target = value; }
        }

        public float MovementSpeed
        {
            get { return movementSpeed; }
            set { movementSpeed = value; }
        }

        public int Group
        {
            get { return group; }
            set { group = value; }
        }

        public string BuilderType
        {
            get { return builderType; }
            set { builderType = value; }
        }

        public Random Random
        {
            get { return random; }
            set { random = value; }
        }

        public Boolean Collition
        {
            get { return collision; }
            set { collision = value; }
        }

        public float DifferenceLeftAbs()
        {
            if (target.FromLeft() - (FromLeft + Width / 2) == 0)
            {
                return 0;
            }
            else
            {
                return Math.Abs(target.FromLeft() - (FromLeft + (Width / 2)));
            }
        }

        public float DifferenceLeftAbs(GameObject gameObject)
        {
            if ((gameObject.FromLeft + (gameObject.Width / 2)) - (FromLeft + (Width / 2)) == 0)
            {
                return 0;
            }
            else
            {
                return Math.Abs((gameObject.FromLeft + (gameObject.Width / 2)) - (FromLeft + (Width / 2)));
            }
        }

        public float DifferenceTopAbs()
        {
            if (target.FromTop() - (FromTop + Width / 2) == 0)
            {
                return 0;
            }
            else
            {
                return Math.Abs(target.FromTop() - (FromTop + (Width / 2)));
            }
        }

        public float DifferenceTopAbs(GameObject gameObject)
        {
            if ((gameObject.FromTop + (gameObject.Width / 2)) - (FromTop + (Width / 2)) == 0)
            {
                return 0;
            }
            else
            {
                return Math.Abs((gameObject.FromTop + (gameObject.Width / 2)) - (FromTop + (Width / 2)));
            }
        }

        public float distanceBetween()
        {
            float differenceLeftAbs;
            if (Target.FromLeft() - (FromLeft + (Width / 2)) == 0)
            {
                differenceLeftAbs = 0;
            }
            else
            {
                differenceLeftAbs = Math.Abs(Target.FromLeft() - (FromLeft + (Width / 2)));
            }

            float differenceTopAbs;
            if (Target.FromTop() - (FromTop + (Height / 2)) == 0)
            {
                differenceTopAbs = 0;
            }
            else
            {
                differenceTopAbs = Math.Abs(Target.FromTop() - (FromTop + (Height / 2)));
            }

            return differenceLeftAbs + differenceTopAbs;
        }

        public float distanceBetween(GameObject gameObject)
        {
            float differenceLeftAbs;
            if (gameObject.FromLeft - (FromLeft + (Width / 2)) == 0)
            {
                differenceLeftAbs = 0;
            }
            else
            {
                differenceLeftAbs = Math.Abs(gameObject.FromLeft - (FromLeft + (Width / 2)));
            }

            float differenceTopAbs;
            if (gameObject.FromTop - (FromTop + (Height / 2)) == 0)
            {
                differenceTopAbs = 0;
            }
            else
            {
                differenceTopAbs = Math.Abs(gameObject.FromTop - (FromTop + (Height / 2)));
            }

            return differenceLeftAbs + differenceTopAbs;
        }

        public float DistanceBetween(float fromLeft, float fromTop)
        {
            float differenceLeftAbs;
            if (fromLeft - (FromLeft + (Width / 2)) == 0)
            {
                differenceLeftAbs = 0;
            }
            else
            {
                differenceLeftAbs = Math.Abs(fromLeft - (FromLeft + (Width / 2)));
            }

            float differenceTopAbs;
            if (fromTop - (FromTop + (Height / 2)) == 0)
            {
                differenceTopAbs = 0;
            }
            else
            {
                differenceTopAbs = Math.Abs(fromTop - (FromTop + (Height / 2)));
            }

            return differenceLeftAbs + differenceTopAbs;
        }

        public BitmapImage getActiveBitmap(String assemblyName, Boolean reload = false)
        {

            if (!bitmaps.ContainsKey(Location))
            {
                Log.Debug("Loading: pack://application:,,,/" + assemblyName + ";component/" + Location);
                BitmapImage newBitmap;
                try {
                    newBitmap = new BitmapImage(new Uri("pack://application:,,,/" + assemblyName + ";component/" + Location, UriKind.Absolute));
                }
                catch
                {
                    newBitmap = new BitmapImage(new Uri("pack://application:,,,/" + assemblyName + ";component/Assets/Sprites/missingTexture.gif", UriKind.Absolute));
                    Log.Warning("Bitmap " + Location + " was not found.");
                }
                bitmaps.Add(Location, newBitmap);

                return newBitmap;
            }
            else
            {
                return bitmaps[Location];
            }
        }

        public Boolean setActiveBitmap(string set)
        {
            if (Location != set)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        Location = set;
                        rectangle.Fill = new ImageBrush
                        {
                            ImageSource = getActiveBitmap(assemblyName)
                        };
                    });
                }
                catch
                {
                    Log.Warning("Can't invoke Application.Current.Dispatcher.");
                }
                return true;
            }

            return false;
        }

        //Any object can edit the gameObjects of the game while the logic is running.
        //And Also get the delta for timed events.
        public Boolean OnTick(List<GameObject> gameObjects, HashSet<String> pressedKeys, float delta)
        {
            if (onTickList.Count == 0)
            {
                return false;
            }
            else
            {
                foreach (IBehaviour behaivior in onTickList)
                {
                    behaivior.OnTick(this, gameObjects, pressedKeys, delta);
                }
            }

            return true;
        }

        //On Death things can happen.
        //public abstract Boolean OnDeath(List<GameObject> gameObjects, HashSet<String> pressedKeys);

        /* IsColliding */
        /*
         * Checks whether or not this gameobject is coliding with the given gameOjbect
         * The argument is the given gameObject
        */
        virtual public Boolean IsColliding(GameObject givenGameObject)
        {
            //Check if you are comparing to youself.
            if (this == givenGameObject || givenGameObject is null)
            {
                return false;
            }

            if (FromLeft < givenGameObject.FromLeft + givenGameObject.Width && FromLeft + Width > givenGameObject.FromLeft)
            {
                if (FromTop < givenGameObject.FromTop + givenGameObject.Height && FromTop + Height > givenGameObject.FromTop)
                {
                    return true;
                }
            }
            return false;
        }



        //reset the gameObject.
        public void Reset()
        {
            //bitmaps = new Dictionary<string, BitmapImage>();
            bitmaps = GameObjectStatic.maps;
            random = new Random(GetHashCode() + (int)DateTime.UtcNow.Ticks);

            width = 0;
            height = 0;
            fromLeft = 0;
            fromTop = 0;

            onTickList.Clear();
            onTickList = new List<IBehaviour>();

            destroyed = false;

            movementSpeed = 0;
            group = 0;

            textBlock = null;
            rectangle = null;

            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    rectangle = new Rectangle();
                }));
            }
            catch
            {
                Log.Warning("Can't invoke Application.Current.Dispatcher.");
            }

            assemblyName = "Labyrint";
            //Default location of the sprite.

            //setActiveBitmap("Assets/redrand.png");
            //location = "Assets/redrand.png";
            //setActiveBitmap(location);
        }

        public float GetOpacity()
        {
            float opacity = 1;
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    opacity = (float)rectangle.Opacity;
                }));
            }
            catch
            {

            }

            return opacity;
        }

        public void SetOpacity(float opacity)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    rectangle.Opacity = opacity;
                }));
            }
            catch
            {
                Log.Warning("Can't invoke Application.Current.Dispatcher.");
                return;
            }
        }

        public void SetText(string text)
        {
            if (textBlock is null)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        textBlock = new TextBlock();
                    }));
                }
                catch
                {
                    Log.Warning("Can't invoke Application.Current.Dispatcher.");
                    return;
                }
            }

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                textBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffffff"));
                textBlock.Text = text;
            }));
        }

        /* CollitionEffect */
        /*
         * Effect that happens when this GameObject collides with the given object.
         * The argument is the given gameObject
        */
        //public abstract Boolean CollitionEffect(List<GameObject> gameObjects, GameObject gameObject);
    }
}
