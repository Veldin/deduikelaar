using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CameraSystem;
using GameObjectFactory;
using LogSystem;

namespace Labyrint
{
    class FollowCameraBehaviour : IBehaviour
    {
        public Camera camera;
        public float fromLeftOffest;
        public float fromTopOffest;

        public float originalFromLeft;
        public float originalFromTop;

        public float addedFromLeft;
        public float addedFromTop;

        public FollowCameraBehaviour(Camera camera)
        {
            this.camera = camera;
        }

        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, HashSet<String> pressedKeys, float delta)
        {
            if (gameobject.Target == null)
            {
                gameobject.Target = new Target(0,0);
                originalFromLeft = gameobject.FromLeft;
                originalFromTop = gameobject.FromTop;

                addedFromLeft = 0;
                addedFromTop = 0;
            }

            fromLeftOffest = camera.GetFromLeft() + camera.GetWidth() / 2;
            fromTopOffest = camera.GetFromTop() + (camera.GetHeight() / 2) + (camera.GetHeight() / 3) ;

            if (float.IsNaN(fromLeftOffest))
            {
                fromLeftOffest = 0;
            }
            if (float.IsNaN(fromTopOffest))
            {
                fromTopOffest = 0;
            }

            //Remember how much is added due to camera effect.
            addedFromLeft = fromLeftOffest;
            addedFromTop = fromTopOffest;

            gameobject.Target.SetFromLeft(fromLeftOffest + originalFromLeft);
            gameobject.Target.SetFromTop(fromTopOffest + originalFromTop);

            return true;
        }

        public bool OnTick(List<GameObject> gameObjects, float delta)
        {
            throw new NotImplementedException();
        }
    }
}
