using System;
using System.IO;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharpSetup.Dialogs;
using File = WixSharp.File;

namespace WixSharpSetup
{
	internal class Program
	{
		/// <summary>
		///     Example Setup for an Chronos Plugin, written with the NuGet-Package WixSharp
		///     <see cref="https://github.com/oleg-shilo/wixsharp" />.
		///     Requirement is the WixToolSet <see cref="http://wixtoolset.org/releases/" />
		/// </summary>
		private static void Main()
		{
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
			var project = new ManagedProject("MockPlugin", //Name of the Plugin
				new Dir(@"%ProgramFiles%\Chronos\Plugins\MockPlugin", //Install-Dir to the Chronos-Plugin Folder
					new File("MockPlugin.dll"),
					/*
					* or
					* new DirFiles("*.dll") Use Wildcards for all Files from one Typ
					*/
					new Dir("de",
						new File("de\\MockPlugin.resources.dll")
					)
				)
			);

			project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
			// Since we have a reference to mockplugin.dll added, it is copied to our current working directory.
            project.SourceBaseDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			//project.OutDir = "<output dir path>";	 //Specific Output Path from the Setup

			#region Setup Infos

			project.ControlPanelInfo.Manufacturer = "Company name";
			project.ControlPanelInfo.Comments = "Product name";
			project.Description = $"Version: {project.Version}";
			project.ControlPanelInfo.Contact = "Company name";
			project.ControlPanelInfo.InstallLocation = "[INSTALLDIR]";

			project.BannerImage = @"Images\Banner.png";
			project.BackgroundImage = @"Images\Background.png";

			project.Version =
				Tasks.GetVersionFromFile(Path.Combine(project.SourceBaseDir,
					"MockPlugin.dll")); //Sets the Installer Verion of AssemblyVersion from the Plugin.dll

			#endregion Setup Infos

			#region Setup GUI

			//custom set of standard UI dialogs
			project.ManagedUI = new ManagedUI();

			project.ManagedUI.InstallDialogs.Add<WelcomeDialog>()
				.Add<LicenceDialog>()
				//.Add<SetupTypeDialog>() Unused dialogs can be taken out or removed
				//.Add<FeaturesDialog>()  
				.Add<InstallDirDialog>()
				.Add<ProgressDialog>()
				.Add<ExitDialog>();

			project.ManagedUI.ModifyDialogs.Add<MaintenanceTypeDialog>()
				.Add<FeaturesDialog>()
				.Add<ProgressDialog>()
				.Add<ExitDialog>();

			#endregion Setup GUI

			project.OutFileName = "MockPlugin-Setup";

			project.BuildMsi();
		}
	}
}