namespace KatKits {
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Text;
  using System.Threading.Tasks;

  public static partial class KatKits {
    /// <summary>
    /// 移动文件
    /// </summary>
    /// <param name="SourceFile"></param>
    /// <param name="TargetFile"></param>
    /// <param name="Force">覆盖？</param>
    public static void MoveFile(string SourceFile, string TargetFile, bool Force = true) {
      FileInfo FI = new FileInfo(SourceFile);
      if (FI.Exists) {
        FileInfo TFI = new FileInfo(TargetFile);
        if (!TFI.Exists) {
          DirectoryInfo DI = new DirectoryInfo(TFI.DirectoryName);
          if (!DI.Exists) {
            DI.Create();
          }
        }
        else {
          if (!Force) {
            return;
          }
        }
        FI.MoveTo(TargetFile);
      }
    }
    /// <summary>
    /// 移动文件
    /// </summary>
    /// <param name="SourceFile"></param>
    /// <param name="TargetFile"></param>
    /// <param name="Force">覆盖？</param>
    public static void MoveFile(FileInfo SourceFile, FileInfo TargetFile, bool Force = true) {
      if (SourceFile.Exists) {
        if (!TargetFile.Exists) {
          DirectoryInfo DI = new DirectoryInfo(TargetFile.DirectoryName);
          if (!DI.Exists) {
            DI.Create();
          }
        }
        else {
          if (!Force) {
            return;
          }
        }
        SourceFile.MoveTo(TargetFile.FullName);
      }
    }
    /// <summary>
    /// 移动文件
    /// </summary>
    /// <param name="SourceFile"></param>
    /// <param name="TargetFile"></param>
    /// <param name="Force">覆盖</param>
    public static void MoveFile(FileInfo SourceFile, string TargetFile, bool Force = true) {
      if (SourceFile.Exists) {
        FileInfo TFI = new FileInfo(TargetFile);
        if (!TFI.Exists) {
          DirectoryInfo DI = new DirectoryInfo(TFI.DirectoryName);
          if (!DI.Exists) {
            DI.Create();
          }
        }
        else {
          if (!Force) {
            return;
          }
        }
        SourceFile.MoveTo(TargetFile);
      }
    }
    /// <summary>
    /// 移动所有符合的文件
    /// </summary>
    /// <param name="SourcePath"></param>
    /// <param name="TargetPath"></param>
    /// <param name="SourceFiles">通配符</param>
    public static void MoveFiles(string SourcePath, string TargetPath, string SourceFiles = "*.*") {
      DirectoryInfo DI = new DirectoryInfo(SourcePath);
      if (DI.Exists) {
        DirectoryInfo TDI = new DirectoryInfo(TargetPath);
        if (!TDI.Exists) {
          TDI.Create();
        }

        FileInfo[] FIs = DI.GetFiles(SourceFiles, SearchOption.TopDirectoryOnly);
        if (FIs != null && FIs.Length > 0) {
          Array.ForEach(FIs, F => { MoveFile(F, Path.Combine(TargetPath, F.Name)); });
        }
      }
    }

    /// <summary>
    /// 写到本地
    /// </summary>
    /// <param name="_Path">路径</param>
    /// <param name="FileName">文件名</param>
    /// <param name="data"></param>
    public static void WriteFileToLocal(string _Path, string FileName, byte[] data) {
      FileName = FileName.Replace('/', '_');
      WriteFileToLocal(Path.Combine(_Path, FileName), data);
    }
    public static async void WriteFileToLocalAsync(string _Path, string FileName, byte[] data) {
      await WriteFileToLocalAsync(Path.Combine(_Path, FileName), data);
    }
    /// <summary>
    /// 写到本地
    /// </summary>
    /// <param name="Path">文件路径</param>
    /// <param name="data"></param>
    public static void WriteFileToLocal(string Path, byte[] data) {
      FileInfo FI = new FileInfo(Path);
      if (FI.Exists) {
        FI.Delete();
      }

      File.WriteAllBytes(FI.FullName, data);
    }
    public static async Task WriteFileToLocalAsync(string Path, byte[] data) {
      await Task.Run(() => {
        FileInfo FI = new FileInfo(Path);
        if (FI.Exists) {
          FI.Delete();
        }

        File.WriteAllBytes(FI.FullName, data);
      });
    }
    /// <summary>
    /// 读取文件
    /// </summary>
    /// <param name="_Path">所在路径</param>
    /// <param name="Name">文件名</param>
    /// <returns></returns>
    public static byte[] ReadFileFromLocal(string _Path, string Name) {
      Name = Name.Replace('/', '_');
      return ReadFileFromLocal(Path.Combine(_Path, Name));
    }
    /// <summary>
    /// 读取文件
    /// </summary>
    /// <param name="Path">所在路径</param>
    /// <returns></returns>
    public static byte[] ReadFileFromLocal(string Path) {
      FileInfo FI = new FileInfo(Path);
      if (FI.Exists) {
        return File.ReadAllBytes(Path);
      }

      return null;
    }

    public static void CopyFile(this FileSystemInfo FI, string ParPath, Action<FileSystemInfo, bool> AfterFileCopied = null) {
      if ((FI.Attributes & FileAttributes.Directory) != 0) {
        var CPar = Path.Combine(ParPath, FI.Name);
        Directory.CreateDirectory(CPar);
        AfterFileCopied?.Invoke(FI, true);
        foreach (var item in new DirectoryInfo(FI.FullName).EnumerateFileSystemInfos()) {
          CopyFile(item, CPar, AfterFileCopied);
        }
      }
      else {
        File.Copy(FI.FullName, Path.Combine(ParPath, FI.Name), true);
        AfterFileCopied?.Invoke(FI, false);
      }
    }
    public static void CopyFile(this IEnumerable<FileSystemInfo> FIs, string ParPath, Action<FileSystemInfo, bool> AfterFileCopied = null) {
      foreach (var FI in FIs) {
        CopyFile(FI, ParPath, AfterFileCopied);
      }
    }


    private static HashSet<char> INVALID_PATH_CHAR;
    private static HashSet<char> INVALID_FILENAME_CHAR;
    /// <summary>
    /// 转换目录路径字符串中的非法字符
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="ReplaceChar"></param>
    /// <returns></returns>
    public static string DealStringToValidPathName(string Name, char ReplaceChar = '^') {
      if (INVALID_PATH_CHAR == null) {
        INVALID_PATH_CHAR = new HashSet<char>(Path.GetInvalidPathChars());
      }

      StringBuilder SB = new StringBuilder(Name);
      for (int i = 0; i < SB.Length; i++) {
        if (INVALID_PATH_CHAR.Contains(SB[i])) {
          SB[i] = ReplaceChar;
        }
      }
      return SB.ToString();
    }
    /// <summary>
    /// 转换目录路径字符串中的非法字符
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="ReplaceChar"></param>
    public static void DealStringToValidPathName(StringBuilder Name, char ReplaceChar = '^') {
      if (INVALID_PATH_CHAR == null) {
        INVALID_PATH_CHAR = new HashSet<char>(Path.GetInvalidPathChars());
      }

      for (int i = 0; i < Name.Length; i++) {
        if (INVALID_PATH_CHAR.Contains(Name[i])) {
          Name[i] = ReplaceChar;
        }
      }
    }
    /// <summary>
    /// 转换路径文件路径中的非法字符
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="ReplaceChar"></param>
    /// <returns></returns>
    public static string DealStringToValidFileOrFolderName(string Name, char ReplaceChar = '^') {
      if (INVALID_FILENAME_CHAR == null) {
        INVALID_FILENAME_CHAR = new HashSet<char>(Path.GetInvalidFileNameChars());
      }

      StringBuilder SB = new StringBuilder(Name);
      for (int i = 0; i < SB.Length; i++) {
        if (INVALID_FILENAME_CHAR.Contains(SB[i])) {
          SB[i] = ReplaceChar;
        }
      }
      return SB.ToString();
    }
    /// <summary>
    /// 转换路径文件路径中的非法字符
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="ReplaceChar"></param>
    /// <returns></returns>
    public static string DealStringToValidFileOrFolderName(StringBuilder Name, char ReplaceChar = '^') {
      if (INVALID_FILENAME_CHAR == null) {
        INVALID_FILENAME_CHAR = new HashSet<char>(Path.GetInvalidFileNameChars());
      }

      for (int i = 0; i < Name.Length; i++) {
        if (INVALID_FILENAME_CHAR.Contains(Name[i])) {
          Name[i] = ReplaceChar;
        }
      }
      return Name.ToString();
    }

    /// <summary>
    /// 目录中随机唯一文件名
    /// </summary>
    /// <param name="Directory"></param>
    /// <param name="ExtName"></param>
    /// <returns></returns>
    public static string RandomUniqueFileName(in string Directory, in string ExtName) {
      var Name = Path.GetRandomFileName() + ExtName;
      while (File.Exists(Path.Combine(Directory, Name))) {
        Name = Path.GetRandomFileName() + ExtName;
      }
      return Name;
    }
    /// <summary>
    /// 目录中随机唯一目录名
    /// </summary>
    /// <param name="Directory"></param>
    /// <returns></returns>
    public static string RandomUniqueDirName(in string Directory) {
      var Name = Path.GetRandomFileName();
      while (System.IO.Directory.Exists(Path.Combine(Directory, Name))) {
        Name = Path.GetRandomFileName();
      }
      return Name;
    }
    /// <summary>
    /// 目录中随机唯一文件名
    /// </summary>
    /// <param name="Directory"></param>
    /// <param name="ExtName"></param>
    /// <returns></returns>
    public static string RandomUniqueFileName(in DirectoryInfo Directory, in string ExtName) {
      return RandomUniqueFileName(Directory.FullName, ExtName);
    }
    /// <summary>
    /// 目录中随机唯一目录名
    /// </summary>
    /// <param name="Directory"></param>
    /// <returns></returns>
    public static string RandomUniqueDirName(in DirectoryInfo Directory) {
      return RandomUniqueDirName(Directory.FullName);
    }

  }
}
