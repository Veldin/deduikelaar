using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiParser;
using GameObjectFactory;

namespace Labyrint
{
    public class HaveAStoryBehaviour : IBehaviour
    {
        private ItemOrder itemOrder;        // An object that holds the id of the story and feedback that needs to be displayed when picked up

        public HaveAStoryBehaviour(GameObject gameObject)
        {
            // Get the next itemOrder from the ApiParserFacade
            itemOrder = ApiParserFacade.NextItemOrder();
            
            // Set the sprite of the pickup
            gameObject.setActiveBitmap("Assets/Sprites/" + ApiParserFacade.GetStory(itemOrder.storyId).icon + ".gif");
        }

        /// <summary>
        /// Returns the storyId of this item
        /// </summary>
        /// <returns>The storyId</returns>
        public int GetStoryId()
        {
            return itemOrder.storyId;
        }

        /// <summary>
        /// Returns the feedbackId of this item
        /// </summary>
        /// <returns>The feedbackId</returns>
        public int GetFeedbackId()
        {
            return itemOrder.feedbackId;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="gameobject"></param>
        /// <param name="gameObjects"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, float delta)
        {
            return true;
        }

        public bool OnTick(List<GameObject> gameObjects, float delta)
        {
            throw new NotImplementedException();
        }
    }
}
