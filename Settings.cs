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
        [DisplayName("Enable")]
        [Description("Enable automatic namespace fixing")]
        public bool Enabled { get; set; } = true;
    }
}