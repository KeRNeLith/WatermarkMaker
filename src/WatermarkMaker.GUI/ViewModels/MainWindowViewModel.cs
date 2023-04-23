using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using WatermarkMaker.Data;
using static WatermarkMaker.Utils.SerializationUtils;

namespace WatermarkMaker.ViewModels
{
    internal sealed class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            CloseCommand = new DelegateCommand(OnCloseCommand);

            _settings = SettingsToSession();
        }

        #region Watermark file

        private string _watermarkFilePath = string.Empty;

        public string WatermarkFilePath
        {
            get => _watermarkFilePath;
            set => SetProperty(ref _watermarkFilePath, value);
        }

        #endregion

        #region Input folder

        private string _inputFolderPath = string.Empty;

        public string InputFolderPath
        {
            get => _inputFolderPath;
            set => SetProperty(ref _inputFolderPath, value);
        }

        #endregion

        #region Output folder

        private string _outputFolderPath = string.Empty;

        public string OutputFolderPath
        {
            get => _outputFolderPath;
            set => SetProperty(ref _outputFolderPath, value);
        }

        #endregion

        #region Watermark settings

        private double _proportion;

        public double Proportion
        {
            get => _proportion;
            set => SetProperty(ref _proportion, value);
        }

        private double _rightOffset;

        public double RightOffset
        {
            get => _rightOffset;
            set => SetProperty(ref _rightOffset, value);
        }

        private double _bottomOffset;

        public double BottomOffset
        {
            get => _bottomOffset;
            set => SetProperty(ref _bottomOffset, value);
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
            WatermarkFilePath = settings.WatermarkFilePath;
            InputFolderPath = settings.InputFolderPath;
            OutputFolderPath = settings.OutputFolderPath;
            Proportion = settings.Proportion;
            RightOffset = settings.RightOffset;
            BottomOffset = settings.BottomOffset;
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