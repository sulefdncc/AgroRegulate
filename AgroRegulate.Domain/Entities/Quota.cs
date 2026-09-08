using AgroRegulate.Domain.Enums;

namespace AgroRegulate.Domain.Entities;

public class Quota
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid FactoryId { get; set; }
    public Factory? Factory { get; set; }

    public ProductType ProductType { get; set; }
    public decimal TotalAllocatedTons { get; set; }
    public decimal RemainingTons { get; set; }
    public int Year { get; set; }
}