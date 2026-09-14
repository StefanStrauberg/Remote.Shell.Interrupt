namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Gates;

/// <summary>
/// Shared property holder for the Gate DTOs. Deliberately does not implement
/// <see cref="IMapWith{T}"/>: nothing ever maps a bare <see cref="BaseGateDTO"/>, and each
/// derived DTO (<see cref="CreateGateDTO"/>, <see cref="GateDTO"/>, <see cref="UpdateGateDTO"/>)
/// registers its own complete AutoMapper map anyway - Name/Community are mapped by
/// convention and TypeOfNetworkDevice is converted string↔enum by AutoMapper's built-in
/// enum conversion, so a mapping here would never be applied to any of them and would
/// only duplicate what they already do.
/// </summary>
public class BaseGateDTO
{
    public string Name { get; set; } = string.Empty;
    public string Community { get; set; } = string.Empty;
    public string TypeOfNetworkDevice { get; set; } = string.Empty;
}
