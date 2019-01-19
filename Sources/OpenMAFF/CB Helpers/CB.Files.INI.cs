
// Copyright Christophe Bertrand
// https://github.com/ChrisBertrandDotNet/CB-Helpers
// https://chrisbertrand.net

// Manages INI files for settings.

// TODO: make concurrent.
// TODO: add tests.
// TODO: clean code.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/* Exemple:
    public class TestParser
    {
        public static void Main()
        {
            var parser = new IniParser(@"C:\test.ini");
     
            String newMessage;
     
            newMessage = parser.GetSetting("appsettings", "msgpart1");
            newMessage += parser.GetSetting("appsettings", "msgpart2");
            newMessage += parser.GetSetting("punctuation", "ex");
     
            Console.WriteLine(newMessage);
        }
    }
*/

namespace CB.Files
{

	/// <summary>
	/// Manages INI files for settings.
	/// <para>Please note this class is *not* thread-safe at the moment.</para>
	/// </summary>
	public class IniParser
	{
		/// <summary>
		/// All settings. Key is {section name, setting name}. Value is setting value.
		/// </summary>
		internal readonly Dictionary<SectionPair, string> settings;

		private readonly String iniFilePath;
		/// <summary>
		/// Ignores characters' upper/lower case.
		/// </summary>
		public readonly bool IgnoreCase;
		/// <summary>
		/// Comparer for section and setting names.
		/// </summary>
		readonly StringComparison NameComparer;

		const string RootName = "ROOT";

		/// <summary>
		/// Section name and setting name.
		/// Note: does not contain the setting value (it is in <see cref="settings"/>).
		/// </summary>
		internal struct SectionPair
		{
			/// <summary>
			/// The section name.
			/// </summary>
			internal String SectionName;
			/// <summary>
			/// The setting name.
			/// </summary>
			internal String SettingName;
#if DEBUG
			public override string ToString()
			{
				return string.Format("[{0}] : {1} = ..", this.SectionName, this.SettingName);
			}
#endif
			public override int GetHashCode()
			{
				return this.SectionName.GetHashCode() ^ this.SettingName.GetHashCode();
			}
		}

		/// <summary>
		/// Compares section names and setting names.
		/// </summary>
		class SectionPairNameComparer : IEqualityComparer<SectionPair>
		{
			readonly IEqualityComparer<string> NameComparer;

			internal SectionPairNameComparer(IEqualityComparer<string> nameComparer)
			{ this.NameComparer = nameComparer; }

			public bool Equals(SectionPair x, SectionPair y)
			{
				return this.NameComparer.Equals(x.SectionName, y.SectionName)
					&& this.NameComparer.Equals(x.SettingName, y.SettingName);
			}

			public int GetHashCode(SectionPair obj)
			{
				return obj.GetHashCode();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ignoreCase">If true, compare names by ignoring character case.</param>
		/// <param name="compareNamesByCurrentCulture">If true, compare names using the current culture. If false, compare using the invariant culture.</param>
		IniParser(bool ignoreCase, bool compareNamesByCurrentCulture)
		{
			IEqualityComparer<string> ecs;
			switch (ignoreCase)
			{
				case false:
					this.NameComparer = compareNamesByCurrentCulture ? StringComparison.CurrentCulture : StringComparison.InvariantCulture;
					ecs = compareNamesByCurrentCulture ? StringComparer.CurrentCulture : StringComparer.InvariantCulture;
					break;
				default:
					this.NameComparer = compareNamesByCurrentCulture ? StringComparison.CurrentCultureIgnoreCase : StringComparison.InvariantCultureIgnoreCase;
					ecs = compareNamesByCurrentCulture ? StringComparer.CurrentCultureIgnoreCase : StringComparer.InvariantCultureIgnoreCase;
					break;
			}
			this.IgnoreCase = ignoreCase;

			this.settings = new Dictionary<SectionPair, string>(new SectionPairNameComparer(ecs));
		}

		/// <summary>
		/// Opens the INI file at the given path and enumerates the values in the IniParser.
		/// <para>If the file does not exist, just returns a new instance.</para>
		/// </summary>
		/// <param name="IniFilePath">Full path to INI file.</param>
		/// <param name="encoding"></param>
		/// <param name="ignoreCase">If true, compare names by ignoring character case.</param>
		/// <param name="compareNamesByCurrentCulture">If true, compare names using the current culture. If false, compare using the invariant culture.</param>
		public IniParser(String IniFilePath, Encoding encoding = null, bool ignoreCase = true, bool compareNamesByCurrentCulture = false)
			: this(ignoreCase, compareNamesByCurrentCulture)
		{
			this.iniFilePath = IniFilePath;

			if (encoding == null)
				encoding = Encoding.UTF8;

			if (File.Exists(iniFilePath))
			{
				var texteINI = File.ReadAllLines(iniFilePath, encoding);
				this.ParseText(texteINI);
			}
		}

		/// <summary>
		/// Parses the stream and enumerates the values.
		/// </summary>
		/// <param name="stream">The stream that stores the INI settings.</param>
		/// <param name="encoding"></param>
		/// <param name="ignoreCase">If true, compare names by ignoring character case.</param>
		/// <param name="compareNamesByCurrentCulture">If true, compare names using the current culture. If false, compare using the invariant culture.</param>
		public IniParser(Stream stream, Encoding encoding = null, bool ignoreCase = true, bool compareNamesByCurrentCulture = false)
	//, bool DisposeStream = false)
	: this(ignoreCase, compareNamesByCurrentCulture)
		{

			List<string> lines = new List<string>();
			using (var tr = new StreamReader(stream, encoding))
			{
				string line;
				while ((line = tr.ReadLine()) != null)
					lines.Add(line);
			}
			this.ParseText(lines);
#if false
			if (encoding == null)
				encoding = Encoding.UTF8;
			this.SetUpperCase = SetUpperCase;

			TextReader iniFile = null;
			String strLine = null;
			String currentRoot = null;
			String[] keyPair = null;

			/*if (File.Exists(iniPath))
			{*/
			try
			{
				iniFile = new StreamReader(stream, encoding);

				strLine = iniFile.ReadLine();

				while (strLine != null)
				{
					strLine = strLine.Trim()/*.ToUpper()*/;

					if (strLine != "")
					{
						if (strLine.StartsWith("[") && strLine.EndsWith("]"))
						{
							currentRoot = strLine.Substring(1, strLine.Length - 2);
						}
						else
						{
							if (strLine.StartsWith("'"))
							{
								// assuming comments start with the apostrophe
								// do nothing
							}
							else
							{
								keyPair = strLine.Split(new char[] { '=' }, 2);

								SectionPair sectionPair;
								String value = null;

								if (currentRoot == null)
									currentRoot = "ROOT";

								sectionPair.Section = SetUpperCase ? currentRoot.ToUpper() : currentRoot;
								sectionPair.Key = SetUpperCase ? keyPair[0].ToUpper() : keyPair[0];

								if (keyPair.Length > 1)
									value = keyPair[1];

								keyPairs.Add(sectionPair, value);
							}
						}
					}

					strLine = iniFile.ReadLine();
				}

			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (iniFile != null)
					iniFile.Close();
				if (DisposeStream)
					stream.Dispose();
			}
			/*}
			else
				throw new FileNotFoundException("Unable to locate " + iniPath);*/
#endif
		}

		/// <summary>
		/// Parses a text that contains the INI lines.
		/// <para>Example: IniAsText = "[A] \n B=1"</para>
		/// </summary>
		/// <param name="IniAsText">This text contains the entire INI 'file'.</param>
		/// <param name="ignoreCase">If true, compare names by ignoring character case.</param>
		/// <param name="compareNamesByCurrentCulture">If true, compare names using the current culture. If false, compare using the invariant culture.</param>
		public static IniParser ParseText(string IniAsText, bool ignoreCase = true, bool compareNamesByCurrentCulture = false)
		{
			var iniParser = new IniParser(ignoreCase, compareNamesByCurrentCulture);
			iniParser.ParseText(IniAsText);
			return iniParser;
		}

		void ParseText(string IniAsText)
		{
			var lines = IniAsText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(s =>
				{
					if (s.EndsWith("\r"))
						return s.Remove(s.Length - 1, 1);
					return s;
				});
			this.ParseText(lines);
		}

		void ParseText(IEnumerable<string> lines)
		{
			String currentRoot = null;
			String[] keyPair = null;

			foreach (var line in lines)
			{
				var strLine = line.Trim()/*.ToUpper()*/;

				if (strLine != "")
				{
					if (strLine.StartsWith("[") && strLine.EndsWith("]"))
					{
						currentRoot = strLine.Substring(1, strLine.Length - 2);
					}
					else
					{
						if (strLine.StartsWith("'"))
						{
							// assuming comments start with the apostrophe
							// do nothing
						}
						else
						{
							keyPair = strLine.Split(new char[] { '=' }, 2);

							SectionPair sectionPair;
							String value = null;

							if (currentRoot == null)
								currentRoot = RootName;

							sectionPair.SectionName = currentRoot;
							sectionPair.SettingName = keyPair[0];

							if (keyPair.Length > 1)
								value = keyPair[1];

							settings.Add(sectionPair, value);
						}
					}
				}
			}
		}

		/// <summary>
		/// Returns the value for the given section, key pair.
		/// Renvoie 'null' si pas de clé ou de section à ce nom.
		/// </summary>
		/// <param name="sectionName">Section name.</param>
		/// <param name="settingName">Key name.</param>
		public String GetSetting(String sectionName, String settingName)
		{
			SectionPair sectionPair;
			sectionPair.SectionName = sectionName;
			sectionPair.SettingName = settingName;

			try
			{
				return (String)settings[sectionPair];
			}
			catch { return null; }
		}

		/// <summary>
		/// Enumerates all lines for given section.
		/// Réalisé d'un bloc en créant une liste.
		/// </summary>
		/// <param name="sectionName">Section to enum.</param>
		public String[] EnumSection(String sectionName)
		{
			ArrayList tmpArray = new ArrayList();

			foreach (SectionPair pair in settings.Keys)
			{
				if (pair.SectionName.Equals(sectionName, this.NameComparer))
					tmpArray.Add(pair.SettingName);
			}

			return (String[])tmpArray.ToArray(typeof(String));
		}

		/// <summary>
		/// Enumerates all lines for given section.
		/// Progressif avec un 'yield', pour les foreach.
		/// </summary>
		/// <param name="sectionName">Section to enum.</param>
		public IEnumerator<string> SectionEnumerator(String sectionName)
		{
			foreach (SectionPair pair in settings.Keys)
			{
				if (pair.SectionName.Equals(sectionName, this.NameComparer))
					yield return pair.SettingName;
			}
		}
		[Obsolete("Use SectionEnumerator.")]
		internal IEnumerator<string> EnumèreSection(String sectionName)
		{
			return this.SectionEnumerator(sectionName);
		}

		/// <summary>
		/// Adds or updates a setting to the table to be saved.
		/// </summary>
		/// <param name="sectionName">Section to add under.</param>
		/// <param name="settingName">Key name to add.</param>
		/// <param name="settingValue">Value of key.</param>
		public void AddSetting(String sectionName, String settingName, String settingValue)
		{
			SectionPair sectionPair;
			sectionPair.SectionName = sectionName;
			sectionPair.SettingName = settingName;

			if (settings.ContainsKey(sectionPair))
				settings.Remove(sectionPair);

			settings.Add(sectionPair, settingValue);
		}

		/// <summary>
		/// Adds or updates a setting to the table to be saved with a null value.
		/// </summary>
		/// <param name="sectionName">Section to add under.</param>
		/// <param name="settingName">Key name to add.</param>
		public void AddSetting(String sectionName, String settingName)
		{
			AddSetting(sectionName, settingName, null);
		}

		/// <summary>
		/// Remove a setting.
		/// </summary>
		/// <param name="sectionName">Section to add under.</param>
		/// <param name="settingName">Key name to add.</param>
		public void DeleteSetting(String sectionName, String settingName)
		{
			SectionPair sectionPair;
			sectionPair.SectionName = sectionName;
			sectionPair.SettingName = settingName;

			if (settings.ContainsKey(sectionPair))
				settings.Remove(sectionPair);
		}

		/// <summary>
		/// Save settings to new file.
		/// </summary>
		/// <param name="newFilePath">New file path.</param>
		public bool SaveSettings(String newFilePath)
		{
			ArrayList sections = new ArrayList();
			String tmpValue = "";
			String strToSave = "";

			foreach (SectionPair sectionPair in settings.Keys)
			{
				if (!sections.Contains(sectionPair.SectionName))
					sections.Add(sectionPair.SectionName);
			}

			foreach (String section in sections)
			{
				strToSave += ("[" + section + "]\r\n");

				foreach (SectionPair sectionPair in settings.Keys)
				{
					if (sectionPair.SectionName == section)
					{
						tmpValue = (String)settings[sectionPair];

						if (tmpValue != null)
							tmpValue = "=" + tmpValue;

						strToSave += (sectionPair.SettingName + tmpValue + "\r\n");
					}
				}

				strToSave += "\r\n";
			}

			try
			{
				TextWriter tw = new StreamWriter(newFilePath);
				tw.Write(strToSave);
				tw.Close();
			}
			catch (Exception)
			{
				throw;
			}
			return true;
		}

		/// <summary>
		/// Save settings back to ini file.
		/// </summary>
		public bool SaveSettings()
		{
			if (!string.IsNullOrEmpty(iniFilePath))
				return SaveSettings(iniFilePath);
			return false;
		}
	}

}