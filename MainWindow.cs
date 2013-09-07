using System;
using Gtk;
using System.Net;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;


public partial class MainWindow: Gtk.Window
{	
	public static String LauncherVersion = "0.01";
	private bool firstRun = true;
	private bool startThread = false;
	private bool updateGame = false;
	private bool updateLauncher = false;
	private bool letInstall = false;
	private Thread thread;
	private Thread time;
	private WebClient wc;
	private int updateProgress = 0;
	private List<string> installed = new List<string>();
	private List<float> versions = new List<float>();
	private string[] applications = new string[5];
	
	private string website = "http://www.nada.kth.se/~csundlof/";
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		//Log.text
		//System.Threading.Thread thread = new System.Threading.Thread(CheckLauncherVersion(main, Log));
		Build ();
		LaunchButton.Sensitive = false;
		applications[0] = "Game1";
		applications[1] = "test";
		DetectInstalledApplications();
		if(!installed.Contains(SelectedGame.ActiveText))
		{
			LaunchButton.Label = "Install " + SelectedGame.ActiveText;
		}
		else
		{
			LaunchButton.Label = "Launch " + SelectedGame.ActiveText;
		}
		if(firstRun){
			thread = new Thread(CheckLauncherVersion);
			thread.IsBackground = true;
			Log.Buffer.Text = "Current version of the Launcher is " + LauncherVersion + ".\n" + Log.Buffer.Text;
	    	DownloadProgress.Text = (DownloadProgress.Fraction * 100).ToString() + "%";
			thread.Start();
			firstRun = false;
		}
		time = new Thread(timer);
		time.IsBackground =  true;
		//time.Start();
	}
	
	private void timer(){
		while(true){
			Gtk.Application.Invoke (delegate {
			Timer.Text = Convert.ToInt32(Timer.Text) + 1 + "";
			});
			Thread.Sleep(1000);
		}
	}
	#region launcher update
	private void CheckLauncherVersion(){
		wc = new WebClient();
		string url = website + "version.txt"; 
		Gtk.Application.Invoke (delegate {
		Log.Buffer.Text = "Checking for updates to the Launcher..\n" + Log.Buffer.Text;
		DownloadProgress.Fraction = 0.50;
	    DownloadProgress.Text = (DownloadProgress.Fraction * 100).ToString() + "%";
		});
		try{
			byte[] content = wc.DownloadData(url);
			string version = System.Text.Encoding.Default.GetString(content);
			if(float.Parse(version)>float.Parse(LauncherVersion)){
				Gtk.Application.Invoke (delegate {
	    			DownloadProgress.Text = "Found a new version of the Launcher(v" + version + ")";
					Log.Buffer.Text = "A new version of the Launcher was found(v" + version + ")!\n" + Log.Buffer.Text;
				});
				DownloadLauncher(float.Parse(version));
			}
			if(float.Parse(version)==float.Parse(LauncherVersion)){
				Gtk.Application.Invoke (delegate {
					DownloadProgress.Fraction = 1.00;
					DownloadProgress.Text = "Your Launcher is up to date";
					Log.Buffer.Text = "Your Launcher is up to date\n" + Log.Buffer.Text;
				});
				Thread.Sleep(1000);
				if(installed.Contains(SelectedGame.ActiveText))
					CheckGameVersion();
				else
					letInstall = true;
			}
		}
		catch{
			Gtk.Application.Invoke (delegate {
			Log.Buffer.Text = "Could not check for the latest version online, are you connected to the internet?\n" + Log.Buffer.Text;
			DownloadProgress.Text = "An error occurred.";
			DownloadProgress.Fraction = 1.00;
			});
		}
		//if((float)LauncherVersion<(float)content)
			// code to download and update launcher
	}
	
	private void DownloadLauncher(float version){
	    String url = website + version + ".zip";
		Uri uri = new Uri(url);
		Gtk.Application.Invoke (delegate {
			Log.Buffer.Text = "Downloading new version of the Launcher..\n" + Log.Buffer.Text;
		});
        wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
		updateLauncher = true;
		wc.DownloadFileAsync(uri, "new.zip");
	}
	
	#endregion
	#region game update
	private void CheckGameVersion()
	{
		string url = website + SelectedGame.ActiveText + "/version.txt";
		int vIndex = installed.IndexOf(SelectedGame.ActiveText);
		Gtk.Application.Invoke (delegate {
			DownloadProgress.Fraction = 0.00;
			DownloadProgress.Text = "Checking for updates to " + SelectedGame.ActiveText;
			Log.Buffer.Text = "Current version of application " + SelectedGame.ActiveText + " is " + versions[vIndex] + " ..\n" + Log.Buffer.Text; // ska sedan läsa versionen från spelets mapp
			Log.Buffer.Text = "Checking for updates to " + SelectedGame.ActiveText + " ..\n" + Log.Buffer.Text;
		});
			try{
				byte[] content = wc.DownloadData(url);
				string version = System.Text.Encoding.Default.GetString(content);
				if(float.Parse(version)>versions[vIndex]){
					Gtk.Application.Invoke (delegate {
			    		DownloadProgress.Text = "Found a new version of the selected application(v" + version + ")";
						Log.Buffer.Text = "A new version of " + SelectedGame.ActiveText + " was found(v" + version + ")!\n" + Log.Buffer.Text;
					});
					DownloadGame(float.Parse(version));
				}
				if(float.Parse(version)==versions[vIndex]){
					Gtk.Application.Invoke (delegate {
						DownloadProgress.Fraction = 1.00;
						DownloadProgress.Text = SelectedGame.ActiveText + " is up to date";
						Log.Buffer.Text = SelectedGame.ActiveText + " is up to date\n" + Log.Buffer.Text;
						LaunchButton.Sensitive = true;
					});
				}
			}
			catch{
				Gtk.Application.Invoke (delegate {
					Log.Buffer.Text = "Could not check for the latest version online, are you connected to the internet?\n" + Log.Buffer.Text;
					DownloadProgress.Text = "An error occurred.";
					DownloadProgress.Fraction = 1.00;
				});
			}
		
	}
	
	private void DownloadGame(float version){
	    String url = website + SelectedGame.ActiveText + "/" + version + ".zip";
		Uri uri = new Uri(url);
		Gtk.Application.Invoke (delegate {
		Log.Buffer.Text = "Downloading new version of" + SelectedGame.ActiveText + " ..\n" + Log.Buffer.Text;
		});
        wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
		updateGame = true;
		wc.DownloadFileAsync(uri, "./" + SelectedGame.ActiveText + "/new.zip");
	}
	
	private void updateGameFiles()
	{
		Gtk.Application.Invoke (delegate {
			Log.Buffer.Text = "File was succesfully downloaded, unpacking files..\n" + Log.Buffer.Text;
			DownloadProgress.Text = "Unpacking files";
		});
		Process p = new Process();
		ProcessStartInfo startInfo = new ProcessStartInfo();
		startInfo.FileName = "Scripts/Linux/patchLinuxGame";
		startInfo.Arguments = SelectedGame.ActiveText;
		p = Process.Start(startInfo);
		p.WaitForExit();
		Gtk.Application.Invoke (delegate {
			DownloadProgress.Text = SelectedGame.ActiveText + " is up to date";
			Log.Buffer.Text = SelectedGame.ActiveText + "was successfully updated\n" + Log.Buffer.Text;
		});
		LaunchButton.Sensitive = true;
	}
	#endregion
	#region load data
	
	private void DetectInstalledApplications()
	{
		Log.Buffer.Text = "Getting installed applications\n" + Log.Buffer.Text;
		foreach(string s in applications)
		{
			string file = "./" + s + "/test.txt";
			float v = 0.01f; // kommer i framtiden läsas in från fil
			if(File.Exists(file))
			{
				installed.Add(s);
				Log.Buffer.Text = s + " v" + v + " installed.\n" + Log.Buffer.Text;
				versions.Add(v);
			}
		}
		if(installed.Count==0)
			Log.Buffer.Text = "No installed applications found\n" + Log.Buffer.Text;
	}
	#endregion
	#region events
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		time.Abort();
		thread.Abort();
		Application.Quit ();
		a.RetVal = true;
	}
	
	private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
	{
	    // Displays the operation identifier, and the transfer progress.
	    Console.WriteLine("{0}    downloaded {1} of {2} bytes. {3} % complete...", 
	        (string)e.UserState, 
	        e.BytesReceived, 
	        e.TotalBytesToReceive,
	        e.ProgressPercentage);
		if(updateProgress%200==0){
			Gtk.Application.Invoke (delegate {
	    		DownloadProgress.Fraction = e.ProgressPercentage/100;
				DownloadProgress.Text = "Downloading data " + e.ProgressPercentage.ToString () + "% " + (float)e.BytesReceived/1048576 +"/" + (float)e.TotalBytesToReceive/1048576 + " MB";
			});
		}
		else
			updateProgress++;
		if(e.ProgressPercentage==100 && updateLauncher)
		{
			Gtk.Application.Invoke (delegate {
				Log.Buffer.Text = "File was succesfully downloaded, applying patch in 5 seconds..\n" + Log.Buffer.Text;
			});
			Thread.Sleep(5000);
			Process.Start("Scripts/Linux/patchLinuxLauncher");
			Application.Quit ();
		}
		
		if(e.ProgressPercentage==100 && updateGame)
		{
			updateProgress = 0;
			thread = new Thread(updateGameFiles);
			thread.Start ();
			//mer
		}
	}
	#endregion
}
