using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroRomer
{
    public class FileReader
    {
        public IEnumerable<string> ReadFile(string file)
        {
            if (string.IsNullOrEmpty(file) || string.IsNullOrWhiteSpace(file)) return new List<string>();
            if (!System.IO.File.Exists(file)) return new List<string>();
            try
            {
                var contents = System.IO.File.ReadAllLines(file);
                var contentsToList = new List<string>(contents);
                return contentsToList;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public IEnumerable<string> AddFilenameExtensionToEntries(IEnumerable<string> fileContents)
        {
            var modifiedList = new List<string>();
            foreach (var entry in fileContents)
            {
                if (!string.IsNullOrWhiteSpace(entry) && !entry.Contains("You are missing"))
                {
                    modifiedList.Add($"{entry}.zip");
                }
            }
            return modifiedList;
        }
    }
}
