using EnvDTE;
using EnvDTE80;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

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
            if (!Settings.Enabled || item == null)
                return;

            ThreadHelper.ThrowIfNotOnUIThread();

            if (!item.Name.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                return;

            var dir = Settings.ExcludedDirectory;
            string path = item.FileNames[1];

            if (path.Contains($"{Path.DirectorySeparatorChar}{dir}{Path.DirectorySeparatorChar}"))
            {
                if (Settings.SafeEditing)
                    RoslynRemove(dir, path);
                else
                    ByLineRemove(dir, path);
            }
        }

        protected override void Dispose(bool disposing)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            
            if (disposing && _projectItemsEvents != null)
            {
                _projectItemsEvents.ItemAdded -= OnItemAdded;
            }

            base.Dispose(disposing);
        }

        private static void ByLineRemove(string dir, string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (!line.StartsWith("namespace "))
                    continue;

                string oldNamespace = line.Substring("namespace ".Length);

                oldNamespace = oldNamespace.TrimEnd(';');

                string[] parts = oldNamespace.Split('.');

                parts = parts
                    .Where(x => !string.Equals(x, dir, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                string newNamespace = string.Join(".", parts);

                string ending = lines[i].EndsWith(";") ? ";" : "";

                lines[i] = lines[i].Replace(oldNamespace, newNamespace + ending);

                break;
            }

            File.WriteAllLines(filePath, lines);
        }
        private static void RoslynRemove(string dir, string filePath)
        {
            string content = File.ReadAllText(filePath);

            var root = CSharpSyntaxTree
                    .ParseText(content)
                    .GetRoot();

            var namespaceNode = root
                .DescendantNodes()
                .OfType<BaseNamespaceDeclarationSyntax>()
                .FirstOrDefault();

            if (namespaceNode == null)
                return;

            var fixedName = namespaceNode
                .Name
                .ToString()
                .Replace($".{dir}", "");

            var newNamespace =
                namespaceNode
                .WithName(
                    SyntaxFactory.ParseName(fixedName)
                );

            File.WriteAllText(filePath, root
                .ReplaceNode(namespaceNode, newNamespace)
                .ToFullString()
            );
        }
    }
}
