using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    class Item
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string File { get; set; }

        public Item(string argName, string argType, string argSize)
        {
            Name = argName;
            Type = argType;
            Size = argSize;
        }

        public Item(string argName, string argType, string argSize, string argFile)
        {
            Name = argName;
            Type = argType;
            Size = argSize;
            File = argFile;
        }
    }
}
