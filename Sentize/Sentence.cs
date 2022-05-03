using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sentize
{
    public class Sentence
    {
        public enum Emotion {
            NONE = -1,
            JOY = 0,
            SADNESS = 1,
            ANGER = 2,
            FEAR = 3,
            LOVE = 4,
            SURPRISE = 5,
        };

        internal string GetString()
        {
            throw new NotImplementedException();
        }

        public string Str { get; set; }
        public Emotion ActualEmotion { get; set; }
        public Emotion PredictedEmotion { get; set; }
        public float Confidence { get; set; }

        public Sentence(string str)
        {
            Str = str;
            ActualEmotion = Emotion.NONE;
            PredictedEmotion = Emotion.NONE;
        }

        public Sentence(string str, Emotion actualEmotion)
        {
            Str = str;
            ActualEmotion = actualEmotion;
            PredictedEmotion = Emotion.NONE;
        }

        public bool ContainsOnlyAscii()
        {
            const int MaxAsciiCode = 127;
            return !Str.Any(c => c > MaxAsciiCode);
        }

        public bool WithinMaxLength()
        {
            const int MaxLength = 256;
            return Str.Length <= MaxLength;
        }
    }
}
