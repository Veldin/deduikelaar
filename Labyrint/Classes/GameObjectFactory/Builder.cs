﻿using Labyrint;
using Maze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameObjectFactory
{
    public class Builder
    {
        public Builder()
        {

        }

        public void TransformGameObject(GameObject gameObject, string wantToGet, float fromLeft, float fromTop)
        {
            gameObject.BuilderType = wantToGet;

            gameObject.FromLeft = fromLeft;
            gameObject.FromTop = fromTop;
            gameObject.Collition = true;

            if (wantToGet == "player")
            {
                //gameObject.Width = 15 * 2.5f;
                //gameObject.Height = 15 * 2.5f;

                gameObject.Width = 40f;
                gameObject.Height = 40f;

                gameObject.MovementSpeed = 0;
                gameObject.Group = 1;

                gameObject.TopDrawOffset = 15;

                gameObject.onTickList.Add(new MoveToTargetBehaviour());

                gameObject.Target = new Target(gameObject.FromLeft, gameObject.FromTop);

                gameObject.setActiveBitmap("Assets/Sprites/right1_145_200_32.gif");
            }

            if (wantToGet == "cursor")
            {
                gameObject.Width = 9 * 2.5f;
                gameObject.Height = 13 * 2.5f;

                gameObject.setActiveBitmap("Assets/Cursor1.gif");
            }

            if (wantToGet == "pickup")
            {
                gameObject.Width = 30f;
                gameObject.Height = 30f;

                gameObject.onTickList.Add(new pickupTargetBehaviour(gameObject.FromLeft, gameObject.FromTop));
                gameObject.onTickList.Add(new MoveToTargetBehaviour());
                gameObject.onTickList.Add(new PickupCollisionBehavior());


                gameObject.MovementSpeed = 1200;

                gameObject.Target = new Target(gameObject.FromLeft, gameObject.FromTop);

                gameObject.Collition = false;

                gameObject.setActiveBitmap("Assets/tile2.gif");
            }

            if (wantToGet == "tile")
            {
                gameObject.Width = MazeFacade.tileSize;
                gameObject.Height = MazeFacade.tileSize;

                gameObject.highVisibility = false;

                gameObject.setActiveBitmap("Assets/wall_600_600_16.gif");

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
