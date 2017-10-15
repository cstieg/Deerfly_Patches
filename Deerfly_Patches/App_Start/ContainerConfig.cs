using Deerfly_Patches.Modules.FileStorage;

namespace ChristopherStieg.App_Start
{
    public class ContainerConfig
    {
        // Set container for storage
        public static string contentFolder = "/content";

        public static IFileService storageService = new FileSystemService(contentFolder);
    }
}