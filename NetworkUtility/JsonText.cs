using System;
using System.Web.Script.Serialization;

namespace NetworkUtility
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonText : JsonValue
    {
        private string _value = "";

        /// <summary>
        /// 
        /// </summary>
        public JsonText() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public JsonText(string value) { Value = value; }

        /// <summary>
        /// 
        /// </summary>
        public const string ElementName = "String";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetElementName() { return ElementName; }

        /// <summary>
        /// 
        /// </summary>
        public string Value { get { return _value; } set { _value = (value == null) ? "" : value; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        protected internal override object AsSerializedValue(JavaScriptSerializer serializer) { return Value; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serializer"></param>
        protected override void OnDeserialize(object obj, JavaScriptSerializer serializer) { Value = obj as string; }
    }
}
