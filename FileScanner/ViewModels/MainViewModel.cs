﻿using FileScanner.Commands;
using FileScanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace FileScanner.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
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

        private async void ScanFolderAsync(string dir)
        {
             Items = new ObservableCollection<Item>();
            await Task.Run(() =>
            {
                try
                {

                    foreach (var path in Directory.EnumerateDirectories(dir, "*"))
                    {

                     Item item = new Item(path, "/img/folder.png");

                        App.Current.Dispatcher.BeginInvoke(
                            (Action)delegate ()
                            {  
                                Items.Add(item);
                            });
                    }
                    foreach (var path in Directory.EnumerateFiles(dir, "*"))
                    {
                        Item item = new Item(path, "/img/file.png");

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
            Debug.WriteLine( items.Count());
        }
        IEnumerable<string> GetDirs(string dir)
        {            
            foreach (var d in Directory.EnumerateDirectories(dir, "*"))
            {
                yield return d;
            }
        }

    }
}
