using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: This class needs to be able to handle setting and disabling read-only attributes on files
namespace SSEConfigurationTool.src
{
    internal class FileLocator
    {
        private readonly List<string> searchPaths;
        private readonly List<string> filesToFind;

        public FileLocator(List<string> searchPaths, List<string> filesToFind)
        {
            this.searchPaths = searchPaths;
            this.filesToFind = filesToFind;
        }

        public Dictionary<string, string> FindFiles()
        {
            var foundFiles = new Dictionary<string, string>();

            foreach (var fileName in filesToFind)
            {
                var filePath = FindFile(fileName);
                if (!string.IsNullOrEmpty(filePath))
                {
                    foundFiles[fileName] = filePath;
                }
            }

            return foundFiles;
        }

        private string FindFile(string fileName)
        {
            foreach (var path in searchPaths)
            {
                try
                {
                    var files = Directory.EnumerateFiles(path, fileName, SearchOption.AllDirectories);
                    if (files.Any())
                    {
                        return files.First();
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Access to the path '{path}' is denied: {ex.Message}");
                }
                catch (DirectoryNotFoundException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"The directory '{path}' was not found: {ex.Message}");
                }
                catch (IOException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"An I/O error occurred while accessing '{path}': {ex.Message}");
                }
            }

            return null;
        }

        public Dictionary<string, string> GetMissingFiles(Dictionary<string, string> foundFiles)
        {
            return filesToFind.Except(foundFiles.Keys).ToDictionary(file => file, file => searchPaths.First());
        }
    }
}
