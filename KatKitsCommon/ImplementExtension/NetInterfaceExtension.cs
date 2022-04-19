namespace KatKits.ImplementExtension {
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;
  using System.Net.NetworkInformation;
  using System.Net.Sockets;
  using System.Text;

  public static partial class Kits {
    /// <summary>
    /// 查询所有IPEndPoint,
    /// 过滤：Enabled,EnableMulticast,InterfaceType==Ethernet,IPType==IPv4
    /// 排序(High2Low)：Bandwidth then Mtu
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<IPAddress> EnumeratInternetInterfaces() {
      var IPAttrs = IPGlobalProperties.GetIPGlobalProperties();
      NetworkInterface[] NICs = NetworkInterface.GetAllNetworkInterfaces();
      return (NICs ?? Enumerable.Empty<NetworkInterface>())
        .Where(E => E.OperationalStatus == OperationalStatus.Up)
        .Where(E => E.SupportsMulticast)
        .Where(E => E.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
        .OrderByDescending(E => E.Speed)
        .Select(E => (E, E.GetIPProperties().GetIPv4Properties()))
        .OrderByDescending(E => E.Item2.Mtu)
        .Select(E => E.E.GetIPProperties().UnicastAddresses.Where(EE => EE.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).FirstOrDefault()?.Address)
        .Where(E => E != null);
    }
    /// <summary>
    /// 空闲的UDP端口
    /// </summary>
    /// <param name="Minimum">端口最小值</param>
    /// <param name="Maximum">端口最大值</param>
    /// <returns></returns>
    public static IEnumerable<int> FreeUDPPort(in int Minimum = 1000, in int Maximum = 65535) {
      var IPProps = IPGlobalProperties.GetIPGlobalProperties();
      var UDPDetectedPorts = new HashSet<int>(IPProps.GetActiveUdpListeners().Where(E => E.AddressFamily == AddressFamily.InterNetwork).Select(E => E.Port));
      return Enumerable.Range(Minimum, Maximum - Minimum).Where(E => !UDPDetectedPorts.Contains(E));
    }

    /// <summary>
    /// 空闲的TCP端口
    /// </summary>
    /// <param name="Minimum">端口最小值</param>
    /// <param name="Maximum">端口最大值</param>
    /// <returns></returns>
    public static IEnumerable<int> FreeTCPPort(in int Minimum = 1000, in int Maximum = 65535) {
      var IPProps = IPGlobalProperties.GetIPGlobalProperties();
      var UDPDetectedPorts = new HashSet<int>(IPProps.GetActiveTcpListeners().Where(E => E.AddressFamily == AddressFamily.InterNetwork).Select(E => E.Port));
      return Enumerable.Range(Minimum, Maximum - Minimum).Where(E => !UDPDetectedPorts.Contains(E));
    }
  }
}
