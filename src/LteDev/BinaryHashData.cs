using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev
{
    public class BinaryHashData
    {
        public static readonly UTF8Encoding Encoding = new UTF8Encoding(false, false);
        private List<byte> _data = new List<byte>();
        public void AddString(string value)
        {
            if (!String.IsNullOrEmpty(value))
                _data.AddRange(Encoding.GetBytes(value.ToUpper()));
            _data.Add(0);
        }
        public void AddBytes(byte[] value)
        {
            if (value != null && value.Length > 0)
                _data.AddRange(value);
        }
        public void AddInt64(long value) { _data.AddRange(BitConverter.GetBytes(value)); }
        public void AddDouble(double value) { _data.AddRange(BitConverter.GetBytes(value)); }
        public void AddDate(DateTime? value) { _data.AddRange(BitConverter.GetBytes((value.HasValue) ? ((value.Value.Kind == DateTimeKind.Utc) ? value.Value : ((value.Value.Kind == DateTimeKind.Local) ? value.Value : DateTime.SpecifyKind(value.Value, DateTimeKind.Local)).ToUniversalTime()).ToBinary() : 0L)); }
        public BinaryHashData GetHash(System.Security.Cryptography.HashAlgorithm provider) {
            BinaryHashData result = new BinaryHashData();
            if (_data.Count > 0)
                result._data.AddRange(provider.ComputeHash(_data.ToArray()));
            return result;
        }
        public BinaryHashData() { }
        public static BinaryHashData Parse(string value)
        {
            BinaryHashData result = new BinaryHashData();
            if (value == null || (value = value.Trim()).Length == 0)
                return result;
            for (int i = 0; i < value.Length; i++) {
                if (Char.IsControl(value[i]) || Char.IsWhiteSpace(value[i]) || value[i] == ':' || value[i] == '-' || value[i] == '{' || value[i] == '}')
                    continue;
                if (i == value.Length - 1)
                    throw new FormatException("Input string was not in a correct format.");
                byte b = Byte.Parse(value.Substring(i, 2));
                i++;
                if (b < 16 && value[i] != '0')
                    throw new FormatException("Input string was not in a correct format.");
                result._data.Add(b);
            }
            return result;
        }
        public override string ToString()
        {
            if (_data.Count == 0)
                return "";
            StringBuilder sb = new StringBuilder();
            sb.Append(_data[0].ToString("x2"));
            for (int i = 0; i < _data.Count; i++) {
                switch (i % 32)
                {
                    case 0:
                        if (sb.Length > 0)
                            sb.AppendLine();
                        break;
                    case 8:
                    case 16:
                    case 24:
                        sb.Append(" ");
                        break;
                    default:
                        if ((i % 2) == 0)
                            sb.Append("-");
                        break;
                }
                sb.Append(_data[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}