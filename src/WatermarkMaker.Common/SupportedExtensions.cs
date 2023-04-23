using System;
using System.Collections.Generic;
using System.Linq;

namespace WatermarkMaker
{
    public static class SupportedExtensions
    {
        private const string Png = ".png";

        public static IEnumerable<string> GetExtensions()
        {
            yield return Png;
        }

        public static bool IsSupported(string extension)
        {
            return GetExtensions().Any(ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }

        public static string GetFileFilters()
        {
            return string.Join("|", GetExtensions().Select(ext => $"*{ext}"));
        }
    }
}