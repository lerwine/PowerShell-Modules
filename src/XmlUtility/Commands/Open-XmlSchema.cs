using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Xml;
using System.Xml.Schema;
using Microsoft.PowerShell.Commands;

namespace XmlUtility.Commands
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    [Cmdlet(VerbsCommon.Open, "XmlSchema", DefaultParameterSetName = ParameterSetName_FromPipeline)]
    public class Open_XmlSchema : PSCmdlet
    {
        public const string ParameterSetName_Glob = "Glob";
        public const string ParameterSetName_FromPipeline = "FromPipeline";
        public const string ParameterSetName_Literal = "Literal";
        private XmlReaderSettings _readerSettings;

        [Parameter(HelpMessage = "Path of XML schema file to open", Mandatory = true, ParameterSetName = ParameterSetName_FromPipeline, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string[] InputPath { get; set; }

        [Parameter(HelpMessage = "Path of XML schema file to open", Mandatory = true, ParameterSetName = ParameterSetName_Glob)]
        [ValidateNotNullOrEmpty()]
        public string[] Path { get; set; }

        [Parameter(HelpMessage = "Literal Path of XML schema file to open", Mandatory = true, ParameterSetName = ParameterSetName_Literal)]
        [ValidateNotNullOrEmpty()]
        [Alias("PSPath")]
        public string[] LiteralPath { get; set; }

        [Parameter(HelpMessage = "Settings to use when reading from XML file.")]
        public XmlReaderSettings ReaderSettings { get; set; }

        public SwitchParameter DoNotCompile { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _readerSettings = (ReaderSettings == null) ? new XmlReaderSettings() : ReaderSettings.Clone();
        }

        private void ProcessPSPaths(string[] psPaths, string paramName)
        {
            if (psPaths == null || psPaths.Length == 0)
                return;
            
            Type t = typeof(FileSystemProvider);
            foreach (string path in psPaths.Where(p => !String.IsNullOrEmpty(p)))
            {
                if (Stopping)
                    return;
                try
                {
                    foreach (PathInfo pathInfo in SessionState.Path.GetResolvedPSPathFromPSPath(path))
                    {
                        if (Stopping)
                            return;
                        if (pathInfo.Provider.ImplementingType.AssemblyQualifiedName != t.AssemblyQualifiedName)
                            throw new NotSupportedException("Only filesystem paths are supported by this command.");
                        try { WriteObject(new SchemaLoadResult(pathInfo.ProviderPath, _readerSettings, DoNotCompile.IsPresent)); }
                        catch (FileNotFoundException ex)
                        {
                            WriteError(new ErrorRecord(ex, "PathNotFound", ErrorCategory.ObjectNotFound, pathInfo.Path));
                        }
                        catch (DirectoryNotFoundException ex)
                        {
                            WriteError(new ErrorRecord(ex, "PathNotFound", ErrorCategory.ObjectNotFound, pathInfo.Path));
                        }
                        catch (IOException ex)
                        {
                            WriteError(new ErrorRecord(ex, "PathReadError", ErrorCategory.OpenError, pathInfo.Path));
                        }
                        catch (XmlSchemaException ex)
                        {
                            WriteError(new ErrorRecord(ex, "PathXmlError", ErrorCategory.ParserError, pathInfo.Path));
                        }
                        catch (XmlException ex)
                        {
                            WriteError(new ErrorRecord(ex, "PathXmlError", ErrorCategory.ParserError, pathInfo.Path));
                        }
                        catch (Exception ex)
                        {
                            if (ex is IContainsErrorRecord)
                            {
                                ErrorRecord errorRecord = ((IContainsErrorRecord)ex).ErrorRecord;
                                if (errorRecord != null)
                                {
                                    WriteError(errorRecord);
                                    continue;
                                }
                            }
                            WriteError(new ErrorRecord(ex, "UnexpectedError", ErrorCategory.NotSpecified, pathInfo.Path));
                        }
                    }
                }
                catch (NotSupportedException ex)
                {
                    WriteError(new ErrorRecord(ex, "PathProviderNotSupported", ErrorCategory.NotImplemented, path));
                }
                catch (Exception ex)
                {
                    if (ex is IContainsErrorRecord)
                    {
                        ErrorRecord errorRecord = ((IContainsErrorRecord)ex).ErrorRecord;
                        if (errorRecord != null)
                        {
                            WriteError(errorRecord);
                            continue;
                        }
                    }
                    WriteError(new ErrorRecord(ex, "UnexpectedError", ErrorCategory.NotSpecified, path));
                }
            }
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (Stopping)
                return;

            ProcessPSPaths(Path, "Path");
            ProcessPSPaths(InputPath, "InputPath");

            if (LiteralPath == null)
                return;
            ProviderInfo provider;
            PSDriveInfo drive;
            Type t = typeof(FileSystemProvider);
            foreach (string path in LiteralPath)
            {
                if (String.IsNullOrEmpty(path))
                    continue;
                
                if (Stopping)
                    return;
                string providerPath;
                try
                {
                    providerPath = SessionState.Path.GetUnresolvedProviderPathFromPSPath(path, out provider, out drive);
                    if (provider.ImplementingType.AssemblyQualifiedName != t.AssemblyQualifiedName)
                        throw new NotSupportedException("Only filesystem paths are supported by this command.");
                    try { WriteObject(new SchemaLoadResult(providerPath, _readerSettings, DoNotCompile.IsPresent)); }
                    catch (FileNotFoundException ex)
                    {
                        WriteError(new ErrorRecord(ex, "PathNotFound", ErrorCategory.ObjectNotFound, providerPath));
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        WriteError(new ErrorRecord(ex, "PathNotFound", ErrorCategory.ObjectNotFound, providerPath));
                    }
                    catch (IOException ex)
                    {
                        WriteError(new ErrorRecord(ex, "PathReadError", ErrorCategory.OpenError, providerPath));
                    }
                    catch (XmlSchemaException ex)
                    {
                        WriteError(new ErrorRecord(ex, "PathXmlError", ErrorCategory.ParserError, providerPath));
                    }
                    catch (XmlException ex)
                    {
                        WriteError(new ErrorRecord(ex, "PathXmlError", ErrorCategory.ParserError, providerPath));
                    }
                }
                catch (NotSupportedException ex)
                {
                    WriteError(new ErrorRecord(ex, "PathProviderNotSupported", ErrorCategory.NotImplemented, path));
                }
                catch (Exception ex)
                {
                    if (ex is IContainsErrorRecord)
                    {
                        ErrorRecord errorRecord = ((IContainsErrorRecord)ex).ErrorRecord;
                        if (errorRecord != null)
                        {
                            WriteError(errorRecord);
                            continue;
                        }
                    }
                    WriteError(new ErrorRecord(ex, "UnexpectedError", ErrorCategory.NotSpecified, path));
                }
            }
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
