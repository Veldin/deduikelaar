using CameraSystem; //For items that follow the camera
using Labyrint;
using Maze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GameObjectFactory
{
    public class Builder
    {
        public Builder()
        {
            
        }

        public void TransformGameObject(GameObject gameObject, string wantToGet, float fromLeft, float fromTop, object value = null)
        {
            gameObject.BuilderType = wantToGet;

            gameObject.FromLeft = fromLeft;
            gameObject.FromTop = fromTop;
            gameObject.Collition = true;

            if (wantToGet == "player")
            {
                gameObject.Width = 40f;
                gameObject.Height = 40f;

                gameObject.MovementSpeed = 0;
                gameObject.Group = 1;

                gameObject.TopDrawOffset = 15;

                gameObject.onTickList.Add(new MoveToTargetBehaviour());
                gameObject.onTickList.Add(new PlayerSpriteBehaviour());

                gameObject.Target = new Target(gameObject.FromLeft, gameObject.FromTop);

                gameObject.setActiveBitmap("Assets/Sprites/Player/right1_145_200_32.gif");
            }

            if (wantToGet == "cursor")
            {
                gameObject.Width = 9 * 2.5f;
                gameObject.Height = 13 * 2.5f;

                gameObject.setActiveBitmap("Assets/Cursor1.gif");
            }

            if (wantToGet == "pickup")      // value = new object[2] { browser, camera }
            {
                gameObject.Width = 30f;
                gameObject.Height = 30f;

                
                gameObject.onTickList.Add(new pickupTargetBehaviour(gameObject.FromLeft, gameObject.FromTop));
                gameObject.onTickList.Add(new MoveToTargetBehaviour());
                gameObject.onTickList.Add(new PickupCollisionBehavior(value));
                gameObject.onTickList.Add(new HaveAStoryBehaviour(gameObject));

                gameObject.MovementSpeed = 1200;

                gameObject.Target = new Target(gameObject.FromLeft, gameObject.FromTop);

                gameObject.Collition = false;
                gameObject.setActiveBitmap("Assets/tile.gif");
            }

            if (wantToGet == "tile")
            {
                gameObject.Width = MazeFacade.tileSize;
                gameObject.Height = MazeFacade.tileSize;

                gameObject.highVisibility = false;

                gameObject.setActiveBitmap("Assets/wall_600_600_16.gif");

            }
                
            if (wantToGet == "button")       // value = new object[] { camera, storyId, anwserId, browser }
            {
                object[] val = value as object[];

                gameObject.Width = 70;
                gameObject.Height = 70;

                gameObject.highVisibility = true;

                gameObject.MovementSpeed = 1200;
                gameObject.Collition = false;

                gameObject.onTickList.Add(new FollowCameraBehaviour(val[0] as Camera));
                gameObject.onTickList.Add(new SetToTargetBehaviour());
                gameObject.onTickList.Add(new AddTextBlockBehaviour());
                gameObject.onTickList.Add(new ButtonCursorClickBehaviour(Convert.ToInt32(val[1]), Convert.ToInt32(val[2]), val[3] as WebBrowser));


                gameObject.setActiveBitmap("Assets/tile.gif");
            }

            if (wantToGet == "ControllerAncher")
            {
                gameObject.Width = 50f;
                gameObject.Height = 50f;

                gameObject.Target = new Target(fromLeft, fromTop);

                gameObject.setActiveBitmap("Assets/Redrand.png");
            }

            if (wantToGet == "ControllerCursor")
            {
                gameObject.Width = 46f;
                gameObject.Height = 46f;

                gameObject.setActiveBitmap("Assets/Redrand.png");
            }
        }
    }
}
