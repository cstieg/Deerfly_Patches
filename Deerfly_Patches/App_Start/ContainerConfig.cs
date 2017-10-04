using Deerfly_Patches.Modules.FileStorage;
using Deerfly_Patches.Modules.FileStorage.Amazon;

namespace ChristopherStieg.App_Start
{
    public class ContainerConfig
    {
        // Set container for storage
        public static string domainName = "deerflypatches.com";
        public static string contentFolder = "/content";

        public static IFileService storageService = new FileSystemService(contentFolder);
        //public static IFileService storageService = new S3Service(domainName);
    }
}