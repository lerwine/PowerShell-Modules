using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LteDev.XsdTypeGen.maml
{
    [Serializable]
    public class inline
    {
        [XmlElement("glossaryEntryLink", Namespace = Constants.Xmlns_maml, Type = typeof(glossaryEntryLink))]
        [XmlElement("parameterNameInline", Namespace = Constants.Xmlns_command, Type = typeof(command.parameterNameInline))]
        [XmlElement("parameterValueInline", Namespace = Constants.Xmlns_command, Type = typeof(command.parameterValueInline))]
        [XmlElement("acronym", Namespace = Constants.Xmlns_maml, Type = typeof(acronym))]
        [XmlElement("abbreviation", Namespace = Constants.Xmlns_maml, Type = typeof(abbreviation))]
        [XmlElement("quoteInline", Namespace = Constants.Xmlns_maml, Type = typeof(quoteInline))]
        [XmlElement("date", Namespace = Constants.Xmlns_maml, Type = typeof(date))]
        [XmlElement("foreignPhrase", Namespace = Constants.Xmlns_maml, Type = typeof(foreignPhrase))]
        [XmlElement("phrase", Namespace = Constants.Xmlns_maml, Type = typeof(phrase))]
        [XmlElement("conditionalInline", Namespace = Constants.Xmlns_maml, Type = typeof(conditionalInline))]
        [XmlElement("copyrightInline", Namespace = Constants.Xmlns_maml, Type = typeof(copyrightInline))]
        [XmlElement("corporation", Namespace = Constants.Xmlns_maml, Type = typeof(corporation))]
        [XmlElement("country", Namespace = Constants.Xmlns_maml, Type = typeof(country))]
        [XmlElement("alertInline", Namespace = Constants.Xmlns_maml, Type = typeof(alertInline))]
        [XmlElement("lineBreak", Namespace = Constants.Xmlns_maml, Type = typeof(lineBreak))]
        [XmlElement("footnote", Namespace = Constants.Xmlns_maml, Type = typeof(footnote))]
        [XmlElement("notLocalizable", Namespace = Constants.Xmlns_maml, Type = typeof(notLocalizable))]
        [XmlElement("subscript", Namespace = Constants.Xmlns_maml, Type = typeof(subscript))]
        [XmlElement("superscript", Namespace = Constants.Xmlns_maml, Type = typeof(superscript))]
        [XmlElement("embedObject", Namespace = Constants.Xmlns_maml, Type = typeof(embedObject))]
        [XmlElement("navigationLink", Namespace = Constants.Xmlns_maml, Type = typeof(navigationLink))]
        [XmlElement("shellExecuteLink", Namespace = Constants.Xmlns_maml, Type = typeof(shellExecuteLink))]
        [XmlElement("replaceable", Namespace = Constants.Xmlns_maml, Type = typeof(replaceable))]
        [XmlElement("entityInline", Namespace = Constants.Xmlns_maml, Type = typeof(entityInline))]
        [XmlElement("rangeInline", Namespace = Constants.Xmlns_maml, Type = typeof(rangeInline))]
        [XmlElement("newTerm", Namespace = Constants.Xmlns_maml, Type = typeof(newTerm))]
        [XmlElement("languageKeyword", Namespace = Constants.Xmlns_maml, Type = typeof(languageKeyword))]
        [XmlElement("math", Namespace = Constants.Xmlns_maml, Type = typeof(math))]
        [XmlElement("menuSelection", Namespace = Constants.Xmlns_maml, Type = typeof(menuSelection))]
        [XmlElement("shortcut", Namespace = Constants.Xmlns_maml, Type = typeof(shortcut))]
        [XmlElement("keyCombinationInline", Namespace = Constants.Xmlns_maml, Type = typeof(keyCombinationInline))]
        [XmlElement("userInput", Namespace = Constants.Xmlns_maml, Type = typeof(userInput))]
        [XmlElement("application", Namespace = Constants.Xmlns_maml, Type = typeof(application))]
        [XmlElement("database", Namespace = Constants.Xmlns_maml, Type = typeof(database))]
        [XmlElement("internetUri", Namespace = Constants.Xmlns_maml, Type = typeof(internetUri))]
        [XmlElement("localUri", Namespace = Constants.Xmlns_maml, Type = typeof(localUri))]
        [XmlElement("environmentVariable", Namespace = Constants.Xmlns_maml, Type = typeof(environmentVariable))]
        [XmlElement("errorInline", Namespace = Constants.Xmlns_maml, Type = typeof(errorInline))]
        [XmlElement("hardware", Namespace = Constants.Xmlns_maml, Type = typeof(hardware))]
        [XmlElement("literal", Namespace = Constants.Xmlns_maml, Type = typeof(literal))]
        [XmlElement("markup", Namespace = Constants.Xmlns_maml, Type = typeof(markup))]
        [XmlElement("commandInline", Namespace = Constants.Xmlns_maml, Type = typeof(commandInline))]
        [XmlElement("token", Namespace = Constants.Xmlns_maml, Type = typeof(token))]
        [XmlElement("codeInline", Namespace = Constants.Xmlns_maml, Type = typeof(codeInline))]
        [XmlElement("computerOutputInline", Namespace = Constants.Xmlns_maml, Type = typeof(computerOutputInline))]
        [XmlElement("prompt", Namespace = Constants.Xmlns_maml, Type = typeof(prompt))]
        [XmlElement("ui", Namespace = Constants.Xmlns_maml, Type = typeof(ui))]
        [XmlElement("registryEntryInline", Namespace = Constants.Xmlns_maml, Type = typeof(registryEntryInline))]
        [XmlText(Type = typeof(string))]
        public List<object> Contents { get; set; }
    }
}
