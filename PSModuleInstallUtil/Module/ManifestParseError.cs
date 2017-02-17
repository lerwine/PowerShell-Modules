using System;
using System.Management.Automation;

namespace PSModuleInstallUtil.Module
{
    public class ManifestParseError : ManifestParseErrorBase
    {
        private string _message;
        private string _content;
        private PSTokenType _tokenType = PSTokenType.Unknown;

        public ManifestParseError()
        {
            _content = "";
            _message = "";
        }

        public ManifestParseError(PSParseError error)
            : base((error == null) ? null : error.Token)
        {
            if (error == null)
            {
                _content = "";
                _message = "";
            }
            else
            {
                _message = (error.Message == null) ? "" : error.Message;
                _content = (error.Token.Content == null) ? error.Token.Content : "";
                _tokenType = error.Token.Type;
            }
        }

        public string Message { get { return _message; } }

        public string Content { get { return _content; } }

        public PSTokenType TokenType { get { return _tokenType; } }

        public override bool Equals(IManifestParseError other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (base.Equals(other) || !(other is ManifestParseError))
                return false;

            ManifestParseError e = other as ManifestParseError;
            return _tokenType == e.TokenType && _message == e.Message && _content == e.Content;
        }

        public override int CompareTo(IManifestParseError other)
        {
            if (other == null)
                return 1;

            if (ReferenceEquals(this, other))
                return 0;

            int result = base.CompareTo(other);
            if (result != 0)
                return result;

            if (other is ManifestParseError)
            {
                ManifestParseError e = other as ManifestParseError;
                if ((result = _tokenType.CompareTo(e.TokenType)) != 0 && (result = _message.CompareTo(e.Message)) != 0)
                    return result;

                return _content.CompareTo(e.Content);
            }

            return -1;
        }
    }
}