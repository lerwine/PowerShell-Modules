using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.PsHelp
{
    public static partial class command
    {
        public const string Xmlns_Prefix = "command";
        public const string Xmlns_RootElementName = "command";
        public const string Xmlns_URI = "http://schemas.microsoft.com/maml/dev/command/2004/10";
        public const string ElementName_details = "details";
        public const string ElementName_name = "name";
        public const string ElementName_synonyms = "synonyms";
        public const string ElementName_verb = "verb";
        public const string ElementName_noun = "noun";
        public const string ElementName_vendor = "vendor";
        public const string ElementName_syntax = "syntax";
        public const string ElementName_parameters = "parameters";
        public const string ElementName_inputTypes = "inputTypes";
        public const string ElementName_returnValues = "returnValues";
        public const string ElementName_terminatingErrors = "terminatingErrors";
        public const string ElementName_nonTerminatingErrors = "nonTerminatingErrors";
        public const string ElementName_examples = "examples";

        public static IEnumerable<XmlElement> GetCommandElements(this XmlElement xmlElement)
        {
            return xmlElement.GetChildElements(Xmlns_RootElementName, Xmlns_URI);
        }

        public static XmlElement GetCommandElement(this XmlElement xmlElement, string name)
        {
            return xmlElement.GetChildElements(Xmlns_RootElementName, Xmlns_URI)
                .FirstOrDefault(e => String.Equals(GetCommandName(e), name, StringComparison.InvariantCultureIgnoreCase));
        }

        internal static string GetCommandName(XmlElement element)
        {
            XmlElement nameElement = element.GetChildElements(ElementName_details, Xmlns_URI).GetChildElements(ElementName_name, Xmlns_URI)
                .FirstOrDefault();
            if (nameElement == null || nameElement.IsEmpty)
                return null;

            return nameElement.InnerText.Trim();

        }

        public static XmlElement AddCommandElement(this XmlNode node, string name, XmlNode insertBefore = null)
        {
            return node.AddElement(Xmlns_Prefix, name, Xmlns_URI, insertBefore);
        }

        internal static void UpdateHelp(TestContext testContext, XmlElement helpItemsElement, PSModuleInfo moduleInfo, string[] relativeModulePaths)
        {
            IEnumerable<XmlElement> allElements = helpItemsElement.GetCommandElements();
            foreach (string commandName in moduleInfo.ExportedCmdlets.Keys)
            {
                CmdletInfo cmdletInfo = moduleInfo.ExportedCmdlets[commandName];
                IEnumerable<XmlElement> matching = allElements.Where(e => String.Equals(GetCommandName(e), cmdletInfo.Name, StringComparison.InvariantCultureIgnoreCase));
                XmlElement commandElement = matching.FirstOrDefault();
                if (commandElement != null)
                {
                    allElements = allElements.Where(e => !String.Equals(GetCommandName(e), cmdletInfo.Name, StringComparison.InvariantCultureIgnoreCase));
                    foreach (XmlElement element in matching.Skip(1).ToArray())
                        helpItemsElement.RemoveChild(element);
                }
                else
                    commandElement = helpItemsElement.AddCommandElement(Xmlns_RootElementName);
                
                UpdateHelp(cmdletInfo, PowerShellHelper.GetCommandHelp(testContext, cmdletInfo.Name, cmdletInfo.Module, relativeModulePaths), commandElement);
            }

            foreach (XmlElement element in allElements.ToArray())
                helpItemsElement.RemoveChild(element);
        }

        private static void UpdateHelp(CmdletInfo cmdletInfo, PSObject cmdHelpObj, XmlElement commandElement)
        {
            UpdateDetails(cmdletInfo, cmdHelpObj, (new Func<XmlElement>(() =>
            {
                XmlElement detailsElement = commandElement.GetChildElements(ElementName_details, Xmlns_URI).FirstOrDefault();
                if (detailsElement == null)
                    return commandElement.AddCommandElement(ElementName_details, (commandElement.IsEmpty) ? null : commandElement.FirstChild);
                return detailsElement;
            }))());


            XmlElement descriptionElement = maml.GetDescriptionElement(commandElement);
            XmlElement syntaxElement = commandElement.GetChildElements(ElementName_syntax, Xmlns_URI).FirstOrDefault();
            XmlElement parametersElement = commandElement.GetChildElements(ElementName_parameters, Xmlns_URI).FirstOrDefault();
            XmlElement inputTypesElement = commandElement.GetChildElements(ElementName_inputTypes, Xmlns_URI).FirstOrDefault();
            XmlElement returnValuesElement = commandElement.GetChildElements(ElementName_returnValues, Xmlns_URI).FirstOrDefault();
            XmlElement terminatingErrorsElement = commandElement.GetChildElements(ElementName_terminatingErrors, Xmlns_URI).FirstOrDefault();
            XmlElement nonTerminatingErrorsElement = commandElement.GetChildElements(ElementName_nonTerminatingErrors, Xmlns_URI).FirstOrDefault();
            XmlElement alertSetElement = commandElement.GetChildElements(maml.ElementName_alertSet, maml.Xmlns_URI).FirstOrDefault();
            XmlElement examplesElement = commandElement.GetChildElements(ElementName_examples, Xmlns_URI).FirstOrDefault();
            XmlElement relatedLinksElement = commandElement.GetChildElements(maml.ElementName_relatedLinks, maml.Xmlns_URI).FirstOrDefault();

            if (descriptionElement == null)
            {
                XmlElement following = syntaxElement;
                if (following == null && (following = parametersElement) == null && (following = inputTypesElement) == null && (following = returnValuesElement) == null &&
                    (following = terminatingErrorsElement) == null && (following = nonTerminatingErrorsElement) == null && (following = alertSetElement) == null && 
                    (following = examplesElement) == null)
                    following = relatedLinksElement;
                descriptionElement = commandElement.AddMamlElement(maml.ElementName_description, following);
            }

            if (!descriptionElement.HasText(true))
                maml.SetEmptyMaml(descriptionElement, "Detailed description goes here.");

            if (syntaxElement == null)
            {
                XmlElement following = parametersElement;
                if (following == null && (following = inputTypesElement) == null && (following = returnValuesElement) == null && (following = terminatingErrorsElement) == null &&
                    (following = nonTerminatingErrorsElement) == null && (following = alertSetElement) == null && (following = examplesElement) == null)
                    following = relatedLinksElement;
                syntaxElement = commandElement.AddCommandElement(ElementName_syntax, following);
            }
            syntaxItem.UpdateSyntax(cmdletInfo, cmdHelpObj, syntaxElement);

            if (parametersElement == null)
            {
                XmlElement following = inputTypesElement;
                if (following == null && (following = returnValuesElement) == null && (following = terminatingErrorsElement) == null &&
                    (following = nonTerminatingErrorsElement) == null && (following = alertSetElement) == null && (following = examplesElement) == null)
                    following = relatedLinksElement;
                parametersElement = commandElement.AddCommandElement(ElementName_parameters, following);
            }

            parameter.UpdateParameters(cmdletInfo, cmdHelpObj, parametersElement);

            if (inputTypesElement == null)
            {
                XmlElement following = returnValuesElement;
                if (following == null && (following = terminatingErrorsElement) == null && (following = nonTerminatingErrorsElement) == null && (following = alertSetElement) == null &&
                    (following = examplesElement) == null)
                    following = relatedLinksElement;
                inputTypesElement = commandElement.AddCommandElement(ElementName_inputTypes, following);
            }

            if (returnValuesElement == null)
            {
                XmlElement following = terminatingErrorsElement;
                if (following == null && (following = nonTerminatingErrorsElement) == null && (following = alertSetElement) == null && (following = examplesElement) == null)
                    following = relatedLinksElement;
                returnValuesElement = commandElement.AddCommandElement(ElementName_returnValues, following);
            }

            if (terminatingErrorsElement == null)
            {
                XmlElement following = nonTerminatingErrorsElement;
                if (following == null && (following = alertSetElement) == null && (following = examplesElement) == null)
                    following = relatedLinksElement;
                terminatingErrorsElement = commandElement.AddCommandElement(ElementName_terminatingErrors, following);
            }

            if (nonTerminatingErrorsElement == null)
            {
                XmlElement following = alertSetElement;
                if (following == null && (following = examplesElement) == null)
                    following = relatedLinksElement;
                nonTerminatingErrorsElement = commandElement.AddCommandElement(ElementName_nonTerminatingErrors, following);
            }

            if (alertSetElement == null)
            {
                XmlElement following = examplesElement;
                if (following == null)
                    following = relatedLinksElement;
                alertSetElement = commandElement.AddMamlElement(maml.ElementName_alertSet, following);
            }

            if (examplesElement == null)
            {
                XmlElement following = examplesElement;
                if (following == null)
                    following = relatedLinksElement;
                examplesElement = commandElement.AddCommandElement(ElementName_examples, relatedLinksElement);
            }

            if (relatedLinksElement == null)
                relatedLinksElement = commandElement.AddMamlElement(maml.ElementName_relatedLinks);
        }

        private static void UpdateDetails(CmdletInfo cmdletInfo, PSObject cmdHelpObj, XmlElement detailsElement)
        {
            XmlElement element = detailsElement.GetChildElements(ElementName_name, Xmlns_URI).FirstOrDefault();

            XmlElement descriptionElement = maml.GetDescriptionElement(detailsElement);
            XmlElement copyrightElement = maml.GetCopyrightElement(detailsElement);
            XmlElement synonymsElement = detailsElement.GetChildElements(ElementName_synonyms, Xmlns_URI).FirstOrDefault();
            XmlElement verbElement = detailsElement.GetChildElements(ElementName_verb, Xmlns_URI).FirstOrDefault();
            XmlElement nounElement = detailsElement.GetChildElements(ElementName_noun, Xmlns_URI).FirstOrDefault();
            XmlElement versionElement = detailsElement.GetChildElements(dev.ElementName_version, dev.Xmlns_URI).FirstOrDefault();
            XmlElement vendorElement = detailsElement.GetChildElements(ElementName_vendor, Xmlns_URI).FirstOrDefault();

            if (element == null)
            {
                XmlElement following = descriptionElement;
                if (following == null && (following = copyrightElement) == null && (following = synonymsElement) == null && (following = verbElement) == null && (following = nounElement) == null && (following = versionElement) == null)
                    following = vendorElement;
                element = detailsElement.AddCommandElement(ElementName_name, following);
            }
            element.InnerText = cmdletInfo.Name;

            if (descriptionElement == null)
            {
                XmlElement following = copyrightElement;
                if (following == null && (following = synonymsElement) == null && (following = verbElement) == null && (following = nounElement) == null && (following = versionElement) == null)
                    following = vendorElement;
                descriptionElement = detailsElement.AddMamlElement(maml.ElementName_description, following);
            }

            if (!descriptionElement.HasText(true))
                maml.SetEmptyMaml(descriptionElement, "Synopsis goes here.");

            if (copyrightElement == null)
            {
                XmlElement following = synonymsElement;
                if (following == null && (following = verbElement) == null && (following = nounElement) == null && (following = versionElement) == null)
                    following = vendorElement;
                copyrightElement = detailsElement.AddMamlElement(maml.ElementName_copyright, following);
            }

            if (!copyrightElement.HasText(true))
                maml.SetEmptyMaml(copyrightElement, "Copyright goes here.");

            if (verbElement == null)
            {
                XmlElement following = nounElement;
                if (following == null && (following = versionElement) == null)
                    following = vendorElement;
                verbElement = detailsElement.AddCommandElement(ElementName_verb, following);
            }
            verbElement.InnerText = cmdletInfo.Verb;

            if (nounElement == null)
            {
                XmlElement following = versionElement;
                if (following == null && (following = versionElement) == null)
                    following = vendorElement;
                nounElement = detailsElement.AddCommandElement(ElementName_noun, following);
            }
            nounElement.InnerText = cmdletInfo.Noun;

            if (cmdletInfo.Module.Version != null)
            {
                if (versionElement == null)
                    versionElement = detailsElement.AddDevElement(dev.ElementName_version, vendorElement);
                if (cmdletInfo.Module.Version.Revision < 1)
                    versionElement.InnerText = cmdletInfo.Module.Version.ToString((cmdletInfo.Module.Version.Build < 1) ? 2 : 3);
                else
                    versionElement.InnerText = cmdletInfo.Module.Version.ToString();
            }
        }
    }
}