using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.CertificateCryptography
{
    internal static class Externs
    {
        private static void Assert(Func<bool> func)
        {
            if (!func())
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        internal static SystemTime FileTimeToSystemTime(long fileTime)
        {
            SystemTime result = default(SystemTime);
            Assert(() => _Extern.FileTimeToSystemTime(ref fileTime, out result));
            return result;
        }
        internal static IntPtr CryptAcquireContextW(string container, string provider, int providerType, int flags)
        {
            IntPtr result = IntPtr.Zero;
            Assert(() => _Extern.CryptAcquireContextW(out result, container, provider, providerType, flags));
            return result;
        }

        internal static void CryptReleaseContext(IntPtr providerContext, int flags)
        {
            Assert(() => _Extern.CryptReleaseContext(providerContext, flags));
        }

        internal static IntPtr CryptGenKey(IntPtr providerContext, int algorithmId, int flags)
        {
            IntPtr result = IntPtr.Zero;
            Assert(() => _Extern.CryptGenKey(providerContext, algorithmId, flags, out result));
            return result;
        }

        internal static void CryptDestroyKey(IntPtr cryptKeyHandle)
        {
            Assert(() => _Extern.CryptDestroyKey(cryptKeyHandle));
        }

        internal static byte[] CertStrToNameW(string name, int certificateEncodingType = 0x00010001, int strType = 3)
        {
            IntPtr errorStringPtr;
            int nameDataLength = 0;
            byte[] nameData;
            GCHandle dataHandle = GCHandle.Alloc(name, GCHandleType.Pinned);
            try
            {
                if (_Extern.CertStrToNameW(certificateEncodingType, dataHandle.AddrOfPinnedObject(), strType, IntPtr.Zero, null, ref nameDataLength,
                    out errorStringPtr))
                {
                    nameData = new byte[nameDataLength];
                    if (_Extern.CertStrToNameW(certificateEncodingType, dataHandle.AddrOfPinnedObject(), strType, IntPtr.Zero, nameData, ref nameDataLength,
                        out errorStringPtr))
                        return nameData;
                }
                string error = Marshal.PtrToStringUni(errorStringPtr);
                throw new ArgumentException(error);
            }
            finally { dataHandle.Free(); }
        }

        internal static IntPtr CertCreateSelfSignCertificate(IntPtr providerHandle, CryptoApiBlob subjectIssuerBlob, int flags,
            CryptKeyProviderInformation keyProviderInformation, IntPtr signatureAlgorithm, SystemTime startTime,
            SystemTime endTime, IntPtr extensions)
        {
            IntPtr result = IntPtr.Zero;
            Assert(() =>
            {
                result = _Extern.CertCreateSelfSignCertificate(providerHandle, ref subjectIssuerBlob, flags, ref keyProviderInformation,
                    signatureAlgorithm, ref startTime, ref endTime, extensions);
                return result != IntPtr.Zero;
            });
            return result;
        }

        internal static void CertFreeCertificateContext(IntPtr certificateContext)
        {
            Assert(() => _Extern.CertFreeCertificateContext(certificateContext));
        }

        internal static void CertSetCertificateContextProperty(IntPtr certificateContext, int propertyId, int flags,
            CryptKeyProviderInformation data)
        {
            Assert(() => _Extern.CertSetCertificateContextProperty(certificateContext, propertyId, flags, ref data));
        }

        private static class _Extern
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern bool FileTimeToSystemTime(ref long fileTime, out SystemTime systemTime);

            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            internal static extern bool CryptAcquireContextW(out IntPtr providerContext, string container, string provider, int providerType, int flags);

            [DllImport("advapi32.dll", SetLastError = true)]
            internal static extern bool CryptReleaseContext(IntPtr providerContext, int flags);

            [DllImport("advapi32.dll", SetLastError = true)]
            internal static extern bool CryptGenKey(IntPtr providerContext, int algorithmId, int flags, out IntPtr cryptKeyHandle);

            [DllImport("advapi32.dll", SetLastError = true)]
            internal static extern bool CryptDestroyKey(IntPtr cryptKeyHandle);

            [DllImport("crypt32.dll", SetLastError = true)]
            internal static extern bool CertStrToNameW(int certificateEncodingType, IntPtr x500, int strType, IntPtr reserved, byte[] encoded, ref int encodedLength, out IntPtr errorString);

            [DllImport("crypt32.dll", SetLastError = true)]
            internal static extern IntPtr CertCreateSelfSignCertificate(IntPtr providerHandle, ref CryptoApiBlob subjectIssuerBlob, int flags, ref CryptKeyProviderInformation keyProviderInformation, IntPtr signatureAlgorithm, ref SystemTime startTime, ref SystemTime endTime, IntPtr extensions);

            [DllImport("crypt32.dll", SetLastError = true)]
            internal static extern bool CertFreeCertificateContext(IntPtr certificateContext);

            [DllImport("crypt32.dll", SetLastError = true)]
            internal static extern bool CertSetCertificateContextProperty(IntPtr certificateContext, int propertyId, int flags, ref CryptKeyProviderInformation data);
        }
    }
}
