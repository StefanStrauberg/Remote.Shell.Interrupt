namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public class SPRVlanDTO : IMapWith<SPRVlanL>
{
  public int IdVlan { get; set; }
  public int IdClient { get; set; }
  public bool UseClient { get; set; }
  public bool UseCOD { get; set; }

  void IMapWith<SPRVlanL>.Mapping(Profile profile)
  {
    profile.CreateMap<SPRVlanL, SPRVlanDTO>()
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