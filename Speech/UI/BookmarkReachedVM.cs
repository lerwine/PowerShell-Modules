using System;
using System.Speech.Synthesis;
using System.Windows;

namespace Speech.UI
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class BookmarkReachedVM : DependencyObject
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        #region AudioPosition Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="AudioPosition"/> dependency property.
        /// </summary>
        public const string PropertyName_AudioPosition = "AudioPosition";

        private static readonly DependencyPropertyKey AudioPositionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_AudioPosition, typeof(TimeSpan), typeof(BookmarkReachedVM),
            new PropertyMetadata(default(TimeSpan)));

        /// <summary>
        /// Identifies the <see cref="AudioPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AudioPositionProperty = AudioPositionPropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the AudioPosition Property.
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan AudioPosition
        {
            get { return (TimeSpan)(GetValue(AudioPositionProperty)); }
            private set { SetValue(AudioPositionPropertyKey, value); }
        }
        #endregion

        #region Bookmark Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="Bookmark"/> dependency property.
        /// </summary>
        public const string PropertyName_Bookmark = "Bookmark";

        private static readonly DependencyPropertyKey BookmarkPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Bookmark, typeof(string), typeof(BookmarkReachedVM),
            new PropertyMetadata("", null, SpeechProgressVM.CoerceNonEmptyString));

        /// <summary>
        /// Identifies the <see cref="Bookmark"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BookmarkProperty = BookmarkPropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the Bookmark Property.
        /// <summary>
        /// 
        /// </summary>
        public string Bookmark
        {
            get { return GetValue(BookmarkProperty) as string; }
            private set { SetValue(BookmarkPropertyKey, value); }
        }

        #endregion

        #region RelativePosition Property Members

        // /// <summary>
        // /// Occurs when the value of <see cref="RelativePosition"/> has changed.
        // /// </summary>
        // public event EventHandler RelativePositionPropertyChanged;

        /// <summary>
        /// Defines the name for the <see cref="RelativePosition"/> dependency property.
        /// </summary>
        public const string PropertyName_RelativePosition = "RelativePosition";

        private static readonly DependencyPropertyKey RelativePositionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_RelativePosition, typeof(int), typeof(BookmarkReachedVM),
            new PropertyMetadata(0, null, SpeechProgressVM.CoercePercentageValue));

        /// <summary>
        /// Identifies the <see cref="RelativePosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RelativePositionProperty = RelativePositionPropertyKey.DependencyProperty;

        // TODO: Add summary documentation to the RelativePosition Property.
        /// <summary>
        /// 
        /// </summary>
        public int RelativePosition
        {
            get { return (int)(GetValue(RelativePositionProperty)); }
            private set { SetValue(RelativePositionPropertyKey, value); }
        }

        #endregion

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public BookmarkReachedVM() { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public BookmarkReachedVM(BookmarkReachedEventArgs e, int relativePosition)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            AudioPosition = e.AudioPosition;
            Bookmark = e.Bookmark;
            RelativePosition = relativePosition;
        }
    }
}