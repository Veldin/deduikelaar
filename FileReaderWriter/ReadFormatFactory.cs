using LogSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileReaderWriterSystem
{
    public class ReadFormatFactory
    {
        public ReadFormatFactory()
        {

        }

        public IReadFormat CreateReadFormat(string readFormatId)
        {
            // Define the writeFormat
            IReadFormat readFormat;

            // Create a writeFormat depending on the writeFormatId
            switch (readFormatId)
            {
                default:
                    readFormat = null;
                    break;
            }

            // Return the writeFormat
            return readFormat; 
        }
    }
}
