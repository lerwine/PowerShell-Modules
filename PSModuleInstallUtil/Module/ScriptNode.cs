using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace PSModuleInstallUtil.Module
{
    public abstract class ScriptNode
    {
        private ScriptNode _parentNode, _previousNode, _nextNode, _firstChild;
        private ScriptDocument _ownerDocument;
        private PSToken _token;
        public ScriptNode ParentNode { get { return _parentNode; } }
        public ScriptNode PreviousNode { get { return _previousNode; } }
        public ScriptNode NextNode { get { return _nextNode; } }
        public ScriptNode FirstChild { get { return _firstChild; } }
        public ScriptDocument OwnerDocument { get { return _ownerDocument; } }
        public abstract string Value { get; }
        public abstract string Text { get; }
        public abstract int Start { get; }
        public abstract int Length { get; }
        public abstract ScriptNodeType NodeType { get; }
        protected ScriptNode(ScriptNode parentNode)
        {
            _parentNode = parentNode;
            if (parentNode == null)
            {
                if (this is ScriptDocument)
                    _ownerDocument = this as ScriptDocument;
                return;
            }

            _ownerDocument = parentNode._ownerDocument;

            if (_parentNode._firstChild == null)
            {
                _parentNode._firstChild = this;
                return;
            }
            ScriptNode lastNode = _parentNode._firstChild;
            while (lastNode._nextNode != null)
                lastNode = lastNode._nextNode;
            _previousNode = lastNode;
            lastNode._nextNode = this;
        }
    }
}
