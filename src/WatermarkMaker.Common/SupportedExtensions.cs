using System;
using System.Collections.Generic;
using System.Linq;

namespace WatermarkMaker
{
    public static class SupportedExtensions
    {
        private const string Png = "png";

        public static IEnumerable<string> GetExtensions()
        {
            yield return Png;
        }

        public static bool IsSupported(string extension)
        {
            extension = extension.TrimStart('.');
            return GetExtensions().Any(ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }

        public static string GetSearchPattern()
        {
            return string.Join("|", GetExtensions().Select(ext => $"*.{ext}"));
        }

        public static string GetFileFilters()
        {
            return string.Join("|", GetExtensions().Select(ext => $"Image {ext.ToUpper()} Files (*.{ext})|*.{ext}"));
        }
    }
}