#if FRAMEWORK
namespace KatKits {
  using System.Diagnostics;
  using System.Linq;
  using System.Management;

  public static partial class KatKits {
    public static string GetCommandLineArgs(this Process Process) {
      using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + Process.Id))
      using (ManagementObjectCollection objects = searcher.Get()) {
        return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
      }

    }
  }
}

#endif