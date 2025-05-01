namespace Remote.Shell.Interrupt.Storehouse.Application.Services.Specification;

public class SPRVlanSpecification : GenericSpecification<SPRVlan>, ISPRVlanSpecification
{ 
  public override SPRVlanSpecification Clone()
  {
      // Создаем стандартный клон спецификации
      var clone = new SPRVlanSpecification
      {
          Take = Take,
          Skip = Skip
      };

      if (_criteria is not null)
          clone.AddFilter(_criteria);

      foreach (var include in _includes)
          clone.AddInclude(include);

      return clone;
  }
}
