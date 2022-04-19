namespace KatKits.ImplementExtension {
  using System;
  using System.Collections.Generic;
  using System.Text;

  public static partial class Kits {
    private const int CharLimit = 0x9fa5 - 0x4e00 + 0x0039 - 0x0030 + 0x005a - 0x0041 + 0x007a - 0x0061 + 0x0004;
    private static readonly (int, int)[] Domians = new (int, int)[] { (0x0030, 0x0039), (0x0041, 0x005a), (0x0061, 0x007a), (0x4e00, 0x9fa5), };
    private static System.Random R = new System.Random(DateTime.UtcNow.Millisecond);

    public static string RandomString(int Length, bool NewRandom = false) {
      if (NewRandom) {
        R = new System.Random(DateTime.UtcNow.Millisecond);
      }

      List<byte> Byts = new List<byte>();
      for (int i = 0; i < Length; i++) {
        var RD = R.NextDouble();
        var RR = Convert.ToInt32(Convert.ToDouble(CharLimit) * RD);
        for (int j = 0; j < 4; j++) {
          if (Domians[j].Item1 + RR > Domians[j].Item2) {
            RR -= (Domians[j].Item2 - Domians[j].Item1 + 1);
          }
          else {
            Byts.Add((byte)((Domians[j].Item1 + RR) & 0xFF));
            Byts.Add((byte)((Domians[j].Item1 + RR) >> 8 & 0xFF));
            break;
          }
        }
      }
      return Encoding.Unicode.GetString(Byts.ToArray());
    }
    /// <summary>
    /// 随机字符串（数字+字母（U/L）+汉字）
    /// </summary>
    /// <param name="Length"></param>
    /// <param name="Rate1">[0d,10d)数字出现概率</param>
    /// <param name="Rate2">(Rate1,10d)字母（U/L）出现概率</param>
    /// <param name="NewRandom"></param>
    /// <returns></returns>
    public static string RandomString(int Length, double Rate1, double Rate2, bool NewRandom = false) {
      if (NewRandom) {
        R = new System.Random(DateTime.UtcNow.Millisecond);
      }

      List<byte> Byts = new List<byte>();
      for (int i = 0; i < Length; i++) {
        var D = R.NextDouble();
        if (D < Rate1) {
          var DN = R.NextDouble();
          var RN = Domians[0].Item1 + Convert.ToInt32(Convert.ToDouble((Domians[0].Item2 - Domians[0].Item1 + 1)) * DN);
          Byts.Add((byte)(RN & 0xFF));
          Byts.Add((byte)((RN >> 8) & 0xFF));
        }
        else if (D < Rate2) {
          var DN = R.NextDouble();
          if (DN > 0.5d) {
            var RN = Domians[2].Item1 + Convert.ToInt32(Convert.ToDouble((Domians[2].Item2 - Domians[2].Item1 + 1)) * ((DN - 0.5d) * 2d));
            Byts.Add((byte)(RN & 0xFF));
            Byts.Add((byte)((RN >> 8) & 0xFF));
          }
          else {
            var RN = Domians[1].Item1 + Convert.ToInt32(Convert.ToDouble((Domians[1].Item2 - Domians[1].Item1 + 1)) * (DN * 2d));
            Byts.Add((byte)(RN & 0xFF));
            Byts.Add((byte)((RN >> 8) & 0xFF));
          }
        }
        else {
          var DN = R.NextDouble();
          var RN = Domians[3].Item1 + Convert.ToInt32(Convert.ToDouble((Domians[3].Item2 - Domians[3].Item1 + 1)) * DN);
          Byts.Add((byte)(RN & 0xFF));
          Byts.Add((byte)((RN >> 8) & 0xFF));
        }
      }

      return Encoding.Unicode.GetString(Byts.ToArray());
    }
    /// <summary>
    /// 格式化字符串 替换 {{xxxx}}
    /// </summary>
    /// <param name="Template"></param>
    /// <param name="Values"></param>
    /// <returns></returns>
    public static string Formate(this string Template, params KeyValuePair<string, object>[] Values) {
      for (int i = 0; i < Values.Length; i++) {
        Template = Template.Replace($"{{{Values[i].Key}}}", Values[i].Value.ToString());
      }
      return Template;
    }

    public static string Guid2B64(this Guid Guid) => Convert.ToBase64String(Guid.ToByteArray());
    public static Guid B642Guid(this string B64) => new Guid(Convert.FromBase64String(B64));
    public static string Guid2B642PathName(this Guid Guid) => Convert.ToBase64String(Guid.ToByteArray()).Replace('/', '-');
    public static Guid PathName2B642Guid(this string Name) => new Guid(Convert.FromBase64String(Name.Replace('-', '/')));

    /// <summary>
    /// take part of string from one part to another
    /// </summary>
    /// <param name="Source"></param>
    /// <param name="From"></param>
    /// <param name="To"></param>
    /// <returns></returns>
    public static string Take(this string Source, string From = null, string To = null) {
      if (From == null && To == null) return Source;
      var I1 = From == null ? -1 : Source.IndexOf(From);
      var I2 = To == null ? -1 : Source.IndexOf(To);
      if (I1 == -1 && I2 == -1) return Source;
      if (I1 == -1 && I2 != -1) return Source.Substring(0, I2);
      if (I1 != -1 && I2 == -1) return Source.Substring(I1 + From.Length, Source.Length - I1 - From.Length);
      return Source.Substring(I1 + From.Length, I2 - I1 - From.Length);
    }

  }
}
