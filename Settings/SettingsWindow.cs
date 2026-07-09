using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace ExcludeFromNamespace.Settings
{
    [Guid("ddd7025a-2926-4dde-87e2-70ca1d70bb46")]
    public class SettingsWindow : ToolWindowPane
    {
        public SettingsWindow() : base(null)
        {
            Caption = "Settings";
            Content = new SettingsWindowControl();
        }
    }
}