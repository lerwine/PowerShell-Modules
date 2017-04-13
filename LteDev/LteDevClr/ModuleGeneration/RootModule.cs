using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace LteDevClr.ModuleGeneration
{
    [Serializable]
    public class RootModule
    {
        public static readonly Regex NewLineRegex = new Regex(@"\r\n?|\n", RegexOptions.Compiled);

        public static void WriteManifestCommentedLines(StringBuilder generationEnvironment, params string[] lines)
        {
            WriteManifestCommentedLines(generationEnvironment, (IEnumerable<string>)lines);
        }

        public static void WriteManifestCommentedLines(StringBuilder generationEnvironment, IEnumerable<string> lines)
        {
            if (lines == null)
                return;

            foreach (string l in lines.Where(s => s != null).SelectMany(l => NewLineRegex.Split(l)).Select(s => "# " + s))
                generationEnvironment.AppendLine(l.TrimEnd());
        }

        public static void WriteStringManifestValue(StringBuilder generationEnvironment, string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                generationEnvironment.Append("''");
                return;
            }

            if (value.Contains('\n') || value.Contains('\r'))
            {
                generationEnvironment.AppendLine("@'");
                generationEnvironment.AppendLine(value);
                generationEnvironment.Append("'@");
            }
            else
                generationEnvironment.AppendFormat("'{0}'", value.Replace("'", "''"));
        }

        public static void WriteManifestMember(StringBuilder generationEnvironment, string comment, string name, string value)
        {
            generationEnvironment.AppendLine();
            if (value == null)
            {
                WriteManifestCommentedLines(generationEnvironment, comment, String.Format("{0} = ''", name));
                return;
            }

            WriteManifestCommentedLines(generationEnvironment, comment);
            generationEnvironment.Append(name);
            generationEnvironment.Append(" = ");
            WriteStringManifestValue(generationEnvironment, value);
            generationEnvironment.AppendLine();
        }

        public static void WriteManifestMember(StringBuilder generationEnvironment, string comment, string name, Version value)
        {
            WriteManifestMember(generationEnvironment, comment, name, (value == null) ? null : value.ToString());
        }

        public static void WriteManifestMember(StringBuilder generationEnvironment, string comment, string name, Guid? value)
        {
            WriteManifestMember(generationEnvironment, comment, name, (value.HasValue) ? value.Value.ToString("n") : null);
        }

        public static void WriteManifestMemberArray(StringBuilder generationEnvironment, string comment, string name, IEnumerable<string> collection, bool allowWildCard = false)
        {
            string[] values = (collection == null) ? new string[0] : collection.Where(s => s != null).Select(s => s.Trim()).Where(s => s.Length > 0).ToArray();

            if (values.Length == 0)
            {
                WriteManifestCommentedLines(generationEnvironment, comment, String.Format("{0} = {1}", name, (allowWildCard) ? "@()" : "'*'"));
                return;
            }

            WriteManifestCommentedLines(generationEnvironment, comment);
            generationEnvironment.Append(name);
            if (values.Length == 1 && values[0] == "*")
            {
                generationEnvironment.AppendLine(" = '*'");
                return;
            }

            generationEnvironment.Append(" = @(");
            WriteStringManifestValue(generationEnvironment, values[0]);
            foreach (string s in values.Skip(1))
            {
                generationEnvironment.Append(", ");
                WriteStringManifestValue(generationEnvironment, s);
            }
            generationEnvironment.AppendLine(")");
        }

        public static void WriteManifestMemberArray(StringBuilder generationEnvironment, string comment, string name, IEnumerable<ModuleReference> collection)
        {
            ModuleReference[] modules = (collection == null) ? new ModuleReference[0] : collection.Where(r => r != null && !String.IsNullOrWhiteSpace(r.ModuleName)).ToArray();
            if (!modules.Any(m => !String.IsNullOrWhiteSpace(m.Guid) || !String.IsNullOrWhiteSpace(m.ModuleVersion)))
            {
                WriteManifestMemberArray(generationEnvironment, comment, name, modules.Select(m => m.ModuleName));
                return;
            }

            generationEnvironment.Append(" = @(");
            modules[0].WriteManifestHashValue(generationEnvironment);
            foreach (ModuleReference m in modules.Skip(1))
            {
                generationEnvironment.AppendLine(",");
                generationEnvironment.Append("    ");
                m.WriteManifestHashValue(generationEnvironment);
            }
            generationEnvironment.AppendLine();
            generationEnvironment.AppendLine(")");
        }

        public static void WriteManifestMemberArray(StringBuilder generationEnvironment, string comment, string name, IEnumerable<CommandItem> collection)
        {
            WriteManifestMemberArray(generationEnvironment, comment, name, (collection == null) ? new string[0] : collection.Where(r => r != null).Select(c => c.Name), true);
        }
        
        public void WriteModuleManifest(StringBuilder generationEnvironment)
        {
            WriteManifestCommentedLines(generationEnvironment, "", (String.IsNullOrWhiteSpace(Name)) ? "Module manifest" : String.Format("Module manifest for module '{0}'", Name), String.Format("Generated on: {0:yyyy-MM-DD HH:mm:ss}", DateTime.Now), "");
            generationEnvironment.AppendLine("@{");
            WriteManifestCommentedLines(generationEnvironment, "Script module or binary module file associated with this manifest.");
            if (String.IsNullOrWhiteSpace(Name))
                WriteManifestCommentedLines(generationEnvironment, "RootModule = ''");
            else
                generationEnvironment.AppendLine(String.Format("RootModule = '{0}'", Name));
            WriteManifestMember(generationEnvironment, "Version number of this module.", "ModuleVersion", ModuleVersion);
            WriteManifestMember(generationEnvironment, "ID used to uniquely identify this module.", "GUID", ModuleGuid);
            WriteManifestMember(generationEnvironment, "Author of this module.", "Author", (String.IsNullOrWhiteSpace(Author)) ? "Unknown" : Author);
            WriteManifestMember(generationEnvironment, "Company or vendor of this module", "CompanyName", CompanyName);
            WriteManifestMember(generationEnvironment, "Copyright statement for this module", "Copyright", Copyright);
            WriteManifestMember(generationEnvironment, "Description of the functionality provided by this module", "Description", Description);
            WriteManifestMember(generationEnvironment, "Minimum version of the Windows PowerShell engine required by this module", "PowerShellVersion", PowerShellVersion);
            WriteManifestMember(generationEnvironment, "Name of the Windows PowerShell host required by this module", "PowerShellHostName", PowerShellHostName);
            WriteManifestMember(generationEnvironment, "Minimum version of the Windows PowerShell host required by this module", "PowerShellHostVersion", PowerShellHostVersion);
            WriteManifestMember(generationEnvironment, "Minimum version of Microsoft .NET Framework required by this module", "DotNetFrameworkVersion", DotNetFrameworkVersion);
            WriteManifestMember(generationEnvironment, "Minimum version of the common language runtime (CLR) required by this module", "CLRVersion", CLRVersion);
            WriteManifestMember(generationEnvironment, "Processor architecture (None, X86, Amd64) required by this module", "ProcessorArchitecture", ProcessorArchitecture);
            WriteManifestMemberArray(generationEnvironment, "Modules that must be imported into the global environment prior to importing this module", "RequiredModules", RequiredModules);
            WriteManifestMemberArray(generationEnvironment, "Assemblies that must be loaded prior to importing this module", "RequiredAssemblies", RequiredAssemblies);
            WriteManifestMemberArray(generationEnvironment, "Script files (.ps1) that are run in the caller's environment prior to importing this module.", "ScriptsToProcess", ScriptsToProcess);
            WriteManifestMemberArray(generationEnvironment, "Type files (.ps1xml) to be loaded when importing this module", "TypesToProcess", TypesToProcess);
            WriteManifestMemberArray(generationEnvironment, "Format files (.ps1xml) to be loaded when importing this module", "FormatsToProcess", FormatsToProcess);
            WriteManifestMemberArray(generationEnvironment, "Modules to import as nested modules of the module specified in RootModule/ModuleToProcess", "NestedModules", NestedModules);
            WriteManifestMemberArray(generationEnvironment, "Functions to export from this module", "FunctionsToExport", FunctionsToExport);
            WriteManifestMemberArray(generationEnvironment, "Cmdlets to export from this module", "CmdletsToExport", CmdletsToExport);
            WriteManifestMemberArray(generationEnvironment, "Variables to export from this module", "VariablesToExport", VariablesToExport, true);
            WriteManifestMemberArray(generationEnvironment, "Aliases to export from this module", "AliasesToExport", AliasesToExport, true);
            WriteManifestMemberArray(generationEnvironment, "List of all modules packaged with this module", "ModuleList", ModuleList);
            WriteManifestMemberArray(generationEnvironment, "List of all files packaged with this module", "FileList", FileList);
            WriteManifestMember(generationEnvironment, "HelpInfo URI of this module", "HelpInfoURI", HelpInfoURI);
            WriteManifestMember(generationEnvironment, "Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.", "DefaultCommandPrefix", DefaultCommandPrefix);
            generationEnvironment.AppendLine();
            WriteManifestCommentedLines(generationEnvironment, "Private data to pass to the module specified in RootModule/ModuleToProcess");
            if (String.IsNullOrWhiteSpace(PrivateData))
                WriteManifestCommentedLines(generationEnvironment, "PrivateData = ''");
            generationEnvironment.Append("PrivateData = ");
            generationEnvironment.AppendLine(PrivateData.Trim());
            generationEnvironment.Append("}");
        }

        public const string xmlns_msh = "http://msh";
        public const string xmlns_xml = "http://www.w3.org/XML/1998/namespace";
        public const string ElementName_helpItems = "helpItems";

        public void WriteHelpItems(XmlWriter writer)
        {
            writer.WriteStartElement(ElementName_helpItems, xmlns_msh);
            writer.WriteAttributeString("schema", "maml");
            writer.WriteAttributeString("xmlns", "maml", xmlns_xml, CommandItem.xmlns_maml);
            writer.WriteAttributeString("xmlns", "command", xmlns_xml, CommandItem.xmlns_command);
            writer.WriteAttributeString("xmlns", "dev", xmlns_xml, CommandItem.xmlns_dev);
            writer.WriteAttributeString("xmlns", "MSHelp", xmlns_xml, CommandItem.xmlns_MSHelp);
            if (CmdletsToExport != null)
            {
                foreach (CommandItem cmd in CmdletsToExport.Where(c => c != null))
                    cmd.WriteCommand(writer);
            }
            if (FunctionsToExport != null)
            {
                foreach (CommandItem cmd in FunctionsToExport.Where(c => c != null))
                    cmd.WriteCommand(writer);
            }
            writer.WriteEndElement();
        }

        [XmlIgnore]
        public string Name { get; set;  }

        [XmlIgnore]
        public Version ModuleVersion { get; set; }

        [XmlIgnore]
        public Guid? ModuleGuid { get; set; }
        
        [XmlAttribute]
        public string Author { get; set; }

        [XmlIgnore]
        public string CompanyName { get; set; }

        [XmlIgnore]
        public string Description { get; set;  }

        [XmlIgnore]
        public string Copyright { get; set; }

        [XmlAttribute]
        public string PowerShellVersion { get; set; }

        [XmlAttribute]
        public string PowerShellHostName { get; set; }

        [XmlAttribute]
        public string PowerShellHostVersion { get; set; }

        [XmlAttribute]
        public Version DotNetFrameworkVersion { get; set; }

        [XmlAttribute]
        public Version CLRVersion { get; set; }

        [XmlAttribute]
        public string ProcessorArchitecture { get; set; }

        [XmlElement("RequiredModule")]
        public List<ModuleReference> RequiredModules { get; set; }

        [XmlElement("RequiredAssembly")]
        public List<string> RequiredAssemblies { get; set; }

        [XmlElement("ScriptToProcess")]
        public List<string> ScriptsToProcess { get; set; }

        [XmlElement("TypeToProcess")]
        public List<string> TypesToProcess { get; set; }

        [XmlElement("ScriptToProcess")]
        public List<string> FormatsToProcess { get; set; }

        [XmlElement("NestedModule")]
        public List<string> NestedModules { get; set; }

        [XmlElement("FunctionToExport")]
        public List<CommandItem> FunctionsToExport { get; set; }

        [XmlElement("CmdletToExport")]
        public List<CommandItem> CmdletsToExport { get; set; }

        [XmlElement("VariableToExport")]
        public List<string> VariablesToExport { get; set; }

        [XmlElement("AliasToExport")]
        public List<string> AliasesToExport { get; set; }

        [XmlElement("Module")]
        public List<ModuleReference> ModuleList { get; set; }

        [XmlElement("File")]
        public List<string> FileList { get; set; }

        [XmlElement("PrivateData")]
        public string PrivateData { get; set; }

        [XmlAttribute]
        public string HelpInfoURI { get; set; }

        [XmlAttribute]
        public string DefaultCommandPrefix { get; set; }
    }
}
