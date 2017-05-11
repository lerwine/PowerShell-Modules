using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Xml;
using System.Xml.Linq;

namespace Erwine.Leonard.T.WPF
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public sealed class ValidateXamlAttribute : ValidateEnumeratedArgumentsAttribute
    {
        private readonly string _primaryNamespaceURI;
        private readonly string[] _primaryElementNames;
        public string PrimaryNamespaceURI { get; set; }

        public string[] PrimaryElementNames { get; set; }

        public string NamespaceURI { get; set; }

        public string[] ElementNames { get; set; }

        public string AltNamespaceURI { get; set; }

        public string[] AltElementNames { get; set; }

        public ValidateXamlAttribute()
        {
            _primaryNamespaceURI = null;
            _primaryElementNames = null;
        }

        public ValidateXamlAttribute(string namespaceURI, params string[] elementName)
        {
            _primaryNamespaceURI = namespaceURI;
            _primaryElementNames = elementName;
        }

        public IEnumerable<string[]> GetAllNames()
        {
            string[] names;
            if (_primaryElementNames != null && (names = _primaryElementNames.Where(s => !String.IsNullOrWhiteSpace(s)).ToArray()).Length > 0)
            {
                foreach (string n in names)
                    yield return new string[] { _primaryNamespaceURI, n };
            }
            else if (_primaryNamespaceURI != null)
                yield return new string[] { _primaryNamespaceURI, null };

            if (ElementNames != null && (names = ElementNames.Where(s => !String.IsNullOrWhiteSpace(s)).ToArray()).Length > 0)
            {
                foreach (string n in names)
                    yield return new string[] { NamespaceURI, n };
            }
            else if (NamespaceURI != null)
                yield return new string[] { NamespaceURI, null };

            if (AltElementNames != null && (names = AltElementNames.Where(s => !String.IsNullOrWhiteSpace(s)).ToArray()).Length > 0)
            {
                foreach (string n in names)
                    yield return new string[] { AltNamespaceURI, n };
            }
            else if (AltNamespaceURI != null)
                yield return new string[] { AltNamespaceURI, null };
        }

        protected override void ValidateElement(object element)
        {
            throw new NotImplementedException();
        }
    }
}
