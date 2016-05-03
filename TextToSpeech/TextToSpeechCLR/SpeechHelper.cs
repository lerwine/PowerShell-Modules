using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TextToSpeechCLR
{
    public static class SpeechHelper
    {
        public static VoiceInfo[] GetInstalledVoices(SpeechSynthesizer obj) { return GetInstalledVoices(obj, false); }

        public static VoiceInfo[] GetInstalledVoices(PSObject obj, bool disabled) { return GetInstalledVoices(obj, disabled, null); }

        public static VoiceInfo[] GetInstalledVoices(PSObject obj, CultureInfo culture) { return GetInstalledVoices(obj, false, culture); }

        public static VoiceInfo[] GetInstalledVoices(PSObject obj, bool disabled, CultureInfo culture)
        {
            SpeechSynthesizer speechSynthesizer = (obj.BaseObject is SpeechHelper) ? (obj.BaseObject as SpeechHelper)._speechSynthesizer : obj.BaseObject as SpeechSynthesizer;
            return (((culture == null) ? speechSynthesizer.GetInstalledVoices() : speechSynthesizer.GetInstalledVoices(culture))
                    .Where(v => v.Enabled != disabled).Select(v => v.VoiceInfo)).ToArray();
        }

        public static SpeechSynthesizer SpeechSynthesizerToDefaultAudioDevice()
        {
            SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
            try
            {
                speechSynthesizer.SetOutputToDefaultAudioDevice();
            }
            catch
            {
                speechSynthesizer.Dispose();
                throw;
            }

            return speechSynthesizer;
        }

        public static SpeechSynthesizer SpeechSynthesizerToNull()
        {
            SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
            try
            {
                speechSynthesizer.SetOutputToNull();
            }
            catch
            {
                speechSynthesizer.Dispose();
                throw;
            }

            return speechSynthesizer;
        }

        public static SpeechSynthesizer SpeechSynthesizerToWaveFile(string path, SpeechAudioFormatInfo formatInfo)
        {
            SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
            try
            {
                if (formatInfo == null)
                    speechSynthesizer.SetOutputToWaveFile(path);
                else
                    speechSynthesizer.SetOutputToWaveFile(path, formatInfo);
            }
            catch
            {
                speechSynthesizer.Dispose();
                throw;
            }

            return speechSynthesizer;
        }

        public static SpeechSynthesizer SpeechSynthesizerToAudioStream(Stream audioDestination, SpeechAudioFormatInfo formatInfo)
        {
            SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
            try
            {
                speechSynthesizer.SetOutputToAudioStream(audioDestination, formatInfo);
            }
            catch
            {
                speechSynthesizer.Dispose();
                throw;
            }

            return speechSynthesizer;
        }
    }
}
