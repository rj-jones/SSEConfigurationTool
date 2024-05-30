using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SSEConfigurationTool.src
{
    /// <summary>
    /// Provides methods to create, read, write, and delete INI file entries.
    /// </summary>
    public class IniFile
    {
        private readonly string path;
        private readonly string exeName = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string @default, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile"/> class.
        /// </summary>
        /// <param name="iniPath">The path to the INI file. If null, uses the executable name with .ini extension.</param>
        public IniFile(string iniPath = null)
        {
            path = new FileInfo(iniPath ?? exeName + ".ini").FullName;
        }

        /// <summary>
        /// Reads a value from the INI file.
        /// </summary>
        /// <param name="key">The key to read.</param>
        /// <param name="section">The section to read from. If null, uses the executable name.</param>
        /// <returns>The value associated with the specified key.</returns>
        public string Read(string key, string section = null)
        {
            var retVal = new StringBuilder(255);
            GetPrivateProfileString(section ?? exeName, key, string.Empty, retVal, retVal.Capacity, path);
            return retVal.ToString();
        }

        /// <summary>
        /// Writes a value to the INI file.
        /// </summary>
        /// <param name="key">The key to write.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="section">The section to write to. If null, uses the executable name.</param>
        public void Write(string key, string value, string section = null)
        {
            if (WritePrivateProfileString(section ?? exeName, key, value, path) == 0)
            {
                throw new InvalidOperationException("Failed to write to the INI file.");
            }
        }

        /// <summary>
        /// Deletes a key from the INI file.
        /// </summary>
        /// <param name="key">The key to delete.</param>
        /// <param name="section">The section to delete from. If null, uses the executable name.</param>
        public void DeleteKey(string key, string section = null)
        {
            Write(key, null, section);
        }

        /// <summary>
        /// Deletes a section from the INI file.
        /// </summary>
        /// <param name="section">The section to delete. If null, uses the executable name.</param>
        public void DeleteSection(string section = null)
        {
            Write(null, null, section);
        }

        /// <summary>
        /// Checks if a key exists in the INI file.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="section">The section to check in. If null, uses the executable name.</param>
        /// <returns><c>true</c> if the key exists; otherwise, <c>false</c>.</returns>
        public bool KeyExists(string key, string section = null)
        {
            return !string.IsNullOrEmpty(Read(key, section));
        }
    }
}
