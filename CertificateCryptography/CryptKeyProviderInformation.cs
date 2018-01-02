using System;
using System.Runtime.InteropServices;

namespace Erwine.Leonard.T.CertificateCryptography
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct CryptKeyProviderInformation
    {
        public string pwszContainerName;
        public string pwszProvName;
        public int dwProvType;
        public int dwFlags;
        public int cProvParam;
        public IntPtr rgProvParam;
        public int dwKeySpec;
    }
}