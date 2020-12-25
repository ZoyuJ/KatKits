namespace KatKits {
  using System;
  using System.Collections.Generic;
  using System.Net;
  using System.Text;

public static partial class KatKits {
    /// <summary>
    /// Int16转Bytex2,按书写顺序高位->高位，低位->低位
    /// </summary>
    /// <param name="Number"></param>
    /// <param name="Dest"></param>
    /// <param name="Offset"></param>
    public static void Int16ToByte2InByteArray(in ushort Number, in byte[] Dest, in int Offset) {
      Dest[Offset] = (byte)(Number & 0xFF);
      Dest[Offset + 1] = (byte)((Number >> 8) & 0xFF);
    }
    /// <summary>
    /// Int16转Bytex2,按书写顺序高位->低位，低位->高位
    /// </summary>
    /// <param name="Number"></param>
    /// <param name="Dest"></param>
    /// <param name="Offset"></param>
    public static void Int16ToByte2InByteArrayRev(in ushort Number, in byte[] Dest, in int Offset) {
      Dest[Offset + 1] = (byte)(Number & 0xFF);
      Dest[Offset] = (byte)((Number >> 8) & 0xFF);
    }
    /// <summary>
    /// Bytex2转Int16,按书写顺序高位->高位，低位->低位
    /// </summary>
    /// <param name="Source"></param>
    /// <param name="Offset"></param>
    /// <returns></returns>
    public static ushort Byte2ToInt16FromByteArray(in byte[] Source, in int Offset)
      => (ushort)((Source[Offset] & 0xFF) | (Source[Offset + 1] & 0xFF) << 8);
    /// <summary>
    /// Bytex2转Int16,按书写顺序高位->低位，低位->高位
    /// </summary>
    /// <param name="Source"></param>
    /// <param name="Offset"></param>
    /// <returns></returns>
    public static ushort Byte2ToInt16FromByteArrayRev(in byte[] Source, in int Offset)
      => (ushort)((Source[Offset + 1] & 0xFF) | (Source[Offset] & 0xFF) << 8);
    /// <summary>
    /// Byte->Byte
    /// </summary>
    /// <param name="Number"></param>
    /// <param name="Dest"></param>
    /// <param name="Offset"></param>
    public static void Int8ToByteInByteArray(in byte Number, in byte[] Dest, in int Offset) => Dest[Offset] = Number;
    /// <summary>
    /// Byte->Byte
    /// </summary>
    /// <param name="Dest"></param>
    /// <param name="Offset"></param>
    /// <returns></returns>
    public static byte ByteToInt8FromByteArray(in byte[] Dest, in int Offset) => Dest[Offset];
    /// <summary>
    /// Int32转Bytex4,按书写顺序高位->高位，低位->低位
    /// </summary>
    /// <param name="Number"></param>
    /// <param name="Dest"></param>
    /// <param name="Offset"></param>
    public static void Int32ToByte4InByteArray(in int Number, in byte[] Dest, in int Offset) {
      Dest[Offset] = (byte)(Number & 0xFF);
      Dest[Offset + 1] = (byte)((Number >> 8) & 0xFF);
      Dest[Offset + 2] = (byte)((Number >> 16) & 0xFF);
      Dest[Offset + 3] = (byte)((Number >> 24) & 0xFF);
    }
    /// <summary>
    /// Bytex4转Int32,按书写顺序高位->高位，低位->低位
    /// </summary>
    /// <param name="Source"></param>
    /// <param name="Offset"></param>
    /// <returns></returns>
    public static uint Byte4ToInt32FromByteArray(in byte[] Source, in int Offset)
      => (uint)((Source[Offset] & 0xFF)
        | (Source[Offset + 1] & 0xFF) << 8
        | (Source[Offset + 2] & 0xFF) << 16
        | (Source[Offset + 3] & 0xFF) << 24);
    /// <summary>
    /// Int32转Bytex4,按书写顺序高位->低位，低位->高位
    /// </summary>
    /// <param name="Number"></param>
    /// <param name="Dest"></param>
    /// <param name="Offset"></param>
    public static void Int32ToByte4InByteArrayRev(in uint Number, in byte[] Dest, in int Offset) {
      Dest[Offset + 3] = (byte)(Number & 0xFF);
      Dest[Offset + 2] = (byte)((Number >> 8) & 0xFF);
      Dest[Offset + 1] = (byte)((Number >> 16) & 0xFF);
      Dest[Offset] = (byte)((Number >> 24) & 0xFF);
    }
    /// <summary>
    /// Bytex4转Int32,按书写顺序高位->低位，低位->高位
    /// </summary>
    /// <param name="Source"></param>
    /// <param name="Offset"></param>
    /// <returns></returns>
    public static uint Byte4ToInt32FromByteArrayRev(in byte[] Source, in int Offset)
      => (uint)((Source[Offset + 3] & 0xFF)
        | (Source[Offset + 2] & 0xFF) << 8
        | (Source[Offset + 1] & 0xFF) << 16
        | (Source[Offset] & 0xFF) << 24);
    /// <summary>
    /// Int64转Bytex8,按书写顺序高位->高位，低位->低位
    /// </summary>
    /// <param name="Number"></param>
    /// <param name="Dest"></param>
    /// <param name="Offset"></param>
    public static void Int64ToByte8InByteArray(in ulong Number, in byte[] Dest, in int Offset) {
      Dest[Offset] = (byte)(Number & 0xFF);
      Dest[Offset + 1] = (byte)((Number >> 8) & 0xFF);
      Dest[Offset + 2] = (byte)((Number >> 16) & 0xFF);
      Dest[Offset + 3] = (byte)((Number >> 24) & 0xFF);
      Dest[Offset + 4] = (byte)((Number >> 32) & 0xFF);
      Dest[Offset + 5] = (byte)((Number >> 40) & 0xFF);
      Dest[Offset + 6] = (byte)((Number >> 48) & 0xFF);
      Dest[Offset + 7] = (byte)((Number >> 56) & 0xFF);
    }
    /// <summary>
    /// Bytex8转Int64,按书写顺序高位->高位，低位->低位
    /// </summary>
    /// <param name="Source"></param>
    /// <param name="Offset"></param>
    /// <returns></returns>
    public static ulong Byte8ToInt64FromByteArray(in byte[] Source, in int Offset)
      => (ulong)((Source[Offset] & 0xFF)
        | (Source[Offset + 1] & 0xFF) << 8
        | (Source[Offset + 2] & 0xFF) << 16
        | (Source[Offset + 3] & 0xFF) << 24
        | (Source[Offset + 4] & 0xFF) << 32
        | (Source[Offset + 5] & 0xFF) << 40
        | (Source[Offset + 6] & 0xFF) << 48
        | (Source[Offset + 7] & 0xFF) << 54);
    /// <summary>
    /// Int64转Bytex8,按书写顺序高位->低位，低位->高位
    /// </summary>
    /// <param name="Number"></param>
    /// <param name="Dest"></param>
    /// <param name="Offset"></param>
    public static void Int64ToByte8InByteArrayRev(in ulong Number, in byte[] Dest, in int Offset) {
      Dest[Offset + 7] = (byte)(Number & 0xFF);
      Dest[Offset + 6] = (byte)((Number >> 8) & 0xFF);
      Dest[Offset + 5] = (byte)((Number >> 16) & 0xFF);
      Dest[Offset + 4] = (byte)((Number >> 24) & 0xFF);
      Dest[Offset + 3] = (byte)((Number >> 32) & 0xFF);
      Dest[Offset + 2] = (byte)((Number >> 40) & 0xFF);
      Dest[Offset + 1] = (byte)((Number >> 48) & 0xFF);
      Dest[Offset] = (byte)((Number >> 56) & 0xFF);
    }
    /// <summary>
    /// Bytex8转Int64,按书写顺序高位->低位，低位->高位
    /// </summary>
    /// <param name="Source"></param>
    /// <param name="Offset"></param>
    /// <returns></returns>
    public static ulong Byte8ToInt64FromByteArrayRev(in byte[] Source, in int Offset)
     => (ulong)((Source[Offset + 7] & 0xFF)
        | (Source[Offset + 6] & 0xFF) << 8
        | (Source[Offset + 5] & 0xFF) << 16
        | (Source[Offset + 4] & 0xFF) << 24
        | (Source[Offset + 3] & 0xFF) << 32
        | (Source[Offset + 2] & 0xFF) << 40
        | (Source[Offset + 1] & 0xFF) << 48
        | (Source[Offset] & 0xFF) << 54);

    /// <summary>
    /// IPv4地址转Bytex4,按书写顺序
    /// </summary>
    /// <param name="IP"></param>
    /// <param name="Dest"></param>
    /// <param name="Offset"></param>
    public static void IPv4AddressToByte4InByteArray(in IPAddress IP, in byte[] Dest, in int Offset) {
      var IPB = IP.GetAddressBytes();
      Dest[Offset] = IPB[0];
      Dest[Offset + 1] = IPB[1];
      Dest[Offset + 2] = IPB[2];
      Dest[Offset + 3] = IPB[3];
    }
    /// <summary>
    /// Bytex4转IPv4地址,按书写顺序
    /// </summary>
    /// <param name="Source"></param>
    /// <param name="Offset"></param>
    /// <returns></returns>
#if Core
    public static IPAddress Byte4ToIPv4AddressFromByteArray(in byte[] Source, in int Offset) => new IPAddress(new ReadOnlySpan<byte>(Source, Offset, 4));
#else
    public static IPAddress Byte4ToIPv4AddressFromByteArray(in byte[] Source, in int Offset) => IPAddress.Parse($"{Source[Offset]}.{Source[Offset + 1]}.{Source[Offset + 2]}.{Source[Offset + 3]}");
#endif
    /// <summary>
    /// IPv4+Port转Bytex6,按书写顺序
    /// </summary>
    /// <param name="IPPort"></param>
    /// <param name="Dest"></param>
    /// <param name="Offset"></param>
    public static void IPv4EndPointToByte6InByteArray(in IPEndPoint IPPort, in byte[] Dest, in int Offset) {
      var IPB = IPPort.Address.GetAddressBytes();
      Dest[Offset] = IPB[0];
      Dest[Offset + 1] = IPB[1];
      Dest[Offset + 2] = IPB[2];
      Dest[Offset + 3] = IPB[3];
      Int16ToByte2InByteArray(Convert.ToUInt16(IPPort.Port), Dest, Offset + 4);
    }
    /// <summary>
    /// Bytex6转IPv4+Port,按书写顺序
    /// </summary>
    /// <param name="Source"></param>
    /// <param name="Offset"></param>
    /// <returns></returns>
#if Core
    public static IPEndPoint Byte6ToIPv4EndPointFromByteArray(in byte[] Source, in int Offset) => new IPEndPoint(new IPAddress(new ReadOnlySpan<byte>(Source, Offset, 4)), Byte2ToInt16FromByteArray(Source, Offset + 4));
#else
    public static IPEndPoint Byte6ToIPv4EndPointFromByteArray(in byte[] Source, in int Offset) => new IPEndPoint(IPAddress.Parse($"{Source[Offset]}.{Source[Offset + 1]}.{Source[Offset + 2]}.{Source[Offset + 3]}"), Byte2ToInt16FromByteArray(Source, Offset + 4));
#endif
  }
}
