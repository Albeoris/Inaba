using System;
using System.IO;

namespace Inaba.Framework
{
    public static class FileSystem
    {
        public static void CreateFileDirectory(String filePath)
        {
            String? directoryName = Path.GetDirectoryName(Path.GetFullPath(filePath));
            if (directoryName is null)
                throw new ArgumentException(filePath, nameof(filePath));

            Directory.CreateDirectory(directoryName);
        }
    }
}