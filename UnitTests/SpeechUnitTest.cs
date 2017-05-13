using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speech;
using System.Collections.ObjectModel;
using System.Speech.Synthesis;
using System.Speech.AudioFormat;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Management.Automation;

namespace UnitTests
{
    [TestClass]
    public class SpeechUnitTest
    {
        public const string ModuleName = "Erwine.Leonard.T.Speech";
        public const string RelativeModulePath = @"Deployment\Speech";

        class SpeechPropertyState
        {
            private static int _originalSpeechRate;
            private static int _originalSpeechVolume;
            private static VoiceInfo _originalVoice;
            public SpeechPropertyState(SpeechSynthesizer speechSynthesizer)
            {
                _originalSpeechVolume = speechSynthesizer.Volume;
                _originalSpeechRate = speechSynthesizer.Rate;
                _originalVoice = speechSynthesizer.Voice;
            }
            public void Restore()
            {
                using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
                    Restore(speechSynthesizer);
            }
            public void Restore(SpeechSynthesizer speechSynthesizer)
            {
                speechSynthesizer.Rate = _originalSpeechRate;
                speechSynthesizer.Volume = _originalSpeechVolume;
                if (speechSynthesizer.Voice.Name != _originalVoice.Name)
                    speechSynthesizer.SelectVoice(_originalVoice.Name);
            }
        }

        private static SpeechPropertyState _originalSpeechPropertyState;
        private static VoiceAge[] _allAges;
        private static VoiceGender[] _allGenders;
        private static int _initialSpeechRate;
        private static int _initialSpeechVolume;
        private static VoiceInfo _initialVoice;
        private static InstalledVoice[] _installedVoices;
        private static Dictionary<VoiceAge, Dictionary<VoiceGender, VoiceInfo>> _voicesByAge;
        private static Dictionary<VoiceGender, VoiceInfo> _voicesByGender;
        private TestContext _testContextInstance;
        private static string _wavOutputPath;
        private static string _xmlOutputPath;
        private static string _ssmlOutputPath1;
        private static string _ssmlOutputPath2;
        private static string _txtOutputPath;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get { return _testContextInstance; } set { _testContextInstance = value; } }

#region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            _allGenders = Enum.GetValues(typeof(VoiceGender)).OfType<VoiceGender>().ToArray();
            _allAges = Enum.GetValues(typeof(VoiceAge)).OfType<VoiceAge>().ToArray();
            using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
            {
                _originalSpeechPropertyState = new SpeechPropertyState(speechSynthesizer);
                _initialSpeechRate = speechSynthesizer.Rate;
                _initialSpeechVolume = speechSynthesizer.Volume;
                _initialVoice = speechSynthesizer.Voice;
                _installedVoices = speechSynthesizer.GetInstalledVoices().ToArray();
                _voicesByGender = _allGenders.Select(g =>
                {
                    speechSynthesizer.SelectVoiceByHints(g);
                    return new { Gender = g, Voice = speechSynthesizer.Voice };
                }).ToDictionary(k => k.Gender, v => v.Voice);
                _voicesByAge = _allAges.Select(a => new
                {
                    Age = a,
                    ByGender = _allGenders.Select(g =>
                    {
                        speechSynthesizer.SelectVoiceByHints(g);
                        return new { Gender = g, Voice = speechSynthesizer.Voice };
                    }).ToDictionary(k => k.Gender, v => v.Voice)
                }).ToDictionary(k => k.Age, v => v.ByGender);
                if (speechSynthesizer.Voice.Id != _initialVoice.Id)
                    speechSynthesizer.SelectVoice(_initialVoice.Name);
            }
        }

        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup() { _originalSpeechPropertyState.Restore(); }

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            _wavOutputPath = Path.Combine(TestContext.DeploymentDirectory, Guid.NewGuid().ToString("N") + ".wav");
            _xmlOutputPath = Path.Combine(TestContext.DeploymentDirectory, Guid.NewGuid().ToString("N") + ".xml");
            _ssmlOutputPath1 = Path.Combine(TestContext.DeploymentDirectory, Guid.NewGuid().ToString("N") + ".ssml");
            _ssmlOutputPath2 = Path.Combine(TestContext.DeploymentDirectory, Guid.NewGuid().ToString("N") + ".sml");
            _txtOutputPath = Path.Combine(TestContext.DeploymentDirectory, Guid.NewGuid().ToString("N") + ".txt");

            if (_initialVoice != null)
                return;

            using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
            {
                _originalSpeechPropertyState.Restore(speechSynthesizer);
                _initialSpeechRate = speechSynthesizer.Rate;
                _initialSpeechVolume = speechSynthesizer.Volume;
                _initialVoice = speechSynthesizer.Voice;
            }
        }


        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            _originalSpeechPropertyState.Restore();
            _initialVoice = null;
            foreach (string path in new string[] { _wavOutputPath, _xmlOutputPath, _ssmlOutputPath1, _ssmlOutputPath2, _txtOutputPath })
            {
                if (File.Exists(path))
                    File.Delete(path);
                else if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
        }

#endregion

        [TestMethod]
        [TestCategory("SpeechSynthesis")]
        [Description("Tests TextToSpeechJob constructors which do not produce speech.")]
        public void TextToSpeechJobConstructorTestMethod1()
        {
            var baseConstructorArgs = (new[]
            {
                new { prompts = null as Collection<Prompt>, PreMessage = "Args: prompts = null" },
                new { prompts = new Collection<Prompt>(), PreMessage = "Args: prompts = emtpy" }
            }).SelectMany(a => new[]
            {
                new { prompts = a.prompts, noAutoStart = true, PreMessage = a.PreMessage + ", noAutoStart = true" },
                new { prompts = a.prompts, noAutoStart = false, PreMessage = a.PreMessage + ", noAutoStart = false" }
            }).SelectMany(a => new[]
            {
                new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = null as int?, volume = 100 as int?, PreMessage = a.PreMessage + ", rate = null, volume = 100" },
                new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = -10 as int?, volume = 50 as int?, PreMessage = a.PreMessage + ", rate = -10, volume = 50" },
                new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = 0 as int?, volume = 0 as int?, PreMessage = a.PreMessage + ", rate = 0, volume = 0" },
                new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = 10 as int?, volume = null as int?, PreMessage = a.PreMessage + ", rate = 10, volume = null" }
            }).SelectMany(a => (new[] {
                    new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, PreMessage = a.PreMessage, outputPath = null as string, PostMessage = ", outputPath = null" },
                    new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, PreMessage = a.PreMessage, outputPath = "", PostMessage = ", outputPath = empty" }
                }).Concat((new string[] { _wavOutputPath, _xmlOutputPath, _ssmlOutputPath1, _ssmlOutputPath2, _txtOutputPath }).Select(p =>
                    new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, PreMessage = a.PreMessage, outputPath = p, PostMessage = ", outputPath = [DeploymentDirectory]\\" + Path.GetFileName(p) })
                )
            ).SelectMany(a => new[]
            {
                new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, PreMessage = a.PreMessage, outputPath = a.outputPath, asyncState = null as object, PostMessage = a.PostMessage + ", asyncState = null" },
                new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, PreMessage = a.PreMessage, outputPath = a.outputPath, asyncState = new object(), PostMessage = a.PostMessage + ", asyncState = new object()" }
            });

            var constructorArgs1 = baseConstructorArgs.SelectMany(a => (new[] {
                    new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, outputPath = a.outputPath, asyncState = a.asyncState, voice = null as VoiceInfo, Message = a.PreMessage + ", voice = null" + a.PostMessage }
                }).Concat(_installedVoices.Where(v => v.Enabled).Select(v =>
                    new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, outputPath = a.outputPath, asyncState = a.asyncState, voice = v.VoiceInfo, Message = a.PreMessage + ", voice = " + v.VoiceInfo.Name + a.PostMessage })
                )
            );

            foreach (var a in constructorArgs1)
            {
                System.Diagnostics.Debug.WriteLine("Testing Text-To-Speech: " + a.Message);
                using (TextToSpeechJob target = new TextToSpeechJob(a.prompts, a.noAutoStart, a.rate, a.volume, (a.voice == null) ? null : a.voice.Name, a.outputPath, null, a.asyncState))
                {
                    Assert.IsFalse(target.WaitSpeechProgress(100), a.Message);
                    Assert.IsTrue(target.WaitSpeechCompleted(0), a.Message);
                    Collection<object> output = target.GetOutput();
                    Assert.IsNotNull(output, a.Message);
                    Assert.AreEqual(0, output.Count, a.Message);
                    Assert.AreEqual(SynthesizerState.Ready, target.State, a.Message);
                    Assert.IsTrue(target.IsCompleted, a.Message);
                    Assert.IsTrue((target as IAsyncResult).CompletedSynchronously, a.Message);
                    if (a.asyncState == null)
                        Assert.IsNull(target.AsyncState, a.Message);
                    else
                    {
                        Assert.IsNotNull(target.AsyncState, a.Message);
                        Assert.AreSame(a.asyncState, target.AsyncState, a.Message);
                    }
                    if (String.IsNullOrEmpty(a.outputPath))
                        Assert.IsNull(target.OutputPath);
                    else
                    {
                        Assert.IsNotNull(target.OutputPath);
                        Assert.AreEqual(a.outputPath, target.OutputPath);
                        Assert.IsFalse(File.Exists(a.outputPath));
                    }
                    Assert.AreEqual((a.rate.HasValue) ? a.rate.Value : _initialSpeechRate, target.Rate);
                    Assert.AreEqual((a.volume.HasValue) ? a.volume.Value : _initialSpeechVolume, target.Volume);
                    Assert.IsNotNull(target.Voice);
                    Assert.AreEqual(((a.voice == null) ? _initialVoice : a.voice).Name, target.Voice.Name);
                }
            }

            var constructorArgs2 = baseConstructorArgs.SelectMany(a => _allGenders.Select(g =>
                new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, outputPath = a.outputPath, asyncState = a.asyncState, gender = g, Message = a.PreMessage + ", gender = " + g.ToString("F") + a.PostMessage })
            );

            foreach (var a in constructorArgs2)
            {
                System.Diagnostics.Debug.WriteLine("Testing Text-To-Speech: " + a.Message);
                using (TextToSpeechJob target = new TextToSpeechJob(a.prompts, a.noAutoStart, a.rate, a.volume, a.gender, a.outputPath, null, a.asyncState))
                {
                    Assert.IsFalse(target.WaitSpeechProgress(100), a.Message);
                    Assert.IsTrue(target.WaitSpeechCompleted(0), a.Message);
                    Collection<object> output = target.GetOutput();
                    Assert.IsNotNull(output, a.Message);
                    Assert.AreEqual(0, output.Count, a.Message);
                    Assert.AreEqual(SynthesizerState.Ready, target.State, a.Message);
                    Assert.IsTrue(target.IsCompleted, a.Message);
                    Assert.IsTrue((target as IAsyncResult).CompletedSynchronously, a.Message);
                    if (a.asyncState == null)
                        Assert.IsNull(target.AsyncState, a.Message);
                    else
                    {
                        Assert.IsNotNull(target.AsyncState, a.Message);
                        Assert.AreSame(a.asyncState, target.AsyncState, a.Message);
                    }
                    if (String.IsNullOrEmpty(a.outputPath))
                        Assert.IsNull(target.OutputPath);
                    else
                    {
                        Assert.IsNotNull(target.OutputPath);
                        Assert.AreEqual(a.outputPath, target.OutputPath);
                        Assert.IsFalse(File.Exists(a.outputPath));
                    }
                    Assert.AreEqual((a.rate.HasValue) ? a.rate.Value : _initialSpeechRate, target.Rate);
                    Assert.AreEqual((a.volume.HasValue) ? a.volume.Value : _initialSpeechVolume, target.Volume);
                    Assert.IsNotNull(target.Voice);
                    if (_voicesByGender[a.gender].Gender == a.gender)
                        Assert.AreEqual(_voicesByGender[a.gender].Name, target.Voice.Name);
                }
            }

            var constructorArgs3 = baseConstructorArgs.SelectMany(a => _allGenders.Select(g =>
                new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, outputPath = a.outputPath, asyncState = a.asyncState, gender = g, PreMessage = a.PreMessage + ", gender = " + g.ToString("F"), PostMessage = a.PostMessage })
            ).SelectMany(a => _allAges.Select(g =>
                new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, outputPath = a.outputPath, asyncState = a.asyncState, gender = a.gender, age = g, Message = a.PreMessage + ", age = " + g.ToString("F") + a.PostMessage })
            );

            foreach (var a in constructorArgs3)
            {
                System.Diagnostics.Debug.WriteLine("Testing Text-To-Speech: " + a.Message);
                using (TextToSpeechJob target = new TextToSpeechJob(a.prompts, a.noAutoStart, a.rate, a.volume, a.gender, a.age, a.outputPath, null, a.asyncState))
                {
                    Assert.IsFalse(target.WaitSpeechProgress(100), a.Message);
                    Assert.IsTrue(target.WaitSpeechCompleted(0), a.Message);
                    Collection<object> output = target.GetOutput();
                    Assert.IsNotNull(output, a.Message);
                    Assert.AreEqual(0, output.Count, a.Message);
                    Assert.AreEqual(SynthesizerState.Ready, target.State, a.Message);
                    Assert.IsTrue(target.IsCompleted, a.Message);
                    Assert.IsTrue((target as IAsyncResult).CompletedSynchronously, a.Message);
                    if (a.asyncState == null)
                        Assert.IsNull(target.AsyncState, a.Message);
                    else
                    {
                        Assert.IsNotNull(target.AsyncState, a.Message);
                        Assert.AreSame(a.asyncState, target.AsyncState, a.Message);
                    }
                    if (String.IsNullOrEmpty(a.outputPath))
                        Assert.IsNull(target.OutputPath);
                    else
                    {
                        Assert.IsNotNull(target.OutputPath);
                        Assert.AreEqual(a.outputPath, target.OutputPath);
                        Assert.IsFalse(File.Exists(a.outputPath));
                    }
                    Assert.AreEqual((a.rate.HasValue) ? a.rate.Value : _initialSpeechRate, target.Rate);
                    Assert.AreEqual((a.volume.HasValue) ? a.volume.Value : _initialSpeechVolume, target.Volume);
                    Assert.IsNotNull(target.Voice);
                    if (_voicesByAge[a.age][a.gender].Age == a.age && _voicesByAge[a.age][a.gender].Gender == a.gender)
                        Assert.AreEqual(_voicesByAge[a.age][a.gender].Name, target.Voice.Name);
                }
            }
        }

        [TestMethod]
        [TestCategory("SpeechSynthesis")]
        [Description("Tests TextToSpeechJob constructors which produce speech.")]
        public void TextToSpeechJobConstructorTestMethod2()
        {
            var baseConstructorArgs = (new[]
            {
                new { prompts = new string[] { "Testing Multiple Prompt", "" }, PreMessage = "Args: prompts = multiple" },
                new { prompts = new string[] { "Testing Single Prompt. " }, PreMessage = "Args: prompts = single" }
            }).SelectMany(a => new[]
            {
                new { prompts = a.prompts, noAutoStart = true, PreMessage = a.PreMessage + ", noAutoStart = true" },
                new { prompts = a.prompts, noAutoStart = false, PreMessage = a.PreMessage + ", noAutoStart = false" }
            }).SelectMany(a => new[]
            {
                new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = 10 as int?, volume = 100 as int?, PreMessage = a.PreMessage + ", rate = 10, volume = 100" },
                new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = null as int?, volume = 50 as int?, PreMessage = a.PreMessage + ", rate = null, volume = 50" },
                new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = 0 as int?, volume = 0 as int?, PreMessage = a.PreMessage + ", rate = 0, volume = 0" },
                new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = -10 as int?, volume = null as int?, PreMessage = a.PreMessage + ", rate = 1-0, volume = null" }
            }).SelectMany(a => (new[] {
                    new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, PreMessage = a.PreMessage, outputPath = null as string, PostMessage = ", outputPath = null" },
                    new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, PreMessage = a.PreMessage, outputPath = "", PostMessage = ", outputPath = empty" }
                }).Concat((new string[] { _wavOutputPath, _xmlOutputPath, _ssmlOutputPath1, _ssmlOutputPath2, _txtOutputPath }).Select(p =>
                    new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, PreMessage = a.PreMessage, outputPath = p, PostMessage = ", outputPath = [DeploymentDirectory]\\" + Path.GetFileName(p) })
                )
            ).SelectMany(a => new[]
            {
                new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, PreMessage = a.PreMessage, outputPath = a.outputPath, asyncState = null as object, PostMessage = a.PostMessage + ", asyncState = null" },
                new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, PreMessage = a.PreMessage, outputPath = a.outputPath, asyncState = new object(), PostMessage = a.PostMessage + ", asyncState = new object()" }
            });

            var constructorArgs1 = baseConstructorArgs.SelectMany(a => (new[] {
                    new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, outputPath = a.outputPath, asyncState = a.asyncState, voice = null as VoiceInfo, Message = a.PreMessage + ", voice = null" + a.PostMessage }
                }).Concat(_installedVoices.Where(v => v.Enabled).Select(v =>
                    new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, outputPath = a.outputPath, asyncState = a.asyncState, voice = v.VoiceInfo, Message = a.PreMessage + ", voice = " + v.VoiceInfo.Name + a.PostMessage })
                )
            );

            foreach (var a in constructorArgs1)
            {
                System.Diagnostics.Debug.WriteLine("Testing Text-To-Speech: " + a.Message);
                Collection<Prompt> prompts = new Collection<Prompt>();
                if (a.prompts.Length == 1)
                    prompts.Add(new Prompt(a.prompts[0] + a.Message));
                else
                {
                    foreach (string t in a.prompts.Take(a.prompts.Length - 1))
                        prompts.Add(new Prompt(t));
                    prompts.Add(new Prompt(a.prompts.Last() + a.Message));
                }
                using (TextToSpeechJob target = new TextToSpeechJob(prompts, a.noAutoStart, a.rate, a.volume, (a.voice == null) ? null : a.voice.Name, a.outputPath, null, a.asyncState))
                {
                    Assert.IsFalse(target.IsCompleted, a.Message);
                    Assert.IsFalse((target as IAsyncResult).CompletedSynchronously, a.Message);
                    if (a.asyncState == null)
                        Assert.IsNull(target.AsyncState, a.Message);
                    else
                    {
                        Assert.IsNotNull(target.AsyncState, a.Message);
                        Assert.AreSame(a.asyncState, target.AsyncState, a.Message);
                    }
                    if (String.IsNullOrEmpty(a.outputPath))
                        Assert.IsNull(target.OutputPath);
                    else
                    {
                        Assert.IsNotNull(target.OutputPath);
                        Assert.AreEqual(a.outputPath, target.OutputPath);
                    }
                    Assert.AreEqual((a.rate.HasValue) ? a.rate.Value : _initialSpeechRate, target.Rate);
                    Assert.AreEqual((a.volume.HasValue) ? a.volume.Value : _initialSpeechVolume, target.Volume);
                    Assert.IsNotNull(target.Voice);
                    Assert.AreEqual(((a.voice == null) ? _initialVoice : a.voice).Name, target.Voice.Name);
                    Collection<object> output;
                    if (a.noAutoStart)
                    {
                        Assert.IsFalse(target.WaitSpeechProgress(1000), a.Message);
                        Assert.IsFalse(target.WaitSpeechCompleted(10), a.Message);
                        output = target.GetOutput();
                        Assert.IsNotNull(output, a.Message);
                        Assert.AreEqual(0, output.Count, a.Message);
                        Assert.AreEqual(SynthesizerState.Paused, target.State, a.Message);
                        if (!String.IsNullOrEmpty(target.OutputPath))
                            Assert.IsFalse(File.Exists(target.OutputPath));
                        target.Resume();
                    }
                    else if (!String.IsNullOrEmpty(a.outputPath))
                    {
                        Assert.IsTrue(target.WaitSpeechProgress(1000));
                        System.Diagnostics.Debug.WriteLine("Pausing Text-To-Speech: " + a.Message);
                        target.Pause();
                        target.WaitSpeechProgress(1000);
                        Assert.IsFalse(target.WaitSpeechCompleted(10), a.Message);
                        output = target.GetOutput();
                        Assert.IsNotNull(output, a.Message);
                        Assert.AreNotEqual(0, output.Count, a.Message);
                        Assert.AreEqual(SynthesizerState.Paused, target.State, a.Message);
                        System.Diagnostics.Debug.WriteLine("Resuming Text-To-Speech: " + a.Message);
                        target.Resume();
                    }
                    if (!String.IsNullOrEmpty(a.outputPath))
                        Assert.AreEqual(SynthesizerState.Speaking, target.State, a.Message);
                    Assert.IsTrue(target.WaitSpeechProgress(1000));
                    output = target.GetOutput();
                    Assert.IsNotNull(output, a.Message);
                    Assert.AreNotEqual(0, output.Count, a.Message);
                    if (String.IsNullOrEmpty(a.outputPath))
                        Assert.IsFalse(target.WaitSpeechCompleted(1000));
                    Assert.IsTrue(target.WaitSpeechCompleted(60000));
                    Assert.IsTrue(target.IsCompleted, a.Message);
                    Assert.IsFalse((target as IAsyncResult).CompletedSynchronously, a.Message);
                    if (a.asyncState == null)
                        Assert.IsNull(target.AsyncState, a.Message);
                    else
                    {
                        Assert.IsNotNull(target.AsyncState, a.Message);
                        Assert.AreSame(a.asyncState, target.AsyncState, a.Message);
                    }
                    if (String.IsNullOrEmpty(a.outputPath))
                        Assert.IsNull(target.OutputPath);
                    else
                    {
                        Assert.IsNotNull(target.OutputPath);
                        Assert.AreEqual(a.outputPath, target.OutputPath);
                        FileInfo fileInfo = new FileInfo(a.outputPath);
                        Assert.IsTrue(fileInfo.Exists);
                        Assert.AreNotEqual(0L, fileInfo.Length);
                        fileInfo.Delete();
                    }
                    Assert.AreEqual((a.rate.HasValue) ? a.rate.Value : _initialSpeechRate, target.Rate);
                    Assert.AreEqual((a.volume.HasValue) ? a.volume.Value : _initialSpeechVolume, target.Volume);
                    Assert.IsNotNull(target.Voice);
                    Assert.AreEqual(((a.voice == null) ? _initialVoice : a.voice).Name, target.Voice.Name);
                    output = target.GetOutput();
                    Assert.IsNotNull(output, a.Message);
                    Assert.AreNotEqual(0, output.Count, a.Message);
                }
            }

            var constructorArgs2 = baseConstructorArgs.SelectMany(a => _allGenders.Select(g =>
                new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, outputPath = a.outputPath, asyncState = a.asyncState, gender = g, Message = a.PreMessage + ", gender = " + g.ToString("F") + a.PostMessage })
            );

            foreach (var a in constructorArgs2)
            {
                System.Diagnostics.Debug.WriteLine("Testing Text-To-Speech: " + a.Message);
                Collection<Prompt> prompts = new Collection<Prompt>();
                if (a.prompts.Length == 1)
                    prompts.Add(new Prompt(a.prompts[0] + a.Message));
                else
                {
                    foreach (string t in a.prompts.Take(a.prompts.Length - 1))
                        prompts.Add(new Prompt(t));
                    prompts.Add(new Prompt(a.prompts.Last() + a.Message));
                }
                using (TextToSpeechJob target = new TextToSpeechJob(prompts, a.noAutoStart, a.rate, a.volume, a.gender, a.outputPath, null, a.asyncState))
                {
                    Assert.IsFalse(target.IsCompleted, a.Message);
                    Assert.IsFalse((target as IAsyncResult).CompletedSynchronously, a.Message);
                    if (a.asyncState == null)
                        Assert.IsNull(target.AsyncState, a.Message);
                    else
                    {
                        Assert.IsNotNull(target.AsyncState, a.Message);
                        Assert.AreSame(a.asyncState, target.AsyncState, a.Message);
                    }
                    if (String.IsNullOrEmpty(a.outputPath))
                        Assert.IsNull(target.OutputPath);
                    else
                    {
                        Assert.IsNotNull(target.OutputPath);
                        Assert.AreEqual(a.outputPath, target.OutputPath);
                    }
                    Assert.AreEqual((a.rate.HasValue) ? a.rate.Value : _initialSpeechRate, target.Rate);
                    Assert.AreEqual((a.volume.HasValue) ? a.volume.Value : _initialSpeechVolume, target.Volume);
                    Assert.IsNotNull(target.Voice);
                    if (_voicesByGender[a.gender].Gender == a.gender)
                        Assert.AreEqual(_voicesByGender[a.gender].Name, target.Voice.Name);
                    Collection<object> output;
                    if (a.noAutoStart)
                    {
                        Assert.IsFalse(target.WaitSpeechProgress(1000), a.Message);
                        Assert.IsFalse(target.WaitSpeechCompleted(10), a.Message);
                        output = target.GetOutput();
                        Assert.IsNotNull(output, a.Message);
                        Assert.AreEqual(0, output.Count, a.Message);
                        Assert.AreEqual(SynthesizerState.Paused, target.State, a.Message);
                        if (!String.IsNullOrEmpty(target.OutputPath))
                            Assert.IsFalse(File.Exists(target.OutputPath));
                        target.Resume();
                    }
                    else if (!String.IsNullOrEmpty(a.outputPath))
                    {
                        Assert.IsTrue(target.WaitSpeechProgress(1000));
                        System.Diagnostics.Debug.WriteLine("Pausing Text-To-Speech: " + a.Message);
                        target.Pause();
                        target.WaitSpeechProgress(1000);
                        Assert.IsFalse(target.WaitSpeechCompleted(10), a.Message);
                        output = target.GetOutput();
                        Assert.IsNotNull(output, a.Message);
                        Assert.AreNotEqual(0, output.Count, a.Message);
                        Assert.AreEqual(SynthesizerState.Paused, target.State, a.Message);
                        System.Diagnostics.Debug.WriteLine("Resuming Text-To-Speech: " + a.Message);
                        target.Resume();
                    }
                    Assert.AreEqual(SynthesizerState.Speaking, target.State, a.Message);
                    Assert.IsTrue(target.WaitSpeechProgress(1000));
                    output = target.GetOutput();
                    Assert.IsNotNull(output, a.Message);
                    Assert.AreNotEqual(0, output.Count, a.Message);
                    if (String.IsNullOrEmpty(a.outputPath))
                        Assert.IsFalse(target.WaitSpeechCompleted(1000));
                    Assert.IsTrue(target.WaitSpeechCompleted(60000));
                    Assert.IsTrue(target.IsCompleted, a.Message);
                    Assert.IsFalse((target as IAsyncResult).CompletedSynchronously, a.Message);
                    if (a.asyncState == null)
                        Assert.IsNull(target.AsyncState, a.Message);
                    else
                    {
                        Assert.IsNotNull(target.AsyncState, a.Message);
                        Assert.AreSame(a.asyncState, target.AsyncState, a.Message);
                    }
                    if (String.IsNullOrEmpty(a.outputPath))
                        Assert.IsNull(target.OutputPath);
                    else
                    {
                        Assert.IsNotNull(target.OutputPath);
                        Assert.AreEqual(a.outputPath, target.OutputPath);
                        FileInfo fileInfo = new FileInfo(a.outputPath);
                        Assert.IsTrue(fileInfo.Exists);
                        Assert.AreNotEqual(0L, fileInfo.Length);
                        fileInfo.Delete();
                    }
                    Assert.AreEqual((a.rate.HasValue) ? a.rate.Value : _initialSpeechRate, target.Rate);
                    Assert.AreEqual((a.volume.HasValue) ? a.volume.Value : _initialSpeechVolume, target.Volume);
                    Assert.IsNotNull(target.Voice);
                    if (_voicesByGender[a.gender].Gender == a.gender)
                        Assert.AreEqual(_voicesByGender[a.gender].Name, target.Voice.Name);
                }
            }

            var constructorArgs3 = baseConstructorArgs.SelectMany(a => _allGenders.Select(g =>
                new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, outputPath = a.outputPath, asyncState = a.asyncState, gender = g, PreMessage = a.PreMessage + ", gender = " + g.ToString("F"), PostMessage = a.PostMessage })
            ).SelectMany(a => _allAges.Select(g =>
                new { prompts = a.prompts, noAutoStart = a.noAutoStart, rate = a.rate, volume = a.volume, outputPath = a.outputPath, asyncState = a.asyncState, gender = a.gender, age = g, Message = a.PreMessage + ", age = " + g.ToString("F") + a.PostMessage })
            );

            foreach (var a in constructorArgs3)
            {
                System.Diagnostics.Debug.WriteLine("Testing Text-To-Speech: " + a.Message);
                Collection<Prompt> prompts = new Collection<Prompt>();
                if (a.prompts.Length == 1)
                    prompts.Add(new Prompt(a.prompts[0] + a.Message));
                else
                {
                    foreach (string t in a.prompts.Take(a.prompts.Length - 1))
                        prompts.Add(new Prompt(t));
                    prompts.Add(new Prompt(a.prompts.Last() + a.Message));
                }
                using (TextToSpeechJob target = new TextToSpeechJob(prompts, a.noAutoStart, a.rate, a.volume, a.gender, a.age, a.outputPath, null, a.asyncState))
                {
                    Assert.IsFalse(target.IsCompleted, a.Message);
                    Assert.IsFalse((target as IAsyncResult).CompletedSynchronously, a.Message);
                    if (a.asyncState == null)
                        Assert.IsNull(target.AsyncState, a.Message);
                    else
                    {
                        Assert.IsNotNull(target.AsyncState, a.Message);
                        Assert.AreSame(a.asyncState, target.AsyncState, a.Message);
                    }
                    if (String.IsNullOrEmpty(a.outputPath))
                        Assert.IsNull(target.OutputPath);
                    else
                    {
                        Assert.IsNotNull(target.OutputPath);
                        Assert.AreEqual(a.outputPath, target.OutputPath);
                    }
                    Assert.AreEqual((a.rate.HasValue) ? a.rate.Value : _initialSpeechRate, target.Rate);
                    Assert.AreEqual((a.volume.HasValue) ? a.volume.Value : _initialSpeechVolume, target.Volume);
                    Assert.IsNotNull(target.Voice);
                    if (_voicesByAge[a.age][a.gender].Age == a.age && _voicesByAge[a.age][a.gender].Gender == a.gender)
                        Assert.AreEqual(_voicesByAge[a.age][a.gender].Name, target.Voice.Name);
                    Collection<object> output;
                    if (a.noAutoStart)
                    {
                        Assert.IsFalse(target.WaitSpeechProgress(1000), a.Message);
                        Assert.IsFalse(target.WaitSpeechCompleted(10), a.Message);
                        output = target.GetOutput();
                        Assert.IsNotNull(output, a.Message);
                        Assert.AreEqual(0, output.Count, a.Message);
                        Assert.AreEqual(SynthesizerState.Paused, target.State, a.Message);
                        if (!String.IsNullOrEmpty(target.OutputPath))
                            Assert.IsFalse(File.Exists(target.OutputPath));
                        target.Resume();
                    }
                    else if (!String.IsNullOrEmpty(a.outputPath))
                    {
                        Assert.IsTrue(target.WaitSpeechProgress(1000));
                        System.Diagnostics.Debug.WriteLine("Pausing Text-To-Speech: " + a.Message);
                        target.Pause();
                        target.WaitSpeechProgress(1000);
                        Assert.IsFalse(target.WaitSpeechCompleted(10), a.Message);
                        output = target.GetOutput();
                        Assert.IsNotNull(output, a.Message);
                        Assert.AreNotEqual(0, output.Count, a.Message);
                        Assert.AreEqual(SynthesizerState.Paused, target.State, a.Message);
                        System.Diagnostics.Debug.WriteLine("Resuming Text-To-Speech: " + a.Message);
                        target.Resume();
                    }
                    Assert.AreEqual(SynthesizerState.Speaking, target.State, a.Message);
                    Assert.IsTrue(target.WaitSpeechProgress(1000));
                    output = target.GetOutput();
                    Assert.IsNotNull(output, a.Message);
                    Assert.AreNotEqual(0, output.Count, a.Message);
                    if (String.IsNullOrEmpty(a.outputPath))
                        Assert.IsFalse(target.WaitSpeechCompleted(1000));
                    Assert.IsTrue(target.WaitSpeechCompleted(60000));
                    Assert.IsTrue(target.IsCompleted, a.Message);
                    Assert.IsFalse((target as IAsyncResult).CompletedSynchronously, a.Message);
                    if (a.asyncState == null)
                        Assert.IsNull(target.AsyncState, a.Message);
                    else
                    {
                        Assert.IsNotNull(target.AsyncState, a.Message);
                        Assert.AreSame(a.asyncState, target.AsyncState, a.Message);
                    }
                    if (String.IsNullOrEmpty(a.outputPath))
                        Assert.IsNull(target.OutputPath);
                    else
                    {
                        Assert.IsNotNull(target.OutputPath);
                        Assert.AreEqual(a.outputPath, target.OutputPath);
                        FileInfo fileInfo = new FileInfo(a.outputPath);
                        Assert.IsTrue(fileInfo.Exists);
                        Assert.AreNotEqual(0L, fileInfo.Length);
                        fileInfo.Delete();
                    }
                    Assert.AreEqual((a.rate.HasValue) ? a.rate.Value : _initialSpeechRate, target.Rate);
                    Assert.AreEqual((a.volume.HasValue) ? a.volume.Value : _initialSpeechVolume, target.Volume);
                    Assert.IsNotNull(target.Voice);
                    if (_voicesByAge[a.age][a.gender].Age == a.age && _voicesByAge[a.age][a.gender].Gender == a.gender)
                        Assert.AreEqual(_voicesByAge[a.age][a.gender].Name, target.Voice.Name);
                }
            }
        }

        [TestMethod]
        [TestCategory("SpeechSynthesis")]
        [Description("Tests loading the Speech PowerShell module.")]
        public void SpeechModuleLoadTestMethod()
        {
            string path = Path.GetFullPath(Path.Combine(TestContext.DeploymentDirectory, @"..\..\..\Speech\bin\Debug\Erwine.Leonard.T.Speech2.psd1"));
            Assert.IsTrue(File.Exists(path));
            PSModuleInfo module = PowerShellHelper.LoadPSModuleInfo(TestContext, path);
            ModuleConformance.ModuleValidator.AssertPSModule(module);
        }
    }
}
