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
	private Thread time;
	private WebClient wc;
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		//Log.text
		//System.Threading.Thread thread = new System.Threading.Thread(CheckLauncherVersion(main, Log));
		
		Build ();
		if(firstRun){
			thread = new Thread(CheckLauncherVersion);
			thread.IsBackground = true;
			Log.Buffer.Text = "Current version of the Launcher is " + LauncherVersion + ".\n" + Log.Buffer.Text;
			DownloadProgress.Fraction = 0.33;
	    	DownloadProgress.Text = (DownloadProgress.Fraction * 100).ToString() + "%";
			thread.Start();
			firstRun = false;
		}
		time = new Thread(timer);
		time.IsBackground =  true;
		time.Start();
	}
	
	private void timer(){
		while(true){
			Timer.Text = Convert.ToInt32(Timer.Text) + 1 + "";
			Thread.Sleep(1000);
		}
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		time.Abort();
		thread.Abort();
		Application.Quit ();
		a.RetVal = true;
	}
	
	private void CheckLauncherVersion(){
		wc = new WebClient();
		string url = "http://www.nada.kth.se/~csundlof/version.txt"; 
		Log.Buffer.Text = "Checking for updates to the Launcher..\n" + Log.Buffer.Text;
		DownloadProgress.Fraction = 0.50;
	    DownloadProgress.Text = (DownloadProgress.Fraction * 100).ToString() + "%";
		//try{
			byte[] content = wc.DownloadData(url);
			string version = System.Text.Encoding.Default.GetString(content);
			if(float.Parse(version)>float.Parse(LauncherVersion)){
				DownloadProgress.Fraction = 1.00;
	    		DownloadProgress.Text = "Found a new version of the Launcher(v" + version + ")";
				Log.Buffer.Text = "A new version of the Launcher was found(v" + version + ")!\n" + Log.Buffer.Text;
				DownloadLauncher(float.Parse(version), wc);
			}
			if(float.Parse(version)==float.Parse(LauncherVersion)){
				DownloadProgress.Fraction = 1.00;
				DownloadProgress.Text = "Your version of the Launcher is the latest.";
			}
		//}
		/*catch{
			Log.Buffer.Text = "Could not check for the latest version online, are you connected to the internet?\n" + Log.Buffer.Text;
			DownloadProgress.Text = "An error occurred.";
			DownloadProgress.Fraction = 1.00;
		}*/
		//if((float)LauncherVersion<(float)content)
			// code to download and update launcher
	}
	
	private void DownloadLauncher(float version, WebClient wc){
	    String url = "http://www.nada.kth.se/~csundlof/" + version + ".zip";
		Uri uri = new Uri(url);
		Log.Buffer.Text = "Downloading new version of the Launcher..\n" + Log.Buffer.Text;
		
        wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
		
		wc.DownloadFileAsync(uri, "new.zip");
		Log.Buffer.Text = "File was succesfully downloaded, applying patch in 5 seconds..\n" + Log.Buffer.Text;
		Thread.Sleep(5000);
		System.Diagnostics.Process.Start("./patchLinuxLauncher");
		Application.Quit ();
	}
	
	private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
	{
	    // Displays the operation identifier, and the transfer progress.
	    Console.WriteLine("{0}    downloaded {1} of {2} bytes. {3} % complete...", 
	        (string)e.UserState, 
	        e.BytesReceived, 
	        e.TotalBytesToReceive,
	        e.ProgressPercentage);
	    DownloadProgress.Fraction = e.ProgressPercentage/100;
		DownloadProgress.Text = "Downloading update " + e.ProgressPercentage.ToString () + "% " + (float)e.BytesReceived/1048576 +"/" + (float)e.TotalBytesToReceive/1048576 + " MB";;
	}
}
