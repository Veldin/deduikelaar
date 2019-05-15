using Labyrint;
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
            gameObject.FromLeft = fromLeft;
            gameObject.FromTop = fromTop;


            if (wantToGet == "player")
            {
                gameObject.Width = 24 * 2.5f;
                gameObject.Height = 42 * 2.5f;

                gameObject.MovementSpeed = 200;
                gameObject.Group = 1;

                gameObject.onTickList.Add(new MoveToTargetBehaviour());

                gameObject.Target = new Target(gameObject.FromLeft, gameObject.FromTop);
                //gameObject.AddFromLeft(1000);
                //gameObject.AddFromTop(1000);



                gameObject.setActiveBitmap("Assets/Redrand.png");
            }

            if (wantToGet == "cursor")
            {
                gameObject.Width = 9 * 2.5f;
                gameObject.Height = 13 * 2.5f;

                gameObject.setActiveBitmap("Assets/Cursor1.gif");
            }

            if (wantToGet == "tile")
            {
                gameObject.Width = 250f;
                gameObject.Height = 250f;

                gameObject.setActiveBitmap("Assets/tile.gif");
            }
        }
    }
}
