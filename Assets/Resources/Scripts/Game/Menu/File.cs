using System.Runtime.InteropServices;

using System;
using UnityEngine;

namespace Game.Menu {
	class File {
		

		[DllImport("user32.dll")]
        private static extern void OpenFileDialog(); 

		public static string open() {
			String maindir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
			ofd.InitialDirectory = maindir;
			ofd.Filter = "Images|*.png;*.jpg;*.gif";
			ofd.FilterIndex = 2;
			ofd.RestoreDirectory = false;
			ofd.ShowDialog();
			return ofd.FileName;
		}
	}

}