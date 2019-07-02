using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;
using LogSystem;

namespace Labyrint
{
    public class pickupTargetBehaviour : IBehaviour
    {
        private readonly float destinationFromLeft;
        private readonly float destinationFromTop;

        //The movementspeed the gameobjects gets if they are anchord to the target.
        public bool modifiersApplied; //Public for others to see if the modifiers are applied;
        private readonly int moveSpeedModifier;
        private readonly int sizeModifier;


        //The pickup floats around an anchor.
        private GameObject anchor;

        //private GameObject pointer; //The arrow is used to point to the anchor


        /// <summary>
        /// Initializes a new instance of the pickupTargetBehaviour.
        /// </summary>
        /// <param name="destinationFromLeft">The destination of the pickup from left.</param>
        /// <param name="destinationFromTop">The destination of the pickup from right.</param>
        public pickupTargetBehaviour(float destinationFromLeft, float destinationFromTop)
        {
            
            this.destinationFromLeft = destinationFromLeft;
            this.destinationFromTop = destinationFromTop;

            moveSpeedModifier = 200;
            sizeModifier = 20;

            modifiersApplied = false;
        }

        public bool OnTick(GameObject gameobject, ref GameObjects gameObjects, HashSet<String> pressedKeys, float delta)
        {
            ////If the anchor is null search an anchor.
            //if (pointer == null)
            //{
            //    pointer = GameObjectFactoryFacade.GetGameObject("pointer", destinationFromLeft, destinationFromTop);
            //    pointer.Target = new Target(0,0);
            //    gameObjects.Add(pointer);
            //}
            
            //If the anchor is null search an anchor.
            if (anchor == null)
            {
                foreach (GameObject needle in gameObjects)
                {
                    if (needle.BuilderType == "player")
                    {
                        anchor = needle;
                        break;
                    }
                }
            }

            //If the anchor is still null return false (don't do anything).
            if (anchor == null)
            {
                return false;
            }

            //Calculations for difference.
            float diffFromLeft = anchor.FromLeft - destinationFromLeft;
            float diffFromLeftAbs = Math.Abs(diffFromLeft);

            float diffFromTop = anchor.FromTop - destinationFromTop;
            float diffFromTopAbs = Math.Abs(diffFromTop);

            float diffFromTotal = diffFromLeftAbs + diffFromTopAbs;

            float percentageFromLeft = diffFromTotal - diffFromLeftAbs;
            percentageFromLeft = percentageFromLeft / diffFromTotal * 100;

            float percentageFromTop = diffFromTotal - diffFromTopAbs;
            percentageFromTop = percentageFromTop / diffFromTotal * 100;

            //If the absolute difference is high (when its out of the screen) place the object closer.
            if (diffFromTopAbs > 300 || diffFromLeftAbs > 550)
            {
                //Show the arrow
                //pointer.SetOpacity(1);

                if (!modifiersApplied)
                {
                    gameobject.MovementSpeed += moveSpeedModifier;
                    gameobject.Width -= sizeModifier;
                    gameobject.Height -= sizeModifier;

                    modifiersApplied = true;
                }

                gameobject.Target.SetFromLeft(destinationFromLeft + (diffFromLeft));

                //Place it to the left or right of the player depending on what side the item is.
                if (diffFromLeft > 0)
                {
                    gameobject.Target.AddFromLeft((percentageFromTop * -1) * 2.5f);
                }
                else
                {
                    gameobject.Target.AddFromLeft(percentageFromTop * 2.5f);
                }

                gameobject.Target.SetFromTop(destinationFromTop + (diffFromTop));
                //Place it to the left or right of the player depending on what side the item is.
                if (diffFromTop > 0)
                {
                    gameobject.Target.AddFromTop((percentageFromLeft * -1) * 2.5f);
                }
                else
                {
                    gameobject.Target.AddFromTop(percentageFromLeft * 2.5f);
                }

                //Set the pointer to the right position
                //pointer.Target = new Target(gameobject, false);
                //if (diffFromTop > 0)
                //{
                //    pointer.Target.AddFromTop((percentageFromLeft * -1) / 5);
                //}
                //else
                //{
                //    pointer.Target.AddFromTop(percentageFromLeft / 5);
                //}

                //if (diffFromLeft > 0)
                //{
                //    pointer.Target.AddFromLeft((percentageFromTop * -1) / 5);
                //}
                //else
                //{
                //    pointer.Target.AddFromLeft(percentageFromTop / 5);

                //}
            }
            else
            {
                if (modifiersApplied)
                {
                    gameobject.MovementSpeed -= moveSpeedModifier;
                    gameobject.Width += sizeModifier;
                    gameobject.Height += sizeModifier;

                    modifiersApplied = false;
                }
                //If the object is close anough set its target to its destination
                gameobject.Target.SetFromLeft(destinationFromLeft);
                gameobject.Target.SetFromTop(destinationFromTop);

                //And hide the arrow
                //pointer.SetOpacity(0);
            }

            return true;
        }

        public bool OnTick(ref GameObjects gameObjects, float delta)
        {
            throw new NotImplementedException();
        }
    }
}
