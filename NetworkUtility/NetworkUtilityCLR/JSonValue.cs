using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Collections;

namespace NetworkUtilityCLR
{
    public abstract class JSonValue
    {
        public abstract string GetElementName();
        public Dictionary<string, object> Serialize(JavaScriptSerializer serializer)
        {
            object value = AsSerializedValue(serializer);
            if (value == null)
                return new Dictionary<string, object>();
            if (value is Dictionary<string, object>)
                return value as Dictionary<string, object>;
                
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add(GetElementName(), value);
            return result;
        }
		public virtual void Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
		{
			string name = GetElementName();
			if (dictionary != null && dictionary.ContainsKey(name))
				OnDeserialize(dictionary[name], serializer);
			else
				OnDeserialize(null, serializer);
		}
        protected virtual void OnDeserialize(object obj, JavaScriptSerializer serializer) { }
        protected internal abstract object AsSerializedValue(JavaScriptSerializer serializer);
    }
}
