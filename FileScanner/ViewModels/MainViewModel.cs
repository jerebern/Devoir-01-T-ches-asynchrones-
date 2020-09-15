using FileScanner.Commands;
using FileScanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace FileScanner.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string scannerTimer;
        private string selectedFolder;
        private ObservableCollection<string> folderItems = new ObservableCollection<string>();
        private ObservableCollection<Item> items = new ObservableCollection<Item>();

        public DelegateCommand<string> OpenFolderCommand { get; private set; }
        public DelegateCommand<string> ScanFolderCommand { get; private set; }
        

        public ObservableCollection<Item> Items {
            get => items;
            set
            {
                items = value;
                OnPropertyChanged();
         }
        }
        public ObservableCollection<string> FolderItems { 
            get => folderItems;
            set 
            { 
                folderItems = value;
                OnPropertyChanged();
            }
        }

        public string ScannerTimer
        {

            get => scannerTimer;
            set
            {
                scannerTimer = value;
                OnPropertyChanged();

            }
        }

        public string SelectedFolder
        {
            get => selectedFolder;
            set
            {
                selectedFolder = value;
                OnPropertyChanged();
                ScanFolderCommand.RaiseCanExecuteChanged();
            }
        }

        public MainViewModel()
        {
            OpenFolderCommand = new DelegateCommand<string>(OpenFolder);
            ScanFolderCommand = new DelegateCommand<string>(ScanFolderAsync, CanExecuteScanFolder);
        }

        private bool CanExecuteScanFolder(string obj)
        {
            return !string.IsNullOrEmpty(SelectedFolder);
        }

        private void OpenFolder(string obj)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    SelectedFolder = fbd.SelectedPath;
                }
            }
        }

        private void ScanFolder(string dir)
        {
            FolderItems = new ObservableCollection<string>(GetDirs(dir));
            
            foreach (var item in Directory.EnumerateFiles(dir, "*"))
            {
                FolderItems.Add(item);
            }

            
        }

        //Stop watch source : https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=netcore-3.1
        //Tell if file or folder : https://stackoverflow.com/questions/1395205/better-way-to-check-if-a-path-is-a-file-or-a-directory

        private async void ScanFolderAsync(string dir)
        {
            Stopwatch stopWatch = new Stopwatch();

            await Task.Run(() =>
            {
                try
                {
                    items = new ObservableCollection<Item>();
                    foreach (var path in Directory.EnumerateDirectories(dir, "*"))
                    {

                        Item item = new Item(path, "/Images/file.png");

                        App.Current.Dispatcher.BeginInvoke(
                            (Action)delegate ()
                            {
                                Items.Add(item);
                            });
                    }
                    foreach (var path in Directory.EnumerateDirectories(dir, "*"))
                    {
                        Item item = new Item(path, "/Images/file.png");

                        App.Current.Dispatcher.BeginInvoke(
                            (Action)delegate ()
                            {
                                Items.Add(item);
                            });
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Access denied  ");
                }
            });
            stopWatch.Stop();
            scannerTimer = stopWatch.ElapsedMilliseconds.ToString();
        }
        IEnumerable<string> GetDirs(string dir)
        {            
            foreach (var d in Directory.EnumerateDirectories(dir, "*"))
            {
                yield return d;
            }
        }

        ///TODO : Tester avec un dossier avec beaucoup de fichier
        ///TODO : Rendre l'application asynchrone
        ///TODO : Ajouter un try/catch pour les dossiers sans permission

    }
}
