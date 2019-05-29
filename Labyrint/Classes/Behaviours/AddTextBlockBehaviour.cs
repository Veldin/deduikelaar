using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GameObjectFactory;

namespace Labyrint
{
    public class AddTextBlockBehaviour : IBehaviour
    {
        private TextBlock textBlock;

        public AddTextBlockBehaviour()
        {   
            // Invoke the Ui thread
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                textBlock = new TextBlock();
            }));
        }

        public TextBlock GetTextBlock()
        {
            return textBlock;
        }

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
