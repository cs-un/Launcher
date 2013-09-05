using System;
using Gtk;
using System.Net;
using System.Threading;

public partial class MainWindow: Gtk.Window
{	
	public static String LauncherVersion = "0.01";
	private bool firstRun = true;
	private bool startThread = false;
	private bool updateGame = false;
	private Thread thread;
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		//Log.text
		//System.Threading.Thread thread = new System.Threading.Thread(CheckLauncherVersion(main, Log));
		
		Build ();
		if(firstRun){
			thread = new Thread(CheckLauncherVersion);
			Log.Buffer.Text = "Current version of the Launcher is " + LauncherVersion + ".\n" + Log.Buffer.Text;
			DownloadProgress.Fraction = 0.33;
			DownloadProgress.Text = "33.0%";
			thread.Start();
			firstRun = false;
		}
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
	
	private void CheckLauncherVersion(){
		WebClient wc = new WebClient();
		string url = "http://2b2t.org/CLEE14/version.txt"; 
		Log.Buffer.Text = "Checking for updates to the Launcher..\n" + Log.Buffer.Text;
		DownloadProgress.Fraction = 0.66;
	    DownloadProgress.Text = "66.0%";
		try{
			byte[] content = wc.DownloadData(url);
			string version = System.Text.Encoding.Default.GetString(content);
			if(float.Parse(version)>float.Parse(LauncherVersion)){
				DownloadProgress.Fraction = 1.00;
	    		DownloadProgress.Text = "Found a new version of the Launcher(v" + version + ")";
				DownloadLauncher();
			}
		}
		catch{
			Log.Buffer.Text = "Could not check for the latest version online, are you connected to the internet?\n" + Log.Buffer.Text;
			DownloadProgress.Text = "An error occurred.";
			DownloadProgress.Fraction = 1.00;
		}
		//if((float)LauncherVersion<(float)content)
			// code to download and update launcher
	}
	
	private void DownloadLauncher(){
	}
}
