using System;
using System.IO;
using System.Net;
using RetroRomer.Classes;

namespace RetroRomer
{
    public class Downloader
    {
        public Uri Website { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DestinationPath { get; set; }

        public OperationResult GetFile(string fileUri)
        {
            if (Website == null) Website = new Uri(@"http://bda.retroroms.net/downloads/mame/currentroms/");

            var fullUri = new Uri(Website, fileUri);
            using (var client = new WebClient())
            {
                if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
                    client.Credentials = new NetworkCredential(Username, Password);

                try
                {
                    client.DownloadFile(fullUri, Path.Combine(DestinationPath, fileUri));
                }
                catch (Exception ex)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Information = ex.Message,
                        InnerException = ex.InnerException
                    };
                }
            }
            return new OperationResult {Success = true};
        }
    }
}