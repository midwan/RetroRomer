using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RetroRomer;

namespace RetroRomerTest
{
    [TestClass]
    public class FileWriterTest
    {
        private FileWriter _fileWriter;
        private const string Filename = @"D:\Downloads\test.txt";
        
        [TestInitialize]
        public void Initialize()
        {
            _fileWriter = new FileWriter();
        }

        [TestMethod]
        public void TestWriteFile()
        {
            var fileContents = new List<string>
            {
                "this is line 1",
                "this is line 2",
                "this is line 3"
            };

            var result = _fileWriter.WriteFile(Filename, fileContents);

            Assert.IsTrue(result);
            Assert.IsTrue(File.Exists(Filename));
            Assert.AreEqual(3, File.ReadAllLines(Filename).Length);
        }
    }
}
