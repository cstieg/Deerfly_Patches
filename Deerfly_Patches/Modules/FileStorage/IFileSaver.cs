using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Deerfly_Patches.Modules.FileStorage
{
    interface IFileSaver
    {
        /// <summary>
        /// Saves a file to some storage medium, whether disk or cloud
        /// </summary>
        /// <param name="file">a file derived from a POST request</param>
        /// <returns>the URL by which the file is publicly accessible </returns>
        string SaveFile(HttpPostedFileBase file);
    }
}
