using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Deerfly_Patches.Modules.FileStorage
{
    public class InvalidFileTypeException : Exception
    {
        public InvalidFileTypeException() : base() { }

        public InvalidFileTypeException(string message) : base(message) { }
    }
}