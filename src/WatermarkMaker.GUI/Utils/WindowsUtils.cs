using System.Diagnostics;
using System.IO;

namespace WatermarkMaker.Utils
{
    internal static class WindowsUtils
    {
        private const string ExplorerExecutable = "explorer.exe";

        public static void TrySelectPath(string path)
        {
            if (!File.Exists(path) && !Directory.Exists(path))
                return;

            var startInfo = new ProcessStartInfo
            {
                Arguments = $"/select, \"{path}\"",
                FileName = ExplorerExecutable
            };

            Process.Start(startInfo);
        }

        public static void TryOpenFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                var startInfo = new ProcessStartInfo
                {
                    Arguments = folderPath,
                    FileName = ExplorerExecutable
                };

                Process.Start(startInfo);
            }
        }
    }
}