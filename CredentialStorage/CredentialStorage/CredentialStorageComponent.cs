using System;
using System.ComponentModel;

namespace CredentialStorageCLR
{
    public abstract class CredentialStorageComponent : ICredentialComponent
    {
        public const string NamespaceURI = "urn:Erwine.Leonard.T:PowerShell:CredentialStorage.psm1:CredentialStorage.xsd";

        public event EventHandler Disposed;

        private bool _isDisposed = false;
        private ICredentialSite _site = null;

        protected virtual ICredentialSite Site
        {
            get { return this._site; }
            set
            {
                if (value == null)
                {
                    if (this._site == null)
                        return;
                }
                else
                {
                    if (this._site != null && Object.ReferenceEquals(this._site, value))
                        return;

                    if (value.Component == null || !Object.ReferenceEquals(value.Component, this))
                        throw new InvalidOperationException("Component not set to this item.");
                    if (value.Container == null)
                        throw new InvalidOperationException("Container is cannot be null.");
                    if (Object.ReferenceEquals(value.Container, this._site.Container))
                    {
                        this._site = value;
                        return;
                    }
                }

                ICredentialSite oldSite = this._site;
                this._site = value;
                this.OnSiteChanged(oldSite, value);
            }
        }

        protected abstract void OnSiteChanged(ICredentialSite oldSite, ICredentialSite value);

        ICredentialSite ICredentialComponent.Site
        {
            get { return this.Site; }
            set { this.Site = (ICredentialSite)value; }
        }

        ISite IComponent.Site
        {
            get { return this.Site; }
            set { this.Site = (ICredentialSite)value; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._isDisposed)
                this._isDisposed = true;
        }

        ~CredentialStorageComponent()
        {
            this.Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual bool Equals(ICredentialComponent other)
        {
            return other != null && Object.ReferenceEquals(this, other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        bool IEquatable<ICredentialComponent>.Equals(ICredentialComponent other) { return this.Equals(other); }

        public override bool Equals(object obj) { return this.Equals(obj as ICredentialComponent); }

        protected abstract ICredentialComponent BaseClone();

        ICredentialComponent ICredentialComponent.Clone() { return this.BaseClone(); }

        object ICloneable.Clone() { return this.BaseClone(); }
    }

    public abstract class CredentialStorageComponent<T> : CredentialStorageComponent, ICredentialComponent<T>
        where T : CredentialStorageComponent<T>
    {
        protected override ICredentialSite Site
        {
            get { return base.Site; }
            set { base.Site = (ICredentialSite<T>)value; }
        }

        ICredentialSite<T> ICredentialComponent<T>.Site
        {
            get { return (ICredentialSite < T > )(base.Site); }
            set { base.Site = value; }
        }

        public virtual bool Equals(ICredentialComponent<T> other)
        {
            return other != null && Object.ReferenceEquals(this, other);
        }

        protected override bool Equals(ICredentialComponent other) { return this.Equals(other as ICredentialComponent<T>); }

        protected abstract void OnSiteChanged(ICredentialSite<T> oldSite, ICredentialSite<T> value);

        protected override void OnSiteChanged(ICredentialSite oldSite, ICredentialSite value)
        {
            this.OnSiteChanged((ICredentialSite<T>)oldSite, (ICredentialSite<T>)value);
        }

        protected abstract T Clone();

        protected override ICredentialComponent BaseClone() { return this.Clone(); }

        T ICredentialComponent<T>.Clone() { return this.Clone(); }
    }
}