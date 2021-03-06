﻿using FileReaderWriterSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settings
{
    public static class SettingsFacade
    {
        private static Manager manager;

        /// <summary>
        /// Initialises the manager.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public static bool Init()
        {
            manager = new Manager();

            return true;
        }

        /// <summary>
        /// Calls the PopulateToIni of the manager.
        /// </summary>
        public static void Save()
        {
            manager.PopulateToIni();
            manager.RefeshSettings();
        }

        /// <summary>
        /// Gets the requested value from the manager. The default return is returned if the value was not found.
        /// </summary>
        /// <param name="needle">The key used for the lookup.</param>
        /// <param name="defaultReturn">The default return value</param>
        /// <param name="comment">Set a comment for in the ini file</param>
        /// <returns></returns>
        public static string Get(string needle, string defaultReturn = null, string comment = null)
        {
            return manager.Get(needle, defaultReturn, comment);
        }

        /// <summary>
        /// Gets the requested value from the manager. The default return is returned if the value was not found.
        /// </summary>
        /// <param name="needle">The key used for the lookup.</param>
        /// <param name="defaultReturn">The default return value</param>
        /// <param name="comment">Set a comment for in the ini file</param>
        /// <returns></returns>
        public static int Get(string needle, int defaultReturn = 0, string comment = null)
        {
            return manager.Get(needle, defaultReturn, comment);
        }

        /// <summary>
        /// Get the value of a setting
        /// </summary>
        /// <param name="needle">The name of the setting</param>
        /// <returns>Returns the value of a setting</returns>
        public static string GetSetting(string key)
        {
            return manager.GetSetting(key);
        }

        /// <summary>
        /// Set a value to an already existing setting
        /// </summary>
        /// <param name="key">Setting name</param>
        /// <param name="value">The new value of the setting</param>
        public static void SetSetting(string key, string value)
        {
            manager.SetSetting(key, value);
        }

    }
}
