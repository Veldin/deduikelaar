using LogSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileReaderWriterSystem
{
    public class FileWriter : ILogObserver
    {
        private Dictionary<string, IWriteFormat> writeFormats;
        private WriteFormatFactory factory;
        private static object locker = new object();

        public FileWriter()
        {
            // Create the factory
            factory = new WriteFormatFactory();

            // Create the dictionary with the logFormat
            writeFormats = new Dictionary<string, IWriteFormat> { ["logFormat"] = new LogWriterformat() };

            // Subscribe to the log
            Log.Subscribe(this);
        }

        /// <summary>
        /// This is the Log observer. Every log message that is created is recieved here.
        /// </summary>
        /// <param name="message"></param>
        void ILogObserver.LogUpdate(LogMessage message)
        {
            // Split the dateTime on space so it is date and time
            string[] dataTime = message.GetCreationTime().Split(null);

            // Set the contents of the message in an array
            string[] text =
            {
                message.GetLogLevel().ToString(),
                dataTime[0],
                dataTime[1],
                message.GetFilePath(),
                message.GetMemberName(),
                message.GetLineNumber().ToString(),
                message.GetMessage()
            };

            // Write the array as logFormat in the log file
            WriteText(text, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Labyrint\\Log\\Log.txt", true, "logFormat");
        }

        /// <summary>
        /// This method writes text in a file
        /// </summary>
        /// <param name="text">Give the text in an array. Every element of the array is a seperate line.</param>
        /// <param name="filePath">Give the file path of the file where the text needs to be written in. If it doesn't exist it will be created.</param>
        /// <param name="append">If true the text will be added to the file. If false the new text will overwrite all already existing text.</param>
        public void WriteText(string[] text, string filePath, bool append, string writeFormat = null)
        {
            // Lock the whole process to make it thread safe
            lock (locker)
            {
                // Check if a write format is requested
                if (writeFormat != null)
                {
                    // Check if the write format does already exist
                    if (writeFormats.ContainsKey(writeFormat))
                    {
                        // Write the text with the method of the write format
                        writeFormats[writeFormat].WriteText(text, filePath, append);
                        return;
                    }
                    else
                    {
                        // Try to add the write format to the writeformats dictionarty
                        AddWriteFormat(writeFormat);
                    }

                    // Try to write the text with the writeFormat
                    if (writeFormats.ContainsKey(writeFormat))
                    {
                        writeFormats[writeFormat].WriteText(text, filePath, append);
                        return;
                    }
                }

                using (StreamWriter outputFile = new StreamWriter(filePath, append))
                {
                    // Write the lines one by one in the file
                    foreach (string line in text)
                    {
                        outputFile.WriteLine(line);
                    }
                }
                Log.Debug("Text is written in " + filePath);
            }
        }

        /// <summary>
        /// This method creates a folder at the file path
        /// </summary>
        /// <param name="filePath">The file path</param>
        public void CreateFolder(string filePath)
        {
            Directory.CreateDirectory(filePath);
        }

        /// <summary>
        /// This method adds a new writeFormat to the dictionary
        /// </summary>
        /// <param name="key">The name of the writeFormat</param>
        /// <param name="writeFormat">The actual writeFormat</param>
        private void AddWriteFormat(string key)
        {
            if (factory.CreateWriteFormat(key) != null)
            {
                writeFormats.Add(key, factory.CreateWriteFormat(key));
            }
        }
    }
}
