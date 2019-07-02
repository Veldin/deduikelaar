using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectFactory;

namespace Labyrint
{
    /// <summary>
    /// Interface to implement behaviours for the engine and the gameObject.
    /// </summary>
    public interface IBehaviour
    {
        /// <summary>
        /// Defines any effect a gameObject needs to perform. Gets ran every gameTick
        /// </summary>
        /// <param name="gameobject">The gameObject assosisted with the effect.</param>
        /// <param name="gameObjects">The list of gameObjects that were used for this gameTick.</param>
        /// <param name="pressedKeys">The list of keys that were used pressed.</param>
        /// <param name="delta">The time since the last onTick was performed.</param>
        bool OnTick(GameObject gameobject, ref GameObjects gameObjects, HashSet<String> pressedKeys, float delta);

        /// <summary>
        /// Defines any effect a gameObject or the engine needs to perform. Gets ran every gameTick 
        /// </summary>
        /// <param name="gameObjects">The list of gameObjects that were used for this gameTick.</param>
        /// <param name="delta">The time since the last onTick was performed.</param>
        /// <returns></returns>
        bool OnTick(ref GameObjects gameObjects, float delta);
    }
}
