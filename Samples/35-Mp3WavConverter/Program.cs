using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Sources;

namespace mp3WavConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            //ConvertJsonSTT();
            //ConvertMp3ToWav();
        }

        static void ConvertJsonSTT()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var inFolder = Path.Combine(currentPath, "source");
            var outFolder = Path.Combine(currentPath, "destination");

            var jsonFiles = Directory.GetFiles(inFolder, "*.json");
            foreach (var fileItem in jsonFiles)
            {
                var content = File.ReadAllText(fileItem);

                var result = JsonConvert.DeserializeObject<AzureBatchTranslationFile>(content);
                var translateData = result.AudioFileResults.FirstOrDefault();

                var sttObject = new STT();
                sttObject.AudioLengthInSeconds = translateData.AudioLengthInSeconds;
                sttObject.Display = translateData.CombinedResults[0].Display;

                // only maxest Confidence
                foreach (var segment in translateData.SegmentResults)
                {
                    segment.NBest = new NbestData[] { segment.NBest.OrderByDescending(x => x.Confidence).First() };
                }

                sttObject.SegmentResults = translateData.SegmentResults;

                string[] chineseSymbols = new string[] { "，", "。" };
                Regex matchRegex = new Regex(@"\，|\。");

                foreach (var segment in translateData.SegmentResults)
                {
                    var nBest = segment.NBest.First();

                    // find symbol and previous word, combine it.
                    var matchs = matchRegex.Matches(nBest.Display);
                    var currentIndex = 0;
                    var rawContent = "";

                    foreach (var item in nBest.Words)
                    {
                        currentIndex += item.Word.Length;
                        rawContent += item.Word;
                        var previousWord = matchs.Where(x => x.Index == currentIndex).FirstOrDefault();
                        if (previousWord != null)
                        {
                            item.Word += previousWord.Value;
                            rawContent += previousWord.Value;
                            currentIndex += 1;
                        }
                    }

                    sttObject.Words.AddRange(segment.NBest.First().Words);
                }

                var outputJson = JsonConvert.SerializeObject(sttObject);

                var fileName = Path.GetFileName(fileItem);
                var newFile = Path.Combine(outFolder, fileName);

                File.WriteAllText(newFile, outputJson);

                Console.WriteLine($"transcripted file: {fileName}");
            }
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
