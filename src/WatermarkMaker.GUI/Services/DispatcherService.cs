using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WatermarkMaker.Threading
{
    internal sealed class DispatcherService : IDispatcherService, IDispatcher
    {
        private Dispatcher? _uiDispatcher;

        private void CheckDispatcher()
        {
            if (_uiDispatcher is null)
                throw new InvalidOperationException("The UI dispatcher is not initialized.");
        }

        #region IDispatcherService

        /// <inheritdoc />
        public void Initialize()
        {
            if (_uiDispatcher is not null && _uiDispatcher.Thread.IsAlive)
                return;

            _uiDispatcher = Dispatcher.CurrentDispatcher;
        }

        /// <inheritdoc />
        public void Reset()
        {
            _uiDispatcher = null;
        }

        #endregion

        #region IDispatcher

        /// <inheritdoc />
        public void Invoke(Action action)
        {
            CheckDispatcher();
            _uiDispatcher!.Invoke(action);
        }

        /// <inheritdoc />
        public Task BeginInvoke(Action action, Priority priority)
        {
            CheckDispatcher();
            return _uiDispatcher!.BeginInvoke(action, ToDispatcherPriority(priority)).Task;
        }

        /// <inheritdoc />
        public Task InvokeOnUI(Action action, Priority priority)
        {
            CheckDispatcher();

            if (_uiDispatcher!.CheckAccess())
            {
                action();
                return Task.CompletedTask;
            }

            return _uiDispatcher.BeginInvoke(action, ToDispatcherPriority(priority)).Task;
        }

        private static DispatcherPriority ToDispatcherPriority(Priority priority)
        {
            switch (priority)
            {
                case Priority.High:
                    return DispatcherPriority.Send;

                case Priority.Normal:
                    return DispatcherPriority.Normal;

                case Priority.Input:
                    return DispatcherPriority.Input;

                case Priority.Background:
                    return DispatcherPriority.Background;

                case Priority.Idle:
                    return DispatcherPriority.ApplicationIdle;

                default:
                    throw new NotSupportedException($"{priority} is not supported.");
            }
        }

        #endregion
    }
}