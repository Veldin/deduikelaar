using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileReaderWriterSystem
{
    public interface IWriteFormat
    {
        void WriteText(string[] text, string filePath, bool append);
    }
}
