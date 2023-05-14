using System.Windows;
using Prism.Mvvm;

namespace WatermarkMaker.ViewModels
{
    internal sealed class WindowViewModel : BindableBase
    {
        private WindowState _state;

        public WindowState State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        private double _top;

        public double Top
        {
            get => _top;
            set => SetProperty(ref _top, value);
        }

        private double _left;

        public double Left
        {
            get => _left;
            set => SetProperty(ref _left, value);
        }

        private double _height;

        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        private double _width;

        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }
    }
}