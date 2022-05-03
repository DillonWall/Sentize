using CsvHelper;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using static Sentize.Sentence;

namespace Sentize.Pages
{
    public class SentizeFileIO
    {
        public static List<Sentence> ReadFile(string filename, int sentenceIndex, int emotionIndex)
        {
            List<Sentence> sentences = new List<Sentence>();

            using (TextFieldParser parser = new TextFieldParser(filename))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",", "\t");
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    if (emotionIndex >= 0)
                        sentences.Add(new Sentence(fields[sentenceIndex], (Emotion)Int32.Parse(fields[emotionIndex])));
                    else
                        sentences.Add(new Sentence(fields[sentenceIndex]));
                }
            }

            return sentences;
        }

        public static void SaveToFile(string newFileName, AnalysisResult analysisResult)
        {
            //TODO: Could check if not using actual emotions, but Im fine with outputting -1s for now
            var records = analysisResult.Sentences.Select(s => new List<string> { s.Str, s.ActualEmotion.ToString("d"), s.PredictedEmotion.ToString("d") }).ToList();

            if (newFileName.EndsWith(".csv"))
            {
                using (var writer = new StreamWriter(newFileName))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    foreach (var record in records)
                    {
                        foreach (var field in record)
                        {
                            csv.WriteField(field);
                        }

                        csv.NextRecord();
                    }
                }
            }
            else 
            {
                string[] lines = records.Select(r => (r[0] + "\t" + r[1] + "\t" + r[2])).ToArray();
                File.WriteAllLines(newFileName, lines);
            }
        }
    }
}