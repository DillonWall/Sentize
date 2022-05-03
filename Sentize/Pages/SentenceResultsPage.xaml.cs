using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization.Charting;
using static Sentize.Sentence;
using System.Data.SqlClient;
using System.Data;

namespace Sentize.Pages
{
    /// <summary>
    /// Interaction logic for SentenceResultsPage.xaml
    /// </summary>
    public partial class SentenceResultsPage : Page
    {
        public readonly Dictionary<Emotion, string> EmotionNames = new Dictionary<Emotion, string>()
        {
            { Emotion.NONE, "None" },
            { Emotion.JOY, "Joy" },
            { Emotion.SADNESS, "Sadness" },
            { Emotion.ANGER, "Anger" },
            { Emotion.FEAR, "Fear" },
            { Emotion.LOVE, "Love" },
            { Emotion.SURPRISE, "Surprise" }
        };

        public readonly Dictionary<Emotion, string> EmotionImageFileNames = new Dictionary<Emotion, string>()
        {
            { Emotion.NONE, "None" },
            { Emotion.JOY, "pexels-artem-beliaikin-1485637.jpg" },
            { Emotion.SADNESS, "pexels-bruno-cervera-128817.jpg" },
            { Emotion.ANGER, "pexels-lucas-pezeta-2034913.jpg" },
            { Emotion.FEAR, "A-scared-dog-hiding-under-the-bed.jpg" },
            { Emotion.LOVE, "pexels-meru-bi-6589009.jpg" },
            { Emotion.SURPRISE, "pexels-aloïs-moubax-3523317.jpg" }
        };

        public readonly Dictionary<Emotion, string> EmotionImageCredits = new Dictionary<Emotion, string>()
        {
            { Emotion.NONE, "None" },
            { Emotion.JOY, "Photo by Artem Beliaikin from Pexels" },
            { Emotion.SADNESS, "Photo by Bruno Cervera from Pexels" },
            { Emotion.ANGER, "Photo by Lucas Pezeta from Pexels" },
            { Emotion.FEAR, "A scared puppy might hide. Photography ©hidako | Thinkstock" },
            { Emotion.LOVE, "Photo by Meru Bi from Pexels" },
            { Emotion.SURPRISE, "Photo by Aloïs Moubax from Pexels" }
        };

        private MainWindow mainWindow;
        private MainPage mainPage;
        private AnalysisResult analysisResult;

        public SentenceResultsPage()
        {
            InitializeComponent();
        }

        internal void Init(MainWindow mainWindow, MainPage mainPage)
        {
            this.mainWindow = mainWindow;
            this.mainPage = mainPage;
            this.analysisResult = mainPage.AnalysisResult;

            SetupUIElements();
        }

        private void SetupUIElements()
        {
            this.Dispatcher.Invoke(() =>
            {
                LoadChartData();

                Emotion predicted = analysisResult.Sentences[0].PredictedEmotion;
                Emotion actual = analysisResult.Sentences[0].ActualEmotion;
                lbl_EmotionTitle.Content = EmotionNames[predicted];
                img_EmotionImg.Source = new BitmapImage(new Uri(@"/res/" + EmotionImageFileNames[predicted], UriKind.Relative));
                lbl_ImgCredits.Content = EmotionImageCredits[predicted];
                tb_InputSentence.Text = analysisResult.Sentences[0].Str;

                if (actual == Emotion.NONE)
                    tb_ResultText.Text = "No actual emotion was given.";
                else if (predicted == actual)
                    tb_ResultText.Text = "The sentence was predicted correctly!";
                else
                    tb_ResultText.Text = "The sentence was NOT predicted correctly.";
            });
        }

        private void LoadChartData()
        {
            ((ColumnSeries)chart_Confidence.Series[0]).ItemsSource =
            new KeyValuePair<string, int>[]{
                new KeyValuePair<string, int>(string.Format("{0:N2}%", analysisResult.Sentences[0].Confidence*100), (int)(analysisResult.Sentences[0].Confidence*100))
            };
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.frm_MainWindow.Navigate(mainPage);

            //TODO: any cleanup from resetting?
        }

        private void btn_SendAsFeedback_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you like to help improve this app by sending this sentence as feedback?", "Sentize", MessageBoxButton.YesNoCancel);
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
    }
}
