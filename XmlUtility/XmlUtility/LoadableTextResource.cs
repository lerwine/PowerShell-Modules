using System;
using System.Collections.Generic;
using System.IO;
#if !PSLEGACY
using System.Linq;
#endif
using System.Text;
using System.Text.RegularExpressions;
#if !PSLEGACY
using System.Threading.Tasks;
#endif

namespace XmlUtilityCLR
{
    public abstract class LoadableTextResource<TTarget> : LoadableResource<string[], TTarget>
    {
        public static readonly Regex NewLineRegex = new Regex(@"\r\n?|\n", RegexOptions.Compiled);

        protected LoadableTextResource(Uri uri) : base(uri) { }

        protected abstract override TTarget CreateTarget();

        protected override string[] GetDefaultSourceValue() { return new string[0]; }

        protected override string[] Load(Stream stream, bool isValid)
        {
            using (StreamReader reader = (Encoding == null) ? new StreamReader(stream, true) : new StreamReader(stream, Encoding))
            {
                if (Encoding == null)
                    Encoding = reader.CurrentEncoding;
                return Load(reader, isValid);
            }
        }
        
        protected virtual string[] Load(StreamReader reader, bool isValid) { return NewLineRegex.Split(reader.ReadToEnd()); }

        protected override string[] LoadSourceFromLocal() { return NewLineRegex.Split(File.ReadAllText(SourcePath)); }
    }

    public class LoadableTextResource : LoadableTextResource<string>
    {
        public LoadableTextResource(Uri uri) : base(uri) { }

        protected override string CreateTarget() { return String.Join("\r\n", Source); }
    }
}