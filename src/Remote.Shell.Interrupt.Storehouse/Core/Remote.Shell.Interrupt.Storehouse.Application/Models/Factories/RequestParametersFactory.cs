namespace Remote.Shell.Interrupt.Storehouse.Application.Models.Factories;

/// <summary>
/// Provides factory methods for creating <see cref="RequestParameters"/> with predefined filters.
/// </summary>
internal static class RequestParametersFactory
{
  /// <summary>
  /// Creates request parameters for filtering entities by VLAN tag.
  /// </summary>
  /// <param name="vlanTag">The VLAN tag to filter by.</param>
  /// <returns>
  /// A <see cref="RequestParameters"/> instance with a filter targeting <see cref="SPRVlan.IdVlan"/>.
  /// </returns>
  /// <remarks>
  /// Useful for querying VLAN-related entities using a standardized filter descriptor.
  /// </remarks>
  public static RequestParameters ForVlanTag(int vlanTag) => new()
  {
    Filters = [new FilterDescriptor
    {
      PropertyPath = nameof(SPRVlan.IdVlan),
      Operator = FilterOperator.Equals,
      Value = vlanTag.ToString()
    }]
  };

  /// <summary>
  /// Creates request parameters for filtering entities by client ID.
  /// </summary>
  /// <param name="clientId">The client ID to filter by.</param>
  /// <returns>
  /// A <see cref="RequestParameters"/> instance with a filter targeting <see cref="Client.IdClient"/>.
  /// </returns>
  /// <remarks>
  /// Useful for querying client-related entities using a standardized filter descriptor.
  /// </remarks>
  public static RequestParameters ForClientId(int clientId) => new()
  {
    Filters = [new FilterDescriptor
    {
      PropertyPath = nameof(Client.IdClient),
      Operator = FilterOperator.Equals,
      Value = clientId.ToString()
    }]
  };

  /// <summary>
  /// Creates request parameters for querying an entity by its <c>Id</c> value.
  /// </summary>
  /// <param name="id">The unique identifier to filter by.</param>
  /// <returns>A <see cref="RequestParameters"/> with ID-based filtering applied.</returns>
  public static RequestParameters ForId(Guid id) => new()
  {
    Filters = [new FilterDescriptor
    {
      PropertyPath = "Id",
      Operator = FilterOperator.Equals,
      Value = id.ToString()
    }]
  };
}
