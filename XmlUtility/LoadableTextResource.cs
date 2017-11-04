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

namespace XmlUtility
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class LoadableTextResource<TTarget> : LoadableResource<string[], TTarget>
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex NewLineRegex = new Regex(@"\r\n?|\n", RegexOptions.Compiled);

        /// <summary>
        /// 
        /// </summary>
        protected LoadableTextResource(Uri uri) : base(uri) { }

        /// <summary>
        /// 
        /// </summary>v
        protected abstract override TTarget CreateTarget();

        /// <summary>
        /// 
        /// </summary>
        protected override string[] GetDefaultSourceValue() { return new string[0]; }

        /// <summary>
        /// 
        /// </summary>
        protected override string[] Load(Stream stream, bool isValid)
        {
            using (StreamReader reader = (Encoding == null) ? new StreamReader(stream, true) : new StreamReader(stream, Encoding))
            {
                if (Encoding == null)
                    Encoding = reader.CurrentEncoding;
                return Load(reader, isValid);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual string[] Load(StreamReader reader, bool isValid) { return NewLineRegex.Split(reader.ReadToEnd()); }

        /// <summary>
        /// 
        /// </summary>
        protected override string[] LoadSourceFromLocal() { return NewLineRegex.Split(File.ReadAllText(SourcePath)); }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LoadableTextResource : LoadableTextResource<string>
    {
        /// <summary>
        /// 
        /// </summary>
        public LoadableTextResource(Uri uri) : base(uri) { }

        /// <summary>
        /// 
        /// </summary>
        protected override string CreateTarget() { return String.Join("\r\n", Source); }
    }
}