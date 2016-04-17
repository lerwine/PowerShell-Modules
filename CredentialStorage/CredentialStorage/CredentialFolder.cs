using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CredentialStorageCLR
{
    [Serializable]
    [XmlRoot(ElementName = CredentialFolder.RootElementName, IsNullable = false, Namespace = CredentialFolder.NamespaceURI)]
    public class CredentialFolder : CredentialStorageContainer<CredentialFolder>, ICredentialContent<CredentialFolder>, INestedCredentialContainer<CredentialFolder>
    {
        public const string RootElementName = "Folder";

        private Guid? _id = null;
        private string _name = "";
        private string _normalizedName = null;

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

        [XmlIgnore]
        public ICredentialContainer Owner
        {
            get
            {
                if (this.Site == null)
                    return null;
                if (this.Site.Container is INestedCredentialContainer)
                    return (this.Site.Container as INestedCredentialContainer).Owner;
                return this.Site.Container;
            }
        }

        IComponent INestedContainer.Owner { get { return this.Owner; } }

        public CredentialFolder() : base() { }

        public CredentialFolder(CredentialFolder folder) : base(folder)
        {
            this._name = folder._name;
        }

        public CredentialFolder(IEnumerable<ICredentialContent> collection) : base(collection) { }

        public ICredentialContent<CredentialFolder> EnsureNestable(ICredentialContainer parent)
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

            this.Components.EnsureNestable(parent);

            return this;
        }

        public bool Equals(ICredentialContent<CredentialFolder> other)
        {
            if (other == null)
                return false;

            if (Object.ReferenceEquals(this, other) || this.Id.Equals(other.Id))
                return true;

            return (other is ICredentialContainer<CredentialFolder> && this.Components.Equals(((ICredentialContainer<CredentialFolder>)other).Components));
        }

        bool IEquatable<ICredentialContent>.Equals(ICredentialContent other)
        {
            return this.Equals(other as ICredentialContent<CredentialFolder>);
        }

        public override bool Equals(ICredentialContainer<CredentialFolder> other) { return this.Equals(other as ICredentialContent<CredentialFolder>); }

        public override int GetHashCode() { return this.Id.GetHashCode(); }

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

        protected override void OnSiteChanged(ICredentialSite<CredentialFolder> oldSite, ICredentialSite<CredentialFolder> newSite)
        {
            if (oldSite != null && oldSite.Container.Contains(this))
                oldSite.Container.Remove(this);
            if (newSite != null && !newSite.Container.Contains(this))
                newSite.Container.Add(this);
        }
        
        protected override CredentialFolder Clone() { return new CredentialFolder(this); }

        CredentialFolder ICredentialContent<CredentialFolder>.Clone() { return this.Clone(); }

        ICredentialContent ICredentialContent.EnsureNestable(ICredentialContainer parent) { return this.Clone(); }

        ICredentialContent ICredentialContent.Clone() { return this.Clone(); }

        private sealed class ComponentSite : ICredentialSite<CredentialFolder>
        {
            private ICredentialContainer _container;
            private CredentialFolder _component;

            public ComponentSite(ICredentialContainer container, CredentialFolder component)
            {
                if (container == null)
                    throw new ArgumentNullException("container");

                if (component == null)
                    throw new ArgumentNullException("component");

                this._container = container;
                this._component = component;
            }

            public CredentialFolder Component { get { return this._component; } }

            public ICredentialContainer Container { get { return this._container; } }

            bool ISite.DesignMode { get { return false; } }

            public string Name
            {
                get { return this.Component.NormalizedName; }
                set { this.Component.Name = value; }
            }

            ICredentialComponent<CredentialFolder> ICredentialSite<CredentialFolder>.Component { get { return this.Component; } }

            ICredentialContent ICredentialSite.Component { get { return this.Component; } }

            IComponent ISite.Component { get { return this.Component; } }

            IContainer ISite.Container { get { return this.Container; } }

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
