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
        public SpeechProgressWindow()
        {
            InitializeComponent();

            SpeechProgressVM vm = DataContext as SpeechProgressVM;
            if (vm == null)
            {
                vm = new SpeechProgressVM();
                DataContext = vm;
            }
        }

        public void SpeakAsync(IEnumerable<PromptBuilder> prompts)
        {
            throw new NotImplementedException();
            //if (!CheckAccess())
            //{
            //    Dispatcher.Invoke
            //}
            //if (prompts == null)
            //{
            //    SetSpeechCompleted();
            //    return;
            //}

            //int index = 0;
            //foreach (PromptBuilder promptBuilder in prompts)
            //{
            //    PromptQueueVM item = new PromptQueueVM(promptBuilder, index);
            //    index++;
            //}
            //vm.StartSpeech(prompts);
        }
    }
}
