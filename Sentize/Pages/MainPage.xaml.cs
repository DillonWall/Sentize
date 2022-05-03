using Microsoft.Win32;
using Sentize.Pages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private MainWindow mainWindow;
        private TransitionPage transitionPage;
        private SentenceResultsPage sentenceResultsPage;
        private DataResultsPage dataResultsPage;
        private ErrorMessageBox errorMessageBox;
        private Analyze analyzer;
        private string pwd;

        public AnalysisResult AnalysisResult { get; set; }

        public MainPage()
        {
            InitializeComponent();
        }

        public void Init(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            this.transitionPage = new TransitionPage();
            this.sentenceResultsPage = new SentenceResultsPage();
            this.dataResultsPage = new DataResultsPage();
            this.errorMessageBox = new ErrorMessageBox(tb_ErrorMessage, mainWindow);
            this.analyzer = new Analyze("BERT_Emotion.onnx", "analyze_sentences.py");

            this.cmb_ActualEmotion.ItemsSource = Enum.GetValues(typeof(Emotion));
            this.cmb_ActualEmotion.SelectedItem = Emotion.NONE;
        }

        
        private void Sleep(object length)
        {
            Thread.Sleep((int)length * 1000);
        }

        private void AnalyzeSentence(object sentence)
        {
            this.AnalysisResult = analyzer.AnalyzeSentence((Sentence)sentence);
            this.sentenceResultsPage.Init(mainWindow, this);
        }

        private void btn_AnalyzeSentence_Click(object sender, RoutedEventArgs e)
        {
            Sentence sentence = new Sentence(tb_Sentence.Text);
            if (!sentence.ContainsOnlyAscii())
            {
                errorMessageBox.ShowMessage("Error: The sentence contains non-ASCII characters.");
                return;
            }
            sentence.ActualEmotion = (Emotion)cmb_ActualEmotion.SelectedItem;

            mainWindow.frm_MainWindow.Navigate(transitionPage);
            transitionPage.Transition(
                    AnalyzeSentence,
                    sentence,
                    mainWindow.frm_MainWindow,
                    sentenceResultsPage,
                    null
                );
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Data files (*.txt, *.csv)|*.txt;*.csv|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                tb_FileToAnalyze.Text = openFileDialog.FileName;
            }
        }

        private void AnalyzeFile(object sentences)
        {
            this.AnalysisResult = analyzer.AnalyzeSentences((List<Sentence>)sentences);
            this.dataResultsPage.Init(DataResultsPage.Type.File, mainWindow, this);
        }

        private void btn_AnalyzeFile_Click(object sender, RoutedEventArgs e)
        {
            List<Sentence> sentences = SentizeFileIO.ReadFile(tb_FileToAnalyze.Text, nud_Sentence.Value, nud_Emotion.Value);

            mainWindow.frm_MainWindow.Navigate(transitionPage);
            transitionPage.Transition(
                    AnalyzeFile,
                    sentences,
                    mainWindow.frm_MainWindow,
                    dataResultsPage,
                    null
                );
        }

        private void AnalyzeDatabase(object sentences)
        {
            this.AnalysisResult = analyzer.AnalyzeSentences((List<Sentence>)sentences);
            this.dataResultsPage.Init(DataResultsPage.Type.Database, mainWindow, this, pwd);
        }

        private void btn_AnalyzeDatabase_Click(object sender, RoutedEventArgs e)
        {
            // TODO: add validation
            string server = tb_ServerName.Text;
            string databaseName = tb_DatabaseName.Text;
            string uid = tb_UserName.Text;
            pwd = pb_Password.Password;
            string sentenceColumn = tb_SentenceColumnName.Text;
            string emotionColumn = tb_EmotionColumnName.Text;
            string tableName = tb_TableName.Text;
            bool hasEmotionCol = !string.IsNullOrWhiteSpace(emotionColumn) && emotionColumn.ToLower() != "none";

            string str = "server=" + server + ";database=" + databaseName + ";UID=" + uid + ";password=" + pwd;
            string query = "SELECT " + sentenceColumn;
            if (hasEmotionCol)
                query += ", " + emotionColumn;
            query += " FROM " + tableName;

            SqlConnection con = new SqlConnection(str);
            SqlDataAdapter adapter = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();
            adapter.Fill(dt);

            // Create list of sentences from data
            List<Sentence> sentences = new List<Sentence>();
            foreach(DataRow row in dt.Rows)
            {
                if (hasEmotionCol)
                    sentences.Add(new Sentence((string)row[0], (Sentence.Emotion)(int)row[1])); //TODO: Add error checking for casts
                else
                    sentences.Add(new Sentence((string)row[0]));
                // TODO: validate sentences, if invalid, drop from list
            }

            con.Close();

            mainWindow.frm_MainWindow.Navigate(transitionPage);
            transitionPage.Transition(
                    AnalyzeDatabase,
                    sentences,
                    mainWindow.frm_MainWindow,
                    dataResultsPage,
                    null
                );
        }
    }
}
