using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace RetroRommer.Domain
{
    public class RetroRommerService
    {
        private readonly ILogger _logger;

        public RetroRommerService(ILogger logger)
        {
            _logger = logger;
        }

        public IEnumerable<string> ReadFile(string file)
        {
            if (string.IsNullOrEmpty(file) || string.IsNullOrWhiteSpace(file)) return new List<string>();
            if (!File.Exists(file)) return new List<string>();
            try
            {
                var contents = File.ReadAllLines(file);
                var contentsToList = new List<string>(contents);
                return contentsToList;
            }
            catch (Exception e)
            {
                // ReSharper disable once RedundantVerbatimPrefix
                _logger.Fatal($"Failed to read file {file}\n{@e}");
                return new List<string>();
            }
        }

        public IEnumerable<string> AddFilenameExtensionToEntries(IEnumerable<string> fileContents)
        {
            var modifiedList = new List<string>();
            foreach (var entry in fileContents)
                if (!string.IsNullOrWhiteSpace(entry) && !entry.Contains("You are missing"))
                    modifiedList.Add($"{entry}.zip");
            return modifiedList;
        }

        public async Task<string> GetFile(string file, string userName, string passwd, string destination)
        {
            using var client = new HttpClient();
            var authToken = Encoding.ASCII.GetBytes($"{userName}:{passwd}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(authToken));

            try
            {
                var fileBytes = await client.GetByteArrayAsync(file);
                var localPath = Path.Combine(destination, file);
                _logger.Information($"Downloading file from {file} to {localPath}");
                await File.WriteAllBytesAsync(localPath, fileBytes);
            }
            catch (Exception ex)
            {
                string result;
                // ReSharper disable once RedundantVerbatimPrefix
                _logger.Fatal($"File failed to download: {file}.\n{@ex}");
                var pos1 = ex.Message.IndexOf("(", StringComparison.Ordinal) + 1;
                var pos2 = ex.Message.IndexOf(")", StringComparison.Ordinal);
                if (pos1 != -1 && pos2 != -1)
                    result = ex.Message.Substring(pos1, pos2 - pos1);
                else
                    result = "Error!";

                return result;
            }

            return "OK";
        }
    }
}