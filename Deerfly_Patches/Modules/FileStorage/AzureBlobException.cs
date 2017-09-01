using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Deerfly_Patches.Modules.FileStorage
{
    public class AzureBlobException : Exception
    {
        public AzureBlobException() : base() { }

        public AzureBlobException(string message) : base(message) { }
    }
}