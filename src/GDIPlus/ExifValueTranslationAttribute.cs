﻿using System;

namespace Erwine.Leonard.T.GDIPlus
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public sealed class ExifValueTranslationAttribute : Attribute
    {
        private readonly byte[] _sourceValue;
        private readonly string _displayText;

        public byte[] SourceValue { get { return _sourceValue; } }
        public string DisplayText { get { return _displayText; } }
        public ExifValueTranslationAttribute(byte sourceValue, string displayText)
        {
            _sourceValue = new byte[] { sourceValue };
            _displayText = displayText;
        }
        public ExifValueTranslationAttribute(char sourceValue, string displayText)
        {
            _sourceValue = new byte[] { (byte)sourceValue };
            _displayText = displayText;
        }
        public ExifValueTranslationAttribute(ushort sourceValue, string displayText)
        {
            _sourceValue = BitConverter.GetBytes(sourceValue);
            _displayText = displayText;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}