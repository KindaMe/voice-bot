using System;
using System.IO;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace VoiceBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private String[] _grammarFile = File.ReadAllLines("grammar.txt");
        private String[] _responseFile = File.ReadAllLines("response.txt");

        private SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();

        private Choices _grammarList = new Choices();
        private SpeechRecognitionEngine _speechRecognitionEngine = new SpeechRecognitionEngine();

        public MainWindow()
        {
            _grammarList.Add(_grammarFile);
            Grammar grammar = new Grammar(new GrammarBuilder(_grammarList));

            try
            {
                _speechRecognitionEngine.RequestRecognizerUpdate();
                _speechRecognitionEngine.LoadGrammar(grammar);
                _speechRecognitionEngine.SpeechRecognized += SpeechRecognitionEngineOnSpeechRecognized;
                _speechRecognitionEngine.SetInputToDefaultAudioDevice();
                _speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch
            {
                return;
            }

            _speechSynthesizer.SelectVoiceByHints(VoiceGender.Female);

            InitializeComponent();
        }

        public void Say(String text)
        {
            _speechSynthesizer.SpeakAsync(text);
        }

        private void SpeechRecognitionEngineOnSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            String result = e.Result.Text;

            int resp = Array.IndexOf(_grammarFile, result);

            if (_responseFile[resp].IndexOf('*') == 0)
            {
                if (result.Contains("time"))
                {
                    Say(DateTime.Now.ToString(@"hh\:mm tt"));
                }
                else if(result.Contains("date"))
                {
                    Say(DateTime.Now.ToString(@"dd MMMM yyyy"));
                }
            }
            else
            {
                Say(_responseFile[resp]);
            }
        }
    }
}