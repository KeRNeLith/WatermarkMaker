using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using Prism.Commands;
using Prism.Mvvm;
using WatermarkMaker.Data;
using WatermarkMaker.Properties;
using WatermarkMaker.Threading;
using WatermarkMaker.Utils;
using static WatermarkMaker.SupportedExtensions;
using static WatermarkMaker.Utils.ImageUtils;
using static WatermarkMaker.Utils.SerializationUtils;
using static WatermarkMaker.WatermarkUtils;
using Image = System.Drawing.Image;

namespace WatermarkMaker.ViewModels
{
    internal sealed class MainWindowViewModel : BindableBase
    {
        private readonly IDispatcher _dispatcher;
        private readonly MvvmDialogs.IDialogService _dialogService;

        public MainWindowViewModel(IDispatcher dispatcher, MvvmDialogs.IDialogService dialogService)
        {
            _dispatcher = dispatcher;
            _dialogService = dialogService;

            BrowseWatermarkFileCommand = new DelegateCommand(OnBrowseWatermarkFile);
            BrowseInputFolderCommand = new DelegateCommand(OnBrowseInputFolder);
            BrowseOutputFolderCommand = new DelegateCommand(OnBrowseOutputFolder);
            ApplyCommand = new DelegateCommand(OnApply, CanApply)
                .ObservesProperty(() => WatermarkFilePath)
                .ObservesProperty(() => InputFolderPath)
                .ObservesProperty(() => OutputFolderPath);
            CloseCommand = new DelegateCommand(OnCloseCommand);

            _settings = SettingsToSession();

            GeneratePreviewIfPossible();
        }

        private string _browseInitialFolder = string.Empty;

        #region Watermark file

        private string _watermarkFilePath = string.Empty;

        public string WatermarkFilePath
        {
            get => _watermarkFilePath;
            set => SetProperty(ref _watermarkFilePath, value, GeneratePreviewIfPossible);
        }

        public ICommand BrowseWatermarkFileCommand { get; }

        private void OnBrowseWatermarkFile()
        {
            string initialFolder = string.IsNullOrEmpty(_browseInitialFolder) || !Directory.Exists(_browseInitialFolder)
                ? Environment.CurrentDirectory
                : _browseInitialFolder;

            var options = new OpenFileDialogSettings
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Title = "Select a watermark image",
                InitialDirectory = initialFolder,
                Filter = GetFileFilters(),
                FilterIndex = 0,
                DefaultExt = GetExtensions().First()
            };

            bool? result = _dialogService.ShowOpenFileDialog(this, options);
            if (result.HasValue && result.Value)
            {
                string selectedFilePath = options.FileName;
                string? selectedFolderPath = Path.GetDirectoryName(selectedFilePath);
                if (!string.IsNullOrEmpty(selectedFolderPath))
                {
                    _browseInitialFolder = selectedFolderPath;
                }

                WatermarkFilePath = selectedFilePath;
            }
            else
            {
                _browseInitialFolder = initialFolder;
            }
        }

        #endregion

        #region Input folder

        private string _inputFolderPath = string.Empty;

        public string InputFolderPath
        {
            get => _inputFolderPath;
            set => SetProperty(ref _inputFolderPath, value, GeneratePreviewIfPossible);
        }

        public ICommand BrowseInputFolderCommand { get; }

        private void OnBrowseInputFolder()
        {
            var options = new FolderBrowserDialogSettings
            {
                Description = "Select the input folder",
                ShowNewFolderButton = false
            };

            bool? result = _dialogService.ShowFolderBrowserDialog(this, options);
            if (result.HasValue && result.Value)
            {
                string selectedFolderPath = options.SelectedPath;
                if (PathUtils.PathEquals(selectedFolderPath, OutputFolderPath))
                {
                    _dialogService.ShowMessageBox(
                        this,
                        "Selected input folder is the same as the output folder."
                        + Environment.NewLine
                        + Environment.NewLine
                        + "To avoid conflicts or data loss, output folder has been automatically updated to use \"Output\" sub-folder.",
                        "Output folder conflict",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    OutputFolderPath = Path.Combine(selectedFolderPath, "Output");
                }

                InputFolderPath = selectedFolderPath;
            }
        }

        private string? TryGetPreviewImageFilePath()
        {
            string inputFolderPath = InputFolderPath;
            if (Directory.Exists(inputFolderPath))
            {
                string? previewImageFilePath = Directory
                    .EnumerateFiles(inputFolderPath, GetSearchPattern(), SearchOption.TopDirectoryOnly)
                    .Except(new[] { WatermarkFilePath })
                    .FirstOrDefault();
                return previewImageFilePath;
            }

            return null;
        }

        #endregion

        #region Output folder

        private string _outputFolderPath = string.Empty;

        public string OutputFolderPath
        {
            get => _outputFolderPath;
            set => SetProperty(ref _outputFolderPath, value);
        }

        public ICommand BrowseOutputFolderCommand { get; }

        private void OnBrowseOutputFolder()
        {
            var options = new FolderBrowserDialogSettings
            {
                Description = "Select the output folder",
                ShowNewFolderButton = true
            };

            bool? result = _dialogService.ShowFolderBrowserDialog(this, options);
            if (result.HasValue && result.Value)
            {
                string selectedFolderPath = options.SelectedPath;
                if (PathUtils.PathEquals(selectedFolderPath, InputFolderPath))
                {
                    _dialogService.ShowMessageBox(
                        this,
                        "Selected output folder is the same as the input folder."
                        + Environment.NewLine
                        + Environment.NewLine
                        + "To avoid conflicts or data loss, output folder has been automatically updated to use \"Output\" sub-folder.",
                        "Output folder conflict",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    selectedFolderPath = Path.Combine(selectedFolderPath, "Output");
                }

                OutputFolderPath = selectedFolderPath;
            }
        }

        #endregion

        #region Watermark settings

        private double _proportion;

        public double Proportion
        {
            get => _proportion;
            set => SetProperty(ref _proportion, value, GeneratePreviewIfPossible);
        }

        private double _rightOffset;

        public double RightOffset
        {
            get => _rightOffset;
            set => SetProperty(ref _rightOffset, value, GeneratePreviewIfPossible);
        }

        private double _bottomOffset;

        public double BottomOffset
        {
            get => _bottomOffset;
            set => SetProperty(ref _bottomOffset, value, GeneratePreviewIfPossible);
        }

        #endregion

        #region Apply command

        public ICommand ApplyCommand { get; }

        private bool CanApply()
        {
            return File.Exists(WatermarkFilePath)
                   && Directory.Exists(InputFolderPath)
                   && !string.IsNullOrEmpty(OutputFolderPath);
        }

        private void OnApply()
        {
            const int itemCount = 20;
            var progressDialogViewModel = new ProgressDialogViewModel
            {
                Title = "Applying watermark",
                ProgressMessage = "Start watermark processing...",
                IsIndeterminate = false,
                Minimum = 0,
                Maximum = itemCount
            };

            Task.Run(async () =>
            {
                for (int i = 1; i <= itemCount; i++)
                {
                    (int Progress, string ProgressMessage) progress = (i, $"Fake treatment {i}");
                    _ = _dispatcher.InvokeOnUI(() =>
                    {
                        progressDialogViewModel.Progress = progress.Progress;
                        progressDialogViewModel.ProgressMessage = progress.ProgressMessage;
                    });

                    await Task.Delay(200);
                }
            });

            _dialogService.ShowDialog(this, progressDialogViewModel);
        }

        #endregion

        #region Preview

        private BitmapSource? _noPreviewImage;

        private BitmapSource NoPreviewImage => _noPreviewImage ??= ConvertToBitmap(Resources.NoPreview);

        private BitmapSource? _previewImage;

        public BitmapSource? PreviewImage
        {
            get => _previewImage;
            private set => SetProperty(ref _previewImage, value);
        }

        private CancellationTokenSource? _previewGenerationTokenSource;

        private void GeneratePreviewIfPossible()
        {
            string? previewImageFilePath = TryGetPreviewImageFilePath();
            _previewGenerationTokenSource?.Cancel();
            var tokenSource = new CancellationTokenSource();
            _previewGenerationTokenSource = tokenSource;
            Task _ = GeneratePreviewFromCurrentSettings(previewImageFilePath, tokenSource.Token);
        }

        private async Task GeneratePreviewFromCurrentSettings(string? previewImageFilePath, CancellationToken token)
        {
            Task noPreviewTask = _dispatcher.InvokeOnUI(() => PreviewImage = NoPreviewImage);

            string watermarkFilePath = WatermarkFilePath;
            if (string.IsNullOrWhiteSpace(watermarkFilePath) || !File.Exists(watermarkFilePath))
                return;

            if (string.IsNullOrWhiteSpace(previewImageFilePath) || !File.Exists(previewImageFilePath))
                return;

            var watermarkParams = new WatermarkParams(Proportion, RightOffset, BottomOffset);

            await noPreviewTask;
            await Task.Run(
                async () =>
                {
                    await GeneratePreviewFrom(previewImageFilePath, watermarkFilePath, watermarkParams, token);
                },
                token);
        }

        private async Task GeneratePreviewFrom(
            string imageToTreatFilePath,
            string watermarkFilePath,
            WatermarkParams parameters,
            CancellationToken token)
        {
            using Image watermarkImage = Image.FromFile(watermarkFilePath);
            using Image imageToTreat = Image.FromFile(imageToTreatFilePath);

            if (token.IsCancellationRequested)
                return;
            AddWatermarkImage(imageToTreat, watermarkImage, parameters);

            if (token.IsCancellationRequested)
                return;
            BitmapImage convertedImage = ConvertToBitmap(imageToTreat);

            await _dispatcher.InvokeOnUI(() =>
            {
                if (token.IsCancellationRequested)
                    return;
                PreviewImage = convertedImage;
            });
        }

        #endregion

        #region Session data

        private const string SettingsFileName = "sessionSettings.xml";
        private readonly SessionSettings _settings;

        private void SessionToSettings()
        {
            _settings.WatermarkFilePath = WatermarkFilePath;
            _settings.InputFolderPath = InputFolderPath;
            _settings.OutputFolderPath = OutputFolderPath;
            _settings.Proportion = Proportion;
            _settings.RightOffset = RightOffset;
            _settings.BottomOffset = BottomOffset;
            _settings.BrowseInitialFolder = _browseInitialFolder;
            _settings.SerializeToXml(SettingsFileName);
        }

        private SessionSettings SettingsToSession()
        {
            SessionSettings settings = DeserializeFromXml<SessionSettings>(SettingsFileName) ?? new SessionSettings
            {
                Proportion = 0.2d,
                RightOffset = 0.02d,
                BottomOffset = 0.02d
            };
            _watermarkFilePath = settings.WatermarkFilePath;
            _inputFolderPath = settings.InputFolderPath;
            _outputFolderPath = settings.OutputFolderPath;
            _proportion = settings.Proportion;
            _rightOffset = settings.RightOffset;
            _bottomOffset = settings.BottomOffset;
            _browseInitialFolder = settings.BrowseInitialFolder;
            return settings;
        }

        #endregion

        #region On Close

        public ICommand CloseCommand { get; }

        private void OnCloseCommand()
        {
            SessionToSettings();
        }

        #endregion
    }
}