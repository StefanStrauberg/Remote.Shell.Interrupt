namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;

public class CandidateEdgeDTO : IMapWith<CandidateEdge>
{
  public string? Condition { get; set; }

  public int Priority { get; set; }

  void IMapWith<CandidateEdge>.Mapping(Profile profile)
    => profile.CreateMap<CandidateEdge, CandidateEdgeDTO>();
}
