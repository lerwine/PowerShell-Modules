using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace LteDevClr.ModuleGeneration
{
    [Serializable]
    public class CommandItem
    {
        public const string xmlns_maml = "http://schemas.microsoft.com/maml/2004/10";
        public const string xmlns_command = "http://schemas.microsoft.com/maml/dev/command/2004/10";
        public const string xmlns_dev = "http://schemas.microsoft.com/maml/dev/2004/10";
        public const string xmlns_MSHelp = "http://msdn.microsoft.com/mshelp";
        public const string ElementName_command = "command";
        public const string ElementName_details = "details";
        public const string ElementName_name = "name";
        public const string ElementName_description = "description";
        public const string ElementName_para = "para";
        public const string ElementName_copyright = "";
        public const string ElementName_verb = "verb";
        public const string ElementName_noun = "noun";
        public const string ElementName_version = "version";
        public const string ElementName_syntax = "syntax";
        public const string ElementName_syntaxItem = "syntaxItem";
        public const string ElementName_parameter = "parameter";
        public const string ElementName_parameters = "parameters";
        public const string ElementName_inputTypes = "inputTypes";
        public const string ElementName_returnValues = "returnValues";

        [XmlIgnore]
        public string Name
        {
            get
            {
                if (String.IsNullOrWhiteSpace(Noun))
                {
                    if (String.IsNullOrWhiteSpace(Verb))
                        return null;
                    return Verb.Trim();
                }
                if (String.IsNullOrWhiteSpace(Verb))
                    return Noun.Trim();
                return Verb.Trim() + "-" + Noun.Trim();
            }
        }

        [XmlAttribute]
        public string Verb { get; set; }

        [XmlAttribute]
        public string Noun { get; set; }

        public string Synopsis { get; set; }

        public string Description { get; set; }

        [XmlElement("Link")]
        public List<string> Links { get; set; }

        [XmlElement("OutputType")]
        public List<OutputTypeSpec> OutputTypes { get; set; }

        [XmlElement("Parameter")]
        public List<ParameterSpec> Parameters { get; set; }

        public const string ElementName_ = "";

        public static IEnumerable<string> SplitParagraphs(string text)
        {
            if (text == null || (text = text.Trim()).Length == 0)
                yield break;

            IEnumerable<string> lines = RootModule.NewLineRegex.Split(text).Select(s => s.TrimEnd());
            List<string> paragraph = new List<string>();
            foreach (string s in lines)
            {
                if (s.Length == 0)
                {
                    if (paragraph.Count > 0)
                    {
                        yield return String.Join(Environment.NewLine, paragraph.ToArray());
                        paragraph.Clear();
                    }
                }
                else
                    paragraph.Add(s);
            }

            if (paragraph.Count > 0)
                yield return String.Join(Environment.NewLine, paragraph.ToArray());
        }

        public void WriteCommand(RootModule rootModule, XmlWriter writer)
        {
            if (String.IsNullOrWhiteSpace(Verb) || String.IsNullOrWhiteSpace(Noun))
                return;

            writer.WriteStartElement(ElementName_command, xmlns_command);
            writer.WriteElementString(ElementName_name, xmlns_command, Verb.Trim() + "-" + Noun.Trim());

            writer.WriteStartElement(ElementName_details, xmlns_command);
            string s = Synopsis;
            if (s != null && (s = s.Trim()).Length > 0)
            {
                writer.WriteStartElement(ElementName_description, xmlns_command);
                foreach (string paragraph in SplitParagraphs(s))
                    writer.WriteElementString(ElementName_para, xmlns_maml, paragraph);
                writer.WriteEndElement();
            }

            if (!String.IsNullOrWhiteSpace(rootModule.Copyright))
            {
                writer.WriteStartElement(ElementName_copyright);
                writer.WriteElementString(ElementName_para, xmlns_maml, rootModule.Copyright.Trim());
                writer.WriteEndElement();
            }
            writer.WriteElementString(ElementName_verb, xmlns_command, Verb.Trim());
            writer.WriteElementString(ElementName_noun, xmlns_command, Noun.Trim());
            if (rootModule.ModuleVersion != null)
                writer.WriteElementString(ElementName_version, xmlns_dev, rootModule.ModuleVersion.ToString());
            writer.WriteEndElement(); // </command:details>

            s = Description;
            if (s != null && (s = s.Trim()).Length > 0)
            {
                writer.WriteStartElement(ElementName_description, xmlns_command);
                foreach (string paragraph in SplitParagraphs(s))
                    writer.WriteElementString(ElementName_para, xmlns_maml, paragraph);
                writer.WriteEndElement();
            }

            ParameterSpec[] parameters = (Parameters == null) ? new ParameterSpec[0] : Parameters.Where(p => p != null && !String.IsNullOrWhiteSpace(p.Name)).ToArray();

            writer.WriteStartElement(ElementName_syntax, xmlns_command);

            foreach (string parameterSetName in parameters.SelectMany(p => p.GetParameterSetNames()).Distinct(StringComparer.InvariantCultureIgnoreCase).DefaultIfEmpty(""))
            {
                writer.WriteElementString(ElementName_name, xmlns_maml, Verb.Trim() + "-" + Noun.Trim());
                foreach (ParameterSpec p in parameters)
                    p.WriteSyntaxItem(writer, parameterSetName);
            }
            writer.WriteEndElement();

            if (parameters != null)
            {
                writer.WriteStartElement(ElementName_parameters, xmlns_command);
                foreach (ParameterSpec p in parameters)
                    p.WriteParameter(writer);
                writer.WriteEndElement();
            }

            ParameterSpec[] filtered = parameters.Where(p => p.HasInputType()).ToArray();
            if (filtered.Length > 0)
            {
                writer.WriteStartElement(ElementName_inputTypes, xmlns_command);
                foreach (ParameterSpec p in filtered)
                    p.WriteInputTypes(writer);
                writer.WriteEndElement();
            }

            IEnumerable<OutputTypeSpec> outFiltered = (OutputTypes == null) ? new OutputTypeSpec[0] : OutputTypes.Where(o => o != null).ToArray();
            if (outFiltered.Any())
            {
                writer.WriteStartElement(ElementName_returnValues, xmlns_command);
                foreach (OutputTypeSpec t in outFiltered.GroupBy(o => (o.TypeName == null) ? "" : o.TypeName.Trim(), StringComparer.InvariantCultureIgnoreCase))
                    t.WriteReturnValue(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement(); // </command:command>
        }
    }
}