using FileSystemIndexLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FileSystemIndexUnitTest
{
    /// <summary>
    /// Summary description for DirectoryCrawlIteratorUnitTest
    /// </summary>
    [TestClass]
    public class DirectoryCrawlIteratorUnitTest
    {
        public DirectoryCrawlIteratorUnitTest() { }
        static XDocument _testDirStructure = new XDocument(new XElement("Root",
            new XAttribute("Path", ""),
            new XElement("File", new XAttribute("Name", "File1.txt")),
            new XElement("Directory", new XAttribute("Name", "Folder1"),
                new XElement("File", new XAttribute("Name", "File2.txt")),
                new XElement("File", new XAttribute("Name", "File3.txt")),
                new XElement("File", new XAttribute("Name", "File4.txt")),
                new XElement("Directory", new XAttribute("Name", "FolderA"),
                    new XElement("File", new XAttribute("Name", "File5.txt")),
                    new XElement("File", new XAttribute("Name", "File6.txt")),
                    new XElement("File", new XAttribute("Name", "File7.txt")),
                    new XElement("File", new XAttribute("Name", "File8.txt")),
                    new XElement("File", new XAttribute("Name", "File9.txt")),
                    new XElement("File", new XAttribute("Name", "File10.txt")),
                    new XElement("File", new XAttribute("Name", "File11.txt"))
                ),
                new XElement("Directory", new XAttribute("Name", "FolderB"),
                    new XElement("Directory", new XAttribute("Name", "FolderZ"),
                        new XElement("File", new XAttribute("Name", "File12.txt")),
                        new XElement("File", new XAttribute("Name", "File13.txt")),
                        new XElement("File", new XAttribute("Name", "File14.txt"))
                    ),
                    new XElement("Directory", new XAttribute("Name", "FolderY"),
                        new XElement("File", new XAttribute("Name", "File15.txt"))
                    ),
                    new XElement("Directory", new XAttribute("Name", "FolderX"),
                        new XElement("File", new XAttribute("Name", "File16.txt"))
                    ),
                    new XElement("Directory", new XAttribute("Name", "FolderW"),
                        new XElement("File", new XAttribute("Name", "File17.txt"))
                    )
                ),
                new XElement("Directory", new XAttribute("Name", "FolderC"),
                    new XElement("Directory", new XAttribute("Name", "FolderV"),
                        new XElement("File", new XAttribute("Name", "File18.txt"))
                    )
                )
            ),
            new XElement("Directory", new XAttribute("Name", "Folder2")),
            new XElement("Directory", new XAttribute("Name", "Folder3"),
                new XElement("Directory", new XAttribute("Name", "FolderD"),
                    new XElement("Directory", new XAttribute("Name", "FolderU")),
                    new XElement("Directory", new XAttribute("Name", "FolderT"))
                ),
                new XElement("Directory", new XAttribute("Name", "FolderE"))
            ),
            new XElement("Directory", new XAttribute("Name", "Folder4"),
                new XElement("Directory", new XAttribute("Name", "FolderF"),
                    new XElement("Directory", new XAttribute("Name", "FolderS"),
                        new XElement("Directory", new XAttribute("Name", "FolderM"),
                            new XElement("Directory", new XAttribute("Name", "FolderN"),
                                new XElement("Directory", new XAttribute("Name", "FolderO"),
                                    new XElement("File", new XAttribute("Name", "File19.txt"))
                                )
                            )
                        )
                    )
                )
            )
        ));

        private TestContext _testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return _testContextInstance; }
            set { _testContextInstance = value; }
        }

        public const string PropertyKey_TestDirStructure = "TestDirStructure";

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            Random random = new Random();

            string rootPath;
            do
            {
                rootPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            } while (Directory.Exists(rootPath) || File.Exists(rootPath));

            _testDirStructure.Root.Attribute("Path").Value = rootPath;

            IterateStructure((element, path, isFile) =>
            {
                if (isFile)
                {
                    using (FileStream stream = File.OpenWrite(path))
                    {
                        int length = random.Next(1024) + 1024;
                        for (int i = 0; i < length; i++)
                        {
                            int c = random.Next(175);
                            byte[] bytes;
                            if (c > 160)
                                bytes = Encoding.Unicode.GetBytes("\t");
                            else if (c > 127)
                                bytes = Encoding.Unicode.GetBytes(" ");
                            else if (c < 32)
                                bytes = Encoding.Unicode.GetBytes("\r\n");
                            else
                                bytes = Encoding.Unicode.GetBytes(new char[] { (char)c });
                            stream.Write(bytes, 0, bytes.Length);
                        }

                        stream.Flush();
                    }
                }
                else
                    Directory.CreateDirectory(path);
            });
        }

        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            IterateStructure((element, path, isFile) =>
            {
                if (isFile)
                    File.Delete(path);
            });
            Directory.Delete(RootFolderPath, true);
        }

        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
            
        protected static string RootFolderPath { get { return _testDirStructure.Root.Attribute("Path").Value; } }

        protected static void IterateStructure(Action<XElement, string, bool> action)
        {
            string basePath = RootFolderPath;
            action(_testDirStructure.Root, basePath, false);
            foreach (XElement element in _testDirStructure.Root.Elements("File"))
                action(element, Path.Combine(basePath, element.Attribute("Name").Value), true);
            foreach (XElement element in _testDirStructure.Root.Elements("Directory"))
                IterateStructure(element, Path.Combine(basePath, element.Attribute("Name").Value), action);
        }
        
        protected static void IterateStructure(XElement folderElement, string basePath, Action<XElement, string, bool> action)
        {
            action(folderElement, basePath, false);
            foreach (XElement element in folderElement.Elements("File"))
                action(element, Path.Combine(basePath, element.Attribute("Name").Value), true);
            foreach (XElement element in folderElement.Elements("Directory"))
                IterateStructure(element, Path.Combine(basePath, element.Attribute("Name").Value), action);
        }

        [TestMethod]
        public void BasicIterationTestMethod()
        {
            DirectoryCrawlIterator iterator = new DirectoryCrawlIterator(RootFolderPath);
            IterateStructure((element, path, isfile) =>
            {
                if (isfile)
                {
                    Path.GetFileName(path);
                }
                else
                {
                    Assert.IsTrue(iterator.MoveNext());
                    Assert.IsNotNull(iterator.CurrentDirectory.FullName);
                    Assert.IsNotNull(iterator.CurrentFileList);
                    Assert.AreEqual(path, iterator.CurrentDirectory.FullName);
                }
            });
        }
    }
}
