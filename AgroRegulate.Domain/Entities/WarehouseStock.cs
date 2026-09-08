using AgroRegulate.Domain.Enums;

namespace AgroRegulate.Domain.Entities;

public class WarehouseStock
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ProductType ProductType { get; set; }
    public decimal AvailableTons { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
