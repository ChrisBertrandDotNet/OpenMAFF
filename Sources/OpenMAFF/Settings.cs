
// Copyright (c) Christophe Bertrand. All Rights Reserved.
// https://chrisbertrand.net
// https://github.com/ChrisBertrandDotNet

using CB.Files;
using System;
using System.IO;
using System.Text;

namespace OpenMAFF
{
	public class Settings
	{
		internal static Lazy<Settings> CommonSettings = new Lazy<Settings>(()=>new Settings());
		const string fileName = "Settings.ini";

		const int DefaultDeleteTemporaryFilesAfterXDays = 10;
		const string mainSection = "Infos";
		internal IniParser Ini;

		#region Saved settings

		const string tfd = "TemporaryFilesDirectory";
		internal string TemporaryFilesDirectory;

		const string dtfad = "DeleteTemporaryFilesAfterXDays";
		internal int DeleteTemporaryFilesAfterXDays;

		#endregion Saved settings

		Settings()
		{
			var path = System.Windows.Forms.Application.StartupPath;
			if (Directory.Exists(path))
			{
				this.Ini = new IniParser(Path.Combine(path, fileName), Encoding.UTF8, false);

				this.TemporaryFilesDirectory = Ini.GetSetting(mainSection, tfd);
				if (this.TemporaryFilesDirectory == null || !Directory.Exists(this.TemporaryFilesDirectory))
				{
					this.TemporaryFilesDirectory = System.IO.Path.GetTempPath();
					Ini.AddSetting(mainSection, tfd, this.TemporaryFilesDirectory);
					Ini.SaveSettings();
				}

				var days = Ini.GetSetting(mainSection, dtfad);
				var ok = int.TryParse(days, out this.DeleteTemporaryFilesAfterXDays);
				if (!ok || this.DeleteTemporaryFilesAfterXDays <= 0)
				{
					this.DeleteTemporaryFilesAfterXDays = DefaultDeleteTemporaryFilesAfterXDays; // by default, deletes after 10 days.
					Ini.AddSetting(mainSection, dtfad, this.DeleteTemporaryFilesAfterXDays.ToString());
					Ini.SaveSettings();
				}
			}
		}
	}
}