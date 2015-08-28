using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroRomer.Classes
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Information { get; set; }
        public Exception InnerException { get; set; }
    }
}
