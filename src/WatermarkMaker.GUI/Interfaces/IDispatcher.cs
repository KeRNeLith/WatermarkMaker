using System;
using System.Threading.Tasks;

namespace WatermarkMaker.Threading
{
    internal enum Priority : byte
    {
        /// <summary>
        /// Operations at this priority are processed when the application is idle.
        /// </summary>
        Idle,

        /// <summary>
        /// Operations at this priority are processed after all other non-idle operations are done.
        /// </summary>
        Background,

        /// <summary>
        /// Operations at this priority are processed at the same priority as input.
        /// </summary>
        Input,

        /// <summary>
        /// Operations at this priority are processed at normal priority.
        /// </summary>
        Normal,

        /// <summary>
        /// Operations at this priority are processed at highest priority.
        /// </summary>
        High
    }

    internal interface IDispatcher
    {
        /// <summary>
        /// Executes the <paramref name="action"/> on the UI thread synchronously.
        /// </summary>
        void Invoke(Action action);

        /// <summary>
        /// Pushes the <paramref name="action"/> to be executed by the UI thread asynchronously.
        /// </summary>
        Task BeginInvoke(Action action, Priority priority = Priority.Normal);

        /// <summary>
        /// Executes the <paramref name="action"/> on the UI thread.
        /// If this method is called from the UI thread, the <paramref name="action"/>
        /// is executed immediately. If the method is called from another thread
        /// than UI one, then the <paramref name="action"/> will be added to the
        /// UI thread's dispatcher and executed asynchronously.
        /// </summary>
        Task InvokeOnUI(Action action, Priority priority = Priority.Normal);
    }
}