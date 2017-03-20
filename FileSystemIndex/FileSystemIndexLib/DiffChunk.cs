using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FileSystemIndexLib
{
	public abstract class DiffChunk
	{
        private int _originalStartLine = 1;
        private int _newStartLine = 1;
		
        public int OriginalStartLine
        {
            get { return _originalStartLine; }
            set { _originalStartLine = (value < 0) ? 0 : value; }
        }

        public int NewStartLine
        {
            get { return _newStartLine; }
            set { _newStartLine = (value < 0) ? 0 : value; }
        }
		
		public abstract bool HasDifference { get; }
		
		protected DiffChunk() { }
		
		protected DiffChunk(int originalStartLine, int newStartLine)
		{
			_originalStartLine = (originalStartLine < 1) ? 1 : originalStartLine;
			_newStartLine = (newStartLine < 1) ? 1 : newStartLine;
		}
		
		public abstract void WriteUnifiedDiffTo(TextWriter writer);
	}
	
	public abstract class DiffChunk<TDiffItem, TValue> : DiffChunk
		where TDiffItem : DiffItem<TValue>
	{
        private List<TDiffItem> _lines = new List<TDiffItem>();
        
		public List<TDiffItem> Lines
        {
            get { return _lines; }
            set { _lines = (value == null) ? new List<TDiffItem>() : value; }
        }
        
        public override bool HasDifference
        {
            get
            {
                foreach (TDiffItem line in _lines)
                {
                    if (line.HasDifference)
                        return true;
                }

                return false;
            }
        }
		
		protected DiffChunk() : base() { }
		
		protected DiffChunk(int originalStartLine, int newStartLine, IEnumerable<TDiffItem> lines)
			: base(originalStartLine, newStartLine)
		{
			if (lines == null)
				return;

			foreach (TDiffItem line in lines)
			{
				if (line != null && line.HasDifference)
					_lines.Add(line);
			}
		}
		
		protected DiffChunk(IEnumerable<TValue> originalItems, int originalStartLine, IEnumerable<TValue> newItems, int newStartLine)
			: base(originalStartLine, newStartLine)
		{
			IList<TValue> originalList = (originalItems == null) ? new List<TValue>() : ((originalItems is IList<TValue>) ? originalItems as IList<TValue> : new List<TValue>(originalItems));
			IList<TValue> newList = (newItems == null) ? new List<TValue>() : ((newItems is IList<TValue>) ? newItems as IList<TValue> : new List<TValue>(newItems));
			for (int i = 0; i < originalList.Count && i < newList.Count; i++)
			{
				if (!(IsAbsentValue(originalList[i]) && IsAbsentValue(newList[i])))
					_lines.Add(CreateDiffItem(originalList[i], newList[i]));
			}
			if (originalList.Count < newList.Count)
			{
				for (int i = originalList.Count; i < newList.Count; i++)
				{
					if (!IsAbsentValue(newList[i]))
						_lines.Add(CreateDiffItem(GetAbsentValue(), newList[i]));
				}
			}
			else if (newList.Count < originalList.Count)
			{
				for (int i = newList.Count; i < originalList.Count; i++)
				{
					if (!IsAbsentValue(originalList[i]))
						_lines.Add(CreateDiffItem(originalList[i], GetAbsentValue()));
				}
			}
		}
		
		protected abstract TDiffItem CreateDiffItem(TValue originalValue, TValue newValue);
		protected abstract IEqualityComparer<TValue> Comparer { get; }
		protected abstract bool IsAbsentValue(TValue value);
		protected abstract TValue GetAbsentValue();
		
        public override void WriteUnifiedDiffTo(TextWriter writer)
        {
            int originalLineCount = 0;
            int newLineCount = 0;
            bool hasdifference = false;
            foreach (DiffItem<TValue> line in _lines)
            {
                if (line.HasDifference)
                    hasdifference = true;
                if (line.IsRemoved || line.IsContext)
                    originalLineCount++;
                if (line.IsAdded || line.IsContext)
                    newLineCount++;
            }
            
            if (!hasdifference)
                return;

            writer.Write("@@ ");
            if (originalLineCount == 0)
                writer.Write(OriginalStartLine - 1);
            else
            {
                writer.Write(OriginalStartLine);
                if (originalLineCount > 1)
                {
                    writer.Write(",");
                    writer.Write(originalLineCount);
                }
            }
            
            writer.Write(" ");
            if (newLineCount == 0)
                writer.Write(NewStartLine - 1);
            else
            {
                writer.Write(NewStartLine);
                if (newLineCount > 1)
                {
                    writer.Write(",");
                    writer.Write(newLineCount);
                }
            }
            writer.WriteLine(" @@");
            foreach (DiffItem<TValue> line in _lines)
                line.WriteUnifiedDiffTo(writer);
        }
	}
}