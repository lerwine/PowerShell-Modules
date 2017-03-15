using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FileSystemIndexLib
{
    public class UnifiedFileDiff : UnifiedFileDiff<StringDiffChunkNCS, StringDiffLineNCS, string>
    {
    }

	public abstract class UnifiedFileDiff<TDiffChunk, TDiffLine, TValue>
		where TDiffLine : UnifiedDiffLine<TValue>
		where TDiffChunk : UnifiedDiffChunk<TDiffLine, TValue>
	{
        private string _originalFile = "";
        private string _newFile = "";
        private DateTime _originalModified;
        private DateTime _newModified;
        private Collection<TDiffChunk> _hunks = new Collection<TDiffChunk>();
        
		/*
        public static UnifiedFileDiff FromLines(string originalName, TValue[] originalLines, string newName, TValue[] newLines)
        {
            if (originalName == null)
                throw new ArgumentNullException("originalName");
            if (newName == null)
                throw new ArgumentNullException("newName");
            if (originalName.Trim().Length == 0)
                throw new ArgumentException("Original file name cannot be empty.", "originalName");
            if (newName.Trim().Length == 0)
                throw new ArgumentException("New file name cannot be empty.", "newName");

            LineGroup originalGroup = new LineGroup(originalLines);
            Linegroup newGroup = new LineGroup(newLines);

            UnifiedFileDiff unifiedFileDiff = new UnifiedFileDiff { OriginalFile = originalName, NewFile = newName }
            LineGroup originalDiffStart = originalGroup;
            LineGroup newDiffStart = newGroup;
            while (originalGroup.Count > 0 && newGroup.Count > 0)
            {
                LineGroup ngA;
                LineGroup ogA = LongestCommonSequence(newGroup, out ngA);
                LineGroup ogB;
                LineGroup ngB = newGroup.LongestCommonSequence(this, out ogB);
                int commonSequenceCount;
                if (ogA.Count == 0 && ngB.Count == 0)
                    continue;

                if (originalGroup.StartIndex > originalDiffStart.StartIndex)
                {
                    // originalDiffStart.SubGroup(0, originalGroup.StartIndex - originalDiffStart.StartIndex).ToArray() Lines have been modified starting at originalDiffStart.StartIndex and newDiffStart.StartIndex;
                }

                if (ogA.Count > ngB.Count)
                {
                    // newGroup.SubGroup(0, ngA.StartIndex - newGroup.StartIndex).ToArray() Lines starting at newGroup.StartIndex have been inserted;
                    newGroup = ngA.SubGroup(ngA.Count);
                    originalGroup = ogA.SubGroup(ogA.Count);
                }
                else
                {
                    // originalGroup.SubGroup(0, ogB.StartIndex - originalGroup.StartIndex).ToArray()Lines starting at originalGroup.StartIndex have been removed;
                    newGroup = ngB.SubGroup(ngB.Count);
                    originalGroup = ogB.SubGroup(ogB.Count);
                }
                originalDiffStart = originalGroup.StartIndex;
                newDiffStart = newGroup.StartIndex;
            }
            
            if (originalGroup.StartIndex > originalDiffStart.StartIndex)
            {
                // originalDiffStart.SubGroup(0, originalGroup.StartIndex - originalDiffStart.StartIndex).ToArray() Lines have been modified starting at originalDiffStart.StartIndex and newDiffStart.StartIndex;
            }

            if (originalGroup.Count > 0)
            {
                // Remaining lines of originalGroup have been removed
            }
            else if (newGroup.Count > 0)
            {
                // Remaining lines of newGroup have been added
            }
        }
		*/
        public string OriginalFile
        {
            get { return _originalFile; }
            set
            {
                if (value == null)
                    _originalFile = "";
                else
                {
                    if (value != null && (value.Contains("\r") || value.Contains("\n") || value.Contains("\t")))
                        throw new ArgumentException("Filenames contain newline or tab characters.");
                    _originalFile = value;
                }
            }
        }

        public string NewFile
        {
            get { return _newFile; }
            set
            {
                if (value == null)
                    _newFile = "";
                else
                {
                    if (value != null && (value.Contains("\r") || value.Contains("\n") || value.Contains("\t")))
                        throw new ArgumentException("Filenames contain newline or tab characters.");
                    _newFile = value;
                }
            }
        }
        
        public Collection<TDiffChunk> Hunks
        {
            get { return _hunks; }
            set { _hunks = (value == null) ? new Collection<TDiffChunk>() : value; }
        }
        
        public bool IsAdded { get { return _originalFile.Length == 0 && _newFile.Length > 0; } }
        
        public bool IsRemoved { get { return _newFile.Length == 0 && _originalFile.Length > 0; } }
        
        public bool IsModified
        {
            get
            {
                if (_originalFile.Length == 0 || _newFile.Length == 0)
                    return false;

                foreach (TDiffChunk chunk in _hunks)
                {
                    if (chunk != null && chunk.HasDifference)
                        return true;
                }

                return false;
            }
        }

        public void WriteTo(TextWriter writer)
        {
            if (!IsModified)
                return;

            writer.WriteLine("--- {0}\t{1:yyyy-MM-dd HH:mm:ss.iiiiii zzzzzz}", _originalFile, _originalModified);
            writer.WriteLine("+++ {0}\t{1:yyyy-MM-dd HH:mm:ss.iiiiii zzzzzz}", _newFile, _newModified);

            foreach (TDiffChunk chunk in _hunks)
            {
                if (chunk != null && chunk.HasDifference)
                    chunk.WriteTo(writer);
            }
            writer.WriteLine("");
        }
    }
}