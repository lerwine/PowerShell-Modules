using System.Collections.ObjectModel;
using System.Xml.Schema;

namespace IOUtilityCLR
{
    public class SchemaValidationHandler
    {
        private object _syncRoot = new object();
        private Collection<SchemaValidationError> _errors = new Collection<SchemaValidationError>();
        private SchemaSetCollection _schemaSets = null;

        public Collection<SchemaValidationError> Errors { get { return this._errors; } }

        public SchemaSetCollection SchemaSets
        {
            get
            {
                lock (this._syncRoot)
                {
                    if (this._schemaSets == null)
                        this._schemaSets = new SchemaSetCollection(this.ValidationEventHandler);
                }

                return this._schemaSets;
            }
        }

        public SchemaValidationHandler() { }
        
        private void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            this.Errors.Add(new SchemaValidationError(sender, e));
        }
    }
}
