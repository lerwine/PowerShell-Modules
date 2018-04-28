using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using SecureString = System.Security.SecureString;
using RuntimeHelpers = System.Runtime.CompilerServices.RuntimeHelpers;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Erwine.Leonard.T.CertificateCryptography.Commands
{
    [Cmdlet(VerbsCommon.New, "SelfSignedCertificate")]
    [OutputType(typeof(Oid))]
    public class New_SelfSignedCertificate : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, HelpMessage = "Distinguished name of Issuer.")]
        [ValidateX500DistinguishedName(AllowEmpty = true)]
        public string[] IssuerDN { get; set; }

        [Parameter()]
        public DateTime StartTime { get; set; }

        [Parameter()]
        public DateTime EndTime { get; set; }


        protected override void ProcessRecord()
        {
            if (IssuerDN == null || IssuerDN.Length == 0)
                IssuerDN = new string[] { "" };
            DateTime? startTime = null, endTime = null;
            if (MyInvocation.BoundParameters.ContainsKey("StartTime"))
                startTime = StartTime;
            if (MyInvocation.BoundParameters.ContainsKey("EndTime"))
                endTime = EndTime;
            foreach (string issuerDN in IssuerDN)
            {
                try { WriteObject(GenerateSelfSignedCertificate(issuerDN, startTime, endTime)); }
                catch (Exception e)
                {
                    Hashtable target = new Hashtable();
                    target.Add("IssuerDN", issuerDN);
                    target.Add("StartTime", startTime);
                    target.Add("EndTime", endTime);
                    WriteError(new ErrorRecord(e, "New_SelfSignedCertificate", ErrorCategory.InvalidData, target));
                }
            }
        }

        public static X509Certificate2 GenerateSelfSignedCertificate(string issuerDN, DateTime? startTime = null, DateTime? endTime = null)
        {
            if (issuerDN == null)
                issuerDN = String.Empty;
            var startSystemTime = default(SystemTime);
            if (startTime == null || (DateTime)startTime < DateTime.FromFileTimeUtc(0))
            {
                startTime = DateTime.FromFileTimeUtc(0);
                startSystemTime = SystemTime.Create((DateTime)startTime);
            }
            if (endTime == null)
                endTime = DateTime.MaxValue;
            var endSystemTime = SystemTime.Create((DateTime)endTime);
            string containerName = Guid.NewGuid().ToString();
            IntPtr providerContext = IntPtr.Zero;
            IntPtr cryptKey = IntPtr.Zero;
            IntPtr certificateContext = IntPtr.Zero;
            IntPtr algorithmPointer = IntPtr.Zero;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                providerContext = Externs.CryptAcquireContextW(containerName, null, 1, 0x8);
                cryptKey = Externs.CryptGenKey(providerContext, 1, 0x8000001);
                CryptoApiBlob nameBlob = CryptoApiBlob.Create(issuerDN);
                CryptKeyProvInfo keyProvider = new CryptKeyProvInfo
                {
                    pwszContainerName = containerName,
                    dwProvType = 1,
                    dwKeySpec = 1
                };
                CryptAlgorithmIdentifier algorithm = new CryptAlgorithmIdentifier
                {
                    pszObjId = "1.2.840.113549.1.1.13",
                    Parameters = new CryptoApiBlob()
                };
                algorithmPointer = Marshal.AllocHGlobal(Marshal.SizeOf(algorithm));
                Marshal.StructureToPtr(algorithm, algorithmPointer, false);
                certificateContext = Externs.CertCreateSelfSignCertificate(providerContext, nameBlob, 0, keyProvider, algorithmPointer,
                    startSystemTime, endSystemTime, IntPtr.Zero);
                return new X509Certificate2(certificateContext);
            }
            finally
            {
                if (certificateContext != IntPtr.Zero)
                    Externs.CertFreeCertificateContext(certificateContext);
                if (cryptKey != IntPtr.Zero)
                    Externs.CryptDestroyKey(cryptKey);
                if (providerContext != IntPtr.Zero)
                    Externs.CryptReleaseContext(providerContext, 0);
                if (algorithmPointer != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(algorithmPointer, typeof(CryptAlgorithmIdentifier));
                    Marshal.FreeHGlobal(algorithmPointer);
                }
            }
        }
    }
}
