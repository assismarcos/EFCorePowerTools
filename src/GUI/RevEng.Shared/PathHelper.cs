﻿using System;
using System.IO;

namespace RevEng.Shared
{
    public static class PathHelper
    {
        private const string HeaderConst = "// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>";

        public static string Header { get; set; } = HeaderConst;

        public static string GetAbsPath(string outputPath, string fullName)
        {
            //   ' The output folder can have these patterns:
            //   ' 1) "\\server\folder"
            //   ' 2) "drive:\folder"
            //   ' 3) "..\..\folder"
            //   ' 4) "folder"

            if (outputPath.StartsWith(string.Empty + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar))
            {
                return outputPath;
            }
            else if (outputPath.Length >= 2 && outputPath.ToCharArray()[0] == Path.VolumeSeparatorChar)
            {
                return outputPath;
            }
            else if (outputPath.IndexOf("..\\") != -1)
            {
                var projectFolder = Path.GetDirectoryName(fullName);
                while (outputPath.StartsWith("..\\"))
                {
                    outputPath = outputPath.Substring(3);
                    projectFolder = Path.GetDirectoryName(projectFolder);
                }
                return Path.Combine(projectFolder, outputPath);
            }
            else
            {
                var projectFolder = Path.GetDirectoryName(fullName);
                return Path.Combine(projectFolder, outputPath);
            }
        }

        public static string GetNamespaceFromOutputPath(string directoryPath, string projectDir, string rootNamespace)
        {
            var subNamespace = SubnamespaceFromOutputPath(projectDir, directoryPath);
            return string.IsNullOrEmpty(subNamespace)
                ? rootNamespace
                : rootNamespace + "." + subNamespace;
        }

        // if outputDir is a subfolder of projectDir, then use each subfolder as a subnamespace
        // --output-dir $(projectFolder)/A/B/C
        // => "namespace $(rootnamespace).A.B.C"
        private static string SubnamespaceFromOutputPath(string projectDir, string outputDir)
        {
            if (!outputDir.StartsWith(projectDir, StringComparison.Ordinal))
            {
                return null;
            }

            var subPath = outputDir.Substring(projectDir.Length);

            return !string.IsNullOrWhiteSpace(subPath)
                ? string.Join(
                    ".",
                    subPath.Split(
                        new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries))
                : null;
        }

        public static string MakeDirRelative(string root, string path)
        {
            var relativeUri = new Uri(NormalizeDir(root)).MakeRelativeUri(new Uri(NormalizeDir(path)));

            return Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', Path.DirectorySeparatorChar);
        }

        private static string NormalizeDir(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            var last = path[path.Length - 1];
            return last == Path.DirectorySeparatorChar
                   || last == Path.AltDirectorySeparatorChar
                ? path
                : path + Path.DirectorySeparatorChar;
        }
    }
}
