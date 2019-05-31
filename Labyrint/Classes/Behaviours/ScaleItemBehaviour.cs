using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraSystem;
using GameObjectFactory;
using LogSystem;

namespace Labyrint
{
    //This class resises and moves a gameobject to act like a scaling user interface item.
    class ScaleItemBehaviour : IBehaviour
    {
        float fromLeftRelative;
        float fromTopRelative;
        Camera camera; 

        public ScaleItemBehaviour(float fromLeft, float fromTop, Camera camera)
        {
            fromLeftRelative = fromLeft;
            fromTopRelative = fromTop;

            this.camera = camera;
        }

        public ScaleItemBehaviour(Camera camera)
        {
            fromLeftRelative = 50;
            fromTopRelative = 50;

            this.camera = camera;
        }

        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, HashSet<string> pressedKeys, float delta)
        {
            
            if (gameobject.Target is null)
            {
                gameobject.Target = new Target(0,0);
            }

            float fromLeft = camera.GetFromLeft() + ((camera.GetWidth() / 100) * fromLeftRelative);
            float fromtop = camera.GetFromTop() + ((camera.GetHeight() / 100) * fromTopRelative);

            gameobject.Target.SetFromTop(fromtop);
            gameobject.Target.SetFromLeft(fromLeft);

            return true;
        }

        public bool OnTick(List<GameObject> gameObjects, float delta)
        {
            throw new NotImplementedException();
        }
    }
}
