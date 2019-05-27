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

        public FollowCameraBehaviour(Camera camera)
        {
            this.camera = camera;
        }

        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, float delta)
        {
            if (gameobject.Target == null)
            {
                gameobject.Target = new Target(0,0);
            }

            fromLeftOffest = camera.GetFromLeft() + camera.GetWidth() / 2;
            fromTopOffest = camera.GetFromTop() + camera.GetHeight() / 2;

            if (float.IsNaN(fromLeftOffest))
            {
                fromLeftOffest = 0;
            }
            if (float.IsNaN(fromTopOffest))
            {
                fromTopOffest = 0;
            }

            gameobject.Target.SetFromLeft(fromLeftOffest);
            gameobject.Target.SetFromTop(fromTopOffest);

            return true;
        }

        public bool OnTick(List<GameObject> gameObjects, float delta)
        {
            throw new NotImplementedException();
        }
    }
}
