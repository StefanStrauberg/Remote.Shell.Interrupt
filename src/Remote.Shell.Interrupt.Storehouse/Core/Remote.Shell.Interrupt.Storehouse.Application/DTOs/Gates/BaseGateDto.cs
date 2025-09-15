namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Gates;

public class BaseGateDTO : IMapWith<Gate>
{
    public string Name { get; set; } = string.Empty;
    public string Community { get; set; } = string.Empty;
    public string TypeOfNetworkDevice { get; set; } = string.Empty;

    void IMapWith<Gate>.Mapping(Profile profile)
    {
        profile.CreateMap<BaseGateDTO, Gate>()
            .ForMember(dst => dst.Name,
                       opt => opt.MapFrom(src => src.Name))
            .ForMember(dst => dst.Community,
                       opt => opt.MapFrom(src => src.Community))
            .ForMember(dst => dst.TypeOfNetworkDevice,
                       opt => opt.MapFrom(src => Enum.Parse<TypeOfNetworkDevice>(src.TypeOfNetworkDevice)));
    }
}
