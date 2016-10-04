using System;
using System.Collections.Generic;
using System.IO;

namespace XmlUtilityCLR
{
    public abstract class LoadableBinaryResource<TTarget> : LoadableResource<byte[], TTarget>
    {
        protected LoadableBinaryResource(Uri uri) : base(uri) { }

        protected abstract override TTarget CreateTarget();

        protected override byte[] GetDefaultSourceValue() { return new byte[0]; }

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

        protected override byte[] LoadSourceFromLocal() { return File.ReadAllBytes(SourcePath); }
    }

    public class LoadableBinaryResource : LoadableBinaryResource<byte[]>
    {
        public LoadableBinaryResource(Uri uri) : base(uri) { }

        protected override byte[] CreateTarget() { return Source; }
    }
}
