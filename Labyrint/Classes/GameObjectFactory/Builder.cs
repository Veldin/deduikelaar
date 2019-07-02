using CameraSystem; //For items that follow the camera
using Labyrint;
using LogSystem;
using Maze;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GameObjectFactory
{
    //The builder holds a method that can transform a gameObject by giving a specivic requested as a string
    public class Builder
    {
        public Builder()
        {

        }

        public void TransformGameObject(GameObject gameObject, string wantToGet, float fromLeft, float fromTop, float fromBehind, object value = null)
        {
            gameObject.BuilderType = wantToGet;

            gameObject.FromLeft = fromLeft;
            gameObject.FromTop = fromTop;
            gameObject.FromBehind = fromBehind;
            gameObject.Collition = true;

            switch (wantToGet)
            {
                // Get a Player gameObject
                case "player":
                    gameObject.Width = 45f;
                    gameObject.Height = 45f;

                    // Draw the height higher but don't increase the hitbox.
                    gameObject.TopDrawOffset = 15;

                    // The player has to move to the target and chance sprite
                    gameObject.onTickList.Add(new MoveToTargetBehaviour());
                    gameObject.onTickList.Add(new PlayerSpriteBehaviour());

                    gameObject.Target = new Target(gameObject.FromLeft, gameObject.FromTop);

                    //Set the first sprite, the playerSprite behaviour wil replace it on the first tick.
                    gameObject.setActiveBitmap("Assets/Sprites/Player/right1_145_200_32.gif");
                    break;
                //Get a Cursor looking gameObject
                case "cursor":
                    //Set the bitmap depending on a setting.
                    switch (SettingsFacade.Get("CursorState", "Normal", "Dictates the cursor state [Normal || None]"))
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
                    break;
                // Create a pickup.
                case "pickup":       // value = new object[2] { browser, camera, engine }
                    gameObject.Width = 55f;
                    gameObject.Height = 55f;

                    //Add the onTick effects for the pickup.
                    gameObject.onTickList.Add(new pickupTargetBehaviour(gameObject.FromLeft, gameObject.FromTop)); //Sets the target of this pickup
                    gameObject.onTickList.Add(new MoveToTargetBehaviour()); //Move to the target set by the pickupTarget
                    gameObject.onTickList.Add(new PickupCollisionBehavior(value)); //On collition the pickup is picked up
                    gameObject.onTickList.Add(new HaveAStoryBehaviour(gameObject)); //give this object a story 

                    gameObject.MovementSpeed = 800;

                    gameObject.Target = new Target(gameObject.FromLeft, gameObject.FromTop);

                    gameObject.Collition = false; //Pickups dont colide with the walls
                    break;
                case "tile":
                    //Make a tile the size of the mazefacade tile
                    gameObject.Width = MazeFacade.tileSize;
                    gameObject.Height = MazeFacade.tileSize;

                    gameObject.highVisibility = false;

                    gameObject.setActiveBitmap("Assets/wall_600_600_16.gif");
                    break;
                //Orbs spawn behind items if they are to far away (to indicate direction)
                case "orb": // value is the gameObject the orb needs to get pulled in.
                    gameObject.Width = 50;
                    gameObject.Height = 50;

                    gameObject.Target = new Target(value as GameObject); //The value contains the target the orb apears under

                    gameObject.onTickList.Add(new SetToTargetBehaviour()); //Instead of moving the target, it just gets set to the target
                    gameObject.onTickList.Add(new OrbOpacityBehaviour()); //Hide the orb if its close to a target

                    gameObject.highVisibility = true;
                    gameObject.Collition = false; //Orbs dont colide with the walls

                    gameObject.setActiveBitmap("Assets/Sprites/Orb.gif");

                    break;
                //Pointers can get used to point to a direction, they are used with the orbs.
                case "pointer":
                    gameObject.Width = 10;
                    gameObject.Height = 10;

                    gameObject.onTickList.Add(new SetToTargetBehaviour());

                    gameObject.highVisibility = true;
                    gameObject.Collition = false;

                    gameObject.setActiveBitmap("Assets/Sprites/Orb.gif");

                    break;
                //Create buttons the player can click on 
                case "button":      // value = new object[] { camera, storyId, anwserId, browser }
                    object[] val5 = value as object[];

                    gameObject.Width = 82.35f;
                    gameObject.Height = 70;

                    gameObject.highVisibility = true;

                    gameObject.MovementSpeed = 1200;
                    gameObject.Collition = false;

                    //Resises and moves a gameobject to act like a scaling user interface item.
                    gameObject.onTickList.Add(new ScaleItemBehaviour(gameObject.FromLeft, gameObject.FromTop, val5[0] as Camera));
                    gameObject.onTickList.Add(new SetToTargetBehaviour());

                    //Dictate what to do on click
                    gameObject.onTickList.Add(new ButtonCursorClickBehaviour(Convert.ToInt32(val5[1]), Convert.ToInt32(val5[2]), val5[3] as WebBrowser));

                    break;
                //Spawn a letter icon at a location
                case "letter":      // value = new object[] { camera, storyId, anwserId, browser }
                    object[] val2 = value as object[];
                    //val[0] should contain a Camera
                    //val[1] should contain the orientation (True is horizontal, false is vertical)
                    //val[2] Compas Direction - north / east / south / west

                    if (Convert.ToBoolean(val2[1]))
                    {
                        gameObject.Width = (val2[0] as Camera).GetWidth() * 0.25f; // The Width is 4 / 16 of the camera
                        gameObject.Height = (val2[0] as Camera).GetHeight() * 0.416666f; // The Height is 5 / 12 of the camera
                    }
                    else
                    {
                        gameObject.Width = (val2[0] as Camera).GetWidth() * 0.3125f; // The Width is 5 / 16 of the camera
                        gameObject.Height = (val2[0] as Camera).GetHeight() * 0.3333333f; // The Height is 4 / 12 of the camera
                    }

                    gameObject.highVisibility = true;
                    gameObject.MovementSpeed = 1200;
                    gameObject.Collition = false;
                    gameObject.setActiveBitmap("Assets/Sprites/Letters/Letter" + val2[2] + "_1080_1080.gif");

                    break;
                //ControllerAncher gets used with ControllerCursor for the drag system
                case "ControllerAncher":
                    gameObject.Width = 20;
                    gameObject.Height = 20;

                    gameObject.TopDrawOffset = 20;
                    gameObject.LeftDrawOffset = 20;

                    gameObject.Target = new Target(fromLeft, fromTop);

                    gameObject.setActiveBitmap("Assets/Sprites/DragOuter.gif");
                    break;
                //ControllerCursor gets used with ControllerAncher for the drag system
                case "ControllerCursor":
                    gameObject.Width = 12;
                    gameObject.Height = 12;

                    gameObject.TopDrawOffset = 12;
                    gameObject.LeftDrawOffset = 12;

                    gameObject.setActiveBitmap("Assets/Sprites/DragInner.gif");
                    break;
                case "popupButton":     // value = Camera, MainWindow

                    gameObject.Width = 82.35f;
                    gameObject.Height = 70;

                    gameObject.highVisibility = true;

                    gameObject.MovementSpeed = 1200;
                    gameObject.Collition = false;

                    object[] val3 = value as object[];

                    gameObject.onTickList.Add(new ScaleItemBehaviour(gameObject.FromLeft, gameObject.FromTop, val3[0] as Camera));
                    gameObject.onTickList.Add(new SetToTargetBehaviour());
                    gameObject.onTickList.Add(new OnClickPopupBehaviour(val3[1] as MainWindow));

                    gameObject.setActiveBitmap("Assets/Sprites/Answers/poststamp.png");
                    gameObject.SetText("Quit");

                    Thickness thickness = gameObject.textBlock.Margin;
                    thickness.Left = 25;
                    thickness.Top = 20;
                    gameObject.textBlock.Margin = thickness;

                    break;
            }


        }
    }
}