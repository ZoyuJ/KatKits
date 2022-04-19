namespace KatKits {
  using global::KatKits.ImplementExtension.CollectionExtension;

  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Security.Cryptography;
  using System.Text;

 public static partial class Kits{
    public static byte[] Decrypt(string PW, Stream Stream) {
      using (AesCryptoServiceProvider AES = new AesCryptoServiceProvider()) {
        AES.IV = GetIV(PW);
        AES.Key = GetKey(PW);
        var Dec = AES.CreateDecryptor(AES.Key, AES.IV);
        List<byte> DeDatas = new List<byte>();
        byte[] Buffer = new byte[4096];
        int Len = 0;
        using (CryptoStream CpS = new CryptoStream(Stream, Dec, CryptoStreamMode.Read)) {
          while ((Len = CpS.Read(Buffer, 0, Buffer.Length)) > 0) {
            DeDatas.AddRange(Buffer.Split(0, Len));
          }
          return DeDatas.ToArray();
        }
      }
    }
    public static byte[] Decrypt(string PW, byte[] Data) => Decrypt(PW, Data, 0, Data.Length);
    public static byte[] Decrypt(string PW, byte[] Data, int Index, int Length) {
      using (AesCryptoServiceProvider AES = new AesCryptoServiceProvider()) {
        AES.IV = GetIV(PW);
        AES.Key = GetKey(PW);
        var Dec = AES.CreateDecryptor(AES.Key, AES.IV);
        List<byte> DeDatas = new List<byte>();
        byte[] Buffer = new byte[4096];
        int Len = 0;
        using (MemoryStream MS = new MemoryStream(Data, Index, Length)) {
          using (CryptoStream CpS = new CryptoStream(MS, Dec, CryptoStreamMode.Read)) {
            while ((Len = CpS.Read(Buffer, 0, Buffer.Length)) > 0) {
              DeDatas.AddRange(Buffer.Split(0, Len));
            }
            return DeDatas.ToArray();
          }
        }
      }
    }
    public static byte[] GetKey(string PW) {
      return new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(PW));
    }
    public static byte[] GetIV(string PW) {
      return CRC64ISOHashToByte(Encoding.UTF8.GetBytes(PW));
    }

    public static byte[] Encrypt(string PW, byte[] Data) => Encrypt(PW, Data, 0, Data.Length);
    public static byte[] Encrypt(string PW, byte[] Data, int Index, int Length) {
      using (AesCryptoServiceProvider AES = new AesCryptoServiceProvider()) {
        AES.Key = GetKey(PW);
        AES.IV = GetIV(PW);
        var Enc = AES.CreateEncryptor(AES.Key, AES.IV);
        using (MemoryStream MS = new MemoryStream()) {
          using (CryptoStream CpS = new CryptoStream(MS, Enc, CryptoStreamMode.Write)) {
            CpS.Write(Data, Index, Length);
          }
          return MS.ToArray();
        }
      }
    }
  }
}
