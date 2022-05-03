using PyRunner;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Sentize.Sentence;

namespace Sentize
{
    public class Analyze
    {
        private string modelFileName;
        private string analyzeScriptFileName;
        private PythonRunner pythonRunner;

        public Analyze(string modelFileName, string analyzeScriptFileName)
        {
            this.modelFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res\\models\\" + modelFileName);
            this.analyzeScriptFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res\\analysis_scripts\\" + analyzeScriptFileName);
            this.pythonRunner = new PythonRunner(ConfigurationManager.AppSettings["pythonPath"], 20000);
        }


        private Emotion GetEmotionFromInt(int value)
        {
            return (Emotion)value;
        }

        private Sentence[] RunPythonAnalysis(Sentence[] sentences)
        {
            // Run script to print out all results
            string outputStr;
            string inputStr = sentences
                    .Select(s => s.Str)
                    .Aggregate((s1, s2) => s1 + "|" + s2);
            try
            {
                outputStr = pythonRunner.ExecuteAsync(
                    analyzeScriptFileName,
                    modelFileName,
                    inputStr).Result;
            }
            catch (Exception)
            {
                return null;
            }

            // Parse in results as int array
            List<(int,float)> outputs = outputStr
                .TrimStart('[')
                .TrimEnd(']')
                .Split(',')
                .Select(x =>
                    {
                        string[] temp = Regex.Replace(x, @"\s+", "").Trim('\'').Split(':');

                        return (Int32.Parse(temp[0]), float.Parse(temp[1]));
                    })
                .ToList();

            // Set each sentence's predicted emotion from int
            for (int i = 0; i < sentences.Length; i++)
            {
                sentences[i].PredictedEmotion = GetEmotionFromInt(outputs[i].Item1);
                sentences[i].Confidence = outputs[i].Item2;
            }

            return sentences;
        }

        public AnalysisResult AnalyzeSentence(Sentence sentence)
        {
            AnalysisResult result = new AnalysisResult();

            result.Sentences = RunPythonAnalysis(new Sentence[] { sentence });

            return result;
        }

        internal AnalysisResult AnalyzeSentences(List<Sentence> sentences)
        {
            AnalysisResult result = new AnalysisResult();

            result.Sentences = RunPythonAnalysis(sentences.ToArray());

            return result;
        }
    }
}
