using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace PSModuleInstallUtil.Module
{
    public class ManifestParseErrorBase : IManifestParseError
    {
        private int _endColumn;
        private int _endLine;
        private int _length;
        private int _startIndex;
        private int _startColumn;
        private int _startLine;

        public int EndColumn { get { return _endColumn; } }

        public int EndLine { get { return _endLine; } }

        public int Length { get { return _length; } }

        public int StartIndex { get { return _startIndex; } }

        public int StartColumn { get { return _startColumn; } }

        public int StartLine { get { return _startLine; } }

        public ManifestParseErrorBase() { }

        public ManifestParseErrorBase(PSToken token)
        {
            if (token == null)
                return;

            _endColumn = token.EndColumn;
            _endLine = token.EndLine;
            _length = token.Length;
            _startColumn = token.StartColumn;
            _startIndex = token.Start;
            _startLine = token.StartLine;
        }

        public ManifestParseErrorBase(IManifestParseError error)
        {
            if (error == null)
                return;

            _endColumn = error.EndColumn;
            _endLine = error.EndLine;
            _length = error.Length;
            _startColumn = error.StartColumn;
            _startIndex = error.StartIndex;
            _startLine = error.StartLine;
        }

        public ManifestParseErrorBase(IManifestParseError primaryError, IEnumerable<ManifestParseError> additionalErrors)
        {
            List<IManifestParseError> list = new List<IManifestParseError>();
            if (primaryError != null)
                list.Add(primaryError);
            if (additionalErrors != null)
            {
                foreach (ManifestParseError e in additionalErrors)
                {
                    if (e != null)
                        list.Add(e);
                }
            }

            if (list.Count == 0)
                return;

            primaryError = list[0];
            list.RemoveAt(0);
            list.Sort();

            _endColumn = primaryError.EndColumn;
            _endLine = primaryError.EndLine;
            _length = primaryError.Length;
            _startColumn = primaryError.StartColumn;
            _startIndex = primaryError.StartIndex;
            _startLine = primaryError.StartLine;
            int endIndex = _startIndex + _length;
            foreach (IManifestParseError e in list)
            {
                int i = e.StartIndex + e.Length;
                if (endIndex < i)
                    endIndex = i;
                if (e.EndLine > _endLine)
                {
                    _endLine = e.EndLine;
                    _endColumn = e.EndColumn;
                }
                else if (e.EndLine == _endLine && e.EndColumn > _endColumn)
                    _endColumn = e.EndColumn;
            }
            _length = endIndex = _startIndex;
        }

        public virtual bool Equals(IManifestParseError other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return _startIndex == other.StartIndex && _length == other.Length && _startLine == other.StartLine && _startColumn == other.StartColumn && 
                _endLine == other.EndLine && _endColumn == other.EndColumn;
        }

        public virtual int CompareTo(IManifestParseError other)
        {
            if (other == null)
                return 1;

            if (ReferenceEquals(this, other))
                return 0;

            int result = _startIndex - other.StartIndex;
            if (result == 0 && (result = _length - other.Length) == 0 && (result = _startLine - other.StartLine) == 0 && (result = _startColumn - other.StartColumn) == 0
                    && (result = _endLine - other.EndLine) == 0)
                return _endColumn - other.EndColumn;

            return result;
        }
    }
}