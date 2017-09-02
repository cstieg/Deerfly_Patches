using System.Collections.Generic;

namespace Deerfly_Patches.Modules.FileStorage
{
    public static class ListHelper
    {
        public static List<T> Clone<T>(this List<T> original)
        {
            T[] data = new T[original.Count];
            original.CopyTo(data);
            return new List<T>(data);
        }
    }
}