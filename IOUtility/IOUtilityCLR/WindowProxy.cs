using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace IOUtilityCLR
{
    public class WindowProxy
    {
        private Window _window;
        private Dictionary<string, object> _controls = new Dictionary<string, object>();

        public ReadOnlyDictionary<string, object> Controls { get; private set; }

        public Exception Error { get; private set; }

        public string Title
        {
            get { return (_window == null) ? null : _window.Title; }
            set
            {
                if (_window != null)
                    _window.Title = value ?? "";
            }
        }

        public double Width
        {
            get { return (_window == null) ? 0.0 : _window.Width; }
            set
            {
                if (_window != null)
                    _window.Width = value;
            }
        }

        public double Height
        {
            get { return (_window == null) ? 0.0 : _window.Height; }
            set
            {
                if (_window != null)
                    _window.Height = value;
            }
        }

        public WindowProxy(XamlWindow windowDefinition)
        {
            Controls = new ReadOnlyDictionary<string, object>(_controls);

            try
            {
                XmlNodeReader xmlNodeReader;
                try
                {
                    xmlNodeReader = new XmlNodeReader(windowDefinition.Xaml);
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException("Error creating XML Node Reader", exception);
                }

                try
                {
                    using (xmlNodeReader)
                    {
                        if ((_window = XamlReader.Load(xmlNodeReader) as Window) == null)
                            throw new Exception("Window not loaded");
                    }
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException("Error reading XAML", exception);
                }

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(windowDefinition.Xaml.NameTable);
                nsmgr.AddNamespace("x", XamlWindow.XAML_XMLNAMESPACE);

                foreach (XmlAttribute attribute in windowDefinition.Xaml.SelectNodes("//@x:Name", nsmgr))
                {
                    try
                    {
                        object obj = _window.FindName(attribute.Value);
                        if (obj != null)
                            _controls.Add(attribute.Value, obj);
                    }
                    catch { }
                }
            }
            catch (Exception exception)
            {
                Error = exception;
            }
        }
    }
}