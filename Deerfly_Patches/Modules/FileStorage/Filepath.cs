using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Deerfly_Patches.Modules.FileStorage
{
    public class Filepath
    {
        public Filepath(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; set; }

        public int Length
        {
            get
            {
                return FilePath.Length;
            }
        }

        public int PathLength
        {
            get
            {
                int lastSlash = FilePath.LastIndexOf('/');
                if (lastSlash == -1)
                {
                    return 0;
                }
                return FilePath.LastIndexOf('/');
            }
        }

        public int FilenameLength
        {
            get
            {
                int lastSlash = FilePath.LastIndexOf('/');
                if (lastSlash == -1)
                {
                    return Length;
                }
                return Length - FilePath.LastIndexOf('/') - 1;
            }
        }

        public string Filename
        {
            get
            {
                return FilePath.Substring(Length - FilenameLength);
            }
            set
            {
                FilePath = Path + "/" + value;
            }
        }

        public string Path
        {
            get
            {
                return FilePath.Substring(0, PathLength);
            }
            set
            {
                FilePath = value + "/" + Filename;
            }
        }
    }
}