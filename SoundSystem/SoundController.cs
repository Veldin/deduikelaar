using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.IO;
using LogSystem;
using System.Reflection;

namespace SoundSystem
{
    public class SoundController
    {

        Dictionary<String, SoundPlayer> sounds = new Dictionary<string, SoundPlayer>();

        public void AddSound(String name, String location)
        {
            if (!File.Exists(location))
            {
                Log.Warning("Sound not found (" + name + "): " + location);
                return;
            }
            Log.Debug("Added sound " + name + ": " + location);
            SoundPlayer player = new SoundPlayer(location);
            player.LoadAsync();
            sounds.Add(name, player);
        }

        public void AddResource(String name, String res)
        {
            String loc = Assembly.GetExecutingAssembly().Location + "\\..\\..\\Sounds\\" + res;
            AddSound(name, loc);
        }
        
        public void PlaySound(String name)
        {
            if (sounds.ContainsKey(name))
            {
                sounds[name].Play();
            }
            else
            {
                Log.Warning("Sound " + name + " not found");
            }
        }

        public void StopSound(String name)
        {
            if (sounds.ContainsKey(name))
            {
                sounds[name].Stop();
            }
            else
            {
                Log.Warning("Sound " + name + " not found");
            }
        }
    }
}
