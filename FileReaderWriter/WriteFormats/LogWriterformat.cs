using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileReaderWriterSystem
{
    public class LogWriterformat : IWriteFormat
    {
        public LogWriterformat()
        {

        }

        public void WriteText(string[] text, string filePath, bool append)
        {
            using (StreamWriter outputFile = new StreamWriter(filePath, append))
            {
                // Write the lines one by one in the file
                for (int i = 0; i < text.Length; i++)
                {
                    switch (i % 7)
                    {
                        case 0:
                            outputFile.WriteLine("LogLevel: " + text[i]);
                            break;
                        case 1:
                            outputFile.WriteLine("Date: " + text[i]);
                            break;
                        case 2:
                            outputFile.WriteLine("Time: " + text[i]);
                            break;
                        case 3:
                            outputFile.WriteLine("File: " + text[i]);
                            break;
                        case 4:
                            outputFile.WriteLine("Method: " + text[i]);
                            break;
                        case 5:
                            outputFile.WriteLine("Line number: " + text[i]);
                            break;
                        case 6:
                            outputFile.WriteLine(" ");
                            outputFile.WriteLine("Message: " + text[i]);
                            outputFile.WriteLine(" ");
                            outputFile.WriteLine("-------------------------------------------------------------");
                            break;
                    }
                }
            }
        }
    }
}
