namespace Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;

public enum PortType
{
  other = 1,
  Null = 2,
  ethernetCsmacd = 6, // Standart Ethernet Interface uses method CSMA/CD (Carrier Sense Multiple Access with Collision Detection)
  iso88023Csmacd = 7, // ISO 8802-3 (Ethernet) с CSMA/CD
  iso88024TokenBus = 8, // Token Bus (ISO 8802-4)
  iso88025TokenRing = 9, // Token Ring (ISO 8802-5)
  iso88026Man = 10, // Metropolitan Area Network (ISO 8802-6)
  starLan = 11, // StarLAN
  dlcI = 12, // DLC (Data Link Control) I
  atm = 13, // Asynchronous Transfer Mode (ATM)
  hdlc = 14, // High-Level Data Link Control (HDLC)
  lip = 15, // Link Access Protocol (LAP-B)
  sdlc = 16, // Synchronous Data Link Control (SDLC)
  ds1 = 17, // DS1 (T1)
  e1 = 18, // E1
  basicISDN = 19, // ISDN (Integrated Services Digital Network)
  primaryISDN = 20, // ISDN
  propPointToPoint = 21,
  propMultipoint = 22,
  IEEE80211 = 23, // IEEE 802.11 (Wi-Fi
  loopback = 24,
  eon = 25, // Ethernet over Non-Ethernet (EON)
  ethernet3M = 26, // Ethernet 3 Mbit/sec
  serial = 27,
  frameRelay = 28, // Frame Relay
  x25 = 29, // X.25
  tokenRing = 30, // Token Ring
  protocol = 31,
  vlan = 32, // VLAN (Virtual LAN)
  irb = 53, // Integrated Routing and Bridging
  vtun = 131,
  TenGigEthernet = 135,
  unknown = 150,
  ae = 161 // Aggregated Ethernet
}