using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace ExcludeFromNamespace
{
    public class Settings : DialogPage
    {
        [Category("General")]
        [DisplayName("Excluded Directory Name")]
        [Description("Directory name to remove from namespace")]
        public string ExcludedDirectory { get; set; } = "src";

        [Category("General")]
        [DisplayName("Safe Editing")]
        [Description("Uses syntax analysis to safely modify namespaces instead of simple text replacement. This may be slower.")]
        public bool SafeEditing { get; set; } = true;

        [Category("General")]
        [DisplayName("Enable")]
        [Description("Enable automatic namespace fixing")]
        public bool Enabled { get; set; } = true;
    }
}