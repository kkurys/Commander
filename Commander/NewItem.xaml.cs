using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Commander
{
    /// <summary>
    /// Interaction logic for NewItem.xaml
    /// </summary>
    public partial class NewItem : Window
    {
        private string itemType;
        public NewItem()
        {
            InitializeComponent();
        }
        // NAZWA + ROZSZERZENIE PLIKU !!!

        #region methods
        public void SetItemType(string argItemType)
        {
            itemType = argItemType;
        }
        #endregion

        #region events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = "Utwórz nowy " + itemType;
            NewItemMessageLabel.Content = "Podaj nazwę nowego " + itemType + "u";
        }

        private void OkClose(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
        #endregion

    }
}
