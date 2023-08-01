using LineArtVectorization.Core;
using LineArtVectorization.Models;
using LineArtVectorization.Models.Data;
using LineArtVectorization.Models.Utils;
using LineArtVectorization.Models.Utils.Helpers;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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

        public ObservableCollection<Line> Lines { get; set; }

        #region Commands

        private DelegateCommand _loadCommand;
        public DelegateCommand LoadCommand =>
            _loadCommand ?? (_loadCommand = new DelegateCommand(ExecuteLoadCommand));

        #endregion

        public MainWindowViewModel() 
        {
            Lines = new ObservableCollection<Line>();
        }

        private void ExecuteLoadCommand()
        {
            var openFileDialog = new OpenFileDialog()
            {
                AddExtension = true,
                Filter =
                    "All Documents (*.png;*.jpg;)|*.png;*.jpg;|" +
                    "PNG (*.png)|*.png|" +
                    "JPEG (*.jpg)|*.jpg"
            };

            if (openFileDialog.ShowDialog() is true)
            {
                var image = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute));
                byte[,] pixels = BitmapHelper.BitmapSourceToBlackAndWhitePixelsArray(image, 150);
                Image = BitmapHelper.BinaryPixelsArrayToBitmapSource(pixels);

                var skelet = new Skeletonization();

                var skeletonCurves = skelet.PartialSkeletonization(pixels).Result;

                foreach (var item in skeletonCurves)
                {
                    Lines.AddRange(item.GetLines());
                }
            }
        }
    }
}
