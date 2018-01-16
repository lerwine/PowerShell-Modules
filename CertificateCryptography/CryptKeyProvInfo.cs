using System;
using System.Runtime.InteropServices;

namespace Erwine.Leonard.T.CertificateCryptography
{
    /// <summary>
    /// CRYPT_KEY_PROV_INFO structure used for invoking unmanaged code CryptoAPI functions.
    /// </summary>
    /// <remarks>Contains information about a key container within a cryptographic service provider (CSP).</remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CryptKeyProvInfo
    {
        public const int PROV_RSA_FULL = 1;
        public const int PROV_RSA_SIG = 2;
        public const int PROV_DSS = 3;
        public const int PROV_FORTEZZA = 4;
        public const int PROV_MS_EXCHANGE = 5;
        public const int PROV_SSL = 6;

        /// <summary>
        /// The name of the key container.
        /// </summary>
        /// <remarks>When the <see cref="dwProvType"/> member is zero, this string contains the name of a key within a CNG key storage provider.</remarks>
        public string pwszContainerName;

        /// <summary>
        /// The name of the CSP (Cryptographin Service Provider).
        /// </summary>
        /// <remarks>When the <see cref="dwProvType"/> member is zero, this string contains the name of a CNG key storage provider.</remarks>v
        public string pwszProvName;

        /// <summary>
        /// Specifies the CSP type.
        /// </summary>
        /// <remarks>This can be <see cref="PROV_RSA_FULL"/>, <see cref="PROV_RSA_SIG"/>, <see cref="PROV_DSS"/>, <see cref="PROV_FORTEZZA"/>, <see cref="PROV_MS_EXCHANGE"/>,
        /// <see cref="PROV_SSL"/> or zero. If this member is zero, the key container is one of the CNG key storage providers.</remarks>
        public int dwProvType;

        public int dwFlags;
        public int cProvParam;
        public IntPtr rgProvParam;
        public int dwKeySpec;
    }
}