using System;
using System.Collections.Generic;
using System.Text;

namespace WinTail.Messages
{
    public class FileWrite
    {
        public FileWrite(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; }
    }
}
