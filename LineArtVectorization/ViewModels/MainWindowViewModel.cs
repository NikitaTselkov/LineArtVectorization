using LineArtVectorization.Core;
using LineArtVectorization.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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

                var rleByteEncoder = new RLE<byte>();

                var MCC = rleByteEncoder.GetMCC(pixels);

                var skelet = new Skeletonization();

                var skeletonCurves = skelet.PartialSkeletonization(MCC);

                foreach (var item in skeletonCurves)
                {
                    Lines.Add(new Line
                    {
                        Start = new Point(item.Points.First().X, item.Points.First().Y),
                        End = new Point(item.Points.Last().X, item.Points.Last().Y)
                    });
                }

            }
        }
    }
}
