using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FileSystemIndexLib
{
    /// <summary>
    /// Represents information about a file system volume.
    /// </summary>
    [Serializable()]
    public class VolumeInformation : IEquatable<VolumeInformation>
    {
        private string _rootPathName = "";
        private string _volumeName = "";
        private string _fileSystemName = "";
        private uint _volumeSerialNumber = 0;
        private uint _maximumComponentLength = 0;
        private FileSystemFeature _fileSystemFlags = FileSystemFeature.None;

        /// <summary>
        /// The root directory of the volume.
        /// </summary>
        public string RootPathName
        {
            get { return _rootPathName; }
            set { _rootPathName = value ?? ""; }
        }

        /// <summary>
        /// The name of the volume.
        /// </summary>
        public string VolumeName
        {
            get { return _volumeName; }
            set { _volumeName = value ?? ""; }
        }

        /// <summary>
        /// The name of the file system.
        /// </summary>
        public string FileSystemName
        {
            get { return _fileSystemName; }
            set { _fileSystemName = value ?? ""; }
        }

        /// <summary>
        /// Serial number assigned when operating system formatted volume.
        /// </summary>
        public uint VolumeSerialNumber
        {
            get { return _volumeSerialNumber; }
            set { _volumeSerialNumber = value; }
        }

        /// <summary>
        /// The maximum length, in TCHARs, of a file name component that the file system supports.
        /// </summary>
        public uint MaximumComponentLength
        {
            get { return _maximumComponentLength; }
            set { _maximumComponentLength = value; }
        }

        /// <summary>
        /// Flags describing the capabilities of the filesystem.
        /// </summary>
        public FileSystemFeature FileSystemFlags
        {
            get { return _fileSystemFlags; }
            set { _fileSystemFlags = value; }
        }

        private void SetFlag(FileSystemFeature flag, bool value)
        {
            if (value)
                _fileSystemFlags |= flag;
            else
                _fileSystemFlags &= ~flag;
        }

        /// <summary>
        /// The file system preserves the case of file names when it places a name on disk.
        /// </summary>
        public bool CasePreservedNames
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.CasePreservedNames); }
            set { SetFlag(FileSystemFeature.CasePreservedNames, value); }
        }

        /// <summary>
        /// The file system supports case-sensitive file names.
        /// </summary>
        public bool CaseSensitiveSearch
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.CaseSensitiveSearch); }
            set { SetFlag(FileSystemFeature.CaseSensitiveSearch, value); }
        }

        /// <summary>
        /// The specified volume is a direct access (DAX) volume. This flag was introduced in Windows 10, version 1607.
        /// </summary>
        public bool DaxVolume
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.DaxVolume); }
            set { SetFlag(FileSystemFeature.DaxVolume, value); }
        }

        /// <summary>
        /// The file system supports file-based compression.
        /// </summary>
        public bool FileCompression
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.FileCompression); }
            set { SetFlag(FileSystemFeature.FileCompression, value); }
        }

        /// <summary>
        /// The file system supports named streams.
        /// </summary>
        public bool NamedStreams
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.NamedStreams); }
            set { SetFlag(FileSystemFeature.NamedStreams, value); }
        }

        /// <summary>
        /// The file system preserves and enforces access control lists (ACL).
        /// </summary>
        public bool PersistentACLS
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.PersistentACLS); }
            set { SetFlag(FileSystemFeature.PersistentACLS, value); }
        }

        /// <summary>
        /// The specified volume is read-only.
        /// </summary>
        public bool ReadOnlyVolume
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.ReadOnlyVolume); }
            set { SetFlag(FileSystemFeature.ReadOnlyVolume, value); }
        }

        /// <summary>
        /// The volume supports a single sequential write.
        /// </summary>
        public bool SequentialWriteOnce
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SequentialWriteOnce); }
            set { SetFlag(FileSystemFeature.SequentialWriteOnce, value); }
        }

        /// <summary>
        /// The file system supports the Encrypted File System (EFS).
        /// </summary>
        public bool SupportsEncryption
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SupportsEncryption); }
            set { SetFlag(FileSystemFeature.SupportsEncryption, value); }
        }

        /// <summary>
        /// The specified volume supports extended attributes. An extended attribute is a piece of
        /// application-specific metadata that an application can associate with a file and is not part
        /// of the file's data.
        /// </summary>
        public bool SupportsExtendedAttributes
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SupportsExtendedAttributes); }
            set { SetFlag(FileSystemFeature.SupportsExtendedAttributes, value); }
        }

        /// <summary>
        /// The specified volume supports hard links. For more information, see Hard Links and Junctions.
        /// </summary>
        public bool SupportsHardLinks
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SupportsHardLinks); }
            set { SetFlag(FileSystemFeature.SupportsHardLinks, value); }
        }

        /// <summary>
        /// The file system supports object identifiers.
        /// </summary>
        public bool SupportsObjectIDs
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SupportsObjectIDs); }
            set { SetFlag(FileSystemFeature.SupportsObjectIDs, value); }
        }

        /// <summary>
        /// The file system supports open by FileID. For more information, see FILE_ID_BOTH_DIR_INFO.
        /// </summary>
        public bool SupportsOpenByFileId
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SupportsOpenByFileId); }
            set { SetFlag(FileSystemFeature.SupportsOpenByFileId, value); }
        }

        /// <summary>
        /// The file system supports re-parse points.
        /// </summary>
        public bool SupportsReparsePoints
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SupportsReparsePoints); }
            set { SetFlag(FileSystemFeature.SupportsReparsePoints, value); }
        }

        /// <summary>
        /// The file system supports sparse files.
        /// </summary>
        public bool SupportsSparseFiles
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SupportsSparseFiles); }
            set { SetFlag(FileSystemFeature.SupportsSparseFiles, value); }
        }

        /// <summary>
        /// The volume supports transactions.
        /// </summary>
        public bool SupportsTransactions
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SupportsTransactions); }
            set { SetFlag(FileSystemFeature.SupportsTransactions, value); }
        }

        /// <summary>
        /// The specified volume supports update sequence number (USN) journals. For more information,
        /// see Change Journal Records.
        /// </summary>
        public bool SupportsUsnJournal
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SupportsUsnJournal); }
            set { SetFlag(FileSystemFeature.SupportsUsnJournal, value); }
        }

        /// <summary>
        /// The file system supports Unicode in file names as they appear on disk.
        /// </summary>
        public bool UnicodeOnDisk
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.UnicodeOnDisk); }
            set { SetFlag(FileSystemFeature.UnicodeOnDisk, value); }
        }

        /// <summary>
        /// The specified volume is a compressed volume, for example, a DoubleSpace volume.
        /// </summary>
        public bool VolumeIsCompressed
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.VolumeIsCompressed); }
            set { SetFlag(FileSystemFeature.VolumeIsCompressed, value); }
        }

        /// <summary>
        /// The file system supports disk quotas.
        /// </summary>
        public bool VolumeQuotas
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.VolumeQuotas); }
            set { SetFlag(FileSystemFeature.VolumeQuotas, value); }
        }

        /// <summary>
        /// Retrieves information about the file system and volume associated with the specified root directory.
        /// </summary>
        /// <param name="rootPathName">A string that contains the root directory of the volume to be described.</param>
        /// <param name="volumeNameBuffer">A buffer that receives the name of a specified volume. The buffer size is specified by the <see cref="volumeNameSize"/> parameter.</param>
        /// <param name="volumeNameSize">The length of the <see cref="volumeNameBuffer"/>, in TCHARs. The maximum buffer size is MAX_PATH+1.</param>
        /// <param name="volumeSerialNumber">A variable that receives the volume serial number.</param>
        /// <param name="maximumComponentLength">A variable that receives the maximum length, in TCHARs, of a file name component that a specified file system supports.</param>
        /// <param name="fileSystemFlags">A variable that receives flags associated with the specified file system.</param>
        /// <param name="fileSystemNameBuffer">A buffer that receives the name of the file system, for example, the FAT file system or the NTFS file system. The buffer size is specified by the <see cref="nFileSystemNameSize"/> parameter.</param>
        /// <param name="nFileSystemNameSize">The length of the <see cref="fileSystemNameBuffer"/>, in TCHARs. The maximum buffer size is MAX_PATH+1.</param>
        /// <returns>If all the requested information is retrieved, the return value is nonzero. If not all the requested information is retrieved, the return value is zero.</returns>
        /// <remarks>To get extended error information, use <see cref="Marshal.GetHRForLastWin32Error"/>.</remarks>
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool GetVolumeInformation(string rootPathName, StringBuilder volumeNameBuffer, int volumeNameSize, out uint volumeSerialNumber,
            out uint maximumComponentLength, out FileSystemFeature fileSystemFlags, StringBuilder fileSystemNameBuffer, int nFileSystemNameSize);

        /// <summary>
        /// Create new empty instance of <see cref="VolumeInformation"/>
        /// </summary>
        public VolumeInformation() { }

        /// <summary>
        /// Create new instance of <see cref="VolumeInformation"/> from a root path name.
        /// </summary>
        /// <param name="rootPathName"></param>
        public VolumeInformation(string rootPathName)
        {
            if (rootPathName == null)
                throw new ArgumentNullException("rootPathName");

            if (rootPathName.Length == 0)
                throw new ArgumentException("Root path name cannot be empty.", "rootPathName");

            RootPathName = rootPathName;
            Refresh();
        }

        public void Refresh()
        {
            if (RootPathName.Length == 0)
                throw new InvalidOperationException("Root path name is empty.");

            StringBuilder volumeNameBuffer = new StringBuilder(261);
            StringBuilder fileSystemNameBuffer = new StringBuilder(261);
            uint volumeSerialNumber, maximumComponentLength;
            FileSystemFeature fileSystemFlags;
            if (!GetVolumeInformation(RootPathName, volumeNameBuffer, volumeNameBuffer.Capacity, out volumeSerialNumber, out maximumComponentLength,
                    out fileSystemFlags, fileSystemNameBuffer, fileSystemNameBuffer.Capacity))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

            VolumeName = volumeNameBuffer.ToString();
            FileSystemName = fileSystemNameBuffer.ToString();
            VolumeSerialNumber = volumeSerialNumber;
            MaximumComponentLength = maximumComponentLength;
            FileSystemFlags = fileSystemFlags;
        }

        public override string ToString() { return String.Format("{0:X4}-{1:X4}", _volumeSerialNumber >> 16, _volumeSerialNumber & 0x0000FFFF); }

        public override int GetHashCode() { return _volumeSerialNumber.GetHashCode(); }

        public bool Equals(VolumeInformation other)
        {
            return other != null && (ReferenceEquals(this, other) || (_volumeSerialNumber == other._volumeSerialNumber && _fileSystemFlags == other._fileSystemFlags &&
                _maximumComponentLength == other._maximumComponentLength && _rootPathName == other._rootPathName && _volumeName == other._volumeName &&
                _fileSystemName == other._fileSystemName));
        }

        public override bool Equals(object obj) { return Equals(obj as VolumeInformation); }
    }
}
