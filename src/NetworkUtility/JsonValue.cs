using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace NetworkUtility
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class JsonValue
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract string GetElementName();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="serializer"></param>
        public virtual void Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            string name = GetElementName();
            if (dictionary != null && dictionary.ContainsKey(name))
                OnDeserialize(dictionary[name], serializer);
            else
                OnDeserialize(null, serializer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serializer"></param>
        protected virtual void OnDeserialize(object obj, JavaScriptSerializer serializer) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        protected internal abstract object AsSerializedValue(JavaScriptSerializer serializer);
    }
}
