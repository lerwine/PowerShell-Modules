using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CredentialStorageCLR
{
    [Serializable]
    [XmlRoot(ElementName = CredentialItem.RootElementName, IsNullable = false, Namespace = CredentialItem.NamespaceURI)]
    public class CredentialItem : CredentialStorageComponent<CredentialItem>, ICredentialContent<CredentialItem>
    {
        public const string RootElementName = "Credential";

        private Guid? _id = null;
        private string _name = "";
        private string _normalizedName = null;
        private string _userName = null;
        private string _password = null;
        private string _location = null;

        public CredentialItem() { }

        public CredentialItem(CredentialItem credentialItem)
        {
            if (credentialItem == null)
                return;
            
            this._name = credentialItem._name;
        }

        public Guid Id
        {
            get
            {
                if (!this._id.HasValue)
                    this._id = Guid.NewGuid();

                return this._id.Value;
            }
        }

        public string Name
        {
            get { return this._name; }
            set
            {
                string s = (value == null) ? "" : value;
                if (this._name == s)
                    return;

                this._name = s;
                this._normalizedName = null;
            }
        }

        public string NormalizedName
        {
            get
            {
                if (this._normalizedName == null)
                    this._normalizedName = CredentialComponentCollection.ConvertToNormalizedName(this._name);

                return this._normalizedName;
            }
        }

        public string UserName
        {
            get { return this._userName; }
            set { this._userName = (String.IsNullOrEmpty(value)) ? null : value; }
        }

        public string Password
        {
            get { return this._password; }
            set { this._password = (String.IsNullOrEmpty(value)) ? null : value; }
        }

        public string Location
        {
            get { return this._location; }
            set { this._location = value; }
        }

        public ICredentialContent<CredentialItem> EnsureNestable(ICredentialContainer parent)
        {
            foreach (ICredentialContainer item in parent.GetParents())
            {
                if (Object.ReferenceEquals(item, this))
                    return this.Clone();
            }

            foreach (ICredentialContent item in parent.Components)
            {
                if (item is CredentialFolder && Object.ReferenceEquals(item, this))
                    return this.Clone();
            }
            
            return this;
        }

        public bool Equals(ICredentialContent<CredentialItem> other)
        {
            return other != null && (Object.ReferenceEquals(this, other) || this.Id.Equals(other.Id));
        }

        public override int GetHashCode() { return this.Id.GetHashCode(); }

        bool IEquatable<ICredentialContent>.Equals(ICredentialContent other) { return this.Equals(other as ICredentialContent<CredentialItem>); }

        public void SetContainer(ICredentialContainer parent)
        {
            if (parent == null)
                this.Site = null;
            else
            {
                if (this.Site != null && !Object.ReferenceEquals(this.Site.Container, parent))
                    this.Site = new ComponentSite(parent, this);
            }
        }

        protected override void OnSiteChanged(ICredentialSite<CredentialItem> oldSite, ICredentialSite<CredentialItem> newSite)
        {
            if (oldSite != null && oldSite.Container.Contains(this))
                oldSite.Container.Remove(this);
            if (newSite != null && !newSite.Container.Contains(this))
                newSite.Container.Add(this);
        }

        ICredentialContent ICredentialContent.EnsureNestable(ICredentialContainer parent) { return this.EnsureNestable(parent); }

        protected override CredentialItem Clone() { return new CredentialItem(this); }

        CredentialItem ICredentialContent<CredentialItem>.Clone() { return this.Clone(); }

        ICredentialContent ICredentialContent.Clone() { return this.Clone(); }

        private class ComponentSite : ICredentialSite<CredentialItem>
        {
            private ICredentialContainer _container;
            private CredentialItem _component;

            public CredentialItem Component { get { return this._component; } }

            public ICredentialContainer Container { get { return this._container; } }

            bool ISite.DesignMode { get { return false; } }

            public string Name
            {
                get { return this.Component.NormalizedName; }
                set { this.Component.Name = value; }
            }

            ICredentialComponent<CredentialItem> ICredentialSite<CredentialItem>.Component { get { return this.Component; } }

            ICredentialContent ICredentialSite.Component { get { return this.Component; } }

            IComponent ISite.Component { get { return this.Component; } }

            IContainer ISite.Container { get { return this.Container; } }

            public ComponentSite(ICredentialContainer container, CredentialItem component)
            {
                if (container == null)
                    throw new ArgumentNullException("container");

                if (component == null)
                    throw new ArgumentNullException("component");

                this._container = container;
                this._component = component;
            }

            object IServiceProvider.GetService(Type serviceType)
            {
                if (serviceType.IsAssignableFrom(this.Component.GetType()))
                    return this.Component;

                if (serviceType.IsAssignableFrom(this.Container.GetType()))
                    return this.Container;

                return null;
            }
        }
    }
}
