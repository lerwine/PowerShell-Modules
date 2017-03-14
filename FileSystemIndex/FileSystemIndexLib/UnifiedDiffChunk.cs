using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FileSystemIndexLib
{
	public abstract class UnifiedDiffChunk<TDiffLine, TValue>
		where TDiffLine : UnifiedDiffLine<TValue>
	{
        private int _originalStartLine = 1;
        private int _newStartLine = 1;
        private List<TDiffLine> _innerLines = new List<TDiffLine>();
		private ReadOnlyCollection<TDiffLine> _lines = null;
        
        public int OriginalStartLine { get { return _originalStartLine; } }

        public int NewStartLine { get { return _newStartLine; } }

		public ReadOnlyCollection<TDiffLine> Lines
		{
			get
			{
				if (_lines == null)
					_lines = new ReadOnlyCollection<TDiffLine>(_innerLines);
				return _lines;
			}
		}
        
        public bool HasDifference
        {
            get
            {
                foreach (TDiffLine line in _lines)
                {
                    if (line.HasDifference)
                        return true;
                }

                return false;
            }
        }
		
		protected UnifiedDiffChunk(int originalStartLine, int newStartLine, IEnumerable<TDiffLine> lines)
		{
			_originalStartLine = (originalStartLine < 1) ? 1 : originalStartLine;
			_newStartLine = (newStartLine < 1) ? 1 : newStartLine;
			if (lines != null)
			{
				foreach (TDiffLine line in lines)
				{
					if (line != null && line.HasDifference)
						_innerLines.Add(line);
				}
			}
		}
		
		protected abstract TDiffLine CreateDiffLine(TValue originalValue, TValue newValue);
		protected abstract UnifiedDiffChunk<TDiffLine, TValue> CreateDiffChunk(int originalStartLine, int newStartLine, IEnumerable<TDiffLine> lines);
		protected abstract IEqualityComparer<TValue> Comparer { get; }
		protected abstract bool IsAbsentValue(TValue value);
		protected abstract TValue GetAbsentValue();
		protected abstract TValue CoerceNonAbsentValue(TValue value);
		
		private IEnumerable<TValue> AsNonAbsent(IEnumerable<TValue> source)
		{
			if (source == null)
				yield break;
			
			foreach (TValue value in source)
				yield return CoerceNonAbsentValue(value);
		}
		
		private IEnumerable<TDiffLine> CreateDiffContent(IList<TValue> originalItems, IList<TValue> newItems)
		{
			int oCount = (originalItems == null) ? 0 : originalItems.Count;
			int nCount = (newItems == null) ? 0 : newItems.Count;
			for (int i = 0; i < oCount && i < nCount; i++)
			{
				if (!(IsAbsentValue(originalItems[i]) && IsAbsentValue(newItems[i])))
					yield return CreateDiffLine(originalItems[i], newItems[i]);
			}
			if (oCount < nCount)
			{
				for (int i = oCount; i < nCount; i++)
				{
					if (!IsAbsentValue(newItems[i]))
						yield return CreateDiffLine(GetAbsentValue(), newItems[i]);
				}
			}
			else if (nCount < oCount)
			{
				for (int i = nCount; i < oCount; i++)
				{
					if (!IsAbsentValue(originalItems[i]))
						yield return CreateDiffLine(originalItems[i], GetAbsentValue());
				}
			}
		}
		
		private void Create(List<UnifiedDiffChunk<TDiffLine, TValue>> collection, CommonSequenceSearcher<TValue> originalSearcher, CommonSequenceSearcher<TValue> newSearcher)
		{
			if (originalSearcher.Count == 0)
			{
				if (newSearcher.Count > 0)
					collection.Add(CreateDiffChunk(originalSearcher.RelativeIndex, newSearcher.RelativeIndex + 1, CreateDiffContent(originalSearcher, newSearcher)));
				return;
			}
			else if (newSearcher.Count == 0)
			{
				collection.Add(CreateDiffChunk(originalSearcher.RelativeIndex + 1, newSearcher.RelativeIndex, CreateDiffContent(originalSearcher, newSearcher)));
				return;	
			}
	
			int originalIndex, newIndex;
			int length = originalSearcher.GetLongestCommonSequence(newSearcher, out originalIndex, out newIndex);
			if (length == 0)
			{
				collection.Add(CreateDiffChunk(originalSearcher.RelativeIndex + 1, newSearcher.RelativeIndex + 1, CreateDiffContent(originalSearcher, newSearcher)));
				return;
			}
			
			CommonSequenceSearcher<TValue> oSearcher = originalSearcher.Take(originalIndex);
			CommonSequenceSearcher<TValue> nSearcher = newSearcher.Take(newIndex);
			if (oSearcher.Count > 0 || nSearcher.Count > 0)
				Create(collection, oSearcher, nSearcher);
			oSearcher = originalSearcher.Skip(newIndex + length);
			nSearcher = newSearcher.Skip(newIndex + length);
			if (oSearcher.Count > 0 || nSearcher.Count > 0)
				Create(collection, oSearcher, nSearcher);
		}
		
		private IEnumerable<UnifiedDiffChunk<TDiffLine, TValue>> MergeConsecutive(List<UnifiedDiffChunk<TDiffLine, TValue>> collection)
		{
			if (collection.Count == 0)
				yield break;
			
			UnifiedDiffChunk<TDiffLine, TValue> preceding = collection[0];
			yield return preceding;
			if (collection.Count == 1)
				yield break;
			int originalStartLine = preceding._originalStartLine + preceding._innerLines.Count;
			int newStartLine = preceding._newStartLine + preceding._innerLines.Count;
			for (int i = 1; i < collection.Count; i++)
			{
				UnifiedDiffChunk<TDiffLine, TValue> current = collection[i];
				if (originalStartLine == current._originalStartLine && newStartLine == current._newStartLine)
				{
					originalStartLine += current._innerLines.Count;
					newStartLine += current._innerLines.Count;
					preceding._innerLines.AddRange(current._innerLines);
				}
				else
				{
					yield return current;
					preceding = current;	
					originalStartLine = preceding._originalStartLine + preceding._innerLines.Count;
					newStartLine = preceding._newStartLine + preceding._innerLines.Count;
				}
			}
		}
		
		public IEnumerable<UnifiedDiffChunk<TDiffLine, TValue>> Create(IEnumerable<TValue> originalItems, IEnumerable<TValue> newItems)
		{
			List<UnifiedDiffChunk<TDiffLine, TValue>> collection = new List<UnifiedDiffChunk<TDiffLine, TValue>>();
			Create(collection, new CommonSequenceSearcher<TValue>(AsNonAbsent(originalItems)), new CommonSequenceSearcher<TValue>(AsNonAbsent(newItems)));
			return MergeConsecutive(collection);
		}

        public void WriteTo(TextWriter writer)
        {
            int originalLineCount = 0;
            int newLineCount = 0;
            bool hasdifference = false;
            foreach (UnifiedDiffLine<TValue> line in _lines)
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
                writer.Write(_originalStartLine - 1);
            else
            {
                writer.Write(_originalStartLine);
                if (originalLineCount > 1)
                {
                    writer.Write(",");
                    writer.Write(originalLineCount);
                }
            }
            
            writer.Write(" ");
            if (newLineCount == 0)
                writer.Write(_newStartLine - 1);
            else
            {
                writer.Write(_newStartLine);
                if (newLineCount > 1)
                {
                    writer.Write(",");
                    writer.Write(newLineCount);
                }
            }
            writer.WriteLine(" @@");
            foreach (UnifiedDiffLine<TValue> line in _lines)
                line.WriteTo(writer);
        }
	}
}