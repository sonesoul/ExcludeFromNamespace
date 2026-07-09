using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace ExcludeFromNamespace
{
    public partial class SettingsWindowControl : UserControl
    {
        private Settings _options;

        public SettingsWindowControl(ExcludeFromNamespace.Package package)
        {
            InitializeComponent();
            _options = package.Options;
            _options.LoadSettingsFromStorage();

            FolderBox.Text = _options.ExcludedDirectory;
            EnabledBox.IsChecked = _options.Enabled;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _options.ExcludedDirectory = FolderBox.Text;
            _options.Enabled = EnabledBox.IsChecked == true;

            _options.SaveSettingsToStorage();
        }
    }
}