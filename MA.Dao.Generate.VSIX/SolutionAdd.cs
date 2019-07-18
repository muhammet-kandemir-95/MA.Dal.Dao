using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using EnvDTE;
using EnvDTE80;
using System.IO;

namespace MA.Dao.Generate.VSIX
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SolutionAdd
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("1f180676-19d2-4c84-a765-1812f70601b0");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionAdd"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private SolutionAdd(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static SolutionAdd Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new SolutionAdd(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            try
            {
                GetActiveDirectory("test.cs", "class test{}");
                VsShellUtilities.ShowMessageBox(
                    this.ServiceProvider,
                    "MESSAGE",
                    "Success.",
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
            catch (Exception ex)
            {
                VsShellUtilities.ShowMessageBox(
                    this.ServiceProvider,
                    "WARNING",
                    ex.Message,
                    OLEMSGICON.OLEMSGICON_WARNING,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }


        }

        #region Variables
        private static DTE2 currentDTE2 = null;
        public static DTE2 CurrentDTE2
        {
            get
            {
                if (currentDTE2 == null)
                    currentDTE2 = Package.GetGlobalService(typeof(SDTE)) as DTE2;
                return currentDTE2;
            }
        }
        #endregion

        #region Methods
        public static void GetActiveDirectory(string fileName, string txt)
        {
            IntPtr hierarchyPointer, selectionContainerPointer;
            Object selectedObject = null;
            IVsMultiItemSelect multiItemSelect;
            uint projectItemId;

            IVsMonitorSelection monitorSelection =
                    (IVsMonitorSelection)Package.GetGlobalService(
                    typeof(SVsShellMonitorSelection));

            monitorSelection.GetCurrentSelection(out hierarchyPointer,
                                                 out projectItemId,
                                                 out multiItemSelect,
                                                 out selectionContainerPointer);

            IVsHierarchy selectedHierarchy = Marshal.GetTypedObjectForIUnknown(
                                                 hierarchyPointer,
                                                 typeof(IVsHierarchy)) as IVsHierarchy;

            if (selectedHierarchy != null)
            {
                ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(
                                                  projectItemId,
                                                  (int)__VSHPROPID.VSHPROPID_ExtObject,
                                                  out selectedObject));
            }

            Project selectedProject = selectedObject as Project;
            ProjectItems projectItems = null;

            string saveDirectory = "";
            if (selectedProject != null)
            {
                saveDirectory = Path.GetDirectoryName(selectedProject.FullName) + "\\";
                projectItems = selectedProject.ProjectItems;
            }
            else
            {
                var projectItem = CurrentDTE2.SelectedItems.Item(1).ProjectItem;
                projectItems = projectItem.ProjectItems;
                saveDirectory = projectItem.Properties.Item(6).Value.ToString();
            }
            if (File.Exists(saveDirectory + fileName))
                throw new Exception("Exists file!");

            File.WriteAllText(saveDirectory + fileName, txt);
            projectItems.AddFromFile(saveDirectory + fileName);
        }
        #endregion
    }
}
