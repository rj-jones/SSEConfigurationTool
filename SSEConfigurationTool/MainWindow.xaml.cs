using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using SSEConfigurationTool.src;
using System.ComponentModel;
using System.Collections.Specialized;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SSEConfigurationTool
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private IniFile _skyrimCustomIni;
        private ObservableDictionary<string, string> _skyrimCustomIniValues;
        private Dictionary<string, string> _skyrimCustomIniValuesDefault;

        public MainWindow(IniFile skyrimCustomIni)
        {
            this.InitializeComponent();
            ResizeAndCenterWindow(1280, 720);
            _skyrimCustomIni = skyrimCustomIni;
            // TODO: Update where we setup the default values (read from the INI file)
            _skyrimCustomIniValues = new ObservableDictionary<string, string>();
            _skyrimCustomIniValues.CollectionChanged += SkyrimCustomIniValues_CollectionChanged;
            _skyrimCustomIniValuesDefault = new Dictionary<string, string> {
                { "fDefault1stPersonFOV", "65" },
            };
        }

        private bool HasChanges()
        {
            foreach (var key in _skyrimCustomIniValues.Keys)
            {
                if (_skyrimCustomIniValues[key] != _skyrimCustomIniValuesDefault[key])
                {
                    return true;
                }
            }
            return false;
        }

        private void SkyrimCustomIniValues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (e.Action == NotifyCollectionChangedAction.Add)
            //{
            //    // Handle additions
            //}
            //else if (e.Action == NotifyCollectionChangedAction.Remove)
            //{
            //    // Handle removals
            //}
            //else if (e.Action == NotifyCollectionChangedAction.Replace)
            //{
            //    // Handle replacements
            //}

            var hasChanges = HasChanges();
            if (hasChanges && this.saveChanges.Content.ToString() != "Save Changes*")
            {
                this.saveChanges.Content = "Save Changes*";
            }
            else if (!hasChanges && this.saveChanges.Content.ToString() != "Save Changes")
            {
                this.saveChanges.Content = "Save Changes";
            }
        }

        // TODO: Add a cancel/clear button for each input that only shows when the value is different from the default value.
        private void FovSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            double newValue = e.NewValue;
            if (_skyrimCustomIniValues != null)
            {
                _skyrimCustomIniValues["fDefault1stPersonFOV"] = newValue.ToString();
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (HasChanges())
            {
                foreach (var key in _skyrimCustomIniValues.Keys)
                {
                    // TODO: Need to know if the write was successful or not so we can
                    // determine whether or not we need to remove the key from the dictionary.
                    _skyrimCustomIni.Write(key, _skyrimCustomIniValues[key], "Display");
                    _skyrimCustomIniValues.Remove(key);
                }
            }
        }

        private void ResizeAndCenterWindow(int width, int height)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            // Resize the window
            appWindow.Resize(new SizeInt32(width, height));

            // Get the current display's working area
            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);

            // Calculate the center position
            var centerX = (displayArea.WorkArea.Width - width) / 2;
            var centerY = (displayArea.WorkArea.Height - height) / 2;

            // Move the window to the center position
            appWindow.Move(new PointInt32(centerX, centerY));
        }
    }
}
