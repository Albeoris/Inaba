using System;
using System.IO;

namespace Inaba.PAC
{
    public sealed class OutputPathBuilder : IOutputPathBuilder
    {
        private readonly String _outputDirectory;

        public OutputPathBuilder(String outputDirectory)
        {
            _outputDirectory = outputDirectory;
        }

        public String GetOutputPath(String relativePath, Int32 fileIndex)
        {
            String outputPath = Path.Combine(_outputDirectory, relativePath);
            String directoryPath = Path.GetDirectoryName(outputPath);
            String fileName = Path.GetFileNameWithoutExtension(outputPath);
            String extension = Path.GetExtension(outputPath);
            return Path.Combine(directoryPath, $"{fileIndex:D6}_{fileName}{extension}");
        }
    }
}