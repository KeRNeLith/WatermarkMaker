using CommandLine;

namespace WatermarkMaker
{
    internal sealed class Options
    {
        [Option('f', "inputFile", Default = "", HelpText = "Path to the input image.", SetName = "file")]
        public string InputFile { get; set; } = string.Empty;

        [Option('i', "inputFolder", Default = "", HelpText = "Path to the folder of input images.", SetName = "folder")]
        public string InputFolder { get; set; } = string.Empty;

        [Option('o', "outputFolder", Default = "", HelpText = "Path to the folder for output images.")]
        public string OutputFolder { get; set; } = string.Empty;

        [Option('w', "watermarkFile", Default = "", Required = true, HelpText = "Path to the watermark image.")]
        public string WatermarkFile { get; set; } = string.Empty;

        [Option('p', "proportion", Default = 0.15, HelpText = "Proportion % of the target image that will be used for watermark [0.01 - 1].")]
        public double Proportion { get; set; }

        [Option('r', "xOffset", Default = 0.01, HelpText = "Offset % from right of target image to position watermark [0 - 1].")]
        public double XOffsetRatio { get; set; }

        [Option('b', "yOffset", Default = 0.01, HelpText = "Offset % from bottom of target image to position watermark [0 - 1].")]
        public double YOffsetRatio { get; set; }
    }
}