namespace WatermarkMaker.Threading
{
    internal interface IDispatcherService
    {
        /// <summary>
        /// Initializes the dispatcher service.
        /// This method should be early called once on the UI thread to ensure that
        /// the dispatcher is initialized.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Resets the dispatcher service.
        /// </summary>
        void Reset();
    }
}