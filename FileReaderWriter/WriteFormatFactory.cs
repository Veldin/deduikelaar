using LogSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileReaderWriterSystem
{
    public class WriteFormatFactory
    {
        public WriteFormatFactory()
        {

        }

        public IWriteFormat CreateWriteFormat(string writeFormatId)
        {
            // Define the writeFormat
            IWriteFormat writeFormat;

            // Create a writeFormat depending on the writeFormatId
            switch (writeFormatId)
            {
                case "logFormat":
                    writeFormat = new LogWriterformat();
                    break;
                default:
                    writeFormat = null;
                    break;
            }

            Log.Debug(writeFormat);
            // Return the writeFormat
            return writeFormat; 
        }
    }
}
