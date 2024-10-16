namespace Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;

public enum PortStatus
{
  up = 1,
  down = 2,
  testing = 3,
  unknown = 4,
  dormant = 5,
  notPresent = 6,
  lowerLayerDown = 7
}
