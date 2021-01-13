using System;
using System.Collections.Generic;
using System.Text;

namespace WinTail.Messages
{
    public class InputError
    {
        public string Reason { get; }
        public InputError(string reason)
        {
            Reason = reason;
        }
    }
}
