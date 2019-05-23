using LogSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileReaderWriterSystem
{
    class FileReader
    {
        private ReadFormatFactory factory;
        private Dictionary<string, IReadFormat> readFormats;

        public FileReader()
        {
            factory = new ReadFormatFactory();

            readFormats = new Dictionary<string, IReadFormat>();
        }

        /// <summary>
        /// This method reads the text in a text file and returns the whole text
        /// </summary>
        /// <param name="filePath">Give the file path of the file</param>
        /// <returns>Returns all text from the file</returns>
        public string ReadFile(string filePath, string readFormat = null, object value = null)
        {
            // Check if a write format is requested
            if (readFormat != null)
            {
                // Check if the write format does already exist
                if (readFormats.ContainsKey(readFormat))
                {
                    // Write the text with the method of the write format
                    readFormats[readFormat].ReadFile(filePath, value);
                    return null;
                }
                else
                {
                    // Try to add the write format to the writeformats dictionarty
                    AddReadFormat(readFormat);
                }

                // Try to write the text with the writeFormat
                if (readFormats.ContainsKey(readFormat))
                {
                    readFormats[readFormat].ReadFile(filePath, value);
                    return null;
                }
            }

            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(filePath))
                {
                    // Read the stream to a string
                    String line = sr.ReadToEnd();

                    // Log the file that is read
                    Log.Debug("The following file is read: " + filePath);

                    Log.Debug(line);
                    // Return the contents of the file
                    return line;
                }
            }
            catch (IOException e)
            {
                // Throw a warning
                Log.Warning("File '" + filePath + "' could not be read");
                Log.Warning(e.Message);
            }

            return null;
        }

        /// <summary>
        /// This method returns an array with the names of all files and folders in the folder
        /// </summary>
        /// <param name="filePath">The file path of the folder</param>
        /// <returns>All the names of the files in the folder</returns>
        public string[] CheckFolder(string filePath)
        {
            // Create and empty DirectoryInfo
            DirectoryInfo dirInfo = null;

            // Check if the file path is valid
            try
            {
                // Create a DirectoryInf o from the directory of the given pathfile
                dirInfo = new DirectoryInfo(filePath);
            }
            catch
            {
                Log.Warning("The given file path: '" + filePath + "' is not valid");
                return null;
            }

            // Get all file with an extention and folders
            FileInfo[] files = dirInfo.GetFiles("*.*");
            DirectoryInfo[] folders = dirInfo.GetDirectories();

            // Create an array to place the names in
            string[] fileNames = new string[files.Length + folders.Length];

            // Set the names of all files with extention in the string array
            for (int i = 0; i < files.Length; i++)
            {
                fileNames[i] = files[i].Name;
            }

            // Set the names of the folders in the string array
            for (int j = 0; j < folders.Length; j++)
            {
                fileNames[fileNames.Length - folders.Length + j] = folders[j].Name;
            }
            
            Log.Debug("The contents of the following folder is shown: " + filePath);

            return fileNames;
        }

        /// <summary>
        /// This method adds a new writeFormat to the dictionary
        /// </summary>
        /// <param name="key">The name of the writeFormat</param>
        /// <param name="writeFormat">The actual writeFormat</param>
        private void AddReadFormat(string key)
        {
            if (factory.CreateReadFormat(key) != null)
            {
                readFormats.Add(key, factory.CreateReadFormat(key));
            }
        }

    }
}
