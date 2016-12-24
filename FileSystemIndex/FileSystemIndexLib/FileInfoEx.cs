using System;
using System.IO;
using System.Security.Cryptography;

namespace FileSystemIndexLib
{
    public class FileInfoEx : IEquatable<FileInfoEx>
    {
        private string _name = "";
        private string _directoryName = "";
        private DateTime _creationTime;
        private DateTime _lastWriteTime;
        private HashCodeEx _hashCodeEx;

        public string Name
        {
            get { return _name; }
            set { _name = value ?? ""; }
        }

        public string DirectoryName
        {
            get { return _directoryName; }
            set { _directoryName = value ?? ""; }
        }

        public long Length
        {
            get { return _hashCodeEx.Length; }
            set
            {
                if (_hashCodeEx.Length != value)
                    _hashCodeEx = new HashCodeEx(_hashCodeEx.Sha256HashCode, value);
            }
        }

        public DateTime CreationTime
        {
            get { return _creationTime; }
            set { _creationTime = value; }
        }

        public DateTime LastWriteTime
        {
            get { return _lastWriteTime; }
            set { _lastWriteTime = value; }
        }

        public int Sha256HashCode
        {
            get { return _hashCodeEx.Sha256HashCode; }
            set
            {
                if (_hashCodeEx.Sha256HashCode != value)
                    _hashCodeEx = new HashCodeEx(value, _hashCodeEx.Length);
            }
        }

        public long LongHashCode { get { return _hashCodeEx.LongHashCode; } }

        public FileInfoEx() { }

        public FileInfoEx(string name, string directoryName, long length, DateTime creationTime, DateTime lastWriteTime, int sha256HashCode)
        {
            Name = name;
            DirectoryName = directoryName;
            CreationTime = creationTime;
            LastWriteTime = lastWriteTime;
            _hashCodeEx = new HashCodeEx(sha256HashCode, length);
        }

        public FileInfoEx(FileInfo fileInfo, int sha256HashCode)
        {
            if (fileInfo == null)
                throw new ArgumentNullException("fileInfo");

            Name = fileInfo.Name;
            DirectoryName = fileInfo.DirectoryName;
            CreationTime = fileInfo.CreationTime;
            LastWriteTime = fileInfo.LastWriteTime;
            _hashCodeEx = new HashCodeEx(sha256HashCode, fileInfo.Length);
        }

        public static FileInfoEx Create(FileInfo fileInfo, SHA256Managed algorithm)
        {
            if (fileInfo == null)
                throw new ArgumentNullException("fileInfo");

            if (algorithm == null)
                throw new ArgumentNullException("algorithm");

            using (FileStream fileStream = fileInfo.OpenRead())
                return new FileInfoEx(fileInfo, GetChecksum(fileStream, algorithm));
        }

        public static FileInfoEx Create(FileInfo fileInfo)
        {
            if (fileInfo == null)
                throw new ArgumentNullException("fileInfo");

            using (FileStream fileStream = fileInfo.OpenRead())
                return new FileInfoEx(fileInfo, GetChecksum(fileStream));
        }

        public override string ToString() { return String.Format("{0}: {1}", _hashCodeEx.ToString(), Path.Combine(_directoryName, _name)); }

        public override int GetHashCode() { return _hashCodeEx.Sha256HashCode; }

        public bool Equals(FileInfoEx other)
        {
            return other != null && (ReferenceEquals(this, other) || (_hashCodeEx.Equals(other._hashCodeEx) && _name == other._name &&
                _directoryName == other._directoryName && _creationTime == other._creationTime && _lastWriteTime == other._lastWriteTime));
        }

        public override bool Equals(object obj) { return Equals(obj as FileInfoEx); }

        public static SHA256Managed GetHashAlgorithm() { return new SHA256Managed(); }

        public static int GetChecksum(Stream stream, SHA256Managed algorithm)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm");

            if (stream == null)
                throw new ArgumentNullException("stream");

            byte[] checksum = algorithm.ComputeHash(stream);
            return BitConverter.ToInt32(checksum, 0);
        }

        public static int GetChecksum(string file, SHA256Managed algorithm)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            if (file.Length == 0)
                throw new ArgumentException("File name cannot be empty.", "file");

            using (FileStream stream = File.OpenRead(file))
                return GetChecksum(file, algorithm);
        }

        public static int GetChecksum(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (SHA256Managed algorithm = GetHashAlgorithm())
                return GetChecksum(stream, algorithm);
        }

        public static int GetChecksum(string file)
        {
            using (SHA256Managed algorithm = GetHashAlgorithm())
                return GetChecksum(file, algorithm);
        }

        public static bool HasFlag(FileAttributes value, FileAttributes flag) { return value.HasFlag(flag); }
    }
}
