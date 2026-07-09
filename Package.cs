using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ExcludeFromNamespace
{
    [PackageRegistration(
    UseManagedResourcesOnly = true,
    AllowsBackgroundLoading = true)]

    [ProvideAutoLoad(
    Microsoft.VisualStudio.Shell.Interop.UIContextGuids80.SolutionExists,
    PackageAutoLoadFlags.BackgroundLoad)]

    [Guid(PackageGuidString)]

    [ProvideOptionPage(
    typeof(Settings),
    "Exclude From Namespace",
    "General",
    0,
    0,
    true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(SettingsWindow))]
    public sealed class Package : AsyncPackage
    {
        public const string PackageGuidString = "6efff7ff-fae2-4b54-b1d6-22a6221eeb3a";

        public static Settings Settings { get; private set; }

        private DTE2 _dte;
        private ProjectItemsEvents _projectItemsEvents;
        
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            _dte = await GetServiceAsync(typeof(DTE)) as DTE2;

            if (_dte == null)
                return;

            Settings = (Settings)GetDialogPage(typeof(Settings));
            
            _projectItemsEvents = (_dte.Events as Events2).ProjectItemsEvents;
            _projectItemsEvents.ItemAdded += OnItemAdded;

            await SettingsWindowCommand.InitializeAsync(this);
        }

        private void OnItemAdded(ProjectItem item)
        {
            if (item == null || !Settings.Enabled)
                return;

            ThreadHelper.ThrowIfNotOnUIThread();

            var dir = Settings.ExcludedDirectory;

            if (item.Name.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            {
                string filePath = item.FileNames[1];

                if (filePath.Contains($"{Path.DirectorySeparatorChar}{dir}{Path.DirectorySeparatorChar}"))
                {
                    string content = File.ReadAllText(filePath);

                    if (content.Contains(dir))
                    {
                        File.WriteAllText(filePath, content.Replace($".{dir}", string.Empty));
                    }
                }
            }
        }
    }
}
