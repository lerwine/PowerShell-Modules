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
    public class ConstructorListItemVM : MethodBaseListItemVM
    {
        public ConstructorListItemVM() : base() { }

        public ConstructorListItemVM(ConstructorListItemVM vm) : base(vm)
        {
        }

        public ConstructorListItemVM(ConstructorInfo model)
            : base(model)
        {

        }
    }
}