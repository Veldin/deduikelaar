using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;

namespace Labyrint
{
    public interface IBehaviour
    {
        bool OnTick(GameObject gameobject, List<GameObject> gameObjects, float delta);
    }
}
