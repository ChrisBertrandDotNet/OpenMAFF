
// Copyright (c) Christophe Bertrand. All Rights Reserved.
// https://chrisbertrand.net
// https://github.com/ChrisBertrandDotNet

#if false
#define EFFACE_FICHIERS_TEMPORAIRES // Pose des problèmes de timing avec certains navigateurs Web.
#endif

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;

namespace OpenMAFF
{
	static class Program
	{
		/// <summary>
		/// Point d'entrée principal de l'application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Maintenance.RemoveOldTemporaryFiles();

			if (args.Length == 0)
			{
#if false
				// IUG:
				Application.Run(new Form1());
				return 0;
#else
				var messageErreur = "The first parameter must be a MAFF file name.";
				AfficheErreur(messageErreur);
				Environment.ExitCode = 1;
				return 1;
#endif
			}

			// ouverture du fichier passé en paramètre:
			return OuvreFichierMAFF(args[0]);
		}

		static void AfficheErreur(string messageErreur)
		{
			MessageBox.Show(messageErreur, "OpenMAFF : Error");
		}

		static int OuvreFichierMAFF(string fichierMAFF)
		{
			string messageErreur = null;
			if (!File.Exists(fichierMAFF))
			{
				messageErreur = "This file does not exist: " + fichierMAFF + "\n";
				if (!fichierMAFF.StartsWith("\""))
					messageErreur += "Did you forget to add quotation marks (\") ?";
			}
			if (messageErreur != null)
			{
				AfficheErreur(messageErreur);
				Environment.ExitCode = 1;
				return 1;
			}
			return OuvreFichierMAFF2(fichierMAFF);
		}

		static int OuvreFichierMAFF2(string fichierMAFF)
		{
			DirectoryInfo répertoire = null;
			int ret = 0;

			try
			{
				répertoire = ExtraitFichiers(fichierMAFF);

				if (répertoire == null)
					return 2;

				{
					const string nomIndexHTML = "index.html";
					const string nomIndexHTM = "index.htm";

					var f = répertoire.GetFiles(nomIndexHTML);
					if (f.Length == 0)
						f = répertoire.GetFiles(nomIndexHTM);
					if (f.Length > 0)
						ret = OuvreHTMLAvecNavigateur("\"" + f[0].FullName + "\"");
					else
					{
						var sousRéps = répertoire.EnumerateDirectories();
						var sb = new StringBuilder();
						bool plusieursFichiers = false;
						foreach (var sousRép in sousRéps)
						{
							var f2 = sousRép.GetFiles(nomIndexHTML);
							if (f2.Length == 0)
								f2 = sousRép.GetFiles(nomIndexHTM);
							if (f2.Length > 0)
							{
								if (sb.Length > 0)
								{
									sb.Append(' '); // espace entre les noms.
									plusieursFichiers = true;
								}
								sb.Append('"');
								sb.Append(f2[0].FullName);
								sb.Append('"');
							}
						}
						if (sb.Length > 0)
							if (plusieursFichiers)
								ret = OuvrePlusieursHTMLAvecNavigateur(sb.ToString());
							else
								ret = OuvreHTMLAvecNavigateur(sb.ToString());
					}
				}
			}
			catch (Exception ex)
			{
				AfficheErreur(ex.Message);
			}
			finally // à la fin, on efface le répertoire temporaire.
			{
#if EFFACE_FICHIERS_TEMPORAIRES
				if (répertoire != null && Directory.Exists(répertoire.FullName))
					try { répertoire.Delete(true); }
					catch { } // Ça n'est pas une catastrophe, le système l'effacera pour nous plus tard.
#endif
			}
			return ret;
		}

		static DirectoryInfo ExtraitFichiers(string fichier)
		{
			try
			{
				var temp = Settings.CommonSettings.Value.TemporaryFilesDirectory; //System.IO.Path.GetTempPath();
				if (!string.IsNullOrEmpty(temp) && Directory.Exists(temp))
				{
					var répertoire = Path.Combine(temp,
						Common.TempDirectoriesPrefix +// "Maff_" +
							Guid.NewGuid().ToString()); // Le guid évite les collisions (sauf si pas de chance).
					var di = Directory.CreateDirectory(répertoire);

					ZipFile.ExtractToDirectory(fichier, répertoire);


					return di;
				}
			}
			catch (Exception ex)
			{
				AfficheErreur(ex.Message);
			}

			return null;
		}

		/// <summary>
		/// Ouvre le document HTML sans se préoccuper de quelle application est sencé l'ouvrir.
		/// </summary>
		/// <param name="fichierHTML"></param>
		/// <returns></returns>
		static int OuvreHTMLAvecNavigateur(string fichierHTML)
		{
			try
			{
				using (var proc = Process.Start(fichierHTML))
				{
#if EFFACE_FICHIERS_TEMPORAIRES
					proc.WaitForExit();
#endif
				}
				return 0;
			}
			catch (Exception ex)
			{
				AfficheErreur(ex.Message);
				return 4;
			}
		}

		/// <summary>
		/// Cherche d'abord quelle application ouvre les documents HTML, puis l'appelle avec plusieurs noms de fichier, pour que l'application les ouvre dans des onglets.
		/// </summary>
		/// <param name="fichiersHTML"></param>
		/// <returns></returns>
		static int OuvrePlusieursHTMLAvecNavigateur(string fichiersHTML)
		{
			try
			{
				var fichier1 = fichiersHTML.Split(new char[] { '"' })[1];
				var app = FindExecutable(fichier1);
				if (string.IsNullOrEmpty(app))
					return 5;
				using (var proc = Process.Start(app, fichiersHTML))
				{
#if EFFACE_FICHIERS_TEMPORAIRES
					proc.WaitForExit();
#endif
				}
				return 0;
			}
			catch (Exception ex)
			{
				AfficheErreur(ex.Message);
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