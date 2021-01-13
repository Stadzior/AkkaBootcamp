using System;
using System.Collections.Generic;
using System.Text;

namespace WinTail.Messages
{
    public class ValidationError : InputError
    {
        public ValidationError(string reason) : base(reason)
        {
        }
    }
}
