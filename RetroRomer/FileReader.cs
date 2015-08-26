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

        public IEnumerable<string> ProcessEntries(IEnumerable<string> fileContents)
        {
            var modifiedList = fileContents.Select(entry => $"{entry}.zip").ToList();
            return modifiedList;
        }
    }
}
