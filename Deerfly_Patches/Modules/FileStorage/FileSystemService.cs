using System.IO;
using System.Text.RegularExpressions;
using System.Web.Hosting;

namespace Deerfly_Patches.Modules.FileStorage
{
    public class FileSystemService : IFileManager
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

        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        public void DeleteFilesWithWildcard(string filePath)
        {
            Filepath filepath = new Filepath(filePath);

            if (!filepath.Path.Equals(BaseUrlPath))
            {
                throw new DirectoryNotFoundException("Invalid directory");
            }

            Filename filename = new Filename(filepath.Filename);

            Regex findWidthTag = new Regex("-w[0-9]+$");
            filename.BaseName = findWidthTag.Replace(filename.BaseName, "-w*");
            filepath.Filename = filename.FileName;
            filepath.Path = BaseDiskPath;
            var files = Directory.GetFiles(filepath.Path, filepath.Filename);
            foreach (string file in files)
            {
                DeleteFile(file);
            }
        }
    }
}