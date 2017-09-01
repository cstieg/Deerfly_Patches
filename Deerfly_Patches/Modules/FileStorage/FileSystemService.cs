using System.IO;
using System.Web;
using System.Web.Hosting;

namespace Deerfly_Patches.Modules.FileStorage
{
    public class FileSystemService : IFileSaver
    {
        public string BaseDiskPath { get; set; }
        public string BaseUrlPath { get; set; }

        public FileSystemService(string baseDiskPath, string baseUrlPath = "")
        {
            BaseDiskPath = baseDiskPath;
            if (baseUrlPath == "")
            {
                BaseUrlPath = HostingEnvironment.MapPath("~" + BaseDiskPath);
            }
            else
            {
                BaseUrlPath = baseUrlPath;
            }
        }

        public string SaveFile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength != 0)
            {
                string filePath = Path.Combine(BaseDiskPath, file.FileName);
                string fileUrl = BaseUrlPath + "/" + file.FileName;
                file.SaveAs(filePath);
                return fileUrl;
            }
            return "";
        }
    }
}