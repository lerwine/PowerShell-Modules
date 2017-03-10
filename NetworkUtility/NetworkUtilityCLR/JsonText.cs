using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Collections;

namespace NetworkUtilityCLR
{
    public class JsonText : JSonValue
    {
		public JsonText() { }
		public JsonText(string value) { Value = value; }
        public const string ElementName = "String";
        public override string GetElementName() { return ElementName; }
        public string Value { get; set; }
        protected internal override object AsSerializedValue(JavaScriptSerializer serializer) { return Value; }
		protected override void OnDeserialize(object obj, JavaScriptSerializer serializer) { Value = obj as string; }
    }
}
