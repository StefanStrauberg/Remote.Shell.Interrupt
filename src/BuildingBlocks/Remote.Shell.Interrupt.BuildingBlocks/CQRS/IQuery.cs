namespace Remote.Shell.Interrupt.BuildingBlocks.CQRS;

public interface IQuery<out TResponse> : IRequest<TResponse>
  where TResponse : notnull
{ }
