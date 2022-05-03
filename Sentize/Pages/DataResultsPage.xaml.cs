using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Sentize.Sentence;

namespace Sentize.Pages
{
    /// <summary>
    /// Interaction logic for DataResultsPage.xaml
    /// </summary>
    public partial class DataResultsPage : Page
    {
        public enum Type 
        {
            File,
            Database
        }

        private MainWindow mainWindow;
        private MainPage mainPage;
        private AnalysisResult analysisResult;
        private string password;
        private Type type;

        public DataResultsPage()
        {
            InitializeComponent();
        }

        internal void Init(Type type, MainWindow mainWindow, MainPage mainPage, string pwd="")
        {
            this.type = type;
            this.mainWindow = mainWindow;
            this.mainPage = mainPage;
            this.analysisResult = mainPage.AnalysisResult;
            password = pwd;

            this.Dispatcher.Invoke(() =>
            {
                if (type == Type.File)
                    lbl_ResultsTitle.Content = "File Results";
                else
                    lbl_ResultsTitle.Content = "Database Results";

                LoadChartData();
            });
        }

        public void LoadChartData()
        {
            // Calculate accuracies
            int[] totals = new int[7];
            int[] correctTotals = new int[7];
            foreach (Sentence sentence in analysisResult.Sentences)
            {
                totals[(int)sentence.ActualEmotion + 1]++;
                if (sentence.ActualEmotion == sentence.PredictedEmotion)
                    correctTotals[(int)sentence.ActualEmotion + 1]++;
            }

            float total = totals.Skip(1).Sum();
            float correctTotal = correctTotals.Skip(1).Sum();
            float accuracy = (correctTotal / total) * 100;
            ((ColumnSeries)chart_OverallAccuracy.Series[0]).ItemsSource =
                new KeyValuePair<string, int>[]{
                    new KeyValuePair<string, int>(string.Format("{0:N2}%", accuracy), (int)(accuracy))
                };

            float[] accuracies = new float[7];
            for (int i = 0; i < accuracies.Length; i++)
            {
                if (totals[i] != 0)
                    accuracies[i] = ((float)correctTotals[i] / (float)totals[i]) * 100;
                else
                    accuracies[i] = 0;
            }
            ((ColumnSeries)chart_IndividualAccuracy.Series[0]).ItemsSource =
                new KeyValuePair<string, int>[]{
                    new KeyValuePair<string, int>(string.Format("Joy {0:N2}%", (int)accuracies[(int)Emotion.JOY + 1]), (int)accuracies[(int)Emotion.JOY + 1]),
                    new KeyValuePair<string, int>(string.Format("Sadness {0:N2}%", (int)accuracies[(int)Emotion.SADNESS + 1]), (int)accuracies[(int)Emotion.SADNESS + 1]),
                    new KeyValuePair<string, int>(string.Format("Anger {0:N2}%", (int)accuracies[(int)Emotion.ANGER + 1]), (int)accuracies[(int)Emotion.ANGER + 1]),
                    new KeyValuePair<string, int>(string.Format("Fear {0:N2}%", (int)accuracies[(int)Emotion.FEAR + 1]), (int)accuracies[(int)Emotion.FEAR + 1]),
                    new KeyValuePair<string, int>(string.Format("Love {0:N2}%", (int)accuracies[(int)Emotion.LOVE + 1]), (int)accuracies[(int)Emotion.LOVE + 1]),
                    new KeyValuePair<string, int>(string.Format("Surprise {0:N2}%", (int)accuracies[(int)Emotion.SURPRISE + 1]), (int)accuracies[(int)Emotion.SURPRISE + 1])
                };

            ((BarSeries)chart_PredictionTotals.Series[0]).ItemsSource =
                new KeyValuePair<string, int>[]{
                    new KeyValuePair<string, int>("Joy", analysisResult.Sentences.Count(s => s.PredictedEmotion == Emotion.JOY)),
                    new KeyValuePair<string, int>("Sadness", analysisResult.Sentences.Count(s => s.PredictedEmotion == Emotion.SADNESS)),
                    new KeyValuePair<string, int>("Anger", analysisResult.Sentences.Count(s => s.PredictedEmotion == Emotion.ANGER)),
                    new KeyValuePair<string, int>("Fear", analysisResult.Sentences.Count(s => s.PredictedEmotion == Emotion.FEAR)),
                    new KeyValuePair<string, int>("Love", analysisResult.Sentences.Count(s => s.PredictedEmotion == Emotion.LOVE)),
                    new KeyValuePair<string, int>("Surprise", analysisResult.Sentences.Count(s => s.PredictedEmotion == Emotion.SURPRISE)),
                };
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.frm_MainWindow.Navigate(mainPage);

            //TODO: any cleanup from resetting?
        }

        private void btn_SendAsFeedback_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you like to help improve this app by sending the sentences as feedback?", "Sentize", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    // REMOVED FOR PRIVACY REASONS
                    MessageBox.Show("This feature has been redacted with the public release due to privacy reasons...", "Sentize");
                    break;
                case MessageBoxResult.No:
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        private void btn_SaveResults_Click(object sender, RoutedEventArgs e)
        {
            string newFileName = "";
            string promptStr;
            if (type == Type.File)
            {
                newFileName = mainPage.tb_FileToAnalyze.Text.Insert(mainPage.tb_FileToAnalyze.Text.LastIndexOf('.'), "_analyzed");
                promptStr = "This will save the results with a new column in the file '" + newFileName + "'. Continue?";
            }
            else
            {
                promptStr = "This will save the results in a new column titled 'predicted_results'. Continue?";
            }

            MessageBoxResult result = MessageBox.Show(promptStr, "Sentize", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    if (type == Type.File)
                    {
                        SentizeFileIO.SaveToFile(newFileName, analysisResult);
                    }
                    else
                    {
                        // Add column if it doesnt exist
                        string str = "server=" + mainPage.tb_ServerName.Text + ";database=" + mainPage.tb_DatabaseName.Text + ";UID=" + mainPage.tb_UserName.Text + ";password=" + password;
                        string query =
                            "IF COL_LENGTH('" + mainPage.tb_TableName.Text + "', 'predicted_emotion') IS NULL " +
                            "BEGIN " +
                            "    ALTER TABLE " + mainPage.tb_TableName.Text +
                            "    ADD predicted_emotion INT; " +
                            "END";

                        SqlConnection con = new SqlConnection(str);
                        SqlCommand cmd = new SqlCommand(query, con);
                        con.Open();

                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();

                        con.Close();

                        // Store in database
                        foreach (Sentence s in analysisResult.Sentences)
                        {
                            query =
                                "UPDATE " + mainPage.tb_TableName.Text +
                                " SET predicted_emotion = " + s.PredictedEmotion.ToString("d") +
                                " WHERE " + mainPage.tb_SentenceColumnName.Text + "='" + s.Str.Replace("\'", "'+CHAR(39)+'") + "';";

                            cmd = new SqlCommand(query, con);
                            con.Open();

                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();

                            con.Close();
                        }
                    }

                    MessageBox.Show("Results Saved!", "Sentize");
                    break;
                case MessageBoxResult.No:
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }
    }
}
