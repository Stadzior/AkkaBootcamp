using System;
using System.Collections.Generic;
using System.Text;

namespace WinTail.Messages
{
    public class InputSuccess
    { 
        public string Reason { get; }
        public InputSuccess(string reason)
        {
            Reason = reason;
        }
    }
}
