﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;

namespace Labyrint
{
    class OrbOpacityBehaviour : IBehaviour
    {

        private pickupTargetBehaviour anchor;

        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, HashSet<String> pressedKeys, float delta)
        {
            //try to find a pickupTargetBehaviour to use as an anchor;
            if (anchor is null)
            {
                foreach (GameObject needle in gameObjects)
                {
                    if (needle == gameobject.Target.GetGameObject())
                    {
                        foreach (IBehaviour behaviour in needle.onTickList)
                        {
                            if (behaviour is pickupTargetBehaviour)
                            {
                                anchor = behaviour as pickupTargetBehaviour;
                            }
                        }
                    }
                }
            }

            if (anchor is null)
            {
                return false;
            }

            if (anchor.modifiersApplied)
            {
                gameobject.SetOpacity(1);
            }
            else
            {
                gameobject.SetOpacity(0);
            }


            return false;
        }

        public bool OnTick(List<GameObject> gameObjects, float delta)
        {
            throw new NotImplementedException();
        }
    }
}