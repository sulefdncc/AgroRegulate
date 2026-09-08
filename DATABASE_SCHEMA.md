# 🏛️ AgroRegulate Veritabanı ve Mimari Şeması

Bu doküman, AgroRegulate Tahsis ve Kota Yönetim Sistemi'nin veritabanı tablolarını, ilişkilerini ve alan tiplerini içerir.

---

## 📊 1. Veritabanı Varlık-İlişki (ER) Diyagramı

```mermaid
erDiagram
    FACTORIES ||--o{ QUOTAS : "sahiptir (1:N)"
    FACTORIES ||--o{ ALLOCATION_REQUESTS : "talep eder (1:N)"
    SALES_ANNOUNCEMENTS ||--o{ ALLOCATION_REQUESTS : "içerir (1:N)"

    FACTORIES {
        uuid Id PK
        string Name
        string TaxNumber
        int SectorType "0: Gida, 1: Yem, 2: Kimya, 3: Enerji"
        bool IsActive
    }

    QUOTAS {
        uuid Id PK
        uuid FactoryId FK
        int ProductType "0: Misir, 1: Bugday, 2: SekerPancari, 3: Arpa"
        decimal TotalAllocatedTons
        decimal RemainingTons
        int Year
    }

    SALES_ANNOUNCEMENTS {
        uuid Id PK
        string Title
        int ProductType "0: Misir, 1: Bugday, 2: SekerPancari, 3: Arpa"
        decimal TotalOfferedTons
        decimal AvailableTons
        decimal UnitPrice
        datetime StartDate
        datetime EndDate
        bool IsActive
    }

    ALLOCATION_REQUESTS {
        uuid Id PK
        uuid AnnouncementId FK
        uuid FactoryId FK
        decimal RequestedTons
        int Status "0: Beklemede, 1: Onaylandi, 2: Reddedildi"
        datetime RequestDate
    }