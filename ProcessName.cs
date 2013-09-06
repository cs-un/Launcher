using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Launcher
{
  static class ProcessName
  {
      [DllImport("libc")] // Linux
      private static extern int prctl(int option, byte[] arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5);

      [DllImport("libc")] // BSD
      private static extern void setproctitle(byte[] fmt, byte[] str_arg);

      public static void SetProcessName(string name)
      {
          //if (Environment.OSVersion.Platform != PlatformID.Unix)
         // {
        //      return;
         // }

          try
          {
              if (prctl(15 /* PR_SET_NAME */, Encoding.ASCII.GetBytes(name + "\0"),
                  IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) != 0)
              {

                  throw new ApplicationException("Error setting process name: " +
                      Mono.Unix.Native.Stdlib.GetLastError());
              }
          }

          catch (EntryPointNotFoundException)
          {

              setproctitle(Encoding.ASCII.GetBytes("%s\0"),
                  Encoding.ASCII.GetBytes(name + "\0"));
          }
      }

      public static void TrySetProcessName(string name)
      {
          try
          {
              SetProcessName(name);
          }
          catch
          {
				Console.WriteLine("big error");
          }
      }
  }
}