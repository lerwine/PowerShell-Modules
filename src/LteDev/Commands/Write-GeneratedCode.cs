using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace LteDev.Commands
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    [Cmdlet(VerbsCommunications.Write, "Write-GeneratedCode")]
    [OutputType(typeof(string))]
    public class Write_GeneratedCode : PSCmdlet
    {
        public const string ParameterSetName_CompileUnit = "CompileUnit";
        public const string ParameterSetName_Expression = "Expression";
        public const string ParameterSetName_Member = "Member";
        public const string ParameterSetName_NS = "NS";
        public const string ParameterSetName_Statement = "Statement";
        public const string ParameterSetName_Type = "Type";

        #region Properties

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_CompileUnit)]
        [ValidateNotNullOrEmpty()]
        public CodeCompileUnit[] CompileUnit { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_Expression)]
        [ValidateNotNullOrEmpty()]
        public CodeExpression[] Expression { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_Member)]
        [ValidateNotNullOrEmpty()]
        public CodeTypeMember[] Member { get; set; }


        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_NS)]
        [ValidateNotNullOrEmpty()]
        public CodeNamespace[] NS { get; set; }


        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_Statement)]
        [ValidateNotNullOrEmpty()]
        public CodeStatement[] Statement { get; set; }


        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_Type)]
        [ValidateNotNullOrEmpty()]
        public CodeTypeDeclaration[] Type { get; set; }

        [Parameter]
        [ValidateNotNull()]
        public CodeDomProvider Provider { get; set; }
        
        [Parameter]
        [ValidateNotNull()]
        public TextWriter Writer { get; set; }
        
        [Parameter]
        [ValidateNotNull()]
        public CodeGeneratorOptions Options { get; set; }

        #endregion

        #region fields

        private CodeDomProvider _provider = null;

        private TextWriter _writer = null;

        private IndentedTextWriter _indentedWriter = null;

        #endregion

        #region Overrides

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _provider = (Provider == null) ? CodeDomProvider.CreateProvider(New_CodeDomProvider.ParameterSetName_CSharp) : Provider;
            if (Writer == null)
            {
                _writer = new StringWriter();
                _indentedWriter = new IndentedTextWriter(_writer);
            }
            else if (Writer is IndentedTextWriter)
                _indentedWriter = (IndentedTextWriter)Writer;
            else
                _indentedWriter = new IndentedTextWriter(Writer);
        }

        protected override void ProcessRecord()
        {
            if (CompileUnit != null && CompileUnit.Length > 0)
            {
                foreach (CodeCompileUnit compileUnit in CompileUnit)
                {
                    if (compileUnit != null)
                        _provider.GenerateCodeFromCompileUnit(compileUnit, _indentedWriter, Options);
                }
            }
            else if (Expression != null && Expression.Length > 0)
            {
                foreach (CodeExpression expr in Expression)
                {
                    if (expr != null)
                        _provider.GenerateCodeFromExpression(expr, _indentedWriter, Options);
                }   
            }
            else if (Member != null && Member.Length > 0)
            {
                foreach (CodeTypeMember member in Member)
                {
                    if (member != null)
                        _provider.GenerateCodeFromMember(member, _indentedWriter, Options);
                }   
            }
            else if (NS != null && NS.Length > 0)
            {
                foreach (CodeNamespace ns in NS)
                {
                    if (ns != null)
                        _provider.GenerateCodeFromNamespace(ns, _indentedWriter, Options);
                }   
            }
            else if (Statement != null && Statement.Length > 0)
            {
                foreach (CodeStatement st in Statement)
                {
                    if (st != null)
                        _provider.GenerateCodeFromStatement(st, _indentedWriter, Options);
                }   
            }
            else if (Type != null && Type.Length > 0)
            {
                foreach (CodeTypeDeclaration t in Type)
                {
                    if (t != null)
                        _provider.GenerateCodeFromType(t, _indentedWriter, Options);
                }   
            }
        }

        protected override void EndProcessing()
        {
            base.BeginProcessing();
            try
            {
                try
                {
                    if (Provider == null)
                        _provider.Dispose();
                }
                finally
                {
                    if (Writer == null)
                    {
                        try
                        {
                            try
                            {
                                _indentedWriter.Flush();
                                _writer.Flush();
                                _writer.ToString();
                            }
                            finally { _indentedWriter.Dispose(); }
                        }
                        finally { _writer.Dispose(); }
                    }
                    else if (!(Writer is IndentedTextWriter))
                    {
                        try { _indentedWriter.Flush(); }
                        finally { _indentedWriter.Dispose(); }
                    }
                }
            }
            catch (Exception exc)
            {
                WriteError(new ErrorRecord(exc, "WriterCloseError", ErrorCategory.CloseError, null));
            }
        }

        #endregion
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
