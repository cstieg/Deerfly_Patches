using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Deerfly_Patches.Modules.FileStorage
{
    /// <summary>
    /// A helper class to add extensions to the Stream class
    /// </summary>
    public static class StreamHelper
    {
        /// <summary>
        /// An extension to Stream which clones the Stream object to a MemoryStream
        /// </summary>
        /// <param name="stream">The Stream object to be cloned</param>
        /// <returns>A MemoryStream object copy of the original Stream</returns>
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