using System;
using System.Linq;
using System.Xml.Linq;

namespace LteDev.HelpXml
{
    public class PSHelpFactoryBase
    {
        public PSHelpFactory PSHelp { get; private set; }

        protected PSHelpFactoryBase(PSHelpFactory pSHelpFactory)
        {
            PSHelp = pSHelpFactory;
        }

        public static void EnsureContent(XElement element, Func<XNode> getDefaultContent)
        {
            XNode node;
            if (!element.GetTextNodeValues().Any(s => s.Trim().Length > 0) && !element.GetCommentNodeValues().Any(s => s.Trim().Length > 0))
                element.Add((getDefaultContent == null || (node = getDefaultContent()) == null) ? new XComment("Enter text here.") : node);
        }
    }
}