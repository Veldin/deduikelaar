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
        public FileReader()
        {

        }

        /// <summary>
        /// This method reads the text in a text file and returns the whole text
        /// </summary>
        /// <param name="filePath">Give the file path of the file</param>
        /// <returns>Returns all text from the file</returns>
        public string ReadFile(string filePath)
        {
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(filePath))
                {
                    // Read the stream to a string
                    String line = sr.ReadToEnd();

                    // Log the file that is read
                    Log.Debug("The following file is read: " + filePath);

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
            // Create a DirectoryInf o from the directory of the given pathfile
            DirectoryInfo dirInfo = new DirectoryInfo(filePath);

            // Check if the file path is valic
            if (!dirInfo.Exists)
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

            // Debugging all files that will be returned
            foreach (string a in fileNames)
            {
                Log.Debug(a);
            }

            return fileNames;
        }

    }
}
