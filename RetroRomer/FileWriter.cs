using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RetroRomer
{
    public class FileWriter
    {
        public bool WriteFile(string filename, IEnumerable<string> fileContents)
        {
            if (string.IsNullOrEmpty(filename) || string.IsNullOrWhiteSpace(filename)) return false;
            try
            {
                File.WriteAllLines(filename, fileContents);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
