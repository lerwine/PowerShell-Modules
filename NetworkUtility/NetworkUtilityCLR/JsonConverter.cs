using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using System.Collections;

namespace NetworkUtilityCLR
{
    public class JsonConverter : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(JsonText), typeof(JsonArray), typeof(JsonDictionary) })); }
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            JSonValue value = obj as JSonValue;
            if (value == null)
                return new Dictionary<string, object>();
            
            return value.Serialize(serializer);
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");
            
            if (type == typeof(JsonDictionary))
            
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
