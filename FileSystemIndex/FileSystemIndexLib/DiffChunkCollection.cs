using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

		/// <summary>
		/// Name associated with original objects in the comparison.
		/// <summary>
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

		/// <summary>
		/// Date and time when the source of the original objects in the comparison had been last modified.
		/// <summary>
		public DateTime OriginalModified
        {
            get { return _originalModified; }
            set { _originalModified = value; }
        }

		/// <summary>
		/// Name associated with new objects in the comparison.
		/// <summary>
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
        
		/// <summary>
		/// Date and time when the source of the new objects in the comparison had been last modified.
		/// <summary>
		public DateTime NewModified
        {
            get { return _newModified; }
            set { _newModified = value; }
        }
        
		/// <summary>
		/// Indicates whether any items in the current <see cref="DiffChunkCollection" /> represent a modified value.
		/// <summary>
        public abstract bool IsModified { get; }
		
		/// <summary>
		/// Initialize new instance of <see	cref="DiffChunkCollection" />.
		/// <summary>
		protected DiffChunkCollection() { }
		
		/// <summary>
		/// Initialize new instance of <see	cref="DiffChunkCollection{TDiffChunk,TDiffItem,TValue}" />.
		/// <summary>
		/// <param name="originalName">Name associated with original content.</param>
		/// <param name="originalModified">Date and time when content associated with <paramref name="originalName" /> was last modified.</param>
		/// <param name="newName">Name associated with new content.</param>
		/// <param name="newModified">Date and time when content associated with <paramref name="newName" /> was last modified.</param>
		protected DiffChunkCollection(string originalName, DateTime originalModified, string newName, DateTime newModified)
		{
			OriginalName = originalName;
			OriginalModified = originalModified;
			NewName = newName;
			NewModified = newModified;
		}
		
		public abstract void WriteUnifiedDiffTo(TextWriter writer);
		
		public string ToUnifiedDiff()
		{
			using (StringWriter stringWriter = new StringWriter())
			{
				WriteUnifiedDiffTo(stringWriter);
				return stringWriter.ToString();
			}
		}
	}
	
	public abstract class DiffChunkCollection<TDiffChunk, TDiffItem, TValue> : DiffChunkCollection
		where TDiffItem : DiffItem<TValue>
		where TDiffChunk : DiffChunk<TDiffItem, TValue>
	{
        private Collection<TDiffChunk> _chunks = new Collection<TDiffChunk>();

		/// <summary>
		/// Collection <typeparamref name="TDiffChunk" /> items which represent item comparisons.
		/// <summary>
        public Collection<TDiffChunk> Chunks
        {
            get { return _chunks; }
            set { _chunks = (value == null) ? new Collection<TDiffChunk>() : value; }
        }
        
		/// <summary>
		/// Indicates whether any <typeparamref name="TDiffChunk" /> items in the current <see cref="DiffChunkCollection{TDiffChunk,TDiffItem,TValue}" /> represent a modified value.
		/// <summary>
        public override bool IsModified
        {
            get
            {
                foreach (TDiffChunk chunk in _chunks)
                {
                    if (chunk != null && chunk.HasDifference)
                        return true;
                }

                return false;
            }
        }
		
		/// <summary>
		/// Initialize new instance of <see	cref="DiffChunkCollection{TDiffChunk,TDiffItem,TValue}" />.
		/// <summary>
		protected DiffChunkCollection() { }
		
		/// <summary>
		/// Initialize new instance of <see	cref="DiffChunkCollection{TDiffChunk,TDiffItem,TValue}" />.
		/// <summary>
		/// <param name="originalName">Name associated with original content.</param>
		/// <param name="originalModified">Date and time when content associated with <paramref name="originalName" /> was last modified.</param>
		/// <param name="newName">Name associated with new content.</param>
		/// <param name="newModified">Date and time when content associated with <paramref name="newName" /> was last modified.</param>
		/// <param name="chunks">Collection of <typeparamref name="TDiffChunk" /> objects to be added.</param>
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
		
		/// <summary>
		/// Initialize new instance of <see	cref="DiffChunkCollection{TDiffChunk,TDiffItem,TValue}" />.
		/// <summary>
		/// <param name="originalItems">Collection of original <typeparamref name="TValue" /> objects being compared.</param>
		/// <param name="originalName">Name associated with <paramref name="originalItems" />.</param>
		/// <param name="originalModified">Date and time when <paramref name="originalItems" /> was last modified.</param>
		/// <param name="newItems">Collection of new <typeparamref name="TValue" /> objects being compared.</param>
		/// <param name="newName">Name associated with <paramref name="newItems" />.</param>
		/// <param name="newModified">Date and time when <paramref name="newItems" /> was last modified.</param>
		protected DiffChunkCollection(IEnumerable<TValue> originalItems, string originalName, DateTime originalModified, IEnumerable<TValue> newItems, string newName, DateTime newModified)
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
					_chunks.Add(CreateNewChunk(originalSearcher, originalSearcher.RelativeIndex, newSearcher, newSearcher.RelativeIndex + 1));
				return;
			}
			else if (newSearcher.Count == 0)
			{
				_chunks.Add(CreateNewChunk(originalSearcher, originalSearcher.RelativeIndex + 1, newSearcher, newSearcher.RelativeIndex));
				return;	
			}
	
			int originalIndex, newIndex;
			int length = originalSearcher.GetLongestCommonSequence(newSearcher, out originalIndex, out newIndex);
			if (length == 0)
			{
				_chunks.Add(CreateNewChunk(originalSearcher, originalSearcher.RelativeIndex + 1, newSearcher, newSearcher.RelativeIndex + 1));
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
		
		/// <summary>
		/// Create a new, empty <typeparamref name="TDiffChunk" /> object.
		/// <summary>
		/// <returns>A new, empty <typeparamref name="TDiffChunk" /> object.</returns>
		public abstract TDiffChunk CreateNewChunk();
		
		/// <summary>
		/// Create a new <typeparamref name="TDiffChunk" /> object with context information.
		/// <summary>
		/// <param name="originalItems">Collection of original <typeparamref name="TValue" /> objects in comparison.</param>
		/// <param name="originalStartLine">1-based line number associated with first <typeparamref name="TValue" /> object in <paramref name="originalItems" />.</param>
		/// <param name="newItems">Collection of new <typeparamref name="TValue" /> objects in comparison.</param>
		/// <param name="originalStartLine">1-based line number associated with first <typeparamref name="TValue" /> object in <paramref name="newItems" />.</param>
		/// <returns>A new <typeparamref name="TDiffChunk" /> object with context information.</returns>
		protected abstract TDiffChunk CreateNewChunk(IEnumerable<TValue> originalItems, int originalStartLine, IEnumerable<TValue> newItems, int newStartLine);
		
		/// <summary>
		/// Returns a <typeparamref name="TValue" /> object, converting it to a value which does not represent a &quot;missing&quot; item, if necessary.
		/// <summary>
		/// <param name="value"><typeparamref name="TValue" /> object to be coerced.</param>
		/// <returns>A new <typeparamref name="TValue" /> which does not represent a &quot;missing&quot; item.</returns>
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

		/// <summary>
		/// Writes comparison information as Unified Diff text.
		/// <summary>
		/// <param name="source"><seealso cref="TextWriter" /> to write to.</param>
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