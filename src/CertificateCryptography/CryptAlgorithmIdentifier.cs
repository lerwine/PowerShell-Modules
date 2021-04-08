using System;
using System.Runtime.InteropServices;

namespace Erwine.Leonard.T.CertificateCryptography
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct CryptAlgorithmIdentifier
    {
        public string pszObjId;
        public CryptoApiBlob Parameters;
    }
}