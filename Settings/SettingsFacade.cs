using FileReaderWriterSystem;
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
        }

        /// <summary>
        /// Gets the requested value from the manager. The default return is returned if the value was not found.
        /// </summary>
        /// <param name="needle">The key used for the lookup.</param>
        /// <param name="defaultReturn">The default return value</param>
        /// <returns></returns>
        public static string Get(string needle, string defaultReturn = null)
        {
            return manager.Get(needle, defaultReturn);
        }

        /// <summary>
        /// Gets the requested value from the manager. The default return is returned if the value was not found.
        /// </summary>
        /// <param name="needle">The key used for the lookup.</param>
        /// <param name="defaultReturn">The default return value</param>
        /// <returns></returns>
        public static int Get(string needle, int defaultReturn = 0)
        {
            return manager.Get(needle, defaultReturn);
        }
    }
}
