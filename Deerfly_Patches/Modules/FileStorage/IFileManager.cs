using System.IO;

namespace Deerfly_Patches.Modules.FileStorage
{
    public interface IFileManager
    {
        /// <summary>
        /// Saves a file to some storage medium, whether disk or cloud
        /// </summary>
        /// <param name="stream">the stream of the file to save</param>
        /// <returns>the URL by which the file is publicly accessible </returns>
        string SaveFile(Stream stream, string name);
        void DeleteFile(string filePath);
        void DeleteFilesWithWildcard(string filePath);
    }
}
