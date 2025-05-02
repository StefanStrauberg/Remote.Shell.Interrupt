namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.TerminatedNetworkEntities;

public class TerminatedNetworkEntityDTO : IMapWith<TerminatedNetworkEntity>
{
  public string NetworkAddress { get; set; } = string.Empty;
  public string Netmask { get; set; } = string.Empty;

  void IMapWith<TerminatedNetworkEntity>.Mapping(Profile profile)
  {
    profile.CreateMap<TerminatedNetworkEntity, TerminatedNetworkEntityDTO>()
           .ForMember(dest => dest.NetworkAddress,
                      opt => opt.MapFrom(src => ConvertToString(src.NetworkAddress)))
           .ForMember(dest => dest.Netmask,
                      opt => opt.MapFrom(src => ConvertToString(src.Netmask)));
  }

  static string ConvertToString(long address)
  {
    var bytes = BitConverter.GetBytes(address);
    Array.Reverse(bytes); // Изменяем порядок байтов
    return new IPAddress(bytes).ToString();
  }
}