using System;
using Gtk;
using System.Diagnostics;

namespace Launcher
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            PlatformID pid = Environment.OSVersion.Platform;
            bool Windows = false;
            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    Windows = true;
                    break;
                case PlatformID.Unix:
                    Windows = false;
                    break;
                default:
                    Windows = false;
                    break;
            }
			Application.Init ();
			MainWindow win = new MainWindow ();			
			win.Show ();
			Application.Run ();
		}
	}
}
