using System;
using System.Runtime.InteropServices;

namespace Erwine.Leonard.T.CertificateCryptography
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct CryptoApiBlob
    {
        public int cbData;
        public IntPtr pbData;
        internal static CryptoApiBlob Create(string name)
        {
            byte[] nameData = Externs.CertStrToNameW(name);
            GCHandle dataHandle = GCHandle.Alloc(nameData, GCHandleType.Pinned);
            try { return new CryptoApiBlob { cbData = nameData.Length, pbData = dataHandle.AddrOfPinnedObject() }; }
            finally { dataHandle.Free(); }
        }
    }
}