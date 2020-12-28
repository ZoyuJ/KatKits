#if FRAMEWORK
namespace KatKits {
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Text;

  public static partial class KatKits {
    private static string GetCommandLine(this Process process) {
      using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
      using (ManagementObjectCollection objects = searcher.Get()) {
        return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
      }

    }
  }
}

#endif