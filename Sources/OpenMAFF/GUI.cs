
// Copyright (c) Christophe Bertrand. All Rights Reserved.
// https://chrisbertrand.net
// https://github.com/ChrisBertrandDotNet/OpenMAFF

using System.Windows.Forms;

namespace OpenMAFF
{
	internal static class GUI
	{

		/// <summary>
		/// Displays a message box.
		/// </summary>
		/// <param name="errorMmessage"></param>
		internal static void DisplayError(string errorMmessage)
		{
			MessageBox.Show(errorMmessage, "OpenMAFF : Error");
		}

	}
}