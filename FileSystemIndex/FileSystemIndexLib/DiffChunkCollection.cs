using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FileSystemIndexLib
{
	public abstract class DiffChunkCollection
	{
        private string _originalName = "";
        private string _newName = "";
        private DateTime _originalModified;
        private DateTime _newModified;

		public virtual string OriginalName
        {
            get { return _originalName; }
            set
            {
                if (value == null)
                    _originalName = "";
                else
                {
                    if (value != null && (value.Contains("\r") || value.Contains("\n") || value.Contains("\t")))
                        throw new ArgumentException("Filenames contain newline or tab characters.");
                    _originalName = value;
                }
            }
        }

		public DateTime OriginalModified
        {
            get { return _originalModified; }
            set { _originalModified = value; }
        }

        public virtual string NewName
        {
            get { return _newName; }
            set
            {
                if (value == null)
                    _newName = "";
                else
                {
                    if (value != null && (value.Contains("\r") || value.Contains("\n") || value.Contains("\t")))
                        throw new ArgumentException("Filenames contain newline or tab characters.");
                    _newName = value;
                }
            }
        }
        
		public DateTime NewModified
        {
            get { return _newModified; }
            set { _newModified = value; }
        }
        
        public abstract bool IsAdded { get; }
        
        public abstract bool IsRemoved { get; }
        
        public abstract bool IsModified { get; }
		
		protected DiffChunkCollection() { }
		
		protected DiffChunkCollection(string originalName, DateTime originalModified, string newName, DateTime newModified)
		{
			OriginalName = originalName;
			OriginalModified = originalModified;
			NewName = newName;
			NewModified = newModified;
		}
		
		public abstract void WriteUnifiedDiffTo(TextWriter writer);
	}
	
	public abstract class DiffChunkCollection<TDiffChunk, TDiffItem, TValue> : DiffChunkCollection
		where TDiffItem : DiffItem<TValue>
		where TDiffChunk : DiffChunk<TDiffItem, TValue>
	{
        private Collection<TDiffChunk> _chunks = new Collection<TDiffChunk>();

        public Collection<TDiffChunk> Chunks
        {
            get { return _chunks; }
            set { _chunks = (value == null) ? new Collection<TDiffChunk>() : value; }
        }
        
        public override bool IsAdded { get { return OriginalName.Length == 0 && NewName.Length > 0; } }
        
        public override bool IsRemoved { get { return NewName.Length == 0 && OriginalName.Length > 0; } }
        
        public override bool IsModified
        {
            get
            {
                if (OriginalName.Length == 0 || NewName.Length == 0)
                    return false;

                foreach (TDiffChunk chunk in _chunks)
                {
                    if (chunk != null && chunk.HasDifference)
                        return true;
                }

                return false;
            }
        }
		
		protected DiffChunkCollection() { }
		
		protected DiffChunkCollection(string originalName, DateTime originalModified, string newName, DateTime newModified, IEnumerable<TDiffChunk> chunks)
			: base(originalName, originalModified, newName, newModified)
		{
			if (chunks == null)
				return;
			
			foreach (TDiffChunk c in chunks)
			{
				if (c != null)
					_chunks.Add(c);
			}
		}
		
		protected DiffChunkCollection(string originalName, DateTime originalModified, string newName, DateTime newModified, IEnumerable<TValue> originalItems, IEnumerable<TValue> newItems)
			: base(originalName, originalModified, newName, newModified)
		{
			if (originalItems != null || newItems != null)
				Initialize(new CommonSequenceSearcher<TValue>(AsNonAbsent(originalItems)), new CommonSequenceSearcher<TValue>(AsNonAbsent(newItems)));
		}

		private void Initialize(CommonSequenceSearcher<TValue> originalSearcher, CommonSequenceSearcher<TValue> newSearcher)
		{
			if (originalSearcher.Count == 0)
			{
				if (newSearcher.Count > 0)
					_chunks.Add(CreateNewChunk(originalSearcher.RelativeIndex, newSearcher.RelativeIndex + 1, originalSearcher, newSearcher));
				return;
			}
			else if (newSearcher.Count == 0)
			{
				_chunks.Add(CreateNewChunk(originalSearcher.RelativeIndex + 1, newSearcher.RelativeIndex, originalSearcher, newSearcher));
				return;	
			}
	
			int originalIndex, newIndex;
			int length = originalSearcher.GetLongestCommonSequence(newSearcher, out originalIndex, out newIndex);
			if (length == 0)
			{
				_chunks.Add(CreateNewChunk(originalSearcher.RelativeIndex + 1, newSearcher.RelativeIndex + 1, originalSearcher, newSearcher));
				return;
			}
			
			CommonSequenceSearcher<TValue> oSearcher = originalSearcher.Take(originalIndex);
			CommonSequenceSearcher<TValue> nSearcher = newSearcher.Take(newIndex);
			if (oSearcher.Count > 0 || nSearcher.Count > 0)
				Initialize( oSearcher, nSearcher);
			oSearcher = originalSearcher.Skip(newIndex + length);
			nSearcher = newSearcher.Skip(newIndex + length);
			if (oSearcher.Count > 0 || nSearcher.Count > 0)
				Initialize(oSearcher, nSearcher);
		}
		
		public abstract TDiffChunk CreateNewChunk();
		
		protected abstract TDiffChunk CreateNewChunk(int originalStartLine, int newStartLine, IEnumerable<TValue> originalItems, IEnumerable<TValue> newItems);
		
		protected abstract TValue CoerceNonAbsentValue(TValue value);
		
		private IEnumerable<TValue> AsNonAbsent(IEnumerable<TValue> source)
		{
			if (source == null)
				yield break;
			
			foreach (TValue value in source)
				yield return CoerceNonAbsentValue(value);
		}
		
		private static IEnumerable<TDiffChunk> MergeConsecutive(List<TDiffChunk> collection)
		{
			if (collection.Count == 0)
				yield break;
			
			TDiffChunk preceding = collection[0];
			yield return preceding;
			if (collection.Count == 1)
				yield break;
			
			int originalStartLine = preceding.OriginalStartLine + preceding.Lines.Count;
			int newStartLine = preceding.NewStartLine + preceding.Lines.Count;
			for (int i = 1; i < collection.Count; i++)
			{
				TDiffChunk current = collection[i];
				if (originalStartLine == current.OriginalStartLine && newStartLine == current.NewStartLine)
				{
					originalStartLine += current.Lines.Count;
					newStartLine += current.Lines.Count;
					preceding.Lines.AddRange(current.Lines);
				}
				else
				{
					yield return current;
					preceding = current;	
					originalStartLine = preceding.OriginalStartLine + preceding.Lines.Count;
					newStartLine = preceding.NewStartLine + preceding.Lines.Count;
				}
			}
		}

        public override void WriteUnifiedDiffTo(TextWriter writer)
        {
            if (!IsModified)
                return;

            writer.WriteLine("--- {0}\t{1:yyyy-MM-dd HH:mm:ss.iiiiii zzzzzz}", OriginalName, OriginalModified);
            writer.WriteLine("+++ {0}\t{1:yyyy-MM-dd HH:mm:ss.iiiiii zzzzzz}", NewName, NewModified);

            foreach (TDiffChunk chunk in _chunks)
            {
                if (chunk != null && chunk.HasDifference)
                    chunk.WriteUnifiedDiffTo(writer);
            }
            writer.WriteLine("");
        }
    }
}