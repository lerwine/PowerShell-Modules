using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using System.Collections;

namespace NetworkUtilityCLR
{
    public class JsonText : JSonValue
    {
        public const string ElementName = "String";
        public override string GetElementName() { return ElementName; }
        public string Value { get; set; }
        protected override object AsSerializedValue(JavaScriptSerializer serializer) { return Value }
    }
}
