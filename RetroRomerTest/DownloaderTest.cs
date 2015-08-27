using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RetroRomer;

namespace RetroRomerTest
{
    [TestClass]
    public class DownloaderTest
    {
        private Downloader _downloader;
        private readonly Uri _website = new Uri(@"http://blitterstudio.com/files/");
        private const string Username = "midwan";
        private const string Password = "";
        private const string DestinationPath = @"D:\Downloads";
        private const string Filename = "setup.exe";

        [TestInitialize]
        public void Initialize()
        {
            _downloader = new Downloader();
        }

        [TestMethod]
        public void TestDownloadFile_ValidFile_DownloadsFile()
        {
            _downloader.Website = _website;
            _downloader.Username = Username;
            _downloader.Password = Password;
            _downloader.DestinationPath = DestinationPath;

            var result = _downloader.GetFile(Filename);

            Assert.IsTrue(result);
            Assert.IsTrue(File.Exists(Path.Combine(DestinationPath, Filename)));
        }

        [TestMethod]
        public void TestDownloadFile_InvalidFile()
        {
            _downloader.Website = _website;
            _downloader.Username = Username;
            _downloader.Password = Password;
            _downloader.DestinationPath = DestinationPath;
            const string invalidFilename = "setup1.exe";

            var result = _downloader.GetFile(invalidFilename);

            Assert.IsFalse(result);
        }
    }
}
