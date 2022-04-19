namespace KatKits {
  using System;
  using System.Collections.Generic;
  using System.Security.Cryptography;
  using System.Text;

  public static partial class Kits {
    public static string CRC64ISOHash(this byte[] Data) {
      return BitConverter.ToString(new CRC64_ISO().ComputeHash(Data));
    }
    public static byte[] CRC64ISOHashToByte(this byte[] Data) {
      return new CRC64_ISO().ComputeHash(Data);
    }
    public static string MD5Hash(this byte[] Data) {
      return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Data));
    }
    public static string CRC32Hash(this byte[] Data) {
      return BitConverter.ToString(new Crc32().ComputeHash(Data));
    }
  }


  public class Crc32 : HashAlgorithm {
    public const UInt32 DefaultPolynomial = 0xedb88320;
    public const UInt32 DefaultSeed = 0xffffffff;
    private UInt32 hash;
    private readonly UInt32 seed;
    private readonly UInt32[] table;
    private static UInt32[] defaultTable;
    public Crc32() {
      table = InitializeTable(DefaultPolynomial);
      seed = DefaultSeed;
      Initialize();
    }
    public Crc32(UInt32 polynomial, UInt32 seed) {
      table = InitializeTable(polynomial);
      this.seed = seed;
      Initialize();
    }
    public override void Initialize() {
      hash = seed;
    }
    protected override void HashCore(byte[] buffer, int start, int length) {
      hash = CalculateHash(table, hash, buffer, start, length);
    }
    protected override byte[] HashFinal() {
      byte[] hashBuffer = UInt32ToBigEndianBytes(~hash);
      this.HashValue = hashBuffer;
      return hashBuffer;
    }
    public static UInt32 Compute(byte[] buffer) {
      return ~CalculateHash(InitializeTable(DefaultPolynomial), DefaultSeed, buffer, 0, buffer.Length);
    }
    public static UInt32 Compute(UInt32 seed, byte[] buffer) {
      return ~CalculateHash(InitializeTable(DefaultPolynomial), seed, buffer, 0, buffer.Length);
    }
    public static UInt32 Compute(UInt32 polynomial, UInt32 seed, byte[] buffer) {
      return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
    }
    private static UInt32[] InitializeTable(UInt32 polynomial) {
      if (polynomial == DefaultPolynomial && defaultTable != null) {
        return defaultTable;
      }
      UInt32[] createTable = new UInt32[256];
      for (int i = 0; i < 256; i++) {
        UInt32 entry = (UInt32)i;
        for (int j = 0; j < 8; j++) {
          if ((entry & 1) == 1) {
            entry = (entry >> 1) ^ polynomial;
          }
          else {
            entry = entry >> 1;
          }
        }
        createTable[i] = entry;
      }
      if (polynomial == DefaultPolynomial) {
        defaultTable = createTable;
      }
      return createTable;
    }
    private static UInt32 CalculateHash(UInt32[] table, UInt32 seed, byte[] buffer, int start, int size) {
      UInt32 crc = seed;
      for (int i = start; i < size; i++) {
        unchecked {
          crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
        }
      }
      return crc;
    }
    private byte[] UInt32ToBigEndianBytes(UInt32 x) {
      return new byte[] { (byte)((x >> 24) & 0xff), (byte)((x >> 16) & 0xff), (byte)((x >> 8) & 0xff), (byte)(x & 0xff) };
    }
  }
  public class CRC64 : HashAlgorithm {
    public const UInt64 DefaultSeed = 0x0;

    readonly UInt64[] table;

    readonly UInt64 seed;
    UInt64 hash;

    public CRC64(UInt64 polynomial)
        : this(polynomial, DefaultSeed) {
    }

    public CRC64(UInt64 polynomial, UInt64 seed) {
      if (!BitConverter.IsLittleEndian)
        throw new PlatformNotSupportedException("Not supported on Big Endian processors");

      table = InitializeTable(polynomial);
      this.seed = hash = seed;
    }

    public override void Initialize() {
      hash = seed;
    }

    protected override void HashCore(byte[] array, int ibStart, int cbSize) {
      hash = CalculateHash(hash, table, array, ibStart, cbSize);
    }

    protected override byte[] HashFinal() {
      var hashBuffer = UInt64ToBigEndianBytes(hash);
      HashValue = hashBuffer;
      return hashBuffer;
    }

    public override int HashSize { get { return 64; } }

    protected static UInt64 CalculateHash(UInt64 seed, UInt64[] table, IList<byte> buffer, int start, int size) {
      var hash = seed;
      for (var i = start; i < start + size; i++)
        unchecked {
          hash = (hash >> 8) ^ table[(buffer[i] ^ hash) & 0xff];
        }
      return hash;
    }

    static byte[] UInt64ToBigEndianBytes(UInt64 value) {
      var result = BitConverter.GetBytes(value);

      if (BitConverter.IsLittleEndian)
        Array.Reverse(result);

      return result;
    }

    static UInt64[] InitializeTable(UInt64 polynomial) {
      if (polynomial == CRC64_ISO.Iso3309Polynomial && CRC64_ISO.Table != null)
        return CRC64_ISO.Table;

      var createTable = CreateTable(polynomial);

      if (polynomial == CRC64_ISO.Iso3309Polynomial)
        CRC64_ISO.Table = createTable;

      return createTable;
    }

    protected static ulong[] CreateTable(ulong polynomial) {
      var createTable = new UInt64[256];
      for (var i = 0; i < 256; ++i) {
        var entry = (UInt64)i;
        for (var j = 0; j < 8; ++j)
          if ((entry & 1) == 1)
            entry = (entry >> 1) ^ polynomial;
          else
            entry >>= 1;
        createTable[i] = entry;
      }
      return createTable;
    }
  }
  public class CRC64_ISO : CRC64 {
    internal static UInt64[] Table;

    public const UInt64 Iso3309Polynomial = 0xD800000000000000;

    public CRC64_ISO()
        : base(Iso3309Polynomial) {
    }

    public CRC64_ISO(UInt64 seed)
        : base(Iso3309Polynomial, seed) {
    }

    public static UInt64 Compute(byte[] buffer) {
      return Compute(DefaultSeed, buffer);
    }

    public static UInt64 Compute(UInt64 seed, byte[] buffer) {
      if (Table == null)
        Table = CreateTable(Iso3309Polynomial);

      return CalculateHash(seed, Table, buffer, 0, buffer.Length);
    }
  }
}
