using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;

namespace FileScanner.Models
{
    class Items : INotifyPropertyChanged
    {
        private string fileName;
        private string img;

        public string Img
        {
            get => img;
            set
            {
                img = value;
                OnPropertyChanged();
            }
        }

        public string FileName{

            get => fileName;

            set
            {
                fileName = value;
                OnPropertyChanged();
            }
           }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
