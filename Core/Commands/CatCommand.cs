﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Core.Commands
{
    public static class CatCommand
    {

        /// <summary>
        /// Output to console specific lines from a file containing a specific string.
        /// </summary>
        /// <param name="searchString"> Search parameter. </param>
        /// <param name="currentDir">Current directory. </param>
        /// <param name="input"> File name to search in. </param>
        /// <param name="savedFile"> File name where to store the result data. </param>
        /// <returns>string</returns>
        public static string FileOutput(string input, string currentDir, string searchString = null, string savedFile = null)
        {
            var output = new StringBuilder();
            input = SanitizePath(input, currentDir);
            int lineCount = 0;

            if (!File.Exists(input))
            {
                Console.WriteLine("File " + input + " dose not exist!");
                return output.ToString();
            }

            using (var streamReader = new StreamReader(input))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    lineCount++;
                    if (string.IsNullOrWhiteSpace(searchString))
                    {
                        output.AppendLine(line);
                        continue;
                    }

                    if (line.ToLower().Contains(searchString.ToLower()))
                    {
                        output.AppendLine($"Line {lineCount} : {line}");
                    }
                }

                streamReader.Close();
            }

            if (!string.IsNullOrEmpty(savedFile))
            {
                return SaveFileOutput(savedFile, currentDir, output.ToString());
            }
         
            return output.ToString();
        }


        /// <summary>
        /// Output to console lines that contains a specific string.
        /// </summary>
        /// <param name="searchString"> Search parameter. </param>
        /// <param name="currentDir">Current directory. </param>
        /// <param name="paths"> File names to search in. </param>
        /// <param name="savedFile"> File name where to store the result data. </param>
        /// <param name="searchAll"> Search in all files from current directory. </param>
        /// <returns>string</returns>
        public static string MultiFileOutput(string searchString, string currentDir, IEnumerable<string> paths, string savedFile, bool searchAll)
        {
            StringBuilder output = new StringBuilder();
            if (searchAll)
            {
                string[] files = Directory.GetFiles(currentDir);
                foreach (var file in files)
                {
                   // var nFile = SanitizePath(file, currentDir);
                    if (!File.Exists(file))
                    {
                        FileSystem.ErrorWriteLine("File " + file + " dose not exist!");
                        continue;
                    }

                    output.AppendLine($"---------------- {file} ----------------");
                    output.AppendLine(FileOutput(file, currentDir, searchString));
                }
            }
            else
            {
                foreach (var file in paths)
                {
                    var nFile = SanitizePath(file, currentDir);
                    if (!File.Exists(nFile))
                    {
                        FileSystem.ErrorWriteLine("File " + nFile + " dose not exist!");
                        continue;
                    }

                    output.AppendLine($"---------------- {nFile} ----------------");
                    output.AppendLine(FileOutput(nFile, currentDir, searchString));
                }
            }

            if (!string.IsNullOrEmpty(savedFile))
            {
                return SaveFileOutput(savedFile, currentDir, output.ToString());
            }

            return output.ToString();
        }

        private static string SaveFileOutput(string path, string currentDir, string contents)
        {
            path = SanitizePath(path, currentDir);
            File.WriteAllText(path, contents);
            return $"Data saved in {path}";
        }

        private static string SanitizePath(string path, string currentDir)
        {
            return path.Contains(":") && path.Contains(@"\") ? path : $@"{currentDir}\{path}";
        }
    }
}