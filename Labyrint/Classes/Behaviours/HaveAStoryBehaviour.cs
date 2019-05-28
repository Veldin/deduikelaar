using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiParser;
using GameObjectFactory;
using LogSystem;

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
            if (itemOrder != null)
            {
                gameObject.setActiveBitmap("Assets/Sprites/Items/" + ApiParserFacade.GetStory(itemOrder.storyId).icon + ".gif");
            }
            else
            {
                gameObject.setActiveBitmap("Assets/redrand.png");
            }
        }

        public bool HasStory()
        {
            if (itemOrder == null)
            {
                return false;
            }
            return true;
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
        /// Get the html of the story
        /// </summary>
        /// <returns>Returns a string with the html</returns>
        public string GetHtml()
        {
            return ApiParserFacade.GetStory(itemOrder.storyId).html;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="gameobject"></param>
        /// <param name="gameObjects"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, HashSet<string> pressedKeys, float delta)
        {
            return true;
        }

        public bool OnTick(List<GameObject> gameObjects, float delta)
        {
            throw new NotImplementedException();
        }
    }
}
