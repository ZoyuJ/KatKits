namespace KatKits.ImplementExtension {
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public static partial class Kits {
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

    /// <summary>
    /// 收集文件分片
    /// </summary>
    public class SlicedFileServ {
      public SlicedFileServ(LocalStoragerCfg Cfg) {
        _Cfg = Cfg;
        if (!Directory.Exists(Cfg.SlicedFileTempStorage)) Directory.CreateDirectory(Cfg.SlicedFileTempStorage);
      }
      public class LocalStoragerCfg {
        public string SlicedFileTempStorage { get; set; }
      }
      public readonly LocalStoragerCfg _Cfg;

      public void CreateUploadCacheDir(string Name) {
        var DirP = Path.Combine(_Cfg.SlicedFileTempStorage, Name);
        if (!Directory.Exists(DirP))
          Directory.CreateDirectory(DirP);
      }

      /// <summary>
      /// init prealloc file on disk
      /// </summary>
      /// <param name="CacheDirName"></param>
      /// <param name="FileName">origin file name</param>
      /// <param name="Size">origin file size (byte)</param>
      /// <param name="Parts">how many parts</param>
      /// <returns>file path</returns>
      public string CreateFileCache(string CacheDirName, string FileName, long Size, int Parts) {
        var CFP = Path.Combine(_Cfg.SlicedFileTempStorage, CacheDirName, FileName + CacheIntegExten);
        var Fs = File.Create(CFP);
        InitCachedMapAndSize(Fs, Size, Parts);
        Fs.Close();
        return Path.Combine(CacheDirName, FileName);
      }
      /// <summary>
      /// read cache map details from file
      /// </summary>
      /// <param name="Fs"></param>
      /// <param name="Map">cache map</param>
      /// <returns>how many parts</returns>
      private int ReadCachedMap(FileStream Fs, out BitArray Map) {
        Fs.Seek(-4, SeekOrigin.End);
        var BLts = new byte[4];
        Fs.Read(BLts, 0, 4);
        var Rawlen = Convert.ToInt32(Kits.Byte4ToInt32FromByteArray(BLts, 0));
        var L = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Rawlen) / 8.0));
        BLts = new byte[L];
        Fs.Seek(-(L + 4), SeekOrigin.End);
        Fs.Read(BLts, 0, L);
        Map = new BitArray(BLts);
        Fs.Seek(0, SeekOrigin.Begin);
        return Rawlen;
      }
      /// <summary>
      /// init a file cache map , prealloc file size on disk,at last write inited cache map details in file
      /// </summary>
      /// <param name="Fs"></param>
      /// <param name="RawFileSize"></param>
      /// <param name="PartCount"></param>
      private void InitCachedMapAndSize(FileStream Fs, long RawFileSize, int PartCount) {
        var MapL = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(PartCount) / 8.0)) * 8;
        var Map = new BitArray(MapL);
        Map.SetAll(false);
        Fs.SetLength(RawFileSize + Convert.ToInt64((MapL / 8) + 4));
        WriteCachedMap(Fs, Map, PartCount);
      }
      /// <summary>
      /// write file cache map
      /// </summary>
      /// <param name="Fs"></param>
      /// <param name="Map">Cache map</param>
      /// <param name="RawMapLength">how many parts</param>
      /// <returns>Byte Count:Whold length of bytes need write to stream which include CacheMap details</returns>
      private int WriteCachedMap(FileStream Fs, in BitArray Map, int RawMapLength) {
        var NormaledMapLen = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(RawMapLength) / 8.0));
        var WritedBts = new byte[4 + NormaledMapLen];
        Kits.Int32ToByte4InByteArray(RawMapLength, WritedBts, WritedBts.Length - 4);
        Map.CopyTo(WritedBts, 0);
        Fs.Seek(-WritedBts.Length, SeekOrigin.End);
        Fs.Write(WritedBts, 0, WritedBts.Length);
        return WritedBts.Length;
      }
      public const string CacheIntegExten = ".integing";
      /// <summary>
      /// write file part to cache file
      /// </summary>
      /// <param name="CacheDirName"></param>
      /// <param name="FileName"></param>
      /// <param name="Source"></param>
      /// <param name="Offset">write start at</param>
      /// <param name="PartIndex">which part this is</param>
      /// <returns></returns>
      public void WriteFileCache(string CacheDirName, string FileName, Stream Source, long Offset, int PartIndex) {
        var CFP = Path.Combine(_Cfg.SlicedFileTempStorage, CacheDirName, FileName.EndsWith(CacheIntegExten) ? FileName : FileName + CacheIntegExten);
        var sw = File.Open(CFP, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        var MapL = ReadCachedMap(sw, out var Map);
        if (PartIndex >= MapL) throw new IndexOutOfRangeException($"Caching File Block Doesnt Have This Area {PartIndex}/{MapL}");
        Map[PartIndex] = true;
        var CacheMapOffset = WriteCachedMap(sw, Map, MapL);
        sw.Seek(Offset, SeekOrigin.Begin);
        Source.CopyTo(sw);
        bool Done = false;
        if (Map.Cast<bool>().Take(MapL).Where(E => !E).Count() == 0) {
          sw.Seek(0, SeekOrigin.Begin);
          sw.SetLength(sw.Length - CacheMapOffset);
          Done = true;
        }
        sw.Close();
        if (Done) {
          File.Move(CFP, Path.Combine(_Cfg.SlicedFileTempStorage, CacheDirName, FileName.EndsWith(CacheIntegExten) ? Path.GetFileNameWithoutExtension(FileName) : FileName));
        }
      }
      /// <summary>
      /// read file from file
      /// </summary>
      /// <param name="CacheDirName"></param>
      /// <param name="FileName"></param>
      /// <param name="Map"></param>
      /// <param name="MapLength">length of map</param>
      /// <param name="MapDetailLen">how many bytes at the end of file include slice details</param>
      /// <returns></returns>
      public Stream ReadFileCache(string CacheDirName, string FileName, out BitArray Map, out int MapLength, out int MapDetailLen) {
        var FP = Path.Combine(_Cfg.SlicedFileTempStorage, CacheDirName, FileName.EndsWith(CacheIntegExten) ? FileName : FileName + CacheIntegExten);
        if (File.Exists(FP)) {
          var Fs = File.OpenRead(FP);
          var RawMapL = ReadCachedMap(Fs, out Map);
          MapDetailLen = 4 + Convert.ToInt32(Math.Ceiling(Convert.ToDouble(RawMapL) / 8.0));
          Fs.Seek(0, SeekOrigin.Begin);
          MapLength = RawMapL;
          return Fs;
        }
        else {
          FP = Path.Combine(_Cfg.SlicedFileTempStorage, CacheDirName, FileName);
          var Fs = File.OpenRead(FP);
          Map = null;
          MapLength = -1;
          MapDetailLen = -1;
          return Fs;
        }
      }
      public Stream ReadFileCache(string Path, out BitArray Map, out int MapLength, out int MapDetailLen) {
        return ReadFileCache(System.IO.Path.GetDirectoryName(Path), System.IO.Path.GetFileName(Path), out Map, out MapLength, out MapDetailLen);
      }
      public string GetFileCachePath(string Path) {
        return System.IO.Path.Combine(_Cfg.SlicedFileTempStorage, System.IO.Path.GetDirectoryName(Path), System.IO.Path.GetFileName(Path));
      }
      /// <summary>
      /// check is file complet
      /// </summary>
      /// <param name="CacheDirName"></param>
      /// <param name="FileName"></param>
      /// <param name="Map"></param>
      /// <returns>if -1 :has no cache map details;else length of parts</returns>
      public int FileCacheIntegrity(string CacheDirName, string FileName, out BitArray Map) {
        var IntF = Path.Combine(_Cfg.SlicedFileTempStorage, CacheDirName, FileName + CacheIntegExten);
        if (File.Exists(IntF)) {
          var Fs = File.OpenRead(IntF);
          return ReadCachedMap(Fs, out Map);
        }
        Map = null;
        return -1;
      }
      /// <summary>
      /// delete cache file
      /// </summary>
      /// <param name="CacheDirName"></param>
      /// <param name="FileName"></param>
      public void DiscardCachedFile(string CacheDirName, string FileName) {
        try {
          File.Delete(Path.Combine(_Cfg.SlicedFileTempStorage, CacheDirName, FileName + CacheIntegExten));
          File.Delete(Path.Combine(_Cfg.SlicedFileTempStorage, CacheDirName, FileName));
        }
        catch (FileNotFoundException) { }
        catch (DirectoryNotFoundException) { }
        finally {
          try {
            File.Delete(Path.Combine(_Cfg.SlicedFileTempStorage, CacheDirName, FileName));
          }
          catch (FileNotFoundException) { }
          catch (DirectoryNotFoundException) { }
        }
      }
      /// <summary>
      /// delete cache dir
      /// </summary>
      /// <param name="Name"></param>
      public void DiscardCacheDir(string Name) {
        var DirP = Path.Combine(_Cfg.SlicedFileTempStorage, Name);
        if (Directory.Exists(DirP)) Directory.Delete(DirP, true);
      }

    }

  }



}
