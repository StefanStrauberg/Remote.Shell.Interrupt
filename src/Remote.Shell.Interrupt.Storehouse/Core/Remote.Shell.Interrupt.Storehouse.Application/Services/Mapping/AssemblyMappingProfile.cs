namespace Remote.Shell.Interrupt.Storehouse.Application.Services.Mapping;

/// <summary>
/// AutoMapper profile for applying mappings from assembly.
/// </summary>
public sealed class AssemblyMappingProfile : Profile
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AssemblyMappingProfile"/> class 
  /// and applies mapping configurations from the given assembly.
  /// </summary>
  /// <param name="assembly">The assembly containing mapping configurations.</param>
  public AssemblyMappingProfile(Assembly assembly)
            => ApplyMappingFromAssembly(assembly);

  /// <summary>
  /// Scans the provided assembly for types implementing <see cref="IMapWith{T}"/>,
  /// instantiates them, and applies their mapping configurations.
  /// </summary>
  /// <param name="assembly">The assembly to scan for mapping configurations.</param>
  void ApplyMappingFromAssembly(Assembly assembly)
  {
    var types = assembly.GetExportedTypes()
                        .Where(type => type.GetInterfaces()
                                           .Any(x => x.IsGenericType &&
                                                x.GetGenericTypeDefinition() == typeof(IMapWith<>)))
                       .ToList();
    foreach (var type in types)
    {
      var instance = Activator.CreateInstance(type);
      var map = type.GetInterfaceMap(type.GetInterfaces()
                                         .FirstOrDefault()!);
      var interfaceMethod = map.TargetMethods
                                .Where(x => x.Name.Contains("Mapping"))
                                .FirstOrDefault();
      interfaceMethod?.Invoke(instance, [this]);
    }
  }
}
