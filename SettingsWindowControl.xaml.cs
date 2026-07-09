using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace ExcludeFromNamespace
{
    public partial class SettingsWindowControl : UserControl
    {
        private Settings _options;

        public SettingsWindowControl()
        {
            InitializeComponent();
            _options = Package.Settings;
            _options.LoadSettingsFromStorage();

            FolderBox.Text = _options.ExcludedDirectory;
            SafeEditingBox.IsChecked = _options.SafeEditing;
            EnabledBox.IsChecked = _options.Enabled;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _options.ExcludedDirectory = FolderBox.Text;
            _options.SafeEditing = SafeEditingBox.IsChecked == true;
            _options.Enabled = EnabledBox.IsChecked == true;

            _options.SaveSettingsToStorage();
        }
    }
}