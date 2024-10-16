namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Mapping;

public sealed class AssemblyMappingProfile : Profile
{
  public AssemblyMappingProfile(Assembly assembly)
            => ApplyMappingFromAssembly(assembly);

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
