using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace UnitTests.ModuleConformance
{
    public class AssemblyCmdlet
    {
        public CmdletAttribute CmdletAttribute { get; private set; }

        public Type Type { get; private set; }

        private AssemblyCmdlet(Type type, CmdletAttribute attribute)
        {
            Type = type;
            CmdletAttribute = attribute;
        }

        public static IEnumerable<AssemblyCmdlet> GetCmdlets(Assembly assembly)
        {
            if (assembly == null)
                yield break;

            foreach (Type t in assembly.GetTypes())
            {
                CmdletAttribute attribute;
                if (!t.IsAbstract && t.IsPublic && t.GetConstructors().Any(c => c.IsPublic && c.GetParameters().Length == 0) && (attribute = t.GetCustomAttributes<CmdletAttribute>().FirstOrDefault()) != null)
                    yield return new AssemblyCmdlet(t, attribute);
            }
        }
    }
}