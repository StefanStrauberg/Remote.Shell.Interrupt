namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Vlans;

public class VLANDTO : IRegister
{
  public int VLANTag { get; set; } // 10
  public string VLANName { get; set; } = string.Empty; // "VLAN10"

  void IRegister.Register(TypeAdapterConfig config)
  {
    config.NewConfig<VLAN, VLANDTO>()
          .Map(dest => dest.VLANTag,
               src => src.VLANTag)
          .Map(dest => dest.VLANName,
               src => src.VLANName);
  }
}