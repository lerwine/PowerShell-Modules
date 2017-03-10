using System;
using System.IO;

namespace Erwine.Leonard.T.GDIPlus
{
    /// <summary>
    /// 
    /// </summary>
    public class FileNode : CrawledComponent<NameAndExtension>
    {
        /// <summary>
        /// 
        /// </summary>
        protected const string PropertyName_Length = "Length";

        /// <summary>
        /// 
        /// </summary>
        protected const string PropertyName_BaseName = "BaseName";

        /// <summary>
        /// 
        /// </summary>
        protected const string PropertyName_Extension = "Extension";

        /// <summary>
        /// 
        /// </summary>
        protected const string PropertyName_Name = "Name";

        /// <summary>
        /// 
        /// </summary>
        protected const string PropertyName_Directory = "Directory";
        
        private long _length = 0L;
        
        /// <summary>
        /// 
        /// </summary>
        public long Length { get { return _length; } set { _length = (value < 0L) ? 0L : value; } }
        
        #region ICrawledComponent<NameAndExtension> Implementation
        
        /// <summary>
        /// 
        /// </summary>
        public string BaseName { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        /// <summary>
        /// 
        /// </summary>
        public string Extension { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        /// <summary>
        /// 
        /// </summary>
        public ICrawlComponentContainer<NameAndExtension> Directory { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public FileNode Clone(ICrawlComponentContainer<NameAndExtension> parent) { throw new NotImplementedException(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        protected override CrawledComponent<NameAndExtension> CreateClone(ICrawlComponentContainer<NameAndExtension> parent)
        {
            return new FileNode(this, parent);
        }

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
        /// <summary>
        /// 
        /// </summary>
        public FileNode() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        public FileNode(FileInfo file) : base()
        {
            if (file == null)
                throw new ArgumentNullException("file");

            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="parent"></param>
        public FileNode(FileNode item, ICrawlComponentContainer<NameAndExtension> parent)
            : base(item, parent)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            throw new NotImplementedException();
        }
    }
}