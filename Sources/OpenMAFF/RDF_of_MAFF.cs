
// Copyright (c) Christophe Bertrand. All Rights Reserved.
// https://chrisbertrand.net
// https://github.com/ChrisBertrandDotNet/OpenMAFF

using System;
using System.IO;
using System.Xml;

namespace OpenMAFF
{
	/// <summary>
	/// This class is about the RDF we find in MAFF files.
	/// </summary>
	internal class RDF_of_MAFF
	{
		internal readonly string Title;
		internal readonly string OriginalUrl;
		internal readonly string IndexFileName;
		internal readonly string ArchiveTime;

		RDF_of_MAFF(string Title, string OriginalUrl, string IndexFileName, string ArchiveTime)
		{
			this.Title = Title;
			this.OriginalUrl = OriginalUrl;
			this.IndexFileName = IndexFileName;
			this.ArchiveTime = ArchiveTime;
		}

		internal static RDF_of_MAFF New(string fileName)
		{
			XmlReader reader = null;
			XmlDocument doc = null;

			try
			{
				XmlReaderSettings settings = new XmlReaderSettings();
				settings.DtdProcessing = DtdProcessing.Parse;
				settings.ValidationType = ValidationType.DTD;
				//settings.ValidationEventHandler += eventHandler;

				reader = XmlReader.Create(fileName, settings);

				doc = new XmlDocument();
				doc.Load(reader);
			}
			catch (FileNotFoundException) { }
			catch (UriFormatException) { }
			catch (XmlException) { }
			finally
			{
				if (reader != null)
					reader.Close();
			}

			if (doc != null)
			{
				var cn = doc.ChildNodes;
				var root = doc?.GetElementsByTagName("RDF:RDF").Item(0);
				var description = GetChildByTagName(root, "RDF:Description") as XmlElement;
				if (description != null)
				{
					var title = GetChildByTagName(description, "MAF:title")?.Attributes["RDF:resource"]?.Value;
					var originalUrl = GetChildByTagName(description, "MAF:originalurl")?.Attributes["RDF:resource"]?.Value;
					var indexFileName = GetChildByTagName(description, "MAF:indexfilename")?.Attributes["RDF:resource"]?.Value;
					var archiveTime = GetChildByTagName(description, "MAF:archivetime")?.Attributes["RDF:resource"]?.Value;
					return new RDF_of_MAFF(title, originalUrl, indexFileName, archiveTime);
				}
			}
			return null;
		}

		// XML Helper (from library "CB Helpers"):
		static XmlNode GetChildByTagName(XmlNode element, string tagName)
		{
			if (element == null) return null;
			XmlNodeList nl = element.ChildNodes;
			foreach (var node in nl)
			{
				var node2 = node as XmlNode;
				if (node2?.Name == tagName)
					return node2;
			}
			return null;
		}
#if false // Possible future use.
		// Display the validation error.
		private static void ValidationCallback(object sender, ValidationEventArgs args)
		{
			Console.WriteLine("Validation error loading: {0}", filename);
			Console.WriteLine(args.Message);
		}
#endif
	}
}