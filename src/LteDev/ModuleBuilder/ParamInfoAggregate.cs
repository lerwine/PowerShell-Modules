using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LteDev.ModuleBuilder
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class ParamInfoAggregate<T> : InformationAggregator
    {
        public T Parameter { get; private set; }

        public ParamInfoAggregate(T parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");

            Parameter = parameter;
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}