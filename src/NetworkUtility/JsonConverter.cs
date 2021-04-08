using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Collections;

namespace NetworkUtility
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonConverter : JavaScriptConverter
    {
        /// <summary>
        /// 
        /// </summary>
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(JsonText), typeof(JsonArray), typeof(JsonDictionary) })); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            JsonValue value = obj as JsonValue;
            if (value == null)
                return new Dictionary<string, object>();
            
            return value.Serialize(serializer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="type"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");
            
            if (type == typeof(JsonDictionary))
            {
                JsonDictionary jsonDictionary = new JsonDictionary();
                jsonDictionary.Deserialize(dictionary, serializer);
                return jsonDictionary;
            }
            
            if (type == typeof(JsonArray))
            {
                JsonArray jsonArray = new JsonArray();
                jsonArray.Deserialize(dictionary, serializer);
                return jsonArray;
            }
            
            if (type == typeof(JsonText))
            {
                JsonText jsonText = new JsonText();
                jsonText.Deserialize(dictionary, serializer);
                return jsonText;
            }
            
            if (type == typeof(ListItemCollection))
            {
                // Create the instance to deserialize into.
                ListItemCollection list = new ListItemCollection();

                // Deserialize the ListItemCollection's items.
                ArrayList itemsList = (ArrayList)dictionary["List"];
                for (int i=0; i<itemsList.Count; i++)
                    list.Add(serializer.ConvertToType<ListItem>(itemsList[i]));

                return list;
            }
            return null;
        }
    }
}
