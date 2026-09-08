using AgroRegulate.Domain.Enums;

namespace AgroRegulate.Domain.Entities;

public class AllocationRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AnnouncementId { get; set; }
    public Guid FactoryId { get; set; }
    public ProductType ProductType { get; set; } = ProductType.Misir;
    public decimal RequestedTons { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Beklemede;
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
}