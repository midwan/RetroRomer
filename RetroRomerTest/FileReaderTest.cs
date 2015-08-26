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
        private const string Filename = @"D:\Downloads\MAMEUI_miss.txt";

        [TestInitialize]
        public void Initialize()
        {
            _fileReader = new FileReader();

        }

        [TestMethod]
        public void TestReadFile()
        {
            var result = _fileReader.ReadFile(Filename);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public void TestProcessContents()
        {
            var contents = new List<string>
            {
                " You are missing 4288 of 35273 known MAMEUI sets (+ BIOS sets)",
                "",
                "entry1",
                "entry2",
                "entry3"
            };

            var result = _fileReader.AddFilenameExtensionToEntries(contents);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<string>));
            Assert.AreEqual("entry1.zip", result.First());
        }
    }
}
