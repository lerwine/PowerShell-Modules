using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using System.Collections;

namespace NetworkUtilityCLR
{
    public absract class JSonValue
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
        protected abstract object AsSerializedValue(JavaScriptSerializer serializer);
    }
}
