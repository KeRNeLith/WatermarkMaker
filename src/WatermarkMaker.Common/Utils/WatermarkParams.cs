using System;

namespace WatermarkMaker
{
    public sealed class WatermarkParams
    {
        private const double MinimalProportion = 0.01d;
        private const double MaximalProportion = 1.0d;

        private const double MinimalOffset = 0.0d;
        private const double MaximalOffset = 1.0d;

        public WatermarkParams(double proportion, double xOffsetRatio, double yOffsetRatio)
        {
            Proportion = Clamp(MinimalProportion, MaximalProportion, proportion);
            XOffsetRatio = ClampOffset(xOffsetRatio);
            YOffsetRatio = ClampOffset(yOffsetRatio);

            #region Local function

            static double Clamp(double min, double max, double value) => Math.Max(min, Math.Min(max, value));
            static double ClampOffset(double offset) => Clamp(MinimalOffset, MaximalOffset, offset);

            #endregion
        }

        public double Proportion { get; }
        public double XOffsetRatio { get; }
        public double YOffsetRatio { get; }
    }
}