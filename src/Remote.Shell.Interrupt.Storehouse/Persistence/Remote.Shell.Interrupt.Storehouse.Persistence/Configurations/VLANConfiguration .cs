namespace Remote.Shell.Interrupt.Storehouse.Persistence.Configurations;

public class VLANConfiguration : IEntityTypeConfiguration<VLAN>
{
       public void Configure(EntityTypeBuilder<VLAN> builder)
       {
              // Конфигурация каскадного удаления уже настроена в PortConfiguration
              // Дополнительно можно указать конфигурацию для VLAN здесь при необходимости
       }
}