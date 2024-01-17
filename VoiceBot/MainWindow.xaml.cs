using System;
using System.IO;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace VoiceBot
{
    public partial class MainWindow
    {
        private readonly string[] _grammarFile = File.ReadAllLines("grammar.txt");
        private readonly string[] _responseFile = File.ReadAllLines("response.txt");

        private readonly SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();

        private readonly Choices _grammarList = new Choices();
        private readonly SpeechRecognitionEngine _speechRecognitionEngine = new SpeechRecognitionEngine();

        public MainWindow()
        {
            _grammarList.Add(_grammarFile);
            var grammar = new Grammar(new GrammarBuilder(_grammarList));

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

        private void Say(string text)
        {
            _speechSynthesizer.SpeakAsync(text);
        }

        private void SpeechRecognitionEngineOnSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var result = e.Result.Text;

            var resp = Array.IndexOf(_grammarFile, result);

            if (_responseFile[resp].IndexOf('*') == 0)
            {
                switch (_responseFile[resp])
                {
                    case "*time":
                        Say(DateTime.Now.ToString(@"hh\:mm tt"));
                        break;
                    case "*date":
                        Say(DateTime.Now.ToString(@"dd MMMM yyyy"));
                        break;
                    case "*dark mode":
                        Background = System.Windows.Media.Brushes.Black;
                        centerLabel.Foreground = System.Windows.Media.Brushes.White;
                        break;
                    case "*light mode":
                        Background = System.Windows.Media.Brushes.White;
                        centerLabel.Foreground = System.Windows.Media.Brushes.Black;
                        break;
                }
            }
            else
            {
                Say(_responseFile[resp]);
            }
        }
    }
}