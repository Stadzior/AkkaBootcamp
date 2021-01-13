using System;
using System.Collections.Generic;
using System.Text;

namespace WinTail.Messages
{
    public class NullInputError : InputError
    {
        public NullInputError(string reason) : base(reason)
        {
        }
    }
}
