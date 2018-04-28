using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Text;

namespace Erwine.Leonard.T.GDIPlus.Commands
{
    /// <summary>
    /// Open-Image
    /// </summary>
    [Cmdlet(VerbsCommon.Open, "Image", DefaultParameterSetName = ParameterSetName_Path, RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(Image))]
    public class Open_Image : PSCmdlet
    {
        public const string ParameterSetName_Path = "Path";
        public const string ParameterSetName_LiteralPath = "LiteralPath";

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_Path)]
        [ValidateNotNullOrEmpty()]
        [Alias("PSPath")]
        public string[] Path { get; set; }
        
        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_LiteralPath)]
        [ValidateNotNullOrEmpty()]
        public string LiteralPath { get; set; }

        private static void FromContentReader(Collection<IContentReader> readerCollection, Stream stream)
        {
            if (readerCollection == null || readerCollection.Count == 0)
                return;

            foreach (IContentReader reader in readerCollection)
            {
                IList item = reader.Read(1);
                if (item == null || item.Count == 0)
                    continue;
                if (item[0] is byte)
                {
                    stream.WriteByte((byte)(item[0]));
                    FromBinaryContentReader(reader, stream);
                }
                else
                {
                    UTF8Encoding binaryEncoding = new UTF8Encoding(false);

                    WriteContent(item, stream, binaryEncoding);
                    FromContentReader(reader, stream, binaryEncoding);
                }
            }
        }

        private static void WriteContent(IList item, Stream stream, UTF8Encoding binaryEncoding)
        {
            byte[] buffer;
            if (item is byte[])
                buffer = (byte[])item;
            else if (item is IEnumerable<byte>)
                buffer = ((IEnumerable<byte>)item).ToArray();
            else if (item is char[])
            {
                char[] c = (char[])item;
                buffer = (c.Length == 0) ? new byte[0] : binaryEncoding.GetBytes(c, 0, c.Length);
            }
            else if (item is IEnumerable<char>)
            {
                char[] c = ((IEnumerable<char>)item).ToArray();
                buffer = (c.Length == 0) ? new byte[0] : binaryEncoding.GetBytes(c, 0, c.Length);
            }
            else
            {
                foreach (object obj in item)
                {
                    if (obj is byte)
                    {
                        stream.WriteByte((byte)obj);
                        continue;
                    }
                    if (obj is byte[])
                        buffer = (byte[])item;
                    else if (obj is IEnumerable<byte>)
                        buffer = ((IEnumerable<byte>)obj).ToArray();
                    else if (obj is string)
                    {
                        string s = obj as string;
                        buffer = (s.Length == 0) ? new byte[0] : binaryEncoding.GetBytes(s);
                    }
                    else if (obj is char)
                        buffer = binaryEncoding.GetBytes(new char[] { (char)obj }, 0, 1);
                    else if (obj is char[])
                    {
                        char[] c = (char[])obj;
                        buffer = (c.Length == 0) ? new byte[0] : binaryEncoding.GetBytes(c, 0, c.Length);
                    }
                    else if (obj is IEnumerable<char>)
                    {
                        char[] c = ((IEnumerable<char>)obj).ToArray();
                        buffer = (c.Length == 0) ? new byte[0] : binaryEncoding.GetBytes(c, 0, c.Length);
                    }
                    else
                    {
                        string s = LanguagePrimitives.ConvertTo(obj, typeof(string)) as string;
                        buffer = (String.IsNullOrEmpty(s)) ? new byte[0] : binaryEncoding.GetBytes(s);
                    }
                    if (buffer.Length > 0)
                        stream.Write(buffer, 0, buffer.Length);
                }
                return;
            }
            
            if (buffer.Length > 0)
                stream.Write(buffer, 0, buffer.Length);
        }

        private static void FromBinaryContentReader(IContentReader reader, Stream stream)
        {
            for (IList item = reader.Read(32768); item != null && item.Count > 0; item = reader.Read(32768))
            {
                byte[] buffer;
                if (item is byte[])
                    buffer = (byte[])item;
                else if (item is IEnumerable<byte>)
                    buffer = ((IEnumerable<byte>)item).ToArray();
                else
                {
                    for (int i = 0; i < item.Count; i++)
                    {
                        if (item[i] is byte)
                            stream.WriteByte((byte)(item[i]));
                        else
                        {
                            UTF8Encoding binaryEncoding = new UTF8Encoding(false);
                            if (i == 0)
                                WriteContent(item, stream, binaryEncoding);
                            else
                                WriteContent(item.Cast<object>().Skip(i).ToArray(), stream, binaryEncoding);
                            FromContentReader(reader, stream, binaryEncoding);
                            return;
                        }
                    }
                    continue;
                }
                if (buffer.Length > 0)
                    stream.Write(buffer, 0, buffer.Length);
            }
        }

        private static void FromContentReader(IContentReader reader, Stream stream, UTF8Encoding binaryEncoding)
        {
            for (IList item = reader.Read(32768); item != null && item.Count > 0; item = reader.Read(32768))
                WriteContent(item, stream, binaryEncoding);
        }

        protected override void ProcessRecord()
        {
            Collection<PathInfo> pathCollection;
            if (ParameterSetName == ParameterSetName_Path)
            {
                if (Path == null)
                    return;
                foreach (string psPath in Path)
                {
                    if (String.IsNullOrEmpty(psPath))
                        continue;

                    try
                    {
                        if ((pathCollection = SessionState.Path.GetResolvedPSPathFromPSPath(psPath)) == null || pathCollection.Count == 0)
                            throw new ItemNotFoundException("\"" + psPath + "\" not found.");
                    }
                    catch (ItemNotFoundException e)
                    {
                        WriteError(new ErrorRecord(e, "Open_Image.NotFound", ErrorCategory.ObjectNotFound, psPath));
                        continue;
                    }
                    catch (System.Management.Automation.DriveNotFoundException e)
                    {
                        WriteError(new ErrorRecord(e, "Open_Image.NotFound", ErrorCategory.ObjectNotFound, psPath));
                        continue;
                    }
                    catch (System.IO.DriveNotFoundException e)
                    {
                        WriteError(new ErrorRecord(e, "Open_Image.NotFound", ErrorCategory.ObjectNotFound, psPath));
                        continue;
                    }
                    catch (Exception e)
                    {
                        WriteError(new ErrorRecord(e, "Open_Image.Path", ErrorCategory.InvalidArgument, psPath));
                        continue;
                    }
                    foreach (PathInfo path in pathCollection)
                    {
                        bool fileExists;
                        try { fileExists = File.Exists(path.ProviderPath); }
                        catch { fileExists = false; }
                        if (fileExists)
                        {
                            try { WriteObject(Image.FromFile(path.ProviderPath)); }
                            catch (Exception ex)
                            {
                                WriteError(new ErrorRecord(ex, "Open_Image.InvalidImage", ErrorCategory.InvalidData, psPath));
                            }
                        }
                        else
                            ProcessPSPath(path.Path);
                    }
                }
            }
            else
            {
                string path;
                try
                {
                    path = SessionState.Path.GetUnresolvedProviderPathFromPSPath(LiteralPath);
                    if (String.IsNullOrEmpty(path))
                        throw new ItemNotFoundException("\"" + LiteralPath + "\" not found.");
                }
                catch (ItemNotFoundException e)
                {
                    WriteError(new ErrorRecord(e, "Open_Image.NotFound", ErrorCategory.ObjectNotFound, LiteralPath));
                    return;
                }
                catch (System.Management.Automation.DriveNotFoundException e)
                {
                    WriteError(new ErrorRecord(e, "Open_Image.NotFound", ErrorCategory.ObjectNotFound, LiteralPath));
                    return;
                }
                catch (System.IO.DriveNotFoundException e)
                {
                    WriteError(new ErrorRecord(e, "Open_Image.NotFound", ErrorCategory.ObjectNotFound, LiteralPath));
                    return;
                }
                catch (Exception e)
                {
                    WriteError(new ErrorRecord(e, "Open_Image.Path", ErrorCategory.InvalidArgument, LiteralPath));
                    return;
                }
        
                bool fileExists;
                try { fileExists = File.Exists(path); }
                catch { fileExists = false; }
                if (fileExists)
                {
                    try { WriteObject(Image.FromFile(path)); }
                    catch (Exception ex)
                    {
                        WriteError(new ErrorRecord(ex, "Open_Image.InvalidImage", ErrorCategory.InvalidData, LiteralPath));
                    }
                }
                else
                    ProcessPSPath(LiteralPath);
            }
        }

        private void ProcessPSPath(string path)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Collection<IContentReader> readerCollection = null;
                try
                {
                    readerCollection = InvokeProvider.Content.GetReader(path);
                }
                catch (PSNotSupportedException ex)
                {
                    WriteError(new ErrorRecord(ex, "ContentAccessNotSupported", ErrorCategory.NotImplemented, path));
                    return;
                }
                catch (Exception ex)
                {
                    WriteError(new ErrorRecord(ex, "Open_Image.OpenError", ErrorCategory.OpenError, path));
                    return;
                }

                try { FromContentReader(readerCollection, stream); }
                catch (Exception ex)
                {
                    WriteError(new ErrorRecord(ex, "Open_Image.ReadError", ErrorCategory.ReadError, path));
                    return;
                }

                try
                {
                    if (stream.Length == 0L)
                        throw new FormatException("Item is empty.");
                    stream.Seek(0L, SeekOrigin.Begin);
                    WriteObject(Image.FromStream(stream));
                }
                catch (Exception ex)
                {
                    WriteError(new ErrorRecord(ex, "Open_Image.InvalidData", ErrorCategory.InvalidData, path));
                }
            }
        }
    }
}
