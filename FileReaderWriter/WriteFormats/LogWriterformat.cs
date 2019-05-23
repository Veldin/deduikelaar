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
            try
            {
                using (StreamWriter outputFile = new StreamWriter(filePath, append))
                {
                    // Write the lines one by one in the file
                    for (int i = 0; i < text.Length; i++)
                    {
                        switch (i % 7)
                        {
                            case 0:
                                outputFile.Write("LogLevel: " + text[i] + "\t\t");
                                break;
                            case 1:
                                outputFile.Write("Date: " + text[i] + "\t\t");
                                break;
                            case 2:
                                outputFile.Write("Time: " + text[i] + "\t\t");
                                break;
                            case 3:
                                outputFile.WriteLine("");
                                outputFile.Write("File: " + text[i] + "\t\t");
                                break;
                            case 4:
                                outputFile.Write("Method: " + text[i] + "\t\t");
                                break;
                            case 5:
                                outputFile.Write("Line number: " + text[i] + "\t\t");
                                break;
                            case 6:
                                outputFile.WriteLine(" ");
                                outputFile.WriteLine("Message: " + text[i]);
                                outputFile.WriteLine("-------------------------------------------------------------");
                                break;
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}
