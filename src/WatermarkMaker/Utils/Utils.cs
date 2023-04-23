using System;

namespace WatermarkMaker
{
    internal static class Utils
    {
        public static string FixOptionPathFormat(string path)
        {
            return path
                .Replace(Environment.NewLine, @"\n")
                .Replace("\n", @"\n")
                .Replace("\r", @"\r")
                .Replace("\t", @"\t");
        }

        public static bool PathEquals(string path1, string path2)
        {
            path1 = FormatPath(path1);
            path2 = FormatPath(path2);
            return path1.Equals(path2, StringComparison.OrdinalIgnoreCase);

            #region Local function

            static string FormatPath(string path) => path.Replace(@"\", "/");

            #endregion
        }
    }
}