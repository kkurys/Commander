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
        public static readonly RoutedUICommand CopyOrMove = new RoutedUICommand("CopyCopyOrMove", "CopyOrMove", typeof(Commands));
        public static readonly RoutedUICommand Delete = new RoutedUICommand("Delete", "Delete", typeof(Commands));
        public static readonly RoutedUICommand Refresh = new RoutedUICommand("Refresh", "Refresh", typeof(Commands));
        public static readonly RoutedUICommand CreateFileOrDirectory = new RoutedUICommand("CreateFileOrDirectory", "CreateFileOrDirectory", typeof(Commands));
    }
}
