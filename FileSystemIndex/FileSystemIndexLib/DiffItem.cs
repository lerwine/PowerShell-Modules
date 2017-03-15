using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FileSystemIndexLib
{
	public abstract class DiffItem
	{
        public abstract bool IsAdded { get; }
        
        public abstract bool IsRemoved { get; }
        
        public abstract bool IsModified { get; }

        public abstract bool HasDifference { get; }

        public abstract bool IsContext { get; }
		
		public abstract void WriteUnifiedDiffTo(TextWriter writer);

        protected static void WriteUnifiedDiffInsert(TextWriter writer, string text)
        {
            if (text != null)
            {
                writer.Write("+");
                writer.Write(text);
            }
        }

        protected static void WriteUnifiedDiffRemove(TextWriter writer, string text)
        {
            if (text != null)
            {
                writer.Write("-");
                writer.Write(text);
            }
        }
	}
	
	public abstract class DiffItem<T> : DiffItem
	{
        private T _originalValue;
        private T _newValue;
		
        public T OriginalValue
        {
            get { return _originalValue; }
            set { _originalValue = CoerceValue(value); }
        }

        public T NewValue
        {
            get { return _newValue; }
            set { _newValue = CoerceValue(value); }
        }
		
		protected DiffItem(T originalValue, T newValue)
		{
			_originalValue = CoerceValue(originalValue);
			_newValue = CoerceValue(newValue);
		}
		
        public override bool IsAdded { get { return IsAbsentValue(_originalValue) && !IsAbsentValue(_newValue); } }
        
        public override bool IsRemoved { get { return IsAbsentValue(_newValue) && !IsAbsentValue(_originalValue); } }
        
        public override bool IsModified { get { return !(IsAbsentValue(_newValue) || IsAbsentValue(_originalValue) || AreEqual(_newValue, _originalValue)); } }

        public override bool HasDifference { get { return (IsAbsentValue(_newValue)) ? IsAbsentValue(_originalValue) : (!IsAbsentValue(_originalValue) && AreEqual(_newValue, _originalValue)); } }

        public override bool IsContext { get { return !(IsAbsentValue(_newValue) || IsAbsentValue(_originalValue)) && AreEqual(_newValue, _originalValue); } }

		protected abstract bool AreEqual(T originalValue, T newValue);
		
		protected abstract bool IsAbsentValue(T value);
        
		protected abstract string AsStringValue(T value);
        
        protected virtual T CoerceValue(T value) { return value; }

        public override void WriteUnifiedDiffTo(TextWriter writer)
        {
            string originalText = (IsAbsentValue(_originalValue)) ? null : (AsStringValue(_originalValue) ?? "");
            string newText = (IsAbsentValue(_newValue)) ? null : (AsStringValue(_newValue) ?? "");

            if (originalText != null)
            {
                if (newText == null)
                {
                    WriteUnifiedDiffRemove(writer, newText);
                    return;
                }
                
                if (originalText == newText)
                {
                    writer.Write(" ");
                    writer.Write(newText);
                    return;
                }

                WriteUnifiedDiffRemove(writer, originalText);
            }

            WriteUnifiedDiffInsert(writer, newText);
        }
	}
}