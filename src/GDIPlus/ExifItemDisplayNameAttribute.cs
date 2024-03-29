﻿using System;

namespace Erwine.Leonard.T.GDIPlus
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public sealed class ExifItemDisplayNameAttribute : Attribute
    {
        private readonly int _index;
        private readonly string _displayText;

        public int Index { get { return _index; } }
        public string DisplayText { get { return _displayText; } }
        public ExifItemDisplayNameAttribute(int index, string displayText)
        {
            _index = index;
            _displayText = displayText;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}