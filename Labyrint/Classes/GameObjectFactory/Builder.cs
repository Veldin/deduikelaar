﻿using CameraSystem; //For items that follow the camera
using Labyrint;
using LogSystem;
using Maze;
using Settings;
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

            if (wantToGet == "rat")
            {
                gameObject.Width = 15f;
                gameObject.Height = 15f;

                gameObject.MovementSpeed = 600;
                gameObject.Group = 1;

                gameObject.TopDrawOffset = 0;

                gameObject.onTickList.Add(new RatTargetBehaviour());
                gameObject.onTickList.Add(new MoveToTargetBehaviour());
                gameObject.onTickList.Add(new PlayerSpriteBehaviour());


                gameObject.Target = new Target(gameObject.FromLeft, gameObject.FromTop);
            }

            if (wantToGet == "cursor")
            {
                //Set the windowState.
                switch (SettingsFacade.Get("CursorState", "Normal"))
                {
                    case "None":
                        gameObject.Width = 2;
                        gameObject.Height = 2;
                        gameObject.setActiveBitmap("Assets/Sprites/Empty.gif");
                        break;
                    default:
                        gameObject.Width = 9 * 2.5f;
                        gameObject.Height = 13 * 2.5f;
                        gameObject.setActiveBitmap("Assets/Sprites/Cursor1.gif");
                        break;
                }
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

                gameObject.onTickList.Add(new ScaleItemBehaviour(gameObject.FromLeft, gameObject.FromTop,val[0] as Camera));

                //gameObject.onTickList.Add(new FollowCameraBehaviour(val[0] as Camera));
                gameObject.onTickList.Add(new SetToTargetBehaviour());
                gameObject.onTickList.Add(new ButtonCursorClickBehaviour(Convert.ToInt32(val[1]), Convert.ToInt32(val[2]), val[3] as WebBrowser));

                //gameObject.SetText("Button Text");

                //gameObject.setActiveBitmap("Assets/tile.gif");
            }

            if (wantToGet == "cover")       // value = new object[] { camera, storyId, anwserId, browser }
            {
                gameObject.Width = (value as Camera).GetWidth();
                gameObject.Height = (value as Camera).GetHeight();

                gameObject.highVisibility = true;

                gameObject.MovementSpeed = 1200;
                gameObject.Collition = false;

                gameObject.SetOpacity(0);

                //gameObject.FromLeft = (value as Camera).GetFromLeft();
                //gameObject.FromTop = (value as Camera).GetFromTop();

                //gameObject.onTickList.Add(new FollowCameraBehaviour(value as Camera));
                gameObject.onTickList.Add(new ScaleItemBehaviour(gameObject.FromLeft, gameObject.FromTop, value as Camera));

                gameObject.onTickList.Add(new SetToTargetBehaviour());
                gameObject.onTickList.Add(new AnimateOpacityBehaviour(0.5f));

                gameObject.setActiveBitmap("Assets/Sprites/Black.gif");
            }


            if (wantToGet == "letter")       // value = new object[] { camera, storyId, anwserId, browser }
            {
                object[] val = value as object[];
                //val[0] should contain a Camera
                //val[1] should contain the orientation (True is horizontal, false is vertical)
                //val[2] Compas Direction - north / east / south / west

                if (Convert.ToBoolean(val[1]))
                {
                    gameObject.Width = (val[0] as Camera).GetWidth() * 0.25f; // The Width is 4 / 16 of the camera
                    gameObject.Height = (val[0] as Camera).GetHeight() * 0.416666f; // The Height is 5 / 12 of the camera
                }
                else
                {
                    gameObject.Width = (val[0] as Camera).GetWidth() * 0.3125f; // The Width is 5 / 16 of the camera
                    gameObject.Height = (val[0] as Camera).GetHeight() * 0.3333333f; // The Height is 4 / 12 of the camera
                }
                Log.Debug((4f / 16f));

                Log.Debug(Convert.ToBoolean(val[1]));

                Log.Debug(gameObject.Width);

                //gameObject.Height = gameObject.Width * 1.125f;

                gameObject.highVisibility = true;

                gameObject.MovementSpeed = 1200;
                gameObject.Collition = false;


                //gameObject.FromLeft = (value as Camera).GetFromLeft();
                //gameObject.FromTop = (value as Camera).GetFromTop();

                //gameObject.onTickList.Add(new FollowCameraBehaviour(value as Camera));
                gameObject.onTickList.Add(new ScaleItemBehaviour(gameObject.FromLeft, gameObject.FromTop, val[0] as Camera));

                gameObject.onTickList.Add(new SetToTargetBehaviour());
                //gameObject.onTickList.Add(new AnimateOpacityBehaviour(0.5f));

                //val[1] should contain the orientation (True is horizontal, false is vertical)
                if (!Convert.ToBoolean(val[1]))
                {
                    gameObject.setActiveBitmap("Assets/Sprites/Letters/Letter"+ val[2] + "_800_468_128.gif");
                }
                else
                {
                    gameObject.setActiveBitmap("Assets/Sprites/Letters/Letter" + val[2] + "_640_586_128.gif");
                }
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
