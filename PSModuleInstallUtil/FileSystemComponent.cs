using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace PSModuleInstallUtil
{
    public class FileSystemComponent : IComponent
    {
        private ISite _site = null;

        protected FileSystemComponent(string name, IContainer parent) { Initialize(new NestedComponentSite(parent, this, name)); }

        protected FileSystemComponent(string name) : this(name, null) { }

        protected FileSystemComponent(IContainer parent) : this(null, parent) { }

        protected FileSystemComponent() { Initialize(null); }

        protected virtual void Initialize(ISite site) { Site = site; }

        protected ISite Site
        {
            get { return _site; }
            set
            {
                ISite oldSite = _site;
                if (value != null)
                {
                    if (value.Component == null || !ReferenceEquals(value.Component, this))
                        throw new InvalidOperationException("Invalid Site object.");
                    if (!FileDirectoryComponent.Contains(value.Container, this, false))
                    {
                        try
                        {
                            _site = value;
                            value.Container.Add(this);
                        }
                        catch
                        {
                            _site = oldSite;
                            throw;
                        }
                    }
                }
                else
                    _site = value;

                if (oldSite == null)
                    return;
                IContainer container = oldSite.Container;
                if (FileDirectoryComponent.Contains(container, this, false))
                    container.Remove(this);
            }
        }

        ISite IComponent.Site
        {
            get { return _site; }
            set { Site = value; }
        }

        public IComponent Parent
        {
            get
            {
                ISite site = Site;
                IContainer container;
                if (site == null || (container = site.Container) == null)
                    return null;

                if (container is IComponent)
                    return container as IComponent;

                return null;
            }
        }

        public static IComponent GetParent(IComponent child)
        {
            if (child == null)
                return null;

            if (child is INestedContainer)
            {
                IComponent owner = (child as INestedContainer).Owner;
                if (owner != null && !ReferenceEquals(child, owner))
                    return owner;
            }

            ISite site = child.Site;
            if (site == null)
                return null;

            IContainer container = site.Container;
            if (container == null)
                return null;

            if (container is IComponent)
                return container as IComponent;

            if (container is NestedContainer)
                return (child as INestedContainer).Owner;

            return null;
        }

        public static IEnumerable<IComponent> GetAncestors(IContainer container, bool includeCurrent)
        {
            if (container == null)
                return new IComponent[0];

            if (container is IComponent)
                return GetAncestors(container as IComponent, includeCurrent);

            if (container is INestedContainer)
                return GetAncestors((container as INestedContainer).Owner, true);

            return new IComponent[0];
        }

        public static IEnumerable<IComponent> GetAncestors(IComponent component, bool includeCurrent)
        {
            if (component == null)
                yield break;

            for (IComponent c = (includeCurrent) ? component : GetParent(component); component != null; component = GetParent(component))
                yield return c;
        }

        public static Uri ToUri(IComponent component) { return ToUri(GetAncestors(component, true)); }

        public static Uri ToUri(IContainer container) { return ToUri(GetAncestors(container, true)); }
        
        private static Uri ToUri(IEnumerable<IComponent> items)
        {
            string[] names = items.Select(i => i.Site).Select(s => (s == null) ? null : s.Name).ToArray();
            if (names.Length == 0 || names.Any(s => String.IsNullOrEmpty(s)))
                throw new InvalidOperationException("One or more items in path have no name.");

            if (names.Length == 1)
                return new Uri("file://" + Uri.EscapeUriString(names[0]));

            for (int i = 1; i < names.Length; i++)
                names[i] = Uri.EscapeDataString(names[i]);
            int l = names[0].Length - 1;
            char lastC = names[0][l];
            if (lastC == Path.DirectorySeparatorChar || lastC == Path.AltDirectorySeparatorChar)
            {
                if (l == 0)
                    names = names.Skip(1).ToArray();
                else
                {
                    names[0] = Uri.EscapeUriString(names[0].Substring(0, l));
                }
            }
            else
                names[0] = Uri.EscapeUriString(names[0]);
            return new Uri("file://" + String.Join("/", names));
        }

        public Uri ToUri() { return ToUri(this); }

        public static IComponent Find(IComponent component, Uri uri)
        {
            if (component == null || uri == null)
                return null;

            IComponent root = GetAncestors(component, true).LastOrDefault();
            if (root == null)
                return null;

            Uri rootUri = ToUri(root);

            throw new NotImplementedException();
        }

        public static IComponent Find(IContainer component, Uri uri)
        {
            throw new NotImplementedException();
        }

        public event EventHandler Disposed;

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue || !disposing)
                return;

            EventHandler disposed = Disposed;
            disposedValue = true;
            if (disposed != null)
                disposed.Invoke(this, EventArgs.Empty);
        }

        ~FileSystemComponent()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public class NestedComponentSite : ISite
        {
            private string _name;

            public NestedComponentSite(IContainer container, IComponent component) : this(container, component, null) { }

            public NestedComponentSite(IContainer container, IComponent component, string name)
            {
                if (component == null)
                    throw new ArgumentNullException("component");

                Container = container;
                Component = component;
                _name = name;
            }

            public IComponent Component { get; private set; }

            public IContainer Container { get; private set; }

            bool ISite.DesignMode { get { return false; } }

            public string Name
            {
                get { return _name; }
                set
                {
                    if (value == null || Container == null || (_name != null && String.Compare(_name, value, true) == 0))
                    {
                        _name = value;
                        return;
                    }

                    if (FileDirectoryComponent.EnumerateNames(Container).Any(n => String.Compare(n, value, true) == 0))
                        throw new InvalidOperationException("Another item with the same name already exists.");
                    _name = value;
                }
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == null)
                    return null;

                if (serviceType.IsInstanceOfType(this))
                    return this;

                if ((typeof(IContainer)).IsAssignableFrom(serviceType) || (Container != null && serviceType.IsInstanceOfType(Container)))
                    return Container;

                if (serviceType.IsInstanceOfType(Component))
                    return Component;

                return null;
            }
        }
    }
}