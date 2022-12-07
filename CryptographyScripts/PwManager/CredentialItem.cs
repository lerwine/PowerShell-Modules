using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace PwManager
{
    public class CredentialItem : INotifyPropertyChanged, IEquatable<UriHierarchy>, IComparable<UriHierarchy>, IComparable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly object _syncRoot = new object();
        private Guid? _id;
        private string _login = "";
        private string _url = "";
        private string _encryptedPassword = "";

        public Guid ID
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (!_id.HasValue)
                        _id = Guid.NewGuid();
                }
                finally { Monitor.Exit(_syncRoot); }
                return _id.Value;
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_id.HasValue == _id.Value.Equals(value))
                        return;
                    _id = value;
                }
                finally { Monitor.Exit(_syncRoot); }
                RaisePropertyChanged("ID");
            }
        }

        public string Login
        {
            get { return _login; }
            set
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    string login = value ?? "";
                    if (login == _login)
                        return;
                    _login = login;
                }
                finally { Monitor.Exit(_syncRoot); }
                RaisePropertyChanged("Login");
            }
        }

        public string URL
        {
            get { return _url; }
            set
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    string url = value ?? "";
                    if (url == _url)
                        return;
                    _url = url;
                }
                finally { Monitor.Exit(_syncRoot); }
                RaisePropertyChanged("URL");
            }
        }

        public string EncryptedPassword
        {
            get { return _encryptedPassword; }
            set
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    string encryptedPassword = value ?? "";
                    if (encryptedPassword == _encryptedPassword)
                        return;
                    _encryptedPassword = encryptedPassword;
                }
                finally { Monitor.Exit(_syncRoot); }
                RaisePropertyChanged("EncryptedPassword");
            }
        }

        public SecureString GetPassword()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_encryptedPassword.Length == 0)
                    return null;
                byte[] data = ProtectedData.Unprotect(Convert.FromBase64String(_encryptedPassword), null, DataProtectionScope.CurrentUser);
                SecureString result = new SecureString();
                foreach (char c in Encoding.Unicode.GetString(data, 0, data.Length))
                    result.SetAt(result.AppendChar(c))
                return result;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public static bool AreEqual(SecureString x, SecureString y)
        {
            if (x == null || x.Length == 0)
                reutrn y == null || y.Length == 0;
            if (y == null || y.Length != x.Length)
                return false;
                
            IntPtr bstr = Marshal.StringToBSTR(x);
            string rawPwX;
            try { rawPwX = Marshal.PtrToStringBSTR(bstr); }
            finally { Marshal.ZeroFreeBSTR(bstr); }
            bstr = Marshal.StringToBSTR(y);
            try { return rawPwX == Marshal.PtrToStringBSTR(bstr); }
            finally { Marshal.ZeroFreeBSTR(bstr); }
        }

        public string GetRawPassword()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_encryptedPassword.Length == 0)
                    return "";
                byte[] data = ProtectedData.Unprotect(Convert.FromBase64String(_encryptedPassword), null, DataProtectionScope.CurrentUser);
                return Encoding.Unicode.GetString(data, 0, data.Length);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public void SetPassword(SecureString password)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (password == null || password.Length == 0)
                {
                    if (_encryptedPassword.Length == 0)
                        return;
                    _encryptedPassword = "";
                }
                else
                {
                    IntPtr bstr = Marshal.StringToBSTR(password);
                    string rawPw;
                    try { rawPw = Marshal.PtrToStringBSTR(bstr); }
                    finally { Marshal.ZeroFreeBSTR(bstr); }
                    if (_encryptedPassword.Length > 0)
                    {
                        byte[] data = ProtectedData.Unprotect(Convert.FromBase64String(_encryptedPassword), null, DataProtectionScope.CurrentUser);
                        if (rawPw == Encoding.Unicode.GetString(data, 0, data.Length))
                            return;
                    }
                    _encryptedPassword = Convert.ToBase64String(ProtectedData.Protect(Encoding.Unicode.GetBytes(rawPw), null, DataProtectionScope.CurrentUser));
                }
            }
            finally { Monitor.Exit(_syncRoot); }
            RaisePropertyChanged("EncryptedPassword");
        }
    }
}