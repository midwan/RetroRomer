using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RetroRomer
{
    public class Downloader
    {
        public Uri Website { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DestinationPath { get; set; }

        public bool GetFile(string fileUri)
        {
            if (string.IsNullOrWhiteSpace(Website.ToString())) Website = new Uri(@"http://bda.retroroms.net/downloads/mame/currentroms/");

            var fullUri = new Uri(Website, fileUri);
            using (var client = new WebClient())
            {
                if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
                    client.Credentials = new NetworkCredential(Username, Password);
                client.DownloadFile(fullUri, Path.Combine(DestinationPath, fileUri));
            }
            return true;
        }
    }
}
