﻿using System.IO;

namespace Deerfly_Patches.Modules.FileStorage
{
    /// <summary>
    /// An interface for file IO, whether by disk or cloud
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// Saves a file to some storage medium, whether disk or cloud
        /// </summary>
        /// <param name="stream">the stream of the file to save</param>
        /// <returns>the URL by which the file is publicly accessible </returns>
        string SaveFile(Stream stream, string name, bool timeStamped = true);

        /// <summary>
        /// Deletes a file from a storage medium
        /// </summary>
        /// <param name="filePath">The URL of the file to delete</param>
        void DeleteFile(string filePath);

        /// <summary>
        /// Deletes all files from a folder in a storage medium matching a wildcard pattern
        /// </summary>
        /// <param name="filePath">The URL of the file to delete, including wildcards</param>
        void DeleteFilesWithWildcard(string filePath);
    }
}
