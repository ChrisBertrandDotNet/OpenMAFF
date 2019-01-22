
// Copyright (c) Christophe Bertrand. All Rights Reserved.
// https://chrisbertrand.net
// https://github.com/ChrisBertrandDotNet/OpenMAFF

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace OpenMAFF
{
	/// <summary>
	/// The HTML page that contains the index page (in a frame).
	/// <para>Usually called "page.html".</para>
	/// </summary>
	internal class ContainingPage
	{
		const string Model = "<!DOCTYPE html>\n<html>\n\n<!-- OpenMAFF. https://github.com/ChrisBertrandDotNet/OpenMAFF -->\n\n<head>\n<title>{2}</title>\n<style>\n\nhtml {\n	height:100%;\n	overflow: hidden; /* avoid double vertical scroll bars */\n}\n\nbody {\n	display: table;\n	empty-cells: show;\n	border-collapse: collapse;\n	width: 100%;\n	height: 100%;\n	margin:0;\n}\n\n#Informations{\n  background-color:rgb(0,120,215);\n	color:white;\n	text-decoration:none;\n	font-family:Verdana, Geneva, Tahoma, sans-serif;\n	font-size:0.8rem;\n}\n\n#maff {\n	color: rgba(255,255,255,0.25);\n	float:left;\n}\n\n#hide-button-text {\n  cursor: pointer;\n}\n\n#hide-button-input {\n display: none; /* hide the checkboxes */\n}\n\n#hide-button-text\n{\n	float:right;\n	margin-right:1rem;\n}\n\n#hide-button-text:hover\n{\n	background-color:orange;\n	color:black;\n}\n\n#hide-button-text:after {\n  content:'▲';\n  }\n  \n#hide-button-input:checked + #hide-button-text\n{\n  display:none;\n}\n\n#hide-button-input:checked ~ div\n{\n  display:none;\n}\n\n#hide-button-input:checked ~ div a\n{\n  display:none;\n}\n\n#hide-button-input:checked ~ label\n{\n  display:none;\n}\n\n#link-container {\n	display: flex;\n	justify-content: center;\n}\n\n#link-to-original {\n	color:white;\n	text-decoration:none;\n}\n\n#link-to-original:hover {\n	background-color:white;\n	color:rgb(0,120,215);\n}\n\n</style>\n\n<meta content=\"text/html; charset=utf-8\" http-equiv=\"Content-Type\">\n<title>sans titre 1</title>\n</head>\n\n<body>\n\n<div id=\"Informations\" >\n	<input id=\"hide-button-input\" type=\"checkbox\">\n	<label id=\"hide-button-text\" for=\"hide-button-input\" >Hide information bar</label>\n	<label id=\"maff\">MAFF</label>\n	<div id=\"link-container\">\n		<a  id=\"link-to-original\" href=\"{0}\" >Open original page</a>	\n	</div>\n</div>\n\n<div style=\"display: table-row; height: 100%\">\n	<iframe src=\"{1}\" style=\"display: table-row; border:0;width:100%;height:100%\"></iframe>\n</div>\n\n</body>\n</html>\n";

		const string ContainingPageFileName = "page.html";
		const string ContainingPageFileName2 = "page{0}.html";

		/// <summary>
		/// Builds then saves the HTML page that contains the index page as a HTML frame.
		/// </summary>
		/// <param name="indexFile">The index page, saved from the original website. Usually index.html or index.htm</param>
		/// <param name="pageTitle">The original page's title.</param>
		/// <param name="directory">Where the containing page is to be written in.</param>
		/// <returns>The file name (with path) that was saved.</returns>
		internal static string SaveBuiltPage(string indexFile, string originalUrl, string pageTitle, string directory)
		{
			var uri = new Uri(indexFile, System.UriKind.Absolute);

			var text = SimpleFormat(Model, originalUrl, uri.AbsoluteUri, pageTitle);
			var containingPageFileName = ReservePage(directory);
			File.WriteAllText(containingPageFileName, text);
			return containingPageFileName;
		}

		/// <summary>
		/// Replaces one or more format items in a specified string with the string representation of a specified object.
		/// <para>Unlike <see cref="System.String.Format(string, object[])"/>, <see cref="SimpleFormat(string, string[])"/> is very tolerant and takes into account {n} formating only.</para>
		/// </summary>
		/// <param name="text"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		static string SimpleFormat(string text, params string[] parameters)
		{
			var sb = new StringBuilder(text);
			for(int i=0;i<parameters.Length;i++)
			{
				sb.Replace("{"+i.ToString(NumberFormatInfo.InvariantInfo)+"}", parameters[i]);
			}
			return sb.ToString();
		}

		/// <summary>
		/// Reserves a file name in the directory.
		/// Tries various names until finding a free one.
		/// </summary>
		/// <param name="directory"></param>
		/// <returns>The reserved file name (with path).</returns>
		static string ReservePage(string directory)
		{
			var filePath = Path.Combine(directory, ContainingPageFileName);
			if (!File.Exists(filePath))
				return filePath;
			for (int i=0;i<10000;i++)
			{
				filePath = Path.Combine(directory, string.Format(ContainingPageFileName2, i));
				if (!File.Exists(filePath))
					return filePath;
			}
			throw new System.Exception("Unable to reserve a containing page file.");
		}
	}
}