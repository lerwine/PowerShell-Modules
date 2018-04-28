using System;
using System.Collections.Generic;
using System.IO;

namespace XmlUtility
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class LoadableBinaryResource<TTarget> : LoadableResource<byte[], TTarget>
    {
        /// <summary>
        /// 
        /// </summary>
        protected LoadableBinaryResource(Uri uri) : base(uri) { }

        /// <summary>
        /// 
        /// </summary>
        protected abstract override TTarget CreateTarget();

        /// <summary>
        /// 
        /// </summary>
        protected override byte[] GetDefaultSourceValue() { return new byte[0]; }

        /// <summary>
        /// 
        /// </summary>
        protected override byte[] Load(Stream stream, bool isValid)
        {
            List<byte> source = new List<byte>();
            byte[] buffer = new byte[4096];
            for (int c = stream.Read(buffer, 0, buffer.Length); c > 0; c = stream.Read(buffer, 0, buffer.Length))
            {
                if (c == buffer.Length)
                    source.AddRange(buffer);
                else
                {
                    byte[] b = new byte[c];
                    Array.ConstrainedCopy(buffer, 0, b, 0, c);
                    source.AddRange(b);
                }
            }
            return source.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override byte[] LoadSourceFromLocal() { return File.ReadAllBytes(SourcePath); }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LoadableBinaryResource : LoadableBinaryResource<byte[]>
    {
        /// <summary>
        /// 
        /// </summary>
        public LoadableBinaryResource(Uri uri) : base(uri) { }

        /// <summary>
        /// 
        /// </summary>
        protected override byte[] CreateTarget() { return Source; }
    }
}
