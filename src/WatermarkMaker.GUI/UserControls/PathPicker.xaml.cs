using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;

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

            ClearCommand = new ActionCommand(() => Path = string.Empty);
        }

        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
            nameof(Path), typeof(string), typeof(PathPicker), new FrameworkPropertyMetadata(string.Empty, OnPathChanged));

        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        private static void OnPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var pathPicker = (PathPicker)d;
            pathPicker.HasValue = !string.IsNullOrWhiteSpace((string)args.NewValue);
        }

        private static readonly DependencyPropertyKey HasValuePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(HasValue), typeof(bool), typeof(PathPicker), new FrameworkPropertyMetadata(false));

        public bool HasValue
        {
            get => (bool)GetValue(HasValuePropertyKey.DependencyProperty);
            private set => SetValue(HasValuePropertyKey, value);
        }

        public static readonly DependencyProperty OpenCommandProperty = DependencyProperty.Register(
            nameof(OpenCommand), typeof(ICommand), typeof(PathPicker), new FrameworkPropertyMetadata(null));

        public ICommand OpenCommand
        {
            get => (ICommand)GetValue(OpenCommandProperty);
            set => SetValue(OpenCommandProperty, value);
        }

        public static readonly DependencyProperty BrowseCommandProperty = DependencyProperty.Register(
            nameof(BrowseCommand), typeof(ICommand), typeof(PathPicker), new FrameworkPropertyMetadata(null));

        public ICommand BrowseCommand
        {
            get => (ICommand)GetValue(BrowseCommandProperty);
            set => SetValue(BrowseCommandProperty, value);
        }

        private static readonly DependencyPropertyKey ClearCommandPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(ClearCommand), typeof(ICommand), typeof(PathPicker), new FrameworkPropertyMetadata(null));

        public ICommand ClearCommand
        {
            get => (ICommand)GetValue(ClearCommandPropertyKey.DependencyProperty);
            private init => SetValue(ClearCommandPropertyKey, value);
        }
    }
}