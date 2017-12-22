using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace LteDev.TypeBrowser
{
    public class EventListItemVM : DependencyObject
    {
        public EventListItemVM() { }

        public EventListItemVM(EventInfo model)
        {
        }

        public EventListItemVM(EventListItemVM vm)
        {

        }
    }
}