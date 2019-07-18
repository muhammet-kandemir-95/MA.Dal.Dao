using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dao.Generate.VSIX
{
    public static class CommandStd
    {
        #region Variables
        public static ConvertSQL.Table a = null;
        public static DTE2 CurrentDTE2
        {
            get
            {
                return Package.GetGlobalService(typeof(SDTE)) as DTE2;
            }
        }
        #endregion

        #region Methods
        public static void AddProjectToFile(string fileName, string txt, params string[] subDirectories)
        {
            ProjectItems projectItems;
            string saveDirectory;
            string projectPath;
            string projectName;
            GetSaveDirectory(out projectItems, out saveDirectory, out projectPath, out projectName);

            var lastDirectory = saveDirectory;
            foreach (var directory in subDirectories)
            {
                lastDirectory = Path.Combine(lastDirectory, directory);
                if (!Directory.Exists(lastDirectory))
                    Directory.CreateDirectory(lastDirectory);
            }
            var fileResultPath = Path.Combine(lastDirectory, fileName);
            if (File.Exists(fileResultPath))
                throw new Exception("Exists file!");

            File.WriteAllText(fileResultPath, txt);
            projectItems.AddFromFile(fileResultPath);
        }

        public static void GetSaveDirectory(out ProjectItems projectItems, out string saveDirectory, out string projectPath, out string projectName)
        {
            IntPtr hierarchyPointer, selectionContainerPointer;
            Object selectedObject = null;
            IVsMultiItemSelect multiItemSelect;
            uint projectItemId;
            projectPath = "";

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
            projectItems = null;
            saveDirectory = "";
            if (selectedProject != null)
            {
                saveDirectory = Path.GetDirectoryName(selectedProject.FullName) + "\\";
                projectPath = saveDirectory;
                projectItems = selectedProject.ProjectItems;
                projectName = selectedProject.Name;
            }
            else
            {
                var projectItem = CurrentDTE2.SelectedItems.Item(1).ProjectItem;
                projectPath = Path.GetDirectoryName(projectItem.ContainingProject.FullName) + "\\";
                projectName = projectItem.ContainingProject.Name;
                projectItems = projectItem.ContainingProject.ProjectItems;
                saveDirectory = projectItem.Properties.Item(6).Value.ToString();
            }
        }
        #endregion
    }
}
