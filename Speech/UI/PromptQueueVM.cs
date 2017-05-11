using System.Speech.Synthesis;
using System.Windows;

namespace Speech.UI
{
    public class PromptQueueVM : DependencyObject
    {
        #region Length Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="Length"/> dependency property.
        /// </summary>
        public const string PropertyName_Length = "Length";

        private static readonly DependencyPropertyKey LengthPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Length, typeof(int), typeof(PromptQueueVM),
            new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <see cref="Length"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LengthProperty = LengthPropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the Length Property.
        /// <summary>
        /// 
        /// </summary>
        public int Length
        {
            get { return (int)(GetValue(LengthProperty)); }
            private set { SetValue(LengthPropertyKey, value); }
        }
        
        #endregion

        #region Target Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="Target"/> dependency property.
        /// </summary>
        public const string PropertyName_Target = "Target";

        private static readonly DependencyPropertyKey TargetPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Target, typeof(Prompt), typeof(PromptQueueVM),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Target"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetProperty = TargetPropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the Target Property.
        /// <summary>
        /// 
        /// </summary>
        public Prompt Target
        {
            get { return (Prompt)(GetValue(TargetProperty)); }
            private set { SetValue(TargetPropertyKey, value); }
        }

        #endregion

        public PromptQueueVM(PromptBuilder promptBuilder)
        {
            if (promptBuilder == null)
                return;

            Target = new Prompt(promptBuilder);
            Length = (promptBuilder.IsEmpty) ? 0 : promptBuilder.ToXml().Length;
        }
    }
}