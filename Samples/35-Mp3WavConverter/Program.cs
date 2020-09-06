using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace mp3WavConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentPath = Directory.GetCurrentDirectory();
            var inFolder = Path.Combine(currentPath, "in");
            var outFolder = Path.Combine(currentPath, "out");

            ClearFiles(outFolder);

            ConvertFiles(inFolder, outFolder);

            Console.WriteLine("finished");

        }

        static void ConvertFiles (String sourcePath, String destPath)
        {
            var mp3Files = Directory.GetFiles(sourcePath, "*.mp3");

            if (mp3Files.Length == 0)
            {
                Console.WriteLine("not find any mp3 files");
                return;
            }


            foreach (var inFile in mp3Files)
            {
                if (!File.Exists(inFile))
                {
                    continue;
                }

                var name = Path.GetFileNameWithoutExtension(inFile);
                var outFile = Path.Combine(destPath, $"{name}.wav");

                using (var reader = new Mp3FileReader(inFile))
                {
                    WaveFileWriter.CreateWaveFile(outFile, reader);
                }
                
                Console.WriteLine($"converted: {outFile}");
            }
        }

        static void ClearFiles(String path)
        {
            var files = Directory.GetFiles(path);

            if (files.Length == 0)
            {
                return;
            }

            foreach(var filePath in files)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Console.WriteLine($"delete: {filePath}");
                }
            }
        }
    }
}
