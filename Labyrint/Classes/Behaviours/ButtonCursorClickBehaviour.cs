using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ApiParser;
using GameObjectFactory;
using LogSystem;

namespace Labyrint
{
    class ButtonCursorClickBehaviour : IBehaviour
    {
        GameObject cursor;
        private int storyId;
        private int answerId;
        private bool isClicked;
        WebBrowser browser;

        public ButtonCursorClickBehaviour(int storyId, int answerId, WebBrowser browser)
        {
            this.storyId = storyId;
            this.answerId = answerId;
            this.browser = browser;
            isClicked = false;
        }

        public bool OnTick(GameObject gameobject, List<GameObject> gameObjects, HashSet<String> pressedKeys, float delta)
        {
            //If there is no cursor, try to find it.
            if (cursor is null)
            {
                foreach (GameObject needle in gameObjects)
                {

                    if (needle.BuilderType == "cursor")
                    {
                        cursor = needle;
                        break;
                    }
                }

                if (cursor is null)
                {
                    return false;
                }
            }

            if (gameobject.IsColliding(cursor) && pressedKeys.Contains("LeftMouse"))
            { 
                // Give the feedback to the ApiParserFacade
                ApiParserFacade.SaveFeedbackStatistic(storyId, answerId);

                // Unpress all keys
                pressedKeys.Clear();

                // Invoke the Ui thread
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    browser.Visibility = Visibility.Hidden;
                }));

                // Destroy all existing buttons
                foreach (GameObject gameObject in gameObjects)
                {
                    if (gameObject.BuilderType == "button" || gameObject.BuilderType == "cover" || gameObject.BuilderType == "ControllerAncher" || 
                        gameObject.BuilderType == "ControllerCursor" || gameObject.BuilderType == "letter")
                    {
                        gameObject.destroyed = true;
                    }
                    else
                    {
                        foreach (IBehaviour behaviour in gameObject.onTickList)
                        {
                            if (behaviour is MoveToTargetBehaviour)
                            {
                                MoveToTargetBehaviour moveToTargetBehaviour = behaviour as MoveToTargetBehaviour;
                                moveToTargetBehaviour.pauzed = false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public bool OnTick(List<GameObject> gameObjects, float delta)
        {
            throw new NotImplementedException();
        }
    }
}
