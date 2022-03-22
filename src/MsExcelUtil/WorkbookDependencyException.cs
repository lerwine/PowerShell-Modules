using System;
using System.Management.Automation;
using System.Runtime.Serialization;

namespace MsExcelUtil
{
    [Serializable]
    internal class WorkbookDependencyException : RuntimeException
    {
        private readonly ErrorRecord _errorRecord;
        public override ErrorRecord ErrorRecord => _errorRecord;

        public WorkbookDependencyException() { }

        public WorkbookDependencyException(string message, string errorId, ErrorCategory errorCategory, object target) : base(message)
            => _errorRecord = new ErrorRecord(this, errorId, errorCategory, target);

        public WorkbookDependencyException(string message, string errorId, ErrorCategory errorCategory, object target, Exception innerException) : base(message, innerException)
            => _errorRecord = new ErrorRecord(this, errorId, errorCategory, target);

        protected WorkbookDependencyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}