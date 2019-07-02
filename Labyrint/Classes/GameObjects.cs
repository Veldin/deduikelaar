using GameObjectFactory;
using LogSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrint
{
    public class GameObjects : ICollection<GameObject>
    {
        private List<GameObject> gameObjectsList;           // This list contains all the GameObjects which are saved in this class

        public int Count => GetCount();

        public bool IsReadOnly => throw new NotImplementedException();

        public GameObjects()
        {
            // Initializing the class attributes
            gameObjectsList = new List<GameObject>();
        }

        /// <summary>
        /// This method add a GameObject to the gameObjectsList with the posistion based on the fromBehind index
        /// </summary>
        /// <param name="gameObject">The gameObject that needs to be added</param>
        public void Add(GameObject gameObject)
        {
            // Check if the gameObjectList is not empty
            if (!IsEmpty())
            {
                // Loop through the gameObjectsList
                for (int i = 0; i < gameObjectsList.Count; i++)
                {
                    // Check if the fromBehind value is bigger than new gameObjects fromBehind value
                    if (gameObjectsList[i].FromBehind >= gameObject.FromBehind)
                    {
                        // Insert the new gameObject in the gameObjectsList
                        gameObjectsList.Insert(i, gameObject);
                        return;
                    }
                }
                gameObjectsList.Add(gameObject);
            }
            else
            {
                // Add the gameObject to the gameObjectsList
                gameObjectsList.Add(gameObject);
            }
        }

        public GameObject this[int index]    // Indexer declaration  
        {
            // get and set accessors  
            get { return gameObjectsList[index]; }
        }

        /// <summary>
        /// This method adds a collection of gameObjects to this list
        /// </summary>
        /// <param name="gameObjects">The collection of gameObjects that needs to be added</param>
        public void AddRange(IEnumerable<GameObject> gameObjects)
        {
            // Loop through the list with gameObjects
            foreach(GameObject gameObject in gameObjects)
            {
                // Add the GameObject to the gameObjectsList
                Add(gameObject);
            }
        }

        /// <summary>
        /// Get a GameObject from the list
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public GameObject Get(int index)
        {
            return gameObjectsList[index];
        }

        /// <summary>
        /// This method returns the list with GameObjects
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetList()
        {
            return gameObjectsList;
        }

        /// <summary>
        /// This method removes a GameObject from the list
        /// </summary>
        /// <param name="gameObject">The GameObject that needs to be removed</param>
        public void Remove(GameObject gameObject)
        {
            gameObjectsList.Remove(gameObject);
        }

        /// <summary>
        /// The method removes a GameObject from the list
        /// </summary>
        /// <param name="index">The index of the GameObject that needs to be remoeved</param>
        public void RemoveAt(int index)
        {
            gameObjectsList.RemoveAt(index);
        }

        /// <summary>
        /// Clear the list
        /// </summary>
        public void Clear()
        {
            gameObjectsList.Clear();
        }

        /// <summary>
        /// This method checks if the list contains the given GaemObject
        /// </summary>
        /// <param name="gameObject"> The GameObject that needst to be checked</param>
        /// <returns>Returns true if the list contains the GameObject</returns>
        public bool Contains(GameObject gameObject)
        {
            return gameObjectsList.Contains(gameObject);
        }

        public int GetCount()
        {
            return gameObjectsList.Count;
        }

        /// <summary>
        /// This method check whether the GameObjectsList is empty
        /// </summary>
        /// <returns>Returns true if empty</returns>
        public bool IsEmpty()
        {
            // Check if the gameObjectsList is emtpy
            if (gameObjectsList.Count > 0)
            {
                // Return false because it holds GameObjects
                return false;
            } 

            // Return true because it is empty
            return true;
        }

        public void CopyTo(GameObject[] array, int arrayIndex)
        {
            ((ICollection<GameObject>)gameObjectsList).CopyTo(array, arrayIndex);
        }

        bool ICollection<GameObject>.Remove(GameObject item)
        {
            return ((ICollection<GameObject>)gameObjectsList).Remove(item);
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            return ((ICollection<GameObject>)gameObjectsList).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<GameObject>)gameObjectsList).GetEnumerator();
        }

        public void LogOrder()
        {
            for (int i = 0; i < gameObjectsList.Count; i++)
            {
                Log.Debug(gameObjectsList[i].BuilderType + "        " + gameObjectsList[i].FromBehind);
            }
        }
    }
}
