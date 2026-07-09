using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace ExcludeFromNamespace
{
    [Guid("ddd7025a-2926-4dde-87e2-70ca1d70bb46")]
    public class SettingsWindow : ToolWindowPane
    {
        public SettingsWindow() : base(null)
        {
            Caption = "Options";
            Content = new SettingsWindowControl(this.Package as ExcludeFromNamespace.Package);
        }
    }
}