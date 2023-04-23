using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WatermarkMaker
{
    public static class WatermarkUtils
    {
        public static void AddWatermarkImage(
            Image imageToWatermark,
            Image watermarkImage,
            WatermarkParams parameters)
        {
            int xRightOffset = (int)(imageToWatermark.Width * parameters.XOffsetRatio);
            int yBottomOffset = (int)(imageToWatermark.Height * parameters.YOffsetRatio);

            int targetWidth;

            // Landscape
            if (imageToWatermark.Width > imageToWatermark.Height)
            {
                targetWidth = (int)(imageToWatermark.Height * parameters.Proportion);
            }
            // Portrait
            else
            {
                targetWidth = (int)(imageToWatermark.Width * parameters.Proportion);
            }

            double watermarkRatio = watermarkImage.Height / (double)watermarkImage.Width;
            int targetHeight = (int)(targetWidth * watermarkRatio);

            // Draw watermark
            int x = Math.Max(0, imageToWatermark.Width - targetWidth - xRightOffset);
            int y = Math.Max(0, imageToWatermark.Height - targetHeight - yBottomOffset);
            using Graphics graphics = Graphics.FromImage(imageToWatermark);
            graphics.DrawImage(watermarkImage, x, y, targetWidth, targetHeight);
        }

        public static void ProcessWatermarkImage(
            string imageToTreatFilePath,
            string outFolderPath,
            Image watermarkImage,
            WatermarkParams parameters)
        {
            using Image imageToTreat = Image.FromFile(imageToTreatFilePath);
            AddWatermarkImage(imageToTreat, watermarkImage, parameters);

            Directory.CreateDirectory(outFolderPath);
            string outFilePath = Path.Combine(outFolderPath, Path.GetFileName(imageToTreatFilePath));

            using MemoryStream memoryStream = new();
            imageToTreat.Save(memoryStream, ImageFormat.Png);
            File.WriteAllBytes(outFilePath, memoryStream.ToArray());
        }
    }
}