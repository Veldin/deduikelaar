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
        // Holds the settings that are gotten from the file and are written to the file again on save.
        private static Dictionary<string, string> fromFile;

        // Holds default settings that are set in the contructor.
        private static Dictionary<string, string> defaults;

        // Holds the combination of the fromFile and the defaults.
        // This one gets checked for the geting and setting of settings.
        // (The fromFile wil have prioritiy during merging.)
        private static Dictionary<string, string> merge;

        // Holds comments
        private static Dictionary<string, string> comments;

        public Manager()
        {
            fromFile = new Dictionary<string, string>();
            defaults = new Dictionary<string, string>();
            merge = new Dictionary<string, string>();
            comments = new Dictionary<string, string>();


            //Add values to the defaults
            defaults.Add("resolution", "1920x1080");
            defaults.Add("quality", "high");

            PopulateFromIni();

            //Merge the defauts to the fromFile. The fromFile wil have prioritiy;
            merge = fromFile.Concat(defaults)
                .ToLookup(x => x.Key, x => x.Value)
                .ToDictionary(x => x.Key, g => g.First());
        }

        /// <summary>
        /// Reload the settings from the options.ini file
        /// </summary>
        public void RefeshSettings()
        {
            // Clear the fromFile and merge lists
            fromFile.Clear();
            merge.Clear();

            //Populate the fromFile dictionary from the options.ini
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

            //if a comment block is found it gets stored in the comment storre until a key is found
            string commentStore = null;

            foreach (string needle in list)
            {
                //If the first character is a # it is a comment
                if (needle[0] == '#')
                {
                    commentStore += needle.Substring(1);
                    continue;
                }

                string[] pair = needle.Split(':');
                try
                {
                    if (pair[0] is null || pair[1] is null)
                    {
                        continue;
                    }
                    fromFile.Add(pair[0], string.Join(":", pair.Skip(1).ToArray()));

                    if (!string.IsNullOrEmpty(commentStore))
                    {
                        comments.Add(pair[0], commentStore);
                        commentStore = String.Empty;
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Populates the options.ini file from the toFile dictionary
        /// </summary>
        public void PopulateToIni()
        {
            string[] toFile = new String[fromFile.Count + comments.Count];

            int i = 0;
            foreach (var item in fromFile)
            {
                if (comments.ContainsKey(item.Key))
                {
                    toFile[i] = '#' + comments[item.Key];
                    comments.Remove(item.Key);
                    i++;
                }

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
        /// <param name="comment">Set a comment for in the ini file</param>
        /// <returns></returns>
        public string Get(string needle, string defaultReturn = null, string comment = null)
        {
            //try to vind the key in the dictionary
            if (merge.TryGetValue(needle, out string value))
            {
                //return the value assosiated with the key
                return value;
            }

            if (!fromFile.ContainsKey(needle))
            {
                fromFile.Add(needle, defaultReturn);

                if (!(comment is null) && !comments.ContainsKey(needle))
                {
                    comments.Add(needle, comment);
                }
            }

            return defaultReturn;
        }

        /// <summary>
        /// Gets the requested value from the manager. The default return is returned if the value was not found.
        /// The value is added to the fromFile if the needle is not found.
        /// </summary>
        /// <param name="needle">The key used for the lookup.</param>
        /// <param name="defaultReturn">The default return value</param>
        /// <param name="comment">Set a comment for in the ini file</param>
        /// <returns></returns>
        public int Get(string needle, int defaultReturn = 0, string comment = null)
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

            if (!fromFile.ContainsKey(needle))
            {
                fromFile.Add(needle, defaultReturn.ToString());
                if (!(comment is null) && !comments.ContainsKey(needle))
                {
                    comments.Add(needle, comment);
                }
            }

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
