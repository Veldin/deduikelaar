using LogSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileReaderWriterSystem
{
    public static class FileReaderWriterFacade
    {
        private static FileWriter fileWriter = new FileWriter();
        private static FileReader fileReader = new FileReader();
        private static readonly string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Labyrint\\";

        public static void Init()
        {
            fileWriter.CreateFolder(appDataPath);
            fileWriter.CreateFolder(appDataPath + "Log");
            fileWriter.CreateFolder(appDataPath + "Items");
        }

        /// <summary>
        /// This method reads the text in a text file and returns the whole text
        /// </summary>
        /// <param name="filePath">Give the file path of the file</param>
        /// <returns>Returns all text from the file</returns>
        public static string ReadFile(string filePath, string readFormat = null, object value = null)
        {
            return fileReader.ReadFile(appDataPath + filePath, readFormat, value);
        }

        /// <summary>
        /// This method returns an array with the names of all files and folders in the folder
        /// </summary>
        /// <param name="filePath">The file path of the folder</param>
        /// <returns>All the names of the files in the folder</returns>
        public static string[] CheckFolder(string filePath)
        {
            return fileReader.CheckFolder(appDataPath + filePath);
        }

        /// <summary>
        /// This method writes text in a file
        /// </summary>
        /// <param name="text">Give the text in an array. Every element of the array is a seperate line.</param>
        /// <param name="filePath">Give the file path of the file where the text needs to be written in. If it doesn't exist it will be created.</param>
        /// <param name="append">If true the text will be added to the file. If false the new text will overwrite all already existing text.</param>
        public static void WriteText(string[] text, string filePath, bool append, string writeFormat = null)
        {
            fileWriter.WriteText(text, appDataPath + filePath, append, writeFormat);
        }

        /// <summary>
        /// This method deletes files
        /// </summary>
        /// <param name="filePaths">Give an array with file paths of files that need to be deleted</param>
        public static void DeleteFile(string[] filePaths)
        {
            // Loop through the filePaths 
            foreach (string filePath in filePaths)
            {
                // Check if the file exist
                if (File.Exists(appDataPath + filePath))
                {
                    // Delete the file
                    File.Delete(appDataPath + filePath);
                    Log.Debug(appDataPath + filePath + " is deleted");
                }
                else
                {
                    // Throw a warning
                    Log.Warning("Could not delete '" + appDataPath + filePath + "' because the file path is invalid");
                }

            }
        }


    }
}

