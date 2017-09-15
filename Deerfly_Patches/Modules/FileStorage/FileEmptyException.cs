using System;

namespace Deerfly_Patches.Modules.FileStorage
{
    public class FileEmptyException : Exception
    {
        public FileEmptyException() : base() { }

        public FileEmptyException(string message = "File is empty") : base(message) { }
    }
}