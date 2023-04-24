using System.Windows;
using System.Windows.Input;

namespace WatermarkMaker.UserControls
{
    /// <summary>
    /// Interaction logic for PathPicker.xaml
    /// </summary>
    internal sealed partial class PathPicker
    {
        public PathPicker()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
            nameof(Path), typeof(string), typeof(PathPicker), new FrameworkPropertyMetadata(string.Empty));

        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public static readonly DependencyProperty BrowseCommandProperty = DependencyProperty.Register(
            nameof(BrowseCommand), typeof(ICommand), typeof(PathPicker), new FrameworkPropertyMetadata(null));

        public ICommand BrowseCommand
        {
            get => (ICommand)GetValue(BrowseCommandProperty);
            set => SetValue(BrowseCommandProperty, value);
        }
    }
}