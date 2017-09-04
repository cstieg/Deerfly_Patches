using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Deerfly_Patches.Modules.FileStorage
{
    public class Filename
    {
        public Filename(string filename)
        {
            FileName = filename;
        }

        public string FileName { get; set; }

        public int Length
        {
            get
            {
                return FileName.Length;
            }
        }

        public int ExtensionLength
        {
            get
            {
                // example: abc.gif  
                //   Length = 7
                //   LastIndexOf('.') = 3
                //   ExtensionLength = 4 ('.gif')
                return Length - FileName.LastIndexOf('.');
            }
        }

        public string Extension
        {
            get
            {
                return FileName.Substring(Length - ExtensionLength);
            }
            set
            {
                FileName = BaseName + value;
            }
        }


        public string BaseName
        {
            get
            {
                return FileName.Substring(0, Length - ExtensionLength);
            }
            set
            {
                FileName = value + Extension;
            }
        }

    }
}