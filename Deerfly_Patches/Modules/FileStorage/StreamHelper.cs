using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Deerfly_Patches.Modules.FileStorage
{
    public static class StreamHelper
    {
        public static MemoryStream CloneToMemoryStream(this Stream stream)
        {
            long originalStreamPosition = stream.Position;
            stream.Position = 0;
            Type T = stream.GetType();
            MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            stream.Position = originalStreamPosition;
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}