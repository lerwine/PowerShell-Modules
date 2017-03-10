using System;
using System.IO;

namespace Erwine.Leonard.T.GDIPlus
{
    /// <summary>
    /// 
    /// </summary>
    public class DirectoryNode : CrawledComponent<NameAndExtension>, ICrawlComponentContainer<NameAndExtension>
    {
        /// <summary>
        /// 
        /// </summary>
        protected const string PropertyName_Name = "Name";

        /// <summary>
        /// 
        /// </summary>
        protected const string PropertyName_Directory = "Directory";

        /// <summary>
        /// 
        /// </summary>
        protected const string PropertyName_Items = "Items";
        
        private CrawlComponentCollection<NameAndExtension> _items;
        
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
        public CrawlComponentCollection<NameAndExtension> Items { get { return _items; } set { _items = (value == null) ? new CrawlComponentCollection<NameAndExtension>() : value; } }
        
        CrawlComponentCollection<NameAndExtension> ICrawlComponentContainer<NameAndExtension>.ItemCollection { get { throw new NotImplementedException(); } }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public DirectoryNode Clone(ICrawlComponentContainer<NameAndExtension> parent) { throw new NotImplementedException(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        protected override CrawledComponent<NameAndExtension> CreateClone(ICrawlComponentContainer<NameAndExtension> parent)
        {
            return new DirectoryNode(this, parent);
        }

        /// <summary>
        /// 
        /// </summary>
        public DirectoryNode() { throw new NotImplementedException(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        public DirectoryNode(DirectoryInfo directory) : base()
        {
            if (directory == null)
                throw new ArgumentNullException("directory");

            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="parent"></param>
        public DirectoryNode(DirectoryNode item, ICrawlComponentContainer<NameAndExtension> parent)
            : base(item, parent)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            throw new NotImplementedException();
        }
    }
}