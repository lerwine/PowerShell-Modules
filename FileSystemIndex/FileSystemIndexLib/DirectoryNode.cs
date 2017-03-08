using System;
using System.IO;

namespace Erwine.Leonard.T.GDIPlus
{
    public class DirectoryNode : CrawledComponent<NameAndExtension>, ICrawlComponentContainer<NameAndExtension>
    {
		protected const string PropertyName_Name = "Name";
		protected const string PropertyName_Directory = "Directory";
		protected const string PropertyName_Items = "Items";
		
		private CrawlComponentCollection<NameAndExtension> _items;
		
		public string Name { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
		
		public ICrawlComponentContainer<NameAndExtension> Directory { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        
        public CrawlComponentCollection<NameAndExtension> Items { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
		
        CrawlComponentCollection<NameAndExtension> ICrawlComponentContainer<NameAndExtension>.ItemCollection { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
		
		public DirectoryNode Clone(ICrawlComponentContainer<TKey> parent) { throw new NotImplementedException(); }
		
		#region INestedCrawlComponentContainer<NameAndExtension> Implementation
		
        INestedCrawlComponentContainer<NameAndExtension> INestedCrawlComponentContainer<NameAndExtension>.Clone(ICrawlComponentContainer<NameAndExtension> directory) { throw new NotImplementedException(); }
		
        INestedCrawlComponentContainer<NameAndExtension> INestedCrawlComponentContainer<NameAndExtension>.Clone() { throw new NotImplementedException(); }
		
		#region ICrawledComponent<NameAndExtension> Implementation
		
        public DirectoryNode() { throw new NotImplementedException(); }

        public DirectoryNode(DirectoryInfo directory) : base(file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            throw new NotImplementedException();
        }

        public DirectoryNode(DirectoryNode item)
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