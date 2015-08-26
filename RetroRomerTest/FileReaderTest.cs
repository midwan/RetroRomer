using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RetroRomer;

namespace RetroRomerTest
{
    [TestClass]
    public class FileReaderTest
    {
        private FileReader _fileReader;

        [TestInitialize]
        public void Initialize()
        {
            _fileReader = new FileReader();
        }

        [TestMethod]
        public void TestReadFile()
        {
            var file = @"D:\Downloads\MAMEUI_miss.txt";

            var result = _fileReader.ReadFile(file);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public void TestProcessContents()
        {
            var contents = new List<string>
            {
                "entry1",
                "entry2",
                "entry3"
            };

            var result = _fileReader.ProcessEntries(contents);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual("entry1.zip", result.First());
        }
    }
}
