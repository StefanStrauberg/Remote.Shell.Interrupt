namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.SPRVlans;

public class SPRVlanDTO : IMapWith<SPRVlan>
{
  public Guid Id { get; set; }
  public int IdVlan { get; set; }
  public int IdClient { get; set; }
  public bool UseClient { get; set; }
  public bool UseCOD { get; set; }

  void IMapWith<SPRVlan>.Mapping(Profile profile)
  {
    profile.CreateMap<SPRVlan, SPRVlanDTO>()
           .ForMember(dest => dest.Id, 
                      opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.IdVlan, 
                      opt => opt.MapFrom(src => src.IdVlan))
           .ForMember(dest => dest.UseClient, 
                      opt => opt.MapFrom(src => src.UseClient))
           .ForMember(dest => dest.UseCOD, 
                      opt => opt.MapFrom(src => src.UseCOD));
  }
}
