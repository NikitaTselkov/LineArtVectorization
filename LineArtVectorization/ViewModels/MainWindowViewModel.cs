using LineArtVectorization.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LineArtVectorization.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private BitmapSource _image;
        public BitmapSource Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }

        #region Commands

        private DelegateCommand _loadCommand;
        public DelegateCommand LoadCommand =>
            _loadCommand ?? (_loadCommand = new DelegateCommand(ExecuteLoadCommand));

        #endregion

        public MainWindowViewModel()
        {

        }

        private void ExecuteLoadCommand()
        {
            var openFileDialog = new OpenFileDialog()
            {
                AddExtension = true,
                Filter =
                    "All Documents (*.png;*.jpeg;)|*.png;*.jpeg;|" +
                    "PNG (*.png)|*.png|" +
                    "JPEG (*.jpeg)|*.jpeg"
            };

            if (openFileDialog.ShowDialog() is true)
            {
                Image = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute));

                var blackAndWhiteBitmap = BitmapHelper.ConvertFormatToBlackAndWhite(Image, 150);
                Image = BitmapHelper.BitmapToImageSource(blackAndWhiteBitmap);

                var pixelsArray = BitmapHelper.BitmapSourceToPixelsArray(Image);

                var rleEncoder = new DataCompression<int>();
                var rle = rleEncoder.EncodeRLE(Array.ConvertAll(pixelsArray, i => (int)i));
            }
        }

      
    }
}
