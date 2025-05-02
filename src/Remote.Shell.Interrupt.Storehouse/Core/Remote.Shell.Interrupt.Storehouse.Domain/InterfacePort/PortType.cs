namespace Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;

/// <summary>
/// Represents various types of network interface ports.
/// </summary>
public enum PortType
{
  /// <summary>
  /// Other or unspecified interface type.
  /// </summary>
  other = 1,

  /// <summary>
  /// Null interface type.
  /// </summary>
  Null = 2,

  /// <summary>
  /// Standard Ethernet interface using CSMA/CD (Carrier Sense Multiple Access with Collision Detection).
  /// </summary>
  ethernetCsmacd = 6, 

  /// <summary>
  /// ISO 8802-3 Ethernet with CSMA/CD.
  /// </summary>
  iso88023Csmacd = 7, 

  /// <summary>
  /// Token Bus (ISO 8802-4).
  /// </summary>
  iso88024TokenBus = 8, 

  /// <summary>
  /// Token Ring (ISO 8802-5).
  /// </summary>
  iso88025TokenRing = 9,

  /// <summary>
  /// Metropolitan Area Network (ISO 8802-6).
  /// </summary> 
  iso88026Man = 10, 

  /// <summary>
  /// StarLAN network.
  /// </summary>
  starLan = 11, 

  /// <summary>
  /// Data Link Control (DLC) interface.
  /// </summary>
  dlcI = 12, 

  /// <summary>
  /// Asynchronous Transfer Mode (ATM).
  /// </summary>
  atm = 13, 

  /// <summary>
  /// High-Level Data Link Control (HDLC).
  /// </summary>
  hdlc = 14, 

  /// <summary>
  /// Link Access Protocol (LAP-B).
  /// </summary>
  lip = 15, 

  /// <summary>
  /// Synchronous Data Link Control (SDLC).
  /// </summary>
  sdlc = 16, 

  /// <summary>
  /// DS1 (T1) interface.
  /// </summary>
  ds1 = 17, 

  /// <summary>
  /// E1 interface.
  /// </summary>
  e1 = 18, 

  /// <summary>
  /// Basic ISDN (Integrated Services Digital Network).
  /// </summary>
  basicISDN = 19, 

  /// <summary>
  /// Primary ISDN.
  /// </summary>
  primaryISDN = 20,

  /// <summary>
  /// Proprietary point-to-point interface.
  /// </summary>
  propPointToPoint = 21,

  /// <summary>
  /// Proprietary multipoint interface.
  /// </summary>
  propMultipoint = 22,

  /// <summary>
  /// IEEE 802.11 (Wi-Fi).
  /// </summary>
  IEEE80211 = 23, 

  /// <summary>
  /// Software loopback interface.
  /// </summary>
  softwareLoopback = 24,

  /// <summary>
  /// Ethernet over Non-Ethernet (EON).
  /// </summary>
  eon = 25, 

  /// <summary>
  /// Ethernet 3 Mbit/sec.
  /// </summary>
  ethernet3M = 26, 

  /// <summary>
  /// Serial interface.
  /// </summary>
  serial = 27,
  
  /// <summary>
  /// Frame Relay.
  /// </summary>
  frameRelay = 28, 

  /// <summary>
  /// X.25 interface.
  /// </summary>
  x25 = 29, 

  /// <summary>
  /// Token Ring.
  /// </summary>
  tokenRing = 30, 

  /// <summary>
  /// Protocol-based interface.
  /// </summary>
  protocol = 31,

  /// <summary>
  /// VLAN (Virtual LAN).
  /// </summary>
  vlan = 32, 

  /// <summary>
  /// Proprietary virtual interface.
  /// </summary>
  propVirtual = 53, 

  /// <summary>
  /// Tunnel interface.
  /// </summary>
  tunnel = 131,

  /// <summary>
  /// Layer 2 VLAN.
  /// </summary>
  l2vlan = 135,

  /// <summary>
  /// MPLS tunnel.
  /// </summary>
  mplsTunnel = 150,

  /// <summary>
  /// IEEE 802.3ad Link Aggregation (Aggregated Ethernet).
  /// </summary>
  ieee8023adLag = 161 
}