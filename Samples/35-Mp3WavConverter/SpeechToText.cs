using System;
using System.Collections.Generic;
using System.Text;

namespace mp3WavConverter
{
    public class SpeechToText
    {
        public string source { get; set; }
        public DateTime timestamp { get; set; }
        public long durationInTicks { get; set; }
        public string duration { get; set; }
        public Combinedrecognizedphras[] combinedRecognizedPhrases { get; set; }
        public Recognizedphras[] recognizedPhrases { get; set; }
    }

    public class Combinedrecognizedphras
    {
        public int channel { get; set; }
        public string lexical { get; set; }
        public string itn { get; set; }
        public string maskedITN { get; set; }
        public string display { get; set; }
    }

    public class Recognizedphras
    {
        public string recognitionStatus { get; set; }
        public int channel { get; set; }
        public string offset { get; set; }
        public string duration { get; set; }
        public float offsetInTicks { get; set; }
        public float durationInTicks { get; set; }
        public Nbest[] nBest { get; set; }
    }

    public class Nbest
    {
        public float confidence { get; set; }
        public string lexical { get; set; }
        public string itn { get; set; }
        public string maskedITN { get; set; }
        public string display { get; set; }
        public Word[] words { get; set; }
    }

    public class Word
    {
        public string word { get; set; }
        public string offset { get; set; }
        public string duration { get; set; }
        public float offsetInTicks { get; set; }
        public float durationInTicks { get; set; }
        public float confidence { get; set; }
    }
}
