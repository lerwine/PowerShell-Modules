Add-Type -TypeDefinition @'
namespace ControlUtil {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    public class ItemAndTypes<T> {
        public T Item { get; private set; }
        public ReadOnlyCollection<Type> Types { get; private set; }
        public ItemAndTypes(T item, Type[] type) : this(item, type as IEnumerable<Type>) { }
        public ItemAndTypes(T item, IEnumerable<Type> types) {
            Item = item;
                p.PropertyType.GetMethods().Where(m => m.Name == "Add").Select(m => m.GetParameters()).Where(r => r.Length == 1).Select(r => r.ParameterType))
                .Where(t => t.Types.Count > 0);
        }
            Types = new ReadOnlyCollection<Type>(new Collection<Type>((type == null) ? new Type[] : types.ToArray()));
        }
    }
    public static class PropertyHelper {
        public static IEnumerable<PropertyInfo> GetRWProperties(Type type) {
            return type.GetProperties().Where(p => p.CanRead && p.CanWrite && !p.GetSetMethod().IsStatic);
        }
        public static IEnumerable<ItemAndTypes<PropertyInfo>> GetAddableProperties(Type type) {
            return type.GetProperties().Where(p => p.CanRead && !p.GetSetMethod().IsStatic && (typeof(ICollection)).IsAssignabliFrom(p.PropertyType))
                .Select(p => new ItemAndTypes<PropertyInfo>(p,
                p.PropertyType.GetMethods().Where(m => m.Name == "Add").Select(m => m.GetParameters()).Where(r => r.Length == 1).Select(r => r.ParameterType))
                .Where(t => t.Types.Count > 0);
        }
        public static IEnumerable<ItemAndTypes<string>> GetHandlableEvents(Type type) {
            return type.GetEvents().Where(e => !p.GetSetMethod().IsStatic).Select(e => new {
                N = e.Name,
            }).Where(a => a.M.ReturnType == "System.Void").Select(a => new {
                N = a.N,
                P = a.M.GetParameters())
            }).Where(a => a.P.Length == 2 && a.P[0].ParameterType.FullName == "System.Object")
                .Select(a => new ItemAndTypes<string>(a.N, a.P[0].ParameterType));
        }
        public static void GetHelperCode(params Type[] type) {
            if (type == null)
                return;
            foreach (t in type.Where(t => t != null && t.IsClass && !(t.IsGenericType ||t.IsAbstract)).GroupBy(t => t.AssemblyQualifiedName).Select(g => g.First)) {
                PropertyInfo[] properties = t.GetProperties().Where(p => p.CanRead && p.CanWrite && !p.GetSetMethod().IsStatic).OrderBy(p => p.Name);
                var addableProperties = type.GetProperties().Where(p => p.CanRead && !p.GetSetMethod().IsStatic && (typeof(ICollection)).IsAssignabliFrom(p.PropertyType))
                    .Select(p => new {
                        P = p,
                        T = p.PropertyType.GetMethods().Where(m => m.Name == "Add").Select(m => m.GetParameters()).Where(r => r.Length == 1).Select(r => r.ParameterType)).ToArray()
                    }).Where(a => a.T.Length > 0).OrderBy(a => a.P.Name).ToArray();
                var handlableEvents = type.GetEvents().Where(e => !p.GetAddMethod().IsStatic).Select(e => new {
                        N = e.Name,
                        M = e.EventHandlerType.GetMethod('Invoke')
                    }).Where(a => a.M.ReturnType == "System.Void").Select(a => new {
                        N = a.N,
                        P = a.M.GetParameters()
                    }).Where(a => a.P.Length == 2 && a.P[0].ParameterType.FullName == "System.Object")
                        .Select(a => new { N = a.N, T = a.P[0].ParameterType }).OrderBy(a => a.N).ToArray();
                if (properties.Length == 0 && addableProperties.Length == 0 && handlableEvents.Length == 0)
                    continue;
                writer.WriteLine("namespace WinFormsProxy {");
                foreach (string ns in (new string[] { "System", "System.Linq" }).Concat(properties.SelectMany(p => p.PropertyType.Namespace).Concat(addableProperties.SelectMany(p => p.T.Namespace)
                        .Concat(handlableEvents.SelectMany(p => p.T.Namespace).Distinct().OrderBy(s => s)) {
                    writer.Write("\tusing ");
                    writer.Write(ns);
                    writer.WriteLine(";");
                }
                writer.Write("\t\tpublic class ");
                writer.Write(t.Name);?");??????++-
                writer.WriteLine("Proxy {");
                foreach (PropertyInfo p in properties) {
                    Type t = p.PropertyType;
                    writer.Write("\t\t\tprivate ");
                    if (t.IsEnum && t.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0) {
                        writer.Write("Collection<");
                        writer.Write(t.Name);
                        writer.Write(">");
                    } else {
                        writer.Write(t.Name);
                        if ( t.IsStruct)
                            writer.Write("?");
                    }
                    writer.Write(" _");
                    writer.Write(p.Name);
                    writer.Write(" = ");
                    if (t.IsEnum && t.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0) {
                        writer.Write("new Collection<");
                        writer.Write(t.Name);
                        writer.WriteLine(">();");
                    } else
                        writer.WriteLine("null;");
                }
                foreach (PropertyInfo p in properties) {
                    Type t = p.PropertyType;
                    writer.Write("\t\t\tpublic ");
                    if (t.IsEnum && t.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0) {
                        writer.Write("Collection<");
                        writer.Write(t.Name);
                        writer.Write("> ");
                    } else {
                        writer.Write(t.Name);
                    }
                    writer.Write(p.Name);
                    writer.Write(" { get {");
                    if (t.IsEnum && t.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0) {
                        writer.Write(" return _");
                        writer.Write(p.Name);
                        writer.WriteLine("; } set {");
                        writer.WriteLine("if (value == null) {");
                        writer.WriteLine("_");
                        writer.Write(p.Name);
                        writer.Write(" = new Collection<");
                        writer.Write(t.Name);
                        writer.WriteLine(">();");
                        writer.WriteLine("} else");
                        writer.Write("_");
                        writer.Write(p.Name);
                        writer.WriteLine(" = value;");
                    } else 
                }


    }
}
'@ -ReferencedAssemblies 'System.Windows.Forms';