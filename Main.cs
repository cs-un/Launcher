using System;
using Gtk;

namespace Launcher
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//ProcessName.TrySetProcessName("CLEE14");
			Application.Init ();
			MainWindow win = new MainWindow ();			
			win.Show ();
			Application.Run ();
		}
	}
}
