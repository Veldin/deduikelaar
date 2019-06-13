using FileReaderWriterSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settings
{
    public class Manager
    {
        private static Dictionary<string, string> fromFile;
        private static Dictionary<string, string> defaults;
        private static Dictionary<string, string> merge;

        public Manager()
        {
            fromFile = new Dictionary<string, string>();
            defaults = new Dictionary<string, string>();
            merge = new Dictionary<string, string>();

            //Add values to the defaults
            defaults.Add("resolution", "1920x1080");
            defaults.Add("quality", "high");
            defaults.Add("cat", "dog");

            PopulateFromIni();

            //Merge the defauts to the fromFile. The fromFile wil have prioritiy;
            merge = fromFile.Concat(defaults)
                .ToLookup(x => x.Key, x => x.Value)
                .ToDictionary(x => x.Key, g => g.First());
        }

        /// <summary>
        /// Populates the fromFile dictionary from the options.ini
        /// </summary>
        public void PopulateFromIni()
        {
            List<string> list = FileReaderWriterFacade.ReadLines(FileReaderWriterFacade.GetAppDataPath() + "options.ini");

            if (list is null)
            {
                return;
            }

            foreach (string needle in list)
            {
                string[] pair = needle.Split(':');
                try
                {
                    if (pair[0] is null || pair[1] is null)
                    {
                        continue;
                    }
                    fromFile.Add(pair[0], pair[1]);
                }
                catch { }
            }
        }

        /// <summary>
        /// Populates the options.ini file from the toFile dictionary
        /// </summary>
        public void PopulateToIni()
        {
            string[] toFile = new String[fromFile.Count];

            int i = 0;
            foreach (var item in fromFile)
            {
                toFile[i] = item.Key + ":" + item.Value;
                i++;
            }

            FileReaderWriterFacade.WriteText(toFile, FileReaderWriterFacade.GetAppDataPath() + "options.ini", false);
        }

        /// <summary>
        /// Gets the requested value from the manager. The default return is returned if the value was not found.
        /// The value is added to the fromFile if the needle is not found.
        /// </summary>
        /// <param name="needle">The key used for the lookup.</param>
        /// <param name="defaultReturn">The default return value</param>
        /// <returns></returns>
        public string Get(string needle, string defaultReturn = null)
        {
            //try to vind the key in the dictionary
            if (merge.TryGetValue(needle, out string value))
            {
                //return the value assosiated with the key
                return value;
            }

            //fromFile.Add(needle, defaultReturn);

            return defaultReturn;
        }

        /// <summary>
        /// Gets the requested value from the manager. The default return is returned if the value was not found.
        /// The value is added to the fromFile if the needle is not found.
        /// </summary>
        /// <param name="needle">The key used for the lookup.</param>
        /// <param name="defaultReturn">The default return value</param>
        /// <returns></returns>
        public int Get(string needle, int defaultReturn = 0)
        {
            //try to vind the key in the dictionary
            if (merge.TryGetValue(needle, out string value))
            {
                //return the value assosiated with the key
                if (Int32.TryParse(value, out int asInt))
                {
                    return asInt;
                }
                return defaultReturn;
            }

            //fromFile.Add(needle, defaultReturn.ToString());

            return defaultReturn;
        }

        /// <summary>
        /// Get the value of a setting
        /// </summary>
        /// <param name="needle">The name of the setting</param>
        /// <returns>Returns the value of a setting</returns>
        public string GetSetting(string key)
        {
            // Check if the key is in the dictionary 
            if (fromFile.Keys.Contains(key))
            {                
                return fromFile[key];
            }

            return null;
        }

        /// <summary>
        /// Set a value to an already existing setting
        /// </summary>
        /// <param name="key">Setting name</param>
        /// <param name="value">The new value of the setting</param>
        public void SetSetting(string key, string value)
        {
            // Check if the key is in the dictionary 
            if (fromFile.Keys.Contains(key))
            {
                fromFile[key] = value;
            }
        }

    }
}
