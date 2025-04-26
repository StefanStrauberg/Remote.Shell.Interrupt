namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.QueryFilterParser;

public interface IQueryFilterParser
{
  Expression<Func<T, bool>>? ParseFilters<T>(List<FilterDescriptor>? filters);
}
