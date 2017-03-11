using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csarc
{
    [Serializable]
    internal class FileEntry 
    {
        public Int64 OffsetStart { get; set; }
        public Int32 Size { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; } //all actions on that string will preform using the static class Path.

    }
}
