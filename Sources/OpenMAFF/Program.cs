
// Copyright (c) Christophe Bertrand. All Rights Reserved.
// https://chrisbertrand.net
// https://github.com/ChrisBertrandDotNet/OpenMAFF

#if false
#define EFFACE_FICHIERS_TEMPORAIRES // The timing can be problematic with some web browsers.
#endif

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace OpenMAFF
{
	static class Program
	{
		[STAThread]
		static int Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Maintenance.RemoveOldTemporaryFiles();

			if (args.Length == 0)
			{
#if false
				// GUI:
				Application.Run(new Form1());
				return 0;
#else
				var messageErreur = "The first parameter must be a MAFF file name.";
				GUI.DisplayError(messageErreur);
				Environment.ExitCode = 1;
				return 1;
#endif
			}

			return OpenMAFFFile(args[0]);
		}

		static int OpenMAFFFile(string MAFFFile)
		{
			string errorMessage = null;
			if (!File.Exists(MAFFFile))
			{
				errorMessage = "This file does not exist: " + MAFFFile + "\n";
				if (!MAFFFile.StartsWith("\""))
					errorMessage += "Did you forget to add quotation marks (\") ?";
			}
			if (errorMessage != null)
			{
				GUI.DisplayError(errorMessage);
				Environment.ExitCode = 1;
				return 1;
			}
			return OpenMAFFFile2(MAFFFile);
		}

		static int OpenMAFFFile2(string fichierMAFF)
		{
			bool moreThanOnePage;
			var pages = MAFF_File.ExtractPages(fichierMAFF, out moreThanOnePage);
			if (pages == null)
				return 1;

			if (moreThanOnePage)
				return OpenSeveralHTMLWithBrowser(pages);
			else
				return OpenHTMLWithBrowser(pages);
		}

		/// <summary>
		/// Let default application open the HTML document.
		/// </summary>
		/// <param name="HTMLFile"></param>
		/// <returns></returns>
		static int OpenHTMLWithBrowser(string HTMLFile)
		{
			try
			{
				using (var proc = Process.Start(HTMLFile))
				{
#if EFFACE_FICHIERS_TEMPORAIRES
					proc.WaitForExit();
#endif
				}
				return 0;
			}
			catch (Exception ex)
			{
				GUI.DisplayError(ex.Message);
				return 4;
			}
		}

		/// <summary>
		/// First finds the application that opens HTML documents.
		/// Then call it with several file names so that the application opens them in tabs.
		/// </summary>
		/// <param name="HTMLFile"></param>
		/// <returns></returns>
		static int OpenSeveralHTMLWithBrowser(string HTMLFile)
		{
			try
			{
				var file1 = HTMLFile.Split(new char[] { '"' })[1];
				var app = FindExecutable(file1);
				if (string.IsNullOrEmpty(app))
					return 5;
				using (var proc = Process.Start(app, HTMLFile))
				{
#if EFFACE_FICHIERS_TEMPORAIRES
					proc.WaitForExit();
#endif
				}
				return 0;
			}
			catch (Exception ex)
			{
				GUI.DisplayError(ex.Message);
				return 4;
			}
		}

		static bool HasExecutable(string path)
		{
			var executable = FindExecutable(path);
			return !string.IsNullOrEmpty(executable);
		}

		static string FindExecutable(string path)
		{
			var executable = new StringBuilder(1024);
			FindExecutable(path, string.Empty, executable);
			return executable.ToString();
		}

		[System.Runtime.InteropServices.DllImport("shell32.dll", EntryPoint = "FindExecutable")]
		static extern long FindExecutable(string lpFile, string lpDirectory, StringBuilder lpResult);
	}
}