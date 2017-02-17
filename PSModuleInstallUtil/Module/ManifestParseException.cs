using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Runtime.Serialization;

namespace PSModuleInstallUtil.Module
{
    [Serializable]
    internal class ManifestParseException : Exception, IManifestParseError
    {
        [NonSerialized]
        private ReadOnlyCollection<ManifestParseError> _errors;
        [NonSerialized]
        ManifestParseErrorBase _primaryError;
        public const string PropertyName_PrimaryError = "PrimaryError";
        public const string PropertyName_Errors = "Errors";

        protected ManifestParseErrorBase PrimaryError
        {
            get
            {
                if (_primaryError == null)
                    _primaryError = (_errors.Count == 1) ? _errors[0] : new ManifestParseErrorBase();
                return _primaryError;
            }
        }
        
        public int EndColumn { get { return PrimaryError.EndColumn; } }

        public int EndLine { get { return PrimaryError.EndLine; } }

        public int Length { get { return PrimaryError.Length; } }

        public int StartIndex { get { return PrimaryError.StartIndex; } }

        public int StartColumn { get { return PrimaryError.StartColumn; } }

        public int StartLine { get { return PrimaryError.StartLine; } }

        public ReadOnlyCollection<ManifestParseError> Errors
        {
            get
            {
                if (_errors == null)
                    _errors = new ReadOnlyCollection<ManifestParseError>(new ManifestParseError[0]);
                return _errors;
            }
        }

        public ManifestParseException() { }

        public ManifestParseException(PSParseError error) : base((error == null) ? "" : error.Message)
        {
            _errors = new ReadOnlyCollection<ManifestParseError>(new ManifestParseError[] { new ManifestParseError(error) });
            _primaryError = _errors[0];
        }

        public ManifestParseException(IEnumerable<ManifestParseError> errors) : this(new NormalizedErrorDataWithoutInnerException(null, errors)) { }

        public ManifestParseException(string message) : base(message) { }

        public ManifestParseException(IEnumerable<PSParseError> errors) : this(null, errors) { }

        public ManifestParseException(string message, IEnumerable<PSParseError> errors) : this(new NormalizedErrorDataWithoutInnerException(message, errors)) { }

        public ManifestParseException(string message, Exception innerException) : this(new NormalizedErrorDataWithInnerException(message, innerException, null as IEnumerable<PSParseError>)) { }

        public ManifestParseException(string message, Exception innerException, IEnumerable<PSParseError> errors) : this(new NormalizedErrorDataWithInnerException(message, innerException, errors)) { }

        private ManifestParseException(NormalizedErrorDataWithInnerException data)
            : base(data.Message, data.InnerException)
        {
            _errors = new ReadOnlyCollection<ManifestParseError>(data.Errors);
            _primaryError = data.PrimaryError;
        }

        private ManifestParseException(NormalizedErrorDataWithoutInnerException data)
            : base(data.Message)
        {
            _errors = new ReadOnlyCollection<ManifestParseError>(data.Errors);
            _primaryError = data.PrimaryError;
        }

        protected internal class NormalizedErrorDataWithInnerException : NormalizedErrorData
        {
            private Exception _innerException;

            public Exception InnerException { get { return _innerException; } }

            protected internal NormalizedErrorDataWithInnerException(string message, Exception innerException, IEnumerable<PSParseError> errors)
                : base(message, innerException as IManifestParseError, errors)
            {
                _innerException = innerException;
            }
        }

        protected internal class NormalizedErrorDataWithoutInnerException : NormalizedErrorData
        {
            protected internal NormalizedErrorDataWithoutInnerException(string message, IEnumerable<PSParseError> errors) : base(message, null, errors) { }
            protected internal NormalizedErrorDataWithoutInnerException(string message, IEnumerable<ManifestParseError> errors) : base(message, errors) { }
        }

        protected internal class NormalizedErrorData
        {
            private string _message;
            private ManifestParseError[] _errors;
            private ManifestParseErrorBase _primaryError;

            protected internal string Message { get { return _message; } }

            protected internal ManifestParseErrorBase PrimaryError { get { return _primaryError; } }

            protected internal ManifestParseError[] Errors { get { return _errors; } }

            protected NormalizedErrorData(string message, IEnumerable<ManifestParseError> errors)
            {
                if (errors == null)
                    Initialize(message, null, null);
                else if (errors is IList<ManifestParseError>)
                    Initialize(message, null, errors as IList<ManifestParseError>);
                else
                {
                    List<ManifestParseError> errorList = new List<ManifestParseError>();
                    if (errors != null)
                        errorList.AddRange(errors);
                    Initialize(message, null, errorList);
                }
            }

            protected NormalizedErrorData(string message, IManifestParseError primaryError, IEnumerable<PSParseError> errors)
            {
                List<ManifestParseError> errorList = new List<ManifestParseError>();
                if (errors != null)
                {
                    foreach (PSParseError error in errors)
                    {
                        if (error != null)
                            errorList.Add(new ManifestParseError(error));
                    }
                }
                Initialize(message, primaryError, errorList);
            }

            private void Initialize(string message, IManifestParseError primaryError, IList<ManifestParseError> errors)
            {
                List<ManifestParseError> list = new List<Module.ManifestParseError>();
                if (errors == null || errors.Count == 0)
                    _errors = new ManifestParseError[0];
                else
                {
                    foreach (ManifestParseError e in list)
                    {
                        if (e != null)
                            list.Add(e);
                    }
                    list.Sort();
                    _errors = list.ToArray();
                }
                if (String.IsNullOrEmpty(message))
                {
                    if (list.Count == 1)
                        _message = list[0].Message;
                    else if (list.Count > 1)
                        _message = String.Format("{0} manifest parse errors have occurred.", list.Count);
                    else
                        _message = "An unspecifed manifest parse error has occurred.";
                }
                else
                    _message = message;

                if (primaryError != null)
                    _primaryError = (primaryError is ManifestParseErrorBase) ? primaryError as ManifestParseErrorBase : new ManifestParseErrorBase(primaryError);
                else
                    _primaryError = new ManifestParseErrorBase(primaryError, list);
            }
        }

        protected ManifestParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _primaryError = info.GetValue(PropertyName_PrimaryError, typeof(ManifestParseErrorBase)) as ManifestParseErrorBase;
            ManifestParseError[] errors = info.GetValue(PropertyName_Errors, typeof(ManifestParseError[])) as ManifestParseError[];
            if (errors != null)
                _errors = new ReadOnlyCollection<ManifestParseError>(errors);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            if (_primaryError != null && (_errors == null || _errors.Count != 1 || !ReferenceEquals(_primaryError, _errors[0])))
                info.AddValue(PropertyName_PrimaryError, _primaryError, typeof(ManifestParseErrorBase));
            else
                info.AddValue(PropertyName_PrimaryError, null, typeof(ManifestParseErrorBase));
            info.AddValue(PropertyName_Errors, (new List<ManifestParseError>(Errors)).ToArray(), typeof(ManifestParseError[]));
        }
        
        public virtual bool Equals(IManifestParseError other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return PrimaryError.Equals(other);
        }

        public virtual int CompareTo(IManifestParseError other)
        {
            if (other == null)
                return 1;

            if (ReferenceEquals(this, other))
                return 0;

            return PrimaryError.CompareTo(other);
        }
    }
}