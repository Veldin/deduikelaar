using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
namespace SoundSystem
{
    public static class SoundFacade
    {
        private static SoundController soundController = new SoundController();
        public static void Init()
        {
            // Load sounds
            //soundController.AddResource("test", "test.wav");
            
        }

        public static void PlaySound(String name)
        {
            soundController.PlaySound(name);
        }

        public static void StopSound(String name)
        {
            soundController.StopSound(name);
        }
    }
}
