using System;
using System.Collections.Generic;
using System.Text;

namespace WinTail.Messages
{
    /// <summary>
    /// Start tailing the file at user-specified path.
    /// </summary>
    public class StopTail
    {
        public StopTail(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; }
    }
}
