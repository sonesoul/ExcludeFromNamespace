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
    typeof(GeneralOptions),
    "Projects and Solutions",
    "General",
    0,
    0,
    true)]
    public sealed class Package : AsyncPackage
    {
        public const string PackageGuidString = "6efff7ff-fae2-4b54-b1d6-22a6221eeb3a";
        public const string ExcludedDirectory = "src";

        private GeneralOptions Options => (GeneralOptions)GetDialogPage(typeof(GeneralOptions));

        private DTE2 _dte;
        private ProjectItemsEvents _projectItemsEvents;
        
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            _dte = await GetServiceAsync(typeof(DTE)) as DTE2;

            if (_dte == null)
                return;

            _projectItemsEvents = (_dte.Events as Events2).ProjectItemsEvents;

            _projectItemsEvents.ItemAdded += OnItemAdded;
        }

        private void OnItemAdded(ProjectItem item)
        {
            if (item == null || !Options.Enabled)
                return;

            ThreadHelper.ThrowIfNotOnUIThread();

            var dir = Options.ExcludedDirectory;

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
