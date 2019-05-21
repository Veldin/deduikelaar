using LogSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileReaderWriterSystem
{
    class HtmlReadFormat : IReadFormat
    {
        public HtmlReadFormat()
        {
        }

        public object ReadFile(string filePath)
        {
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(filePath))
                {
                    // Read the stream to a string
                    String line = sr.ReadToEnd();

                    // Log the file that is read
                    Log.Debug("The following file is read: " + filePath);

                    // Create a string array to return the storyId, storyType and the html
                    string[] document = new string[3];

                    // Split the storyId off and set it in the Document array
                    string[] soonToBeId = line.Split(new string[] { "storyId: " }, StringSplitOptions.None);
                    document[0] = soonToBeId[1].Split(',')[0];

                    // Split the storyType off and set it in the Document array
                    string[] soonToBeType = line.Split(new string[] { "type: " }, StringSplitOptions.None);
                    document[1] = soonToBeType[1].Split(',')[0];

                    // Split the html off and set it in the Document array
                    string[] soonToBeHtml = line.Split(new string[] { "html: " }, StringSplitOptions.None);
                    document[2] = soonToBeHtml[1].Split(',')[0];

                    foreach(string a in document)
                    {
                        Log.Debug("string: " +a);
                    }

                    // Return the contents of the file
                    return document;
                }
            }
            catch (IOException e)
            {
                // Throw a warning
                Log.Warning("File '" + filePath + "' could not be read");
                Log.Warning(e.Message);
                return null;
            }
        }
    }
}
