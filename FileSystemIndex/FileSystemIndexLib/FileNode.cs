using System;
using System.IO;

namespace Erwine.Leonard.T.GDIPlus
{
    public class FileNode : CrawledComponent<NameAndExtension>
    {
		protected const string PropertyName_Length = "Length";
		protected const string PropertyName_BaseName = "BaseName";
		protected const string PropertyName_Extension = "Extension";
		protected const string PropertyName_Name = "Name";
		protected const string PropertyName_Directory = "Directory";
		
		private ICrawledComponent<NameAndExtension> _key;
        private long _length = 0L;
		private ICrawlComponentContainer<NameAndExtension> _directory;
		
		public long Length { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
		
		#region ICrawledComponent<NameAndExtension> Implementation
		
        NameAndExtension ICrawledComponent<NameAndExtension>.Key { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
		public string BaseName { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
		public string Extension { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
		public string Name { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
		public ICrawlComponentContainer<NameAndExtension> Directory { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        ICrawlComponentContainer<NameAndExtension> ICrawledComponent<NameAndExtension>.Parent { get; set; }
		public FileNode Clone(ICrawlComponentContainer<TKey> parent);
        ICrawledComponent<NameAndExtension> ICrawledComponent<NameAndExtension>.Clone(ICrawlComponentContainer<TKey> parent);
        ICrawledComponent<NameAndExtension> ICrawledComponent<NameAndExtension>.Clone();
		
		#endregion
		
		/*
Attributes                Property       System.IO.FileAttributes Attributes {get;set;}                                                                                    
CreationTime              Property       datetime CreationTime {get;set;}                                                                                                  
CreationTimeUtc           Property       datetime CreationTimeUtc {get;set;}                                                                                               
Directory                 Property       System.IO.DirectoryInfo Directory {get;}                                                                                          
DirectoryName             Property       string DirectoryName {get;}                                                                                                       
Exists                    Property       bool Exists {get;}                                                                                                                
Extension                 Property       string Extension {get;}                                                                                                           
FullName                  Property       string FullName {get;}                                                                                                            
IsReadOnly                Property       bool IsReadOnly {get;set;}                                                                                                        
LastAccessTime            Property       datetime LastAccessTime {get;set;}                                                                                                
LastAccessTimeUtc         Property       datetime LastAccessTimeUtc {get;set;}                                                                                             
LastWriteTime             Property       datetime LastWriteTime {get;set;}                                                                                                 
LastWriteTimeUtc          Property       datetime LastWriteTimeUtc {get;set;}                                                                                              
Length                    Property       long Length {get;}                                                                                                                
Name                      Property       string Name {get;}                                                                                                                
BaseName                  ScriptProperty System.Object BaseName {get=if ($this.Extension.Length -gt 0){$this.Name.Remove($this.Name.Length - $this.Extension.Length)}els...
VersionInfo               ScriptProperty System.Object VersionInfo {get=[System.Diagnostics.FileVersionInfo]::GetVersionInfo($this.FullName);}                             
		*/
        public FileNode() { }

        public FileNode(FileInfo file) : base(file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            throw new NotImplementedException();
        }

        public FileNode(FileNode item)
            : base(item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            throw new NotImplementedException();
        }

        public override string GetFullName()
        {
            throw new NotImplementedException();
        }
    }
}