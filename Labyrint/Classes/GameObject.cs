using Labyrint;
using Maze;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
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

        //Target!
        protected Target target;

        //Strategies
        public List<IBehaviour> onTickList;
        public List<IBehaviour> onDeathList;

        //Location where this gameObject is within the game.
        //This is also used for the hitbox 
        protected float width;
        protected float height;
        protected float fromLeft;
        protected float fromTop;

        protected float health;
        protected float maxHealth;

        //Objects can be facing top, right, bottom and left
        public String direction { get; set; }

        //Offset where to draw the gameObject in the game.
        //The Sprite can be bigger or smaller then the hitbox.
        //The sprite can be more to the left or right then the hitbox.
        protected float leftDrawOffset;
        protected float rightDrawOffset;
        protected float topDrawOffset;
        protected float bottomDrawOffset;

        public Rectangle rectangle;
        public string assemblyName;

        public Random random;
        public Boolean destroyed;

        protected float movementSpeed;
        protected int group;
        protected Boolean collision;

        //Holds the string the builder used to make this object.
        protected string builderType;

        //The sprite location and the CanvasBitmap are stored seperatly
        //This is so the location gets changed more times in a frame the canvasBitmap doesn't have to get loaded more then once a frame.
        //protected CanvasBitmap sprite;
        protected String location;


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
            onDeathList = new List<IBehaviour>();

            destroyed = false;

            movementSpeed = 0;
            group = 0;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                rectangle = new Rectangle();
            }));

            this.health = 1200;

            assemblyName = "Labyrint";
            //Default location of the sprite.

            setActiveBitmap("Assets/redrand.png");
            location = "Assets/redrand.png";
            setActiveBitmap(location);
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

        public float Health
        {
            get { return health; }
            set { health = value; }
        }

        public float MaxHealth
        {
            get { return maxHealth; }
            set { maxHealth = value; }
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

        //Move to target
        public Boolean MoveToTarget(float delta)
        {
            if (Target is null)
            {
                return false;
            }

            float differenceLeftAbs;
            if (Target.FromLeft() - (FromLeft + (Width / 2)) == 0)
            {
                differenceLeftAbs = 0.000000001f;
            }
            else
            {
                differenceLeftAbs = Math.Abs(Target.FromLeft() - (FromLeft + (Width / 2)));
            }

            float differenceTopAbs;
            if (Target.FromTop() - (FromTop + (Height / 2)) == 0)
            {
                differenceTopAbs = 0.0000000001f;
            }
            else
            {
                differenceTopAbs = Math.Abs(Target.FromTop() - (FromTop + (Height / 2)));
            }

            float totalDifferenceAbs = differenceLeftAbs + differenceTopAbs;

            if (!(totalDifferenceAbs < -1 || totalDifferenceAbs > 1))
            {
                return true;
            }

            float originalmoveSpeed = movementSpeed;


            float differenceTopPercent = differenceTopAbs / (totalDifferenceAbs / 100);
            float differenceLeftPercent = differenceLeftAbs / (totalDifferenceAbs / 100);


            float moveTopDistance = movementSpeed * (differenceTopPercent / 100);
            float moveLeftDistance = movementSpeed * (differenceLeftPercent / 100);

            if (Target.FromLeft() > FromLeft)
            {
                AddFromLeft((moveLeftDistance * delta) / 10000);
            }
            else
            {
                AddFromLeft(((moveLeftDistance * delta) / 10000) * -1);
            }

            if (Target.FromTop() > FromTop)
            {
                AddFromTop((moveTopDistance * delta) / 10000);
            }
            else
            {
                AddFromTop(((moveTopDistance * delta) / 10000) * -1);
            }

            return true;
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
                Debug.WriteLine("pack://application:,,,/" + assemblyName + ";component/" + Location, UriKind.Absolute);
                BitmapImage newBitmap = new BitmapImage(new Uri("pack://application:,,,/" + assemblyName + ";component/" + Location, UriKind.Absolute));
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

                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    Location = set;
                    //if (rectangle is null) { rectangle = new Rectangle(); }
                    rectangle.Fill = new ImageBrush
                    {
                        ImageSource = getActiveBitmap(assemblyName)
                    };
                });
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
                    behaivior.OnTick(this, gameObjects, delta);
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
        virtual public Boolean IsColliding(List<GameObject> gameObjects, GameObject givenGameObject)
        {
            //Check if you are comparing to youself.
            if (this == givenGameObject || givenGameObject is null)
            {
                return false;
            }

            //Console.WriteLine(FromLeft + " < " + givenGameObject.FromLeft + givenGameObject.Width);

            if (FromLeft < givenGameObject.FromLeft + givenGameObject.Width && FromLeft + Width > givenGameObject.FromLeft)
            {
                //Console.WriteLine("left-rite");

                if (FromTop < givenGameObject.FromTop + givenGameObject.Height && FromTop + Height > givenGameObject.FromTop)
                {
                    //Console.WriteLine("also top bottom");
                    return true;
                }
            }
            return false;
        }


        //reset
        public void reset()
        {
            //bitmaps = new Dictionary<string, BitmapImage>();
            bitmaps = GameObjectStatic.maps;
            random = new Random(GetHashCode() + (int)DateTime.UtcNow.Ticks);

            width = 0;
            height = 0;
            fromLeft = 0;
            fromTop = 0;

            onTickList = new List<IBehaviour>();
            onDeathList = new List<IBehaviour>();

            destroyed = false;

            movementSpeed = 0;
            group = 0;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                rectangle = new Rectangle();
            }));

            this.health = 0;

            assemblyName = "Labyrint";
            //Default location of the sprite.

            setActiveBitmap("Assets/redrand.png");
            location = "Assets/redrand.png";
            setActiveBitmap(location);
        }


        /* CollitionEffect */
        /*
         * Effect that happens when this GameObject collides with the given object.
         * The argument is the given gameObject
        */
        //public abstract Boolean CollitionEffect(List<GameObject> gameObjects, GameObject gameObject);
    }
}
