using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.IO;
using System.Collections.ObjectModel;
using IOUtilityCLR;
using System.Threading;
using System.Speech.Synthesis;
using System.Speech.AudioFormat;
using System.Diagnostics;

namespace UnitTests
{
    /// <summary>
    /// Summary description for IOUtilityUnitTest
    /// </summary>
    [TestClass]
    public class IOUtilityUnitTest
    {
        public const string ModuleName = "Erwine.Leonard.T.IOUtility";
        public const string RelativeModulePath = @"IOUtility\IOUtility";

        public IOUtilityUnitTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ImportIOUtilityTestMethod()
        {
            PowerShellHelper.TestLoadModule(this.TestContext, ModuleName, RelativeModulePath, ".psm1");
        }

        public class SpeechSynthesizerManager : IDisposable
        {
            TestContext _testContext;
            public Collection<EventArgs> PromptEvents { get; private set; }

            private object _syncRoot = new object();

            public SpeechSynthesizer SpeechSynthesizer { get; private set; }

            public SpeechSynthesizerManager(TestContext testContext)
            {
                PromptEvents = new Collection<EventArgs>();
                _testContext = testContext;
                SpeechSynthesizer = new SpeechSynthesizer();
                SpeechSynthesizer.BookmarkReached += SpeechSynthesizer_BookmarkReached;
                SpeechSynthesizer.PhonemeReached += SpeechSynthesizer_PhonemeReached;
                SpeechSynthesizer.SpeakCompleted += SpeechSynthesizer_SpeakCompleted;
                SpeechSynthesizer.SpeakProgress += SpeechSynthesizer_SpeakProgress;
                SpeechSynthesizer.SpeakStarted += SpeechSynthesizer_SpeakStarted;
                SpeechSynthesizer.StateChanged += SpeechSynthesizer_StateChanged;
                SpeechSynthesizer.VisemeReached += SpeechSynthesizer_VisemeReached;
                SpeechSynthesizer.VoiceChange += SpeechSynthesizer_VoiceChange;
            }

            private void SpeechSynthesizer_VoiceChange(object sender, VoiceChangeEventArgs e)
            {
                PromptEvents.Add(e);
                _testContext.WriteLine("VoiceChange: Id={0}, Name={1}, Gender={2}, Age={3}, Cancelled={4}, Error={5}", e.Voice.Id, e.Voice.Name, 
                    e.Voice.Gender, e.Voice.Age, e.Cancelled, (e.Error == null) ? "(none)" : e.ToString());
            }

            private void SpeechSynthesizer_VisemeReached(object sender, VisemeReachedEventArgs e)
            {
                PromptEvents.Add(e);
                _testContext.WriteLine("VisemeReached: Viseme={0}, AudioPosition={1}, Duration={2}, NextViseme={3}, Cancelled={4}, Error={5}", e.Viseme,
                    e.AudioPosition, e.Duration, e.NextViseme, e.Cancelled, (e.Error == null) ? "(none)" : e.ToString());
            }

            private void SpeechSynthesizer_StateChanged(object sender, StateChangedEventArgs e)
            {
                PromptEvents.Add(e);
                _testContext.WriteLine("StateChanged: State={0}, PreviousState={1}", e.State, e.PreviousState);
            }

            private void SpeechSynthesizer_SpeakStarted(object sender, SpeakStartedEventArgs e)
            {
                PromptEvents.Add(e);
                _testContext.WriteLine("SpeakStarted");
            }

            private void SpeechSynthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
            {
                PromptEvents.Add(e);
                _testContext.WriteLine("SpeakProgress: AudioPosition={0}, CharacterPosition={1}, CharacterCount={2}, Text=\"{3}\", Cancelled={4}, Error={5}", e.AudioPosition,
                    e.CharacterPosition, e.CharacterCount, e.Text, e.Cancelled, (e.Error == null) ? "(none)" : e.ToString());
            }

            private void SpeechSynthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
            {
                PromptEvents.Add(e);
                _testContext.WriteLine("SpeakCompleted: Cancelled={0}, Error={1}", e.Cancelled, (e.Error == null) ? "(none)" : e.ToString());
            }

            private void SpeechSynthesizer_PhonemeReached(object sender, PhonemeReachedEventArgs e)
            {
                PromptEvents.Add(e);
                _testContext.WriteLine("PhonemeReached: Phoneme={0}, Emphasis={1}, AudioPosition={2}, Duration={3}, NextPhoneme={4}, Cancelled={5}, Error={6}", e.Phoneme,
                    e.Emphasis, e.AudioPosition, e.Duration, e.NextPhoneme, e.Cancelled, (e.Error == null) ? "(none)" : e.ToString());
            }

            private void SpeechSynthesizer_BookmarkReached(object sender, BookmarkReachedEventArgs e)
            {
                PromptEvents.Add(e);
                _testContext.WriteLine("BookmarkReached: Bookmark={0}, AudioPosition={1}, Cancelled={2}, Error={3}", e.Bookmark,
                    e.AudioPosition, e.Cancelled, (e.Error == null) ? "(none)" : e.ToString());
            }

            #region IDisposable Support

            protected virtual void Dispose(bool disposing)
            {
                if (!disposing)
                    return;

                SpeechSynthesizer speechSynthesizer;
                lock (_syncRoot)
                {
                    speechSynthesizer = SpeechSynthesizer;
                    SpeechSynthesizer = null;
                }

                if (speechSynthesizer == null)
                    return;

                speechSynthesizer.BookmarkReached -= SpeechSynthesizer_BookmarkReached;
                speechSynthesizer.PhonemeReached -= SpeechSynthesizer_PhonemeReached;
                speechSynthesizer.SpeakCompleted -= SpeechSynthesizer_SpeakCompleted;
                speechSynthesizer.SpeakProgress -= SpeechSynthesizer_SpeakProgress;
                speechSynthesizer.SpeakStarted -= SpeechSynthesizer_SpeakStarted;
                speechSynthesizer.StateChanged -= SpeechSynthesizer_StateChanged;
                speechSynthesizer.VisemeReached -= SpeechSynthesizer_VisemeReached;
                speechSynthesizer.VoiceChange -= SpeechSynthesizer_VoiceChange;
                speechSynthesizer.Dispose();
            }

            public void Dispose() { Dispose(true); }

            #endregion
        }

        [TestMethod]
        public void GetXamlTestMethod()
        {
            string wavPath = Path.Combine(TestContext.ResultsDirectory, "GetWavTest.wav");
            TestContext.WriteLine("WAV output: {0}", wavPath);
            string ssmlPath = Path.Combine(TestContext.ResultsDirectory, "GetXamlTest.ssml");
            TestContext.WriteLine("SSML output: {0}", ssmlPath);
            using (SpeechSynthesizerManager mgr = new SpeechSynthesizerManager(TestContext))
            {
                PromptBuilder builder = new PromptBuilder();
                builder.StartStyle(new PromptStyle(PromptRate.Fast));
                builder.AppendText("Text at beginning. And has multiple sentences");
                builder.StartParagraph();
                builder.StartSentence();
                builder.AppendBookmark("My bookmark");
                builder.AppendText("First sentence enclosed in paragraph sentence tags. Second sentence enclosed in sentence tags.");
                builder.EndSentence();
                builder.AppendText("First sentence enclosed only in paragraph tags. First sentence enclosed only in paragraph tags");
                builder.EndParagraph();
                builder.StartVoice(VoiceGender.Female, VoiceAge.Adult);
                builder.AppendText("The sign said, \"slow children\"", PromptRate.ExtraSlow);
                builder.EndVoice();
                builder.StartVoice(VoiceGender.Female, VoiceAge.Senior);
                builder.AppendText("My senior words", PromptEmphasis.Strong);
                builder.AppendText("My soft words", PromptVolume.ExtraSoft);
                builder.EndVoice();
                builder.StartVoice(VoiceGender.Male, VoiceAge.Teen);
                builder.AppendTextWithAlias("My overconfident words", "The words of mine");
                builder.StartVoice(VoiceGender.Male, VoiceAge.Adult);
                builder.AppendTextWithHint("HOV", SayAs.SpellOut);
                builder.AppendText("lanes", PromptEmphasis.Reduced);
                builder.EndVoice();
                builder.EndVoice();
                builder.EndStyle();
                File.WriteAllText(ssmlPath, builder.ToXml());
                mgr.SpeechSynthesizer.SetOutputToWaveFile(wavPath);
                try { mgr.SpeechSynthesizer.Speak(builder); }
                catch { throw; }
                finally { mgr.SpeechSynthesizer.SetOutputToNull(); }
            }
        }

        [TestMethod]
        public void SpeakSSMLTestMethod()
        {
            string wavPath = Path.Combine(TestContext.ResultsDirectory, "SpeakSSMLTest.wav");
            TestContext.WriteLine("WAV output: {0}", wavPath);
            Debug.WriteLine(String.Format("WAV output: {0}", wavPath));
            using (SpeechSynthesizerManager mgr = new SpeechSynthesizerManager(TestContext))
            {
                mgr.SpeechSynthesizer.SetOutputToWaveFile(wavPath);
                PromptBuilder builder = new PromptBuilder();
                builder.AppendSsmlMarkup(XmlHelp.ExampleSSML.Trim());
                try { mgr.SpeechSynthesizer.Speak(builder); }
                catch { throw; }
                finally { mgr.SpeechSynthesizer.SetOutputToNull(); }
            }
        }
    }
}
