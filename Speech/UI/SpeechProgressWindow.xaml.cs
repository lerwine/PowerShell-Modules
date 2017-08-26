using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Speech.UI
{
    /// <summary>
    /// Interaction logic for SpeechProgressWindow.xaml
    /// </summary>
    public partial class SpeechProgressWindow : Window
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public SpeechProgressWindow()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            InitializeComponent();

            SpeechProgressVM vm = DataContext as SpeechProgressVM;
            if (vm == null)
            {
                vm = new SpeechProgressVM();
                DataContext = vm;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public void SpeakAsync(IEnumerable<PromptBuilder> prompts)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(() => SpeakAsync(prompts));
                return;
            }

            SpeechProgressVM vm = DataContext as SpeechProgressVM;
            if (vm == null)
            {
                vm = new SpeechProgressVM();
                DataContext = vm;
            }
            vm.StartSpeech(prompts);
        }
    }
}
