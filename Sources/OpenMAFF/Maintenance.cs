
// Copyright Christophe Bertrand
// https://chrisbertrand.net
// https://github.com/ChrisBertrandDotNet

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OpenMAFF
{
	internal class Maintenance
	{
		internal static void RemoveOldTemporaryFiles()
		{
			var nbOfDays = Settings.CommonSettings.Value.DeleteTemporaryFilesAfterXDays;
			var dir = new DirectoryInfo(Settings.CommonSettings.Value.TemporaryFilesDirectory);
			var subDirs = dir.EnumerateDirectories();
			var now = DateTime.UtcNow;
			var systemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
			foreach (var subDir in subDirs)
			{
				if (subDir.Name.StartsWith(Common.TempDirectoriesPrefix)) // "Maff_" 
				{
					var dirDate = subDir.LastWriteTimeUtc;
					var days = (now - dirDate).Days;
					if (days >= nbOfDays)
					{

						if (subDir.FullName.Length >= @"x:\".Length && !subDir.FullName.StartsWith(systemPath)) // security checks.
							try { subDir.Delete(true); }
							catch (UnauthorizedAccessException) { }
							catch (System.Security.SecurityException) { }
							catch (DirectoryNotFoundException) { }
							catch (IOException) { }
					}
				}
			}
		}
	}
}