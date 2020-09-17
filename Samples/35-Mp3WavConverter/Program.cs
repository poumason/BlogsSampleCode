using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            var jsonFile = Path.Combine(currentPath, "");
            var content = File.ReadAllText(jsonFile);

            var result = JsonConvert.DeserializeObject<AzureBatchTranslationFile>(content);
            var translateData = result.AudioFileResults.FirstOrDefault();

            var sttObject = new STT();
            sttObject.AudioLengthInSeconds = translateData.AudioLengthInSeconds;
            sttObject.Display = translateData.CombinedResults[0].Display;

            foreach (var segment in translateData.SegmentResults)
            {
                segment.NBest = new NbestData[] { segment.NBest.OrderByDescending(x => x.Confidence).First() };
            }

            sttObject.SegmentResults = translateData.SegmentResults;

            foreach (var segment in translateData.SegmentResults)
            {
                sttObject.Words.AddRange(segment.NBest.First().Words);
            }

            var outputJson = JsonConvert.SerializeObject(sttObject);
            File.WriteAllText("", outputJson);
            Console.WriteLine(translateData.SegmentResults);
            //ConvertMp3ToWav();
        }

        static void ConvertMp3ToWav()
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
