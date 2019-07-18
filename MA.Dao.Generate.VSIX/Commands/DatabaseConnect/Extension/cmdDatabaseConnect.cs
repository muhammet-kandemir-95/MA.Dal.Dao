using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using EnvDTE;
using MA.Dao.Generate.VSIX.Commands.DatabaseConnect.UI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace MA.Dao.Generate.VSIX.Commands.DatabaseConnect.Extension
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class cmdDatabaseConnect
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4133;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("1f180676-19d2-4c84-a765-1812f70601b0");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="cmdDatabaseConnect"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private cmdDatabaseConnect(Package package)
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
        public static cmdDatabaseConnect Instance
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
            Instance = new cmdDatabaseConnect(package);
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
                ProjectItems projectItems;
                string saveDirectory;
                string projectPath;
                string projectName;
                CommandStd.GetSaveDirectory(out projectItems, out saveDirectory, out projectPath, out projectName);

                string beforeApp = "";
                string beforeAppPath = Path.Combine(saveDirectory, "maGenerateBeforeAppPath.madata");
                if (File.Exists(beforeAppPath))
                    beforeApp = File.ReadAllText(beforeAppPath);

                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = "SELECT MA.Dao.Generate.VSIX > MA.Dao.Generate.VSIX.App > .exe";
                    ofd.Multiselect = false;
                    ofd.Filter = "*.exe|*.exe";
                    var beforeControl = (!string.IsNullOrEmpty(beforeApp) && File.Exists(beforeApp));

                    if (beforeControl || ofd.ShowDialog() == DialogResult.OK)
                    {
                        if (!beforeControl)
                            beforeApp = ofd.FileName;
                        var process = System.Diagnostics.Process.Start(beforeApp);
                        process.WaitForExit();

                        var appPath = Path.GetDirectoryName(beforeApp);
                        File.WriteAllText(beforeAppPath, beforeApp);
                        Models.GenerateSQL generate = Models.GenerateSQL.Load(Path.Combine(appPath, "generate.txt"));
                        if (
                            generate.Tables.Count +
                            generate.Views.Count +
                            generate.Procedures.Count +
                            generate.FunctionsScalar.Count +
                            generate.FunctionsTable.Count == 0
                            )
                            return;

                        using (frmDatabaseConnect frm = new frmDatabaseConnect())
                        {
                            frm.Generate = generate;
                            if (frm.ShowDialog() == DialogResult.OK)
                            {
                                generate = frm.Generate;

                                Func<string, string> directoryNameToNameSpace = (string name) => name.Replace(" ", "_").Replace("-", "_");
                                
                                var namespaceP = directoryNameToNameSpace(projectName);
                                var directoryFromProject = saveDirectory.Replace(projectPath, "");
                                var directoriesInProject = directoryFromProject.Split(new string[] { "\\", "\\\\" }, StringSplitOptions.RemoveEmptyEntries);
                                namespaceP += directoriesInProject.Length == 0 ? "" : "." + string.Join(".", directoriesInProject.Select(o => directoryNameToNameSpace(o)).ToArray());

                                Func<string, string> codeAddNamespace = (string code) => code.Replace("namespace MA.Database", "namespace " + namespaceP + ".MA.Database");

                                foreach (var table in generate.Tables)
                                {
                                    try
                                    {
                                        CommandStd.AddProjectToFile(table.SafeName + ".cs", codeAddNamespace(table.Code), "Tables");
                                    }
                                    catch
                                    { }
                                }

                                foreach (var procedure in generate.Procedures)
                                {
                                    try
                                    {
                                        CommandStd.AddProjectToFile(procedure.SafeName + ".cs", codeAddNamespace(procedure.Code), "Procedures");
                                    }
                                    catch
                                    { }
                                }

                                foreach (var function in generate.FunctionsScalar)
                                {
                                    try
                                    {
                                        CommandStd.AddProjectToFile(function.SafeName + ".cs", codeAddNamespace(function.Code), "Functions", "Scalar");
                                    }
                                    catch
                                    { }
                                }

                                foreach (var function in generate.FunctionsTable)
                                {
                                    try
                                    {
                                        CommandStd.AddProjectToFile(function.SafeName + ".cs", codeAddNamespace(function.Code), "Functions", "Table");
                                    }
                                    catch
                                    { }
                                }

                                foreach (var view in generate.Views)
                                {
                                    try
                                    {
                                        CommandStd.AddProjectToFile(view.SafeName + ".cs", codeAddNamespace(view.Code), "Views");
                                    }
                                    catch
                                    { }
                                }
                            }
                        }
                    }
                }
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
    }
}
