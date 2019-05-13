using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;

namespace SoundSystem
{
    public class SoundController
    {

        Dictionary<String, SoundPlayer> sounds = new Dictionary<string, SoundPlayer>();

        public void addSound(String name, String location)
        {
            addSoundAsync(name, location);
        }
        private async void addSoundAsync(String name, string location)
        {
            SoundPlayer player = new SoundPlayer(location);
            player.LoadAsync();
            sounds.Add(name, player);
        }

        public void playSound(String name)
        {
            using (SoundPlayer s = sounds[name])
            {
                s.Play();
            }
        }

        public void stopSound(String name)
        {
            using (SoundPlayer s = sounds[name])
            {
                s.Stop();
            }
        }

        public List<String> getSounds()
        {
            return sounds.Keys.ToList();
        }
    }
}
