using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Prism.Mvvm;
using MvvmDialogs;
using Prism.Commands;

namespace WatermarkMaker.ViewModels
{
    internal sealed class ProgressDialogViewModel : BindableBase, IModalDialogViewModel
    {
        public ProgressDialogViewModel()
        {
            _cancelCommand = new CompositeCommand();
            _cancelCommand.RegisterCommand(
                new DelegateCommand(OnCancelInternal, CanCancelInternal)
                    .ObservesProperty(() => IsIndeterminate)
                    .ObservesProperty(() => Progress)
                    .ObservesProperty(() => Maximum));

            _okCommand = new CompositeCommand();
            _okCommand.RegisterCommand(
                new DelegateCommand(OnOkInternal, CanOkInternal)
                    .ObservesProperty(() => IsIndeterminate)
                    .ObservesProperty(() => Progress)
                    .ObservesProperty(() => Maximum));
        }

        #region General information

        private string _title = "Progression";

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string? _progressMessage;

        public string? ProgressMessage
        {
            get => _progressMessage;
            set => SetProperty(ref _progressMessage, value);
        }

        #endregion

        #region Progress bar related

        private bool _isIndeterminate = true;

        /// <inheritdoc cref="ProgressBar.IsIndeterminate"/>
        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            set => SetProperty(ref _isIndeterminate, value);
        }

        private double _progress;

        /// <inheritdoc cref="RangeBase.Value"/>
        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        private double _minimum;

        /// <inheritdoc cref="RangeBase.Minimum"/>
        public double Minimum
        {
            get => _minimum;
            set => SetProperty(ref _minimum, value);
        }

        private double _maximum = 100.0d;

        /// <inheritdoc cref="RangeBase.Maximum"/>
        public double Maximum
        {
            get => _maximum;
            set => SetProperty(ref _maximum, value);
        }

        #endregion

        #region Result management

        private readonly CompositeCommand _cancelCommand;
        private ICommand? _userCancelCommand;

        public ICommand? CancelCommand
        {
            get => _cancelCommand;
            set
            {
                if (_userCancelCommand is not null)
                {
                    _cancelCommand.UnregisterCommand(_userCancelCommand);
                }

                _userCancelCommand = value;

                if (_userCancelCommand is not null)
                {
                    _cancelCommand.RegisterCommand(_userCancelCommand);
                }
            }
        }

        private bool CanCancelInternal()
        {
            return IsIndeterminate || Progress < Maximum;
        }

        private void OnCancelInternal()
        {
            DialogResult = false;
        }

        private readonly CompositeCommand _okCommand;
        private ICommand? _userOkCommand;

        public ICommand? OkCommand
        {
            get => _okCommand;
            set
            {
                if (_userOkCommand is not null)
                {
                    _okCommand.UnregisterCommand(_userOkCommand);
                }

                _userOkCommand = value;

                if (_userOkCommand is not null)
                {
                    _okCommand.RegisterCommand(_userOkCommand);
                }
            }
        }

        private bool CanOkInternal()
        {
            return IsIndeterminate || (!IsIndeterminate && Progress >= Maximum);
        }

        private void OnOkInternal()
        {
            DialogResult = true;
        }

        private bool? _dialogResult;

        /// <inheritdoc />
        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }

        #endregion
    }
}