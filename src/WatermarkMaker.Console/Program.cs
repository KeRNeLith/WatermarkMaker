using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using CommandLine;
using static WatermarkMaker.WatermarkUtils;
using static WatermarkMaker.SupportedExtensions;
using static WatermarkMaker.PathUtils;

namespace WatermarkMaker
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            int returnCode = ReturnCodes.Success;
            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    #region Input(s)

                    bool isFileTreatment = !string.IsNullOrEmpty(options.InputFile);
                    string inputPath;   // Can be either a file or a folder path depending of parameters

                    if (isFileTreatment)
                    {
                        try
                        {
                            inputPath = Path.GetFullPath(FixOptionPathFormat(options.InputFile));
                        }
                        catch (ArgumentException)
                        {
                            Console.WriteLine($"Failed to parse input image \"{options.InputFile}\".");
                            returnCode = ReturnCodes.Error;
                            return;
                        }

                        if (!File.Exists(inputPath))
                        {
                            Console.WriteLine($"Input image \"{inputPath}\" does not exist.");
                            returnCode = ReturnCodes.Error;
                            return;
                        }

                        string inputExtension = Path.GetExtension(inputPath);
                        if (!IsSupported(inputExtension))
                        {
                            Console.WriteLine($"Input image ({inputExtension}) must be a supported extension ({string.Join(", ", GetExtensions())}).");
                            returnCode = ReturnCodes.Error;
                            return;
                        }
                    }
                    else
                    {
                        try
                        {
                            inputPath = string.IsNullOrEmpty(options.InputFolder)
                                ? Environment.CurrentDirectory
                                : Path.GetFullPath(FixOptionPathFormat(options.InputFolder));
                        }
                        catch (ArgumentException)
                        {
                            Console.WriteLine($"Failed to parse input folder \"{options.InputFolder}\".");
                            returnCode = ReturnCodes.Error;
                            return;
                        }

                        if (!Directory.Exists(inputPath))
                        {
                            Console.WriteLine($"Input folder \"{inputPath}\" does not exist.");
                            returnCode = ReturnCodes.Error;
                            return;
                        }
                    }

                    #endregion

                    #region Output folder

                    string outputFolderPath;
                    try
                    {
                        outputFolderPath = string.IsNullOrEmpty(options.OutputFolder)
                            ? Path.Combine(Environment.CurrentDirectory, "Output")
                            : Path.GetFullPath(FixOptionPathFormat(options.OutputFolder));
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine($"Failed to parse output folder \"{options.OutputFolder}\".");
                        returnCode = ReturnCodes.Error;
                        return;
                    }

                    if (!Directory.Exists(outputFolderPath))
                    {
                        Directory.CreateDirectory(outputFolderPath);
                    }

                    #endregion

                    if (isFileTreatment)
                    {
                        string? inputFolderPath = Path.GetDirectoryName(inputPath);
                        if (inputFolderPath is not null && PathEquals(inputFolderPath, outputFolderPath))
                        {
                            Console.WriteLine("Trying to overwrite original image, consider changing output folder path.");
                            returnCode = ReturnCodes.Error;
                            return;
                        }
                    }
                    else if (PathEquals(inputPath, outputFolderPath))
                    {
                        Console.WriteLine("Trying to overwrite original image(s), consider changing output folder path.");
                        returnCode = ReturnCodes.Error;
                        return;
                    }

                    #region Watermark file

                    string watermarkFilePath;
                    try
                    {
                        watermarkFilePath = Path.GetFullPath(FixOptionPathFormat(options.WatermarkFile));
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine($"Failed to parse watermark image path \"{options.WatermarkFile}\".");
                        returnCode = ReturnCodes.Error;
                        return;
                    }

                    if (!File.Exists(watermarkFilePath))
                    {
                        Console.WriteLine($"Watermark image \"{watermarkFilePath}\" does not exist.");
                        returnCode = ReturnCodes.Error;
                        return;
                    }

                    #endregion

                    using Image watermarkImage = Image.FromFile(watermarkFilePath);
                    var watermarkParams = new WatermarkParams(options.Proportion, options.XOffsetRatio, options.YOffsetRatio);

                    if (isFileTreatment)
                    {
                        ProcessWatermarkImage(inputPath, outputFolderPath, watermarkImage, watermarkParams);
                    }
                    else
                    {
                        IEnumerable<string> imageToTreatPaths = Directory
                            .EnumerateFiles(inputPath, GetSearchPattern(), SearchOption.TopDirectoryOnly)
                            .Except(new[] { watermarkFilePath });
                        foreach (string imagePath in imageToTreatPaths)
                        {
                            ProcessWatermarkImage(imagePath, outputFolderPath, watermarkImage, watermarkParams);
                        }
                    }
                });

            return returnCode;
        }
    }
}