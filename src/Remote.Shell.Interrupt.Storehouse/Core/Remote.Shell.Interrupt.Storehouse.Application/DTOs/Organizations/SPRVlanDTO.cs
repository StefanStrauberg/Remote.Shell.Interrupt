namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;

public class SPRVlanDTO : IMapWith<SPRVlan>
{
  public int IdVlan { get; set; }
  public int IdClient { get; set; }
  public bool UseClient { get; set; }
  public bool UseCOD { get; set; }

  void IMapWith<SPRVlan>.Mapping(Profile profile)
  {
    profile.CreateMap<SPRVlan, SPRVlanDTO>()
           .ForMember(dest => dest.IdVlan,
                      opt => opt.MapFrom(src => src.IdVlan))
           .ForMember(dest => dest.IdClient,
                      opt => opt.MapFrom(src => src.IdClient))
           .ForMember(dest => dest.UseClient,
                      opt => opt.MapFrom(src => src.UseClient))
           .ForMember(dest => dest.UseCOD,
                      opt => opt.MapFrom(src => src.UseCOD));
  }
}