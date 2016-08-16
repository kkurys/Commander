using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        #region events
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
        }
        
        private void ListViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var currentLV = sender as ListView;

            if (currentLV.SelectedIndex < 0)
            {
                return;
            }

            var currentItem = currentLV.SelectedItem as Item;
            string currentPath = currentPathLeft;
            string newPath;
            bool isLeft = true;

            //checking which listview we are using
            if (currentLV.Name == "LVRight")
            {
                isLeft = false;
                currentPath = currentPathRight;
            }

            //checking if we are in drive 
            if (PathIsDrive(currentPath))
            {
                newPath = currentPath + currentItem.Name;
            }
            else
            {
                newPath = currentPath + "\\" + currentItem.Name;
            }

            //
            if (currentItem.IsDirectory())
            {
                if (currentItem.Name == "..")
                {
                    currentPath = LoadDirectory(currentPath, Directory.GetParent(currentPath).ToString(), currentLV);
                }
                else
                {
                    currentPath = LoadDirectory(currentPath, newPath, currentLV);
                }
            }
            else
            {
                Process.Start(currentPath + "\\" + currentItem.Name + currentItem.Type);
            }

            //saving new paths
            if (isLeft)
            {

                currentPathLeft = currentPath;
            }
            else
            {
                currentPathRight = currentPath;
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
                LRight.Content = currentPathRight;
            }
            else
            {
                currentLV = LVLeft;
                currentPathLeft = LoadDirectory(currentPathLeft, currentPath, currentLV);
                LLeft.Content = currentPathLeft;
            }
        }
        #endregion

        #region methods
        private void Copy(string sourcePath, string targetPath, Item itemToCopy)
        {
            try
            {
                if (itemToCopy.IsDirectory())
                {
                    string sourceFile = System.IO.Path.Combine(sourcePath, itemToCopy.Name);
                    string targetFile = System.IO.Path.Combine(targetPath, itemToCopy.Name);

                    CopyDirectory(sourceFile, targetFile, true);
                }
                else
                {
                    string sourceFile = System.IO.Path.Combine(sourcePath, itemToCopy.Name + itemToCopy.Type);
                    string targetFile = System.IO.Path.Combine(targetPath, itemToCopy.Name + itemToCopy.Type);

                    if (File.Exists(targetFile))
                    {
                        if (MessageBox.Show("Czy chcesz nadpisać plik?", "Plik istnieje", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            File.Copy(sourceFile, targetFile, true);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        File.Copy(sourceFile, targetFile);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = System.IO.Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }
            
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                    CopyDirectory(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private void Move(string sourcePath, string targetPath, Item itemToCopy)
        {
            try
            {
                if (itemToCopy.IsDirectory())
                {
                    string sourceFile = System.IO.Path.Combine(sourcePath, itemToCopy.Name);
                    string targetFile = System.IO.Path.Combine(targetPath, itemToCopy.Name);

                    if (Directory.Exists(targetFile))
                    {
                        if (MessageBox.Show("Czy chcesz nadpisać plik?", "Plik istnieje", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            Directory.Move(sourceFile, targetFile);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        Directory.Move(sourceFile, targetFile);
                    }
                }
                else
                {
                    string sourceFile = System.IO.Path.Combine(sourcePath, itemToCopy.Name + itemToCopy.Type);
                    string targetFile = System.IO.Path.Combine(targetPath, itemToCopy.Name + itemToCopy.Type);

                    if (File.Exists(targetFile))
                    {
                        if (MessageBox.Show("Czy chcesz nadpisać plik?", "Plik istnieje", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            File.Move(sourceFile, targetFile);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        File.Move(sourceFile, targetFile);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                    listView.Items.Add(new Item("..", "<dir>", ""));
                }

                foreach (string dir in dirs)
                {
                    FileInfo fileInfo = new FileInfo(dir);
                    listView.Items.Add(new Item(fileInfo.Name, "<dir>", "", dir));
                }

                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    listView.Items.Add(new Item(System.IO.Path.GetFileNameWithoutExtension(file), fileInfo.Extension, SizeConverter(fileInfo.Length), file));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if (listView.Name == "LVRight")
                {
                    LRight.Content = oldPath;
                }
                else
                {
                    LLeft.Content = oldPath;
                }
                return oldPath;
            }
            if (listView.Name == "LVRight")
            {
                LRight.Content = newPath;
            }
            else
            {
                LLeft.Content = newPath;
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

        public bool ButtonsLeft(CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter.ToString() == "CopyRight" || e.Parameter.ToString() == "MoveRight" || e.Parameter.ToString() == "DeleteLeft")
            {
                return true;
            }
            return false;
        }

        public bool ButtonsRight(CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter.ToString() == "CopyLeft" || e.Parameter.ToString() == "MoveLeft" || e.Parameter.ToString() == "DeleteRight")
            {
                return true;
            }
            return false;
        }

        public void RefreshDirectiories()
        {
            LoadDirectory(currentPathLeft, currentPathLeft, LVLeft);
            LoadDirectory(currentPathRight, currentPathRight, LVRight);
        }

        #endregion

        #region commands
        private void ListViewItemSelected(object sender, CanExecuteRoutedEventArgs e)
        {
            if (LVRight != null)
            {
                if (ButtonsRight(e) && LVRight.SelectedIndex >= 0)
                {
                    e.CanExecute = true;
                }
                else if (ButtonsLeft(e) && LVLeft.SelectedIndex >= 0)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
        }

        private void CopyOrMove_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter.ToString() == "CopyRight")
            {
                Item itemToCopy = LVLeft.SelectedItem as Item;
                Copy(currentPathLeft, currentPathRight, itemToCopy);
                //LoadDirectory(currentPathRight, currentPathRight, LVRight);
            }
            else if (e.Parameter.ToString() == "CopyLeft")
            {
                Item itemToCopy = LVRight.SelectedItem as Item;
                Copy(currentPathRight, currentPathLeft, itemToCopy);
                //LoadDirectory(currentPathLeft, currentPathLeft, LVLeft);
            }
            else if (e.Parameter.ToString() == "MoveRight")
            {
                Item itemToCopy = LVLeft.SelectedItem as Item;
                Move(currentPathLeft, currentPathRight, itemToCopy);
                //LoadDirectory(currentPathRight, currentPathRight, LVRight);
                //LoadDirectory(currentPathLeft, currentPathLeft, LVLeft);
            }
            else if (e.Parameter.ToString() == "MoveLeft")
            {
                Item itemToCopy = LVRight.SelectedItem as Item;
                Move(currentPathRight, currentPathLeft, itemToCopy);
                //LoadDirectory(currentPathLeft, currentPathLeft, LVLeft);
                //LoadDirectory(currentPathRight, currentPathRight, LVRight);
            }
            RefreshDirectiories();
        }
        
        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Czy chcesz usunąć pliki?", "Usuń pliki", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (e.Parameter.ToString() == "DeleteRight")
                {
                    Item itemToDelete = LVRight.SelectedItem as Item;
                    if (itemToDelete.IsDirectory())
                    {
                        Directory.Delete(itemToDelete.File, true);
                    }
                    else
                    {
                        File.Delete(itemToDelete.File);
                    }
                }
                else if (e.Parameter.ToString() == "DeleteLeft")
                {
                    Item itemToDelete = LVLeft.SelectedItem as Item;
                    if (itemToDelete.IsDirectory())
                    {
                        Directory.Delete(itemToDelete.File, true);
                    }
                    else
                    {
                        File.Delete(itemToDelete.File);
                    }
                }
                RefreshDirectiories();
            }
        }

        private void CreateFileOrDirectory_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NewItem newItemWindow = new NewItem();

            if (e.Parameter.ToString() == "CreateFileLeft")
            {
                newItemWindow.SetItemType("plik");
                if (newItemWindow.ShowDialog() == true)
                {

                }
            }
            else if (e.Parameter.ToString() == "CreateDirectoryLeft")
            {
                newItemWindow.SetItemType("folder");
                if (newItemWindow.ShowDialog() == true)
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(currentPathLeft, newItemWindow.ItemNameTB.Text));
                }
            }
            else if (e.Parameter.ToString() == "CreateFileRight")
            {
                newItemWindow.SetItemType("plik");
                if (newItemWindow.ShowDialog() == true)
                {

                }
            }
            else if (e.Parameter.ToString() == "CreateDirectoryRight")
            {
                newItemWindow.SetItemType("folder");
                if (newItemWindow.ShowDialog() == true)
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(currentPathRight, newItemWindow.ItemNameTB.Text));
                }
            }
            RefreshDirectiories();
        }

        private void Refresh_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RefreshDirectiories();
        }

        #endregion
    }
}
