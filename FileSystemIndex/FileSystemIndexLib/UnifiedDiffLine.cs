using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FileSystemIndexLib
{
	public abstract class UnifiedDiffLine<T>
	{
        private T _originalValue;
        private T _newValue;
		
        public T OriginalValue { get { return _originalValue; } }

        public T NewValue { get { return _newValue; } }
		
		protected UnifiedDiffLine(T originalValue, T newValue)
		{
			_originalValue = originalValue;
			_newValue = newValue;
		}
		
        public bool IsAdded { get { return IsAbsentValue(_originalValue) && !IsAbsentValue(_newValue); } }
        
        public bool IsRemoved { get { return IsAbsentValue(_newValue) && !IsAbsentValue(_originalValue); } }
        
        public bool IsModified { get { return !(IsAbsentValue(_newValue) || IsAbsentValue(_originalValue) || AreEqual(_newValue, _originalValue)); } }

        public bool HasDifference { get { return (IsAbsentValue(_newValue)) ? IsAbsentValue(_originalValue) : (!IsAbsentValue(_originalValue) && AreEqual(_newValue, _originalValue)); } }

        public bool IsContext { get { return !(IsAbsentValue(_newValue) || IsAbsentValue(_originalValue)) && AreEqual(_newValue, _originalValue); } }

		protected abstract bool AreEqual(T originalValue, T newValue);
		
		protected abstract bool IsAbsentValue(T value);
        
		protected abstract string AsStringValue(T value);
        
        private static void WriteInsert(TextWriter writer, string text)
        {
            if (text != null)
            {
                writer.Write("+");
                writer.Write(text);
            }
        }

        private static void WriteRemove(TextWriter writer, string text)
        {
            if (text != null)
            {
                writer.Write("-");
                writer.Write(text);
            }
        }

        public void WriteTo(TextWriter writer)
        {
            string originalText = (IsAbsentValue(_originalValue)) ? null : (AsStringValue(_originalValue) ?? "");
            string newText = (IsAbsentValue(_newValue)) ? null : (AsStringValue(_newValue) ?? "");

            if (originalText != null)
            {
                if (newText == null)
                {
                    WriteRemove(writer, newText);
                    return;
                }
                
                if (originalText == newText)
                {
                    writer.Write(" ");
                    writer.Write(newText);
                    return;
                }

                WriteRemove(writer, originalText);
            }

            WriteInsert(writer, newText);
        }
	}
}