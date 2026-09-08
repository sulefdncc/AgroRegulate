using AgroRegulate.Domain.Enums;

namespace AgroRegulate.Domain.Entities;

public class Factory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string TaxNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public SectorType SectorType { get; set; }
    public AccountStatus AccountStatus { get; set; } = AccountStatus.Pending;
    public bool IsActive { get; set; } = true;

    public ICollection<Quota> Quotas { get; set; } = new List<Quota>();
    public ICollection<AllocationRequest> AllocationRequests { get; set; } = new List<AllocationRequest>();
}