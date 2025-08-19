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
  /// Creates request parameters for filtering entities by their unique identifier.
  /// </summary>
  /// <param name="id">The GUID identifier to filter by.</param>
  /// <returns>
  /// A <see cref="RequestParameters"/> instance with a filter targeting the <c>Id</c> property.
  /// </returns>
  /// <remarks>
  /// Commonly used for entity retrieval by primary key.
  /// </remarks>
  public static RequestParameters ForId(Guid id) => new()
  {
    Filters = [new FilterDescriptor
    {
      PropertyPath = "Id",
      Operator = FilterOperator.Equals,
      Value = id.ToString()
    }]
  };

  /// <summary>
  /// Creates request parameters for filtering network devices by associated VLAN tags.
  /// </summary>
  /// <param name="vlanTags">A collection of VLAN tags to filter by.</param>
  /// <returns>
  /// A <see cref="RequestParameters"/> instance with a nested filter targeting VLANs within ports.
  /// </returns>
  /// <remarks>
  /// Uses the <c>In</c> operator to match any VLAN tag within the provided set.
  /// The property path traverses <see cref="NetworkDevice.PortsOfNetworkDevice"/> → <see cref="Port.VLANs"/> → <see cref="VLAN.VLANTag"/>.
  /// </remarks>
  public static RequestParameters ForNetworkDevicesByVlans(IEnumerable<int> vlanTags) => new()
  {
    Filters = [new FilterDescriptor
    {
      PropertyPath = $"{nameof(NetworkDevice.PortsOfNetworkDevice)}.{nameof(Port.VLANs)}.{nameof(VLAN.VLANTag)}",
      Operator = FilterOperator.In,
      Value = Converter.ArrayToString(vlanTags)
    }]
  };

  public static RequestParameters Empty() => new()
  {
  };
}
