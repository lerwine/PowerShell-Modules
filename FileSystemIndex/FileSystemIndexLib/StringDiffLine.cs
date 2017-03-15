using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FileSystemIndexLib
{
    public class StringDiffLine : DiffItem<string>
	{
		public StringDiffLine() : base(null, null) { }
		
		public StringDiffLine(string originalValue, string newValue) : base(originalValue, newValue) { }

		protected override bool AreEqual(string originalValue, string newValue)
        {
            return (originalValue == null) ? newValue == null : (newValue != null && StringComparer.InvariantCulture.Equals(originalValue, newValue));
        }
		
		protected override bool IsAbsentValue(string value) { return value == null; }
        
		protected override string AsStringValue(string value) { return (value == null) ? "" : value; }

        protected override string CoerceValue(string value)
        {
            if (value != null && value.Length > 0 && (value.Contains("\n") || value.Contains("\r")))
                throw new FormatException("Line data cannot contain line separator characters");

            return value;
        }
    }
}