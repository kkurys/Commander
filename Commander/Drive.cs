using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    class Drive
    {
        public string Name { get; set; }
        public string Label { get; set; }

        public override string ToString()
        {
            return Label + " (" + Name.Remove(Name.Length - 1) + ")";
        }
    }
}
