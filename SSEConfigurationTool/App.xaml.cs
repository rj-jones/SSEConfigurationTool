using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using SSEConfigurationTool.src;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SSEConfigurationTool
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private IniFile _skyrimCustomIni;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            var searchPaths = new List<string>
            {
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My Games\Skyrim Special Edition",
            };

            // Note: SkyrimPrefs.ini overrides SkyrimCustom.ini which overrides Skyrim.ini
            // see https://stepmodifications.org/wiki/Guide:Skyrim_Configuration_Settings
            var filesToFind = new List<string>
            {
                "Skyrim.ini",
                "SkyrimPrefs.ini",
                "SkyrimCustom.ini"
            };

            var fileLocator = new FileLocator(searchPaths, filesToFind);
            var foundFiles = fileLocator.FindFiles();
            var missingFiles = fileLocator.GetMissingFiles(foundFiles);

            foreach (var file in foundFiles)
            {
                System.Diagnostics.Debug.WriteLine($"Found {file.Key} at {file.Value}");
            }

            foreach (var file in missingFiles)
            {
                System.Diagnostics.Debug.WriteLine($"Could not find {file.Key}, creating {file.Key} at {file.Value}");
                File.Create($"{file.Value}/{file.Key}").Dispose();
            }

            _skyrimCustomIni = new IniFile(foundFiles["SkyrimCustom.ini"]);

            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow(_skyrimCustomIni);
            m_window.Activate();
        }

        private Window m_window;
    }
}
