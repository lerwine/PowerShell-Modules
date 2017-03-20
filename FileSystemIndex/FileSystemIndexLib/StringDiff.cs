using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FileSystemIndexLib
{
    public class StringDiff : DiffChunkCollection<StringDiffChunkNCS, StringDiffLineNCS, string>
    {
		/// <summary>
		/// Splits a source string into invidual lines.
		/// <summary>
		/// <param name="source">Text to be split by lines.</param>
		/// <returns>An enumeration of string values which were individual lines in <paramref name="source" />, or an empty enumeration if <paramref name="source" /> was null.</returns>
		public static IEnumerable<string> SplitLines(string source)
		{
			if (source == null)
				yield break;
			
			int startIndex = 0;
			int endIndex = 0;
			while (endIndex < source.Length)
			{
				if (source[endIndex] == '\r')
				{
					yield return source.Substring(startIndex, endIndex - startIndex);
					startIndex = endIndex + 1;
					if (startIndex < source.Length && source[startIndex] == '\n')
					{
						startIndex++;
						endIndex++;
					}
				}
				else if (source[endIndex] == '\r')
				{
					yield return source.Substring(startIndex, endIndex - startIndex);
					startIndex = endIndex + 1;
				}
				endIndex++;
			}
			if (startIndex < source.Length)
				yield return source.Substring(startIndex);
		}
		
		/// <summary>
		/// Initialize new instance of <see	cref="StringDiff" />.
		/// <summary>
		public StringDiff() { }
		
		/// <summary>
		/// Initialize new instance of <see	cref="StringDiff" />.
		/// <summary>
		/// <param name="originalName">Name associated with original text.</param>
		/// <param name="originalModified">Date and time when text associated with <paramref name="originalName" /> was last modified.</param>
		/// <param name="newName">Name associated with new text.</param>
		/// <param name="newModified">Date and time when text associated with <paramref name="newName" /> was last modified.</param>
		/// <param name="chunks">Collection of <seealso cref="StringDiffChunkNCS" /> objects to be added.</param>
		public StringDiff(string originalName, DateTime originalModified, string newName, DateTime newModified, IEnumerable<StringDiffChunkNCS> chunks)
			: base(originalName, originalModified, newName, newModified, chunks) { }
		
		/// <summary>
		/// Initialize new instance of <see	cref="StringDiff" />.
		/// <summary>
		/// <param name="originalText">Original text being compared.</param>
		/// <param name="originalName">Name associated with <paramref name="originalText" />.</param>
		/// <param name="originalModified">Date and time when <paramref name="originalText" /> was last modified.</param>
		/// <param name="newText">New text being compared.</param>
		/// <param name="newName">Name associated with <paramref name="newText" />.</param>
		/// <param name="newModified">Date and time when <paramref name="newText" /> was last modified.</param>
		public StringDiff(string originalText, string originalName, DateTime originalModified, string newText, string newName, DateTime newModified)
			: base(SplitLines(originalText), originalName, originalModified, SplitLines(newText), newName, newModified) { }
		
		/// <summary>
		/// Create a new, empty <seealso cref="StringDiffChunkNCS" /> object.
		/// <summary>
		/// <returns>A new, empty <seealso cref="StringDiffChunkNCS" /> object.</returns>
		public override StringDiffChunkNCS CreateNewChunk() { return new StringDiffChunkNCS(); }
		
		/// <summary>
		/// Create a new <seealso cref="StringDiffChunkNCS" /> object with context information.
		/// <summary>
		/// <param name="originalItems">Collection of original lines in comparison.</param>
		/// <param name="originalStartLine">1-based line number associated with first line in <paramref name="originalItems" />.</param>
		/// <param name="newItems">Collection of new lines in comparison.</param>
		/// <param name="originalStartLine">1-based line number associated with first line in <paramref name="newItems" />.</param>
		/// <returns>A new <seealso cref="StringDiffChunkNCS" /> object with context information.</returns>
		protected override StringDiffChunkNCS CreateNewChunk(IEnumerable<string> originalItems, int originalStartLine, IEnumerable<string> newItems, int newStartLine)
		{
			return new StringDiffChunkNCS(originalItems, originalStartLine, newItems, newStartLine);
		}
		
		/// <summary>
		/// Returns a string value, converting any null to an empty string.
		/// <summary>
		/// <param name="value">String to be coerced as a non-null value.</param>
		/// <returns>A non-null string.</returns>
		protected override string CoerceNonAbsentValue(string value) { return (value == null) ? "" : value; }
    }
}