using System;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace PSModuleInstallUtil.Module
{
    public class ManifestNodeReader : ICloneable
    {
        private Collection<PSToken> _tokens;
        private int _startRange;
        private int _endRange;
        private int _index;

        public PSToken Current { get { return (_index < _endRange) ? _tokens[_index] : null; } }

        public bool MoveNext()
        {
            if (_index == _endRange)
                return false;
            _index++;
            return _index != _endRange;
        }

        public void Reset() { _index = _startRange; }

        public int Index { get { return _index; } }
        
        public ManifestNodeReader ReadGroup()
        {
            if (Current.Type != PSTokenType.GroupStart)
                return null;

            if (!MoveNext())
                return null;
            int startRange = _index;
            while (Current != null && Current.Type != PSTokenType.GroupEnd)
            {
                if (Current.Type == PSTokenType.GroupStart)
                    ReadGroup();
                else
                    MoveNext();
            }

            if (startRange == _index)
                return null;

            ManifestNodeReader reader = new ManifestNodeReader(this, startRange, _index);
            MoveNext();
            return reader;
        }

        public ManifestNodeReader ReadStatement()
        {
            int startRange = _index;
            while (Current != null && Current.Type != PSTokenType.NewLine && Current.Type != PSTokenType.StatementSeparator)
            {
                if (Current.Type == PSTokenType.GroupStart)
                    ReadGroup();
                else
                    MoveNext();
            }

            if (startRange == _index)
                return null;

            ManifestNodeReader reader = new ManifestNodeReader(this, startRange, _index);
            MoveNext();
            return reader;
        }

        public ManifestNodeReader(string sourceText)
        {
            Collection<PSParseError> errors;
            _tokens = PSParser.Tokenize(sourceText, out errors);
            if (errors != null && errors.Count > 0)
                throw new ManifestParseException(errors);
            _startRange = 0;
            _endRange = _tokens.Count;
            _index = _endRange;
        }

        public ManifestNodeReader Clone()
        {
            ManifestNodeReader target = new ManifestNodeReader(this, _startRange, _endRange);
            target._index = target._startRange;
            return target;
        }

        object ICloneable.Clone() { return Clone(); }

        private ManifestNodeReader(ManifestNodeReader source, int startRange, int endRange)
        {
            _index = startRange;
            _startRange = startRange;
            _endRange = endRange;
            _tokens = source._tokens;
        }
    }
}