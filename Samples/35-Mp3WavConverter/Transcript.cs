using System;
using System.Collections.Generic;
using System.Text;

namespace mp3WavConverter.Custom
{
    public class Transcript
    {
        public string full_text { get; set; }

        public List<Sentence> sentences { get; set; }

        public Transcript()
        {
            sentences = new List<Sentence>();
        }
    }

    public class Sentence : TextUnit
    {
        public List<TextUnit> words;

        public Sentence()
        {
            words = new List<TextUnit>();
        }
    }

    public class TextUnit
    {
        public string text { get; set; }

        public int start_at_ms { get; set; }

        public int duration_ms { get; set; }
    }
}
