using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;
using LogSystem;

namespace Labyrint
{
    class SpaceButtonsHorisontallyBehaviour : IBehaviour
    {
        private List<GameObject> loopList;
        private List<GameObject> buttonList;

        private int spaceBetween;
        private int countButtons;

        public SpaceButtonsHorisontallyBehaviour()
        {
            loopList = new List<GameObject>();
            buttonList = new List<GameObject>();

            //Space between the buttons
            spaceBetween = 100;
        }

        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, HashSet<String> pressedKeys, float delta)
        {
            throw new NotImplementedException();
        }

        public bool OnTick(List<GameObject> gameObjects, float delta)
        {
            loopList.Clear();
            buttonList.Clear();

            lock (gameObjects)
            {
                loopList.AddRange(gameObjects);
            }

            int i = 0;
            foreach (GameObject needle in loopList)
            {
                if (needle.BuilderType == "button")
                {
                    buttonList.Add(needle);
                    i++;
                }
            }

            if (i == countButtons)
            {
                return false; //The amount of buttons didn't change.
            }
            countButtons = i;

            //The amount of buttons changed, so try to reposition the rest of the buttons.


            float fromLeftOffsetStart = (buttonList.Count() * -0.5f + 0.5f) * spaceBetween;

            int count = 0;
            foreach (GameObject needle in buttonList)
            {
                needle.AddFromLeft(fromLeftOffsetStart);

                if(count > 0)
                    needle.AddFromLeft(count * spaceBetween);

                
                foreach (IBehaviour behaviour in needle.onTickList)
                {
                    if (behaviour is FollowCameraBehaviour)
                    {
                        FollowCameraBehaviour followCameraBehaviour = behaviour as FollowCameraBehaviour;
                        followCameraBehaviour.originalFromLeft -= (needle.Width / 2) * -1;
                    }
                }

                count++;
            }

            return true;
        }
    }
}
