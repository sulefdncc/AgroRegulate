using AgroRegulate.Domain.Enums;

namespace AgroRegulate.Domain.Entities;

public class SalesAnnouncement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public ProductType ProductType { get; set; }
    public decimal TotalOfferedTons { get; set; }
    public decimal AvailableTons { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<AllocationRequest> AllocationRequests { get; set; } = new List<AllocationRequest>();
}