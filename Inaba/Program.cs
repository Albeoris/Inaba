using System;
using System.IO;
using Inaba.CPK;
using Inaba.Huffman;
using Inaba.PAC;

namespace Inaba
{
    class Program
    {
        static void Main(String[] args)
        {
            try
            {
                var mode = args[0];
                switch (mode)

                {
                    case "unpack":
                    {
                        String pacFile = args[1];
                        String outputDirectory = args[2];
                        Unpack(pacFile, outputDirectory);
                        break;
                    }
                    case "decompress":
                    {
                        String inputFile = args[1];
                        String outputFile = args[2];
                        Decompress(inputFile, outputFile);
                        break;
                    }
                    default:
                    {
                        throw new NotSupportedException(mode);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine();
                Console.WriteLine("unpack <pacFile> <outputDirectory>");
                Console.WriteLine("decompress <pacFile> <outputFile>");
                Console.WriteLine("decompress <cpkPath> <outputFile>");
                Console.WriteLine();
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
        }

        private static void Unpack(String pacFile, String outputDirectory)
        {
            if (!File.Exists(pacFile))
                throw new FileNotFoundException(pacFile);

            if (!String.Equals(Path.GetExtension(pacFile), ".pac", StringComparison.OrdinalIgnoreCase))
                throw new NotSupportedException($"Unexpected argument: {pacFile}. Expected: .pac file path.");

            if (File.Exists(outputDirectory))
                throw new NotSupportedException($"Unexpected argument: file {outputDirectory}. Expected: output directory path.");

            Directory.CreateDirectory(outputDirectory);

            using var input = File.OpenRead(pacFile);

            DataReader reader = new DataReader();
            IOutputPathBuilder pathBuilder = new OutputPathBuilder(outputDirectory);
            PacUnpacker unpacker = new PacUnpacker(input, reader, pathBuilder);
            unpacker.Unpack();
        }

        private static void Decompress(String inputFile, String outputFile)
        {
            if (!File.Exists(inputFile))
                throw new FileNotFoundException(inputFile);

            if (Directory.Exists(outputFile))
                throw new NotSupportedException($"Unexpected argument: directory {outputFile}. Expected: output file path.");

            var extension = Path.GetExtension(inputFile);
            if (String.Equals(extension, ".pac", StringComparison.OrdinalIgnoreCase))
            {
                PacDecompressor.Decompress(inputFile, outputFile, new DataReader());
            }
            else if (String.Equals(extension, ".cpk", StringComparison.OrdinalIgnoreCase))
            {
                CpkDecompressor.Decompress(inputFile, outputFile, new DataReader());
            }
            else
            {
                throw new NotSupportedException($"Unexpected argument: {inputFile}. Expected: .pac or .cpk file path.");
            }
        }
    }
}