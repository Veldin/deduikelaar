using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileReaderWriterSystem
{
    public interface IReadFormat
    {
        object ReadFile(string filePath);
    }
}
