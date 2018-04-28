using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FileSystemIndexLib
{
    public abstract class DiffItem<T>
    {
		protected DiffItem(T originalValue, T newValue) { }
		public bool HasDifference { get { throw new NotImplementedException(); } }
		public bool IsRemoved { get { throw new NotImplementedException(); } }
		public bool IsContext { get { throw new NotImplementedException(); } }
		public bool IsAdded { get { throw new NotImplementedException(); } }
        public bool WriteUnifiedDiffTo(TextWriter writer)
        {
            throw new NotImplementedException();
        }

        protected abstract bool AreEqual(T originalValue, T newValue);
        protected abstract bool IsAbsentValue(T value);
        protected abstract string AsStringValue(T value);
    }
}