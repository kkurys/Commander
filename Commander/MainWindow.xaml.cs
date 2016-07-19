using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Commander
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string currentPathRight, currentPathLeft;
        List<Drive> drivesList;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            drivesList = new List<Drive>();
            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in drives)
            {
                Drive newDrive = new Drive();
                try
                {
                    newDrive.Label = drive.VolumeLabel;
                }
                catch
                {
                    newDrive.Label = drive.DriveType.ToString();
                }
                newDrive.Name = drive.Name;
                drivesList.Add(newDrive);

            }
            CBLeft.ItemsSource = drivesList;
            CBRight.ItemsSource = drivesList;

            currentPathLeft = ((Drive) CBLeft.SelectedItem).Name;
            currentPathRight = ((Drive) CBRight.SelectedItem).Name;

            //LoadDirectory(currentPathRight, currentPathRight, LVRight);
            //LoadDirectory(currentPathLeft, currentPathLeft, LVLeft);
        }


        private void ListViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var currentLV = sender as ListView;
            var currentItem = currentLV.SelectedItem as Item;
            string currentPath = currentPathLeft;
            string newPath;
            bool isLeft = true;

            if (currentLV.Name == "LVRight")
            {
                isLeft = false;
                currentPath = currentPathRight;
            }

            if (PathIsDrive(currentPath))
            {
                newPath = currentPath + currentItem.Name;
            }
            else
            {
                newPath = currentPath + "\\" + currentItem.Name;
            }

            if (currentItem.Type == "<dir>")
            {
                if (isLeft)
                {
                    
                    currentPathLeft = LoadDirectory(currentPathLeft, newPath, currentLV);
                }
                else
                {
                    currentPathRight = LoadDirectory(currentPathRight, newPath, currentLV);
                }
            }
            else if (currentItem.Name == "..")
            {
                if (isLeft)
                {
                    currentPathLeft = LoadDirectory(currentPathLeft, Directory.GetParent(currentPathLeft).ToString(), currentLV);
                }
                else
                {
                    currentPathRight = LoadDirectory(currentPathRight, Directory.GetParent(currentPathRight).ToString(), currentLV);
                }
            }

        }

        private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentCB = sender as ComboBox;
            ListView currentLV = null;
            string currentPath = ((Drive)currentCB.SelectedItem).Name;

            if (currentCB.Name == "CBRight")
            {
                currentLV = LVRight;
                currentPathRight = LoadDirectory(currentPathRight, currentPath, currentLV);
            }
            else
            {
                currentLV = LVLeft;
                currentPathLeft = LoadDirectory(currentPathLeft, currentPath, currentLV);
            }
        }

        private string LoadDirectory(string oldPath, string newPath, ListView listView)
        {
            try
            {
                string[] files = Directory.GetFiles(newPath);
                string[] dirs = Directory.GetDirectories(newPath);

                listView.Items.Clear();
                
                if (!PathIsDrive(newPath))
                {
                    listView.Items.Add(new Item("..", "", ""));
                }

                foreach (string dir in dirs)
                {
                    FileInfo fileInfo = new FileInfo(dir);
                    listView.Items.Add(new Item(fileInfo.Name, "<dir>", "", dir));
                }

                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    listView.Items.Add(new Item(fileInfo.Name, fileInfo.Extension, SizeConverter(fileInfo.Length), file));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return oldPath;
            }
            return newPath;
        }

        private bool PathIsDrive(string path)
        {
            foreach (Drive drive in drivesList)
            {
                if (path == drive.Name)
                {
                    return true;
                }
            }
            return false;
        }

        private string SizeConverter(double size)
        {
            if (size < 1024)
            {
                return string.Format("{0:f2} B", size);
            }
            else if (size < 1024 * 1024)
            {
                return string.Format("{0:f2} KB", size / 1024);
            }
            else if (size < 1024 * 1024 * 1024)
            {
                return string.Format("{0:f2} MB", size / (1024 * 1024));
            }
            else
            {
                return string.Format("{0:f2} GB", size / (1024 * 1024 * 1024));
            }
        }
    }
}
