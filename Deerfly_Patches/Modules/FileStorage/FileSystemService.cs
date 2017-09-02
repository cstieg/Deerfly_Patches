using System.IO;
using System.Web.Hosting;

namespace Deerfly_Patches.Modules.FileStorage
{
    public class FileSystemService : IFileSaver
    {
        public string BaseDiskPath { get; set; }
        public string BaseUrlPath { get; set; }

        public FileSystemService(string baseUrlPath, string baseDiskPath = "")
        {
            BaseUrlPath = baseUrlPath;
            if (baseDiskPath == "")
            {
                BaseDiskPath = HostingEnvironment.MapPath("~" + BaseUrlPath);
            }
            else
            {
                BaseDiskPath = baseDiskPath;
            }
        }

        public string SaveFile(Stream stream, string name)
        {
            if (stream.Length != 0)
            {
                string filePath = Path.Combine(BaseDiskPath, name);
                string fileUrl = BaseUrlPath + "/" + name;
                using (FileStream savingFile = File.Create(filePath))
                {
                    stream.CopyTo(savingFile);
                }
                return fileUrl;
            }
            return "";
        }
    }
}