using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Identity;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Users;

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Users.Queries.GetUsersByFilter;

/// <summary>
/// Retrieves a filtered, paginated, sorted list of accounts for the admin Users page.
/// </summary>
public sealed record GetUsersByFilterQuery(RequestParameters Parameters) : CQRS.IQuery<PagedList<UserDTO>>;

internal class GetUsersByFilterQueryHandler(IIdentityService identityService)
  : CQRS.IQueryHandler<GetUsersByFilterQuery, PagedList<UserDTO>>
{
  public async ValueTask<PagedList<UserDTO>> Handle(GetUsersByFilterQuery request, CancellationToken cancellationToken)
    => await identityService.GetUsersByFilterAsync(request.Parameters, cancellationToken);
}
