
// Copyright (c) Christophe Bertrand. All Rights Reserved.
// https://chrisbertrand.net
// https://github.com/ChrisBertrandDotNet/OpenMAFF

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace OpenMAFF
{
	/// <summary>
	/// A helper for MAFF files.
	/// </summary>
	internal static class MAFF_File
	{
		const string indexHTML = "index.html";
		const string indexHTM = "index.htm";
		const string indexRDF = "index.rdf";

		/// <summary>
		/// Extracts files of the MAFF archive.
		/// </summary>
		/// <param name="fichierMAFF"></param>
		/// <returns>All page names, separated by a space. Or null if something went wrong.</returns>
		internal static string ExtractPages(string fichierMAFF, out bool moreThanOnePage)
		{
			moreThanOnePage = false;

			DirectoryInfo directory = null;

			try
			{
				directory = ExtractFiles(fichierMAFF);

				if (directory == null)
					return null;

				{
					var f = directory.GetFiles(indexHTML);
					if (f.Length == 0)
						f = directory.GetFiles(indexHTM);
					if (f.Length > 0) // There is a page at the root of the MAFF. We take into account malformatted MAFF too.
						return ("\"" + f[0].FullName + "\"");
					else
					{
						var subDirs = directory.EnumerateDirectories();
						var sb = new StringBuilder();
						foreach (var subDir in subDirs)
						{
							var page = GetPageFileName(subDir);
							if (page!=null)
							{
								if (sb.Length > 0)
								{
									sb.Append(' '); // insert a space between names.
									moreThanOnePage = true;
								}
								sb.Append('"');
								sb.Append(page);
								sb.Append('"');
							}
						}
						if (sb.Length > 0)
						{
							return sb.ToString();
						}
					}
				}
			}
			catch (Exception ex)
			{
				GUI.DisplayError(ex.Message);
			}
#if EFFACE_FICHIERS_TEMPORAIRES
			finally // à la fin, on efface le répertoire temporaire.
			{
				if (répertoire != null && Directory.Exists(répertoire.FullName))
					try { répertoire.Delete(true); }
					catch { } // Ça n'est pas une catastrophe, le système l'effacera pour nous plus tard.
			}
#endif
			return null;
		}

		static string GetPageFileName(DirectoryInfo subDir)
		{
			var file = subDir.GetFiles(indexRDF).FirstOrDefault();
			if (file != null)
			{
				var rdf = RDF_of_MAFF.New(file.FullName);
				if (rdf!=null)
				{
					var indexFile =subDir.GetFiles(rdf.IndexFileName).FirstOrDefault();
					if (indexFile != null)
					{
						return ContainingPage.SaveBuiltPage(indexFile.FullName, rdf.OriginalUrl, rdf.Title, subDir.FullName);
					}
				}
			}
			return GetPageFileName2(subDir); // better than nothing.
		}

		/// <summary>
		/// Finds an index.html or index.htm page and returns it.
		/// </summary>
		/// <param name="subDir"></param>
		/// <returns></returns>
		static string GetPageFileName2(DirectoryInfo subDir)
		{
			var f2 = subDir.GetFiles(indexHTML);
			if (f2.Length == 0)
				f2 = subDir.GetFiles(indexHTM);
			if (f2.Length > 0)
				return f2[0].FullName;
			return null;
		}

		static DirectoryInfo ExtractFiles(string fichier)
		{
			try
			{
				var temp = Settings.CommonSettings.Value.TemporaryFilesDirectory; //System.IO.Path.GetTempPath();
				if (!string.IsNullOrEmpty(temp) && Directory.Exists(temp))
				{
					var répertoire = Path.Combine(temp,
						Common.TempDirectoriesPrefix +// "Maff_" +
							Guid.NewGuid().ToString()); // Guid avoids collisions (in fact they are just improbable).
					var di = Directory.CreateDirectory(répertoire);

					ZipFile.ExtractToDirectory(fichier, répertoire);

					return di;
				}
			}
			catch (Exception ex)
			{
				GUI.DisplayError(ex.Message);
			}

			return null;
		}

	}
}