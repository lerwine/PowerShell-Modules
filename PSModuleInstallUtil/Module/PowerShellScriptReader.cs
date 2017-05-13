using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PSModuleInstallUtil.Module
{
    public class PowerShellScriptReader
    {
        public PowerShellScriptReader(string path)
        {
            using (StreamReader reader = new StreamReader(path))
                Initialize(reader);
        }

        public PowerShellScriptReader(Stream path)
        {
            using (StreamReader reader = new StreamReader(path))
                Initialize(reader);
        }

        public PowerShellScriptReader(string path, bool detectEncodingFromByteOrderMarks)
        {
            using (StreamReader reader = new StreamReader(path, detectEncodingFromByteOrderMarks))
                Initialize(reader);

        }

        public PowerShellScriptReader(Stream path, bool detectEncodingFromByteOrderMarks)
        {
            using (StreamReader reader = new StreamReader(path, detectEncodingFromByteOrderMarks))
                Initialize(reader);

        }

        public PowerShellScriptReader(string path, Encoding encoding)
        {
            using (StreamReader reader = new StreamReader(path, encoding))
                Initialize(reader);

        }

        public PowerShellScriptReader(Stream path, Encoding encoding)
        {
            using (StreamReader reader = new StreamReader(path, encoding))
                Initialize(reader);

        }

        public PowerShellScriptReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            using (StreamReader reader = new StreamReader(path, encoding, detectEncodingFromByteOrderMarks))
                Initialize(reader);
        }

        public PowerShellScriptReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            using (StreamReader reader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks))
                Initialize(reader);
        }

        private void Initialize(StreamReader reader)
        {
            System.Collections.ObjectModel.Collection<PSParseError> errors;
            PSParser.Tokenize(reader.ReadToEnd(), out errors);
        }
    }
}
