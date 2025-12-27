using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Serilog;

namespace RetroRommer.Domain;

public enum DownloadType
{
    Rom,
    Bios,
    Chd,
    Sample
}

public class DownloadItem
{
    public string SetName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DownloadType Type { get; set; }
}

public class RetroRommerService
{
    private readonly ILogger _logger;

    public RetroRommerService(ILogger logger)
    {
        _logger = logger;
    }

    public IEnumerable<DownloadItem> ParseReport(string file)
    {
        var results = new List<DownloadItem>();
        if (string.IsNullOrEmpty(file) || !File.Exists(file)) return results;

        try
        {
            var lines = File.ReadAllLines(file);
            string currentSet = string.Empty;
            
            var processedSetsRom = new HashSet<string>();
            var processedSetsSample = new HashSet<string>();

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                // Check for missing items first
                if (trimmed.StartsWith("missing rom:"))
                {
                     if (string.IsNullOrEmpty(currentSet)) continue;
                     
                     var content = trimmed.Substring("missing rom:".Length).Trim();
                     var crcIndex = content.IndexOf("[", StringComparison.OrdinalIgnoreCase);
                     if (crcIndex > 0) content = content.Substring(0, crcIndex).Trim();
                     
                     if (string.IsNullOrWhiteSpace(content)) continue;

                     if (content.EndsWith(".chd", StringComparison.OrdinalIgnoreCase))
                     {
                         results.Add(new DownloadItem 
                         { 
                             SetName = currentSet, 
                             FileName = content, 
                             Type = DownloadType.Chd 
                         });
                     }
                     else
                     {
                         if (!processedSetsRom.Contains(currentSet))
                         {
                             results.Add(new DownloadItem 
                             { 
                                 SetName = currentSet, 
                                 FileName = $"{currentSet}.zip", 
                                 Type = DownloadType.Rom 
                             });
                             processedSetsRom.Add(currentSet);
                         }
                     }
                     continue;
                }
                
                if (trimmed.StartsWith("missing sample:"))
                {
                    if (string.IsNullOrEmpty(currentSet)) continue;

                    if (!processedSetsSample.Contains(currentSet))
                    {
                        results.Add(new DownloadItem 
                        { 
                            SetName = currentSet, 
                            FileName = $"{currentSet}.zip", 
                            Type = DownloadType.Sample 
                        });
                        processedSetsSample.Add(currentSet);
                    }
                    continue;
                }

                // Set detection: "Game Name [setname]"
                if (trimmed.EndsWith("]") && trimmed.Contains("["))
                {
                    // Exclude lines that start with "missing" or contain "rom:"
                    // Although we checked for "missing rom" above, "missing machine" etc might fall through.
                    if (!trimmed.StartsWith("missing", StringComparison.OrdinalIgnoreCase))
                    {
                        var lastOpen = trimmed.LastIndexOf('[');
                        if (lastOpen != -1)
                        {
                            var possibleSet = trimmed.Substring(lastOpen + 1, trimmed.Length - lastOpen - 2);
                            // Ignore metadata tags like 'sampleof:', 'cloneof:'
                            if (!possibleSet.Contains(':'))
                            {
                                currentSet = possibleSet;
                            }
                        }
                    }
                    continue;
                }
            }
        }
        catch (Exception e)
        {
             _logger.Fatal($"Failed to parse file {file}\n{e}");
        }

        return results;
    }

    public async Task<string> GetFile(string website, DownloadItem item, string userName, string passwd, string destination)
    {
        using var client = new HttpClient();
        var authToken = Encoding.ASCII.GetBytes($"{userName}:{passwd}");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

        string urlPath;
        string localFolder;
        
        if (!website.EndsWith("/")) website += "/";
        
        // Base logic
        switch (item.Type)
        {
            case DownloadType.Chd:
                urlPath = $"CHDs/{item.SetName}/{item.FileName}";
                localFolder = Path.Combine(destination, "CHDs", item.SetName);
                break;
            case DownloadType.Sample:
                 urlPath = $"samples/{item.FileName}";
                 localFolder = Path.Combine(destination, "samples");
                 break;
            case DownloadType.Bios:
                 urlPath = $"bios/{item.FileName}";
                 localFolder = Path.Combine(destination, "bios");
                 break;
            case DownloadType.Rom:
            default:
                 urlPath = $"currentroms/{item.FileName}";
                 localFolder = Path.Combine(destination, "currentroms");
                 break;
        }

        try 
        {
            return await TryDownload(client, website + urlPath, localFolder, item.FileName);
        }
        catch (HttpRequestException ex) when (item.Type == DownloadType.Rom) 
        {
             _logger.Warning($"Failed to find {item.FileName} in currentroms, trying bios folder...");
             var biosUrl = $"bios/{item.FileName}";
             var biosFolder = Path.Combine(destination, "bios");
             try 
             {
                return await TryDownload(client, website + biosUrl, biosFolder, item.FileName);
             }
             catch (Exception innerEx)
             {
                 return HandleException(item.FileName, innerEx);
             }
        }
        catch (Exception ex)
        {
             return HandleException(item.FileName, ex);
        }
    }

    private async Task<string> TryDownload(HttpClient client, string url, string folder, string fileName)
    {
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        
        _logger.Information($"Downloading {fileName} from {url} to {folder}");
        using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        
        if (!response.IsSuccessStatusCode)
        {
             throw new HttpRequestException($"HTTP {response.StatusCode}: {response.ReasonPhrase}");
        }

        using var stream = await response.Content.ReadAsStreamAsync();
        using var fileStream = new FileStream(Path.Combine(folder, fileName), FileMode.Create, FileAccess.Write, FileShare.None);
        await stream.CopyToAsync(fileStream);
        
        return "OK";
    }

    private string HandleException(string file, Exception ex)
    {
        _logger.Fatal($"File failed to download: {file}.\n{ex}");
        // Return a more readable error if possible, otherwise the exception message
        return ex.Message;
    }
}