using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace PSModuleInstallUtil
{
    public class FileDirectoryComponent : FileSystemComponent, INestedContainer
    {
        private InstanceMutableComponentCollection _components;
        private IList<IComponent> _mutator;

        public static FileDirectoryComponent Create(DirectoryInfo directory)
        {
            if (directory == null)
                return null;

            return new FileDirectoryComponent(directory.Name, (directory.Parent == null) ? null : FileDirectoryComponent.Create(directory.Parent));
        }

        public FileDirectoryComponent(string name, IContainer parent) : base(name, parent) { }

        public FileDirectoryComponent(string name) : base(name) { }

        public FileDirectoryComponent(IContainer parent) : base(parent) { }

        public FileDirectoryComponent() : base() { }

        protected override void Initialize(ISite site)
        {
            _components = InstanceMutableComponentCollection.Create(out _mutator);
            base.Initialize(site);
        }

        IComponent INestedContainer.Owner { get { return Parent; } }

        public InstanceMutableComponentCollection Components { get { return _components; } }

        ComponentCollection IContainer.Components { get { return _components; } }

        public void Add(IComponent component)
        {
            if (component == null)
                throw new ArgumentNullException("component");

            if (Contains(component))
                throw new ArgumentException("Item already exists in this collection.", "component");

            ISite site = component.Site;
            string name = (site == null) ? null : site.Name;
            if (name != null && Contains(this, name))
                throw new ArgumentException("Another item with that name already exists.", "component");
            if (!ReferenceEquals(site.Component, component) || site.Container == null || !ReferenceEquals(site.Container, this))
                component.Site = new NestedComponentSite(this, component, name);
            if (!Contains(component))
            {
                component.Disposed += Component_Disposed;
                _mutator.Add(component);
            }

        }

        public void Add(IComponent component, string name)
        {
            if (component == null)
                throw new ArgumentNullException("component");

            if (Contains(component))
                throw new ArgumentException("Item already exists in this collection.", "component");

            if (name != null && Contains(this, name))
                throw new ArgumentException("Another item with that name already exists.", "component");

            ISite site = component.Site;
            string n = (site == null) ? null : site.Name;
            if (!ReferenceEquals(site.Component, component) || site.Container == null || !ReferenceEquals(site.Container, this) || ((n == null) ? name != null : (name == null || n != name)))
                component.Site = new NestedComponentSite(this, component, name);
            if (!Contains(component))
            {
                _mutator.Add(component);
                component.Disposed += Component_Disposed;
            }
        }

        public void RemoveAt(int index)
        {
            IComponent component = this.Components[index];
            if (Remove(component))
                component.Dispose();
        }

        public bool Remove(IComponent component)
        {
            if (!Contains(component))
                return false;
            ISite site = component.Site;
            string name = (site == null) ? null : site.Name;
            component.Disposed -= Component_Disposed;
            _mutator.Remove(component);
            site = component.Site;
            if (site != null && site.Container != null && ReferenceEquals(site.Container, this))
            {
                if (name == null)
                    component.Site = null;
                else
                    component.Site = new NestedComponentSite(null, component, name);
            }
            return true;
        }

        void IContainer.Remove(IComponent component) { Remove(component); }

        public static bool Contains(IContainer container, IComponent component, bool recursive)
        {
            if (container == null || component == null)
                return false;

            foreach (IComponent item in AsEnumerable(container))
            {
                if (ReferenceEquals(item, component))
                    return true;
            }

            if (!recursive)
                return false;
            
            foreach (IComponent item in AsEnumerable(container))
            {
                if (item is IContainer && Contains((item as IContainer), component, true))
                    return true;
            }

            return false;
        }
        
        public static bool Contains(IContainer container, string name)
        {
            if (name == null)
                return false;

            return EnumerateNames(container).Any(n => String.Compare(n, name, true) == 0);
        }

        public bool Contains(IComponent component, bool recursive) { return Contains(this, component, recursive); }

        public bool Contains(IComponent component) { return Contains(this, component, false); }

        public static IEnumerable<IComponent> AsEnumerable(IContainer container)
        {
            ComponentCollection components;
            if (container != null && (components = container.Components) != null)
            {
                foreach (IComponent item in components)
                    yield return item;
            }
        }
        
        public static IEnumerable<string> EnumerateNames(IContainer container)
        {
            return AsEnumerable(container).Select(i => i.Site).Where(s => s != null).Select(s => s.Name).Where(n => n != null);
        }

        public IEnumerable<IComponent> AsEnumerable() { return AsEnumerable(this); }

        private void Component_Disposed(object sender, EventArgs e)
        {
            IComponent component = sender as IComponent;
            if (Contains(component))
                _mutator.Remove(component);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            
            try
            {
                IComponent[] components = AsEnumerable().ToArray();
                _mutator.Clear();
                foreach (var a in components.Select(c => new { S = c.Site, C = c }).Where(a => a.S != null && a.S.Container != null && Contains(a.S.Container, a.C, false)))
                    a.C.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}