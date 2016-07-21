using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Commander
{
    class Commands
    {
        public static readonly RoutedUICommand Copy = new RoutedUICommand("Copy", "Copy", typeof(Commands));
    }
}
