using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace LteDev.ModuleBuilder
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class AggregateInfoFactory : InformationAggregator
    {
        private List<AssemblyInfo> _assemblyContextCache = new List<AssemblyInfo>();
        private List<ModuleInfoAggregate> _moduleContextCache = new List<ModuleInfoAggregate>();

        public CommandInvocationIntrinsics InvokeCommand { get; private set; }

        public ProviderIntrinsics InvokeProvider { get; private set; }

        public PathIntrinsics Path { get; private set; }

        public AssemblyInfo GetAssemblyContext(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            AssemblyInfo context;
            Monitor.Enter(_assemblyContextCache);
            try
            {
                context = _assemblyContextCache.FirstOrDefault(c => c.Equals(assembly));
                if (context == null)
                {
                    context = new AssemblyInfo(assembly, this);
                    _assemblyContextCache.Add(context);
                }
            }
            finally { Monitor.Exit(_assemblyContextCache); }
            return context;
        }

        public ModuleInfoAggregate GetModuleContext(PSModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
                throw new ArgumentNullException("moduleInfo");
            ModuleInfoAggregate context;
            Monitor.Enter(_moduleContextCache);
            try
            {
                context = _moduleContextCache.FirstOrDefault(c => c.Equals(moduleInfo));
                if (context == null)
                {
                    context = new ModuleInfoAggregate(moduleInfo, this);
                    _moduleContextCache.Add(context);
                }
            }
            finally { Monitor.Exit(_moduleContextCache); }
            return context;
        }

        public CLRTypeInfo GetAssemblyTypeContext(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return GetAssemblyContext(type.Assembly).GetTypeContext(type);
        }

        public CLRPropertyInfo GetAssemblyPropertyContext(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            return GetAssemblyTypeContext(propertyInfo.ReflectedType).GetPropertyContext(propertyInfo);
        }

        public static AggregateInfoFactory Create(PSCmdlet cmdlet)
        {
            if (cmdlet == null)
                throw new ArgumentNullException("cmdlet");

            return new AggregateInfoFactory(cmdlet.InvokeCommand, cmdlet.InvokeProvider, cmdlet.SessionState.Path);
        }

        public static AggregateInfoFactory Create(SessionState sessionState)
        {
            if (sessionState == null)
                throw new ArgumentNullException("sessionState");

            return new AggregateInfoFactory(sessionState.InvokeCommand, sessionState.InvokeProvider, sessionState.Path);
        }

        public AggregateInfoFactory(CommandInvocationIntrinsics invokeCommand, ProviderIntrinsics invokeProvider, PathIntrinsics path)
        {
            InvokeCommand = invokeCommand;
            InvokeProvider = invokeProvider;
            Path = path;
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}