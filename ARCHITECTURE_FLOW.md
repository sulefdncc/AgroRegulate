```markdown
# ⚡ AgroRegulate Uçtan Uca Sistem Akış Mimarisi

```mermaid
sequenceDiagram
    autonumber
    actor Fabrika as 🏭 Fabrika Kullanıcısı (Frontend)
    participant API as 🌐 AgroRegulate.API
    participant MediatR as 🎯 Application (MediatR Handler)
    participant Redis as ⚡ Redis (Kota & Kilit)
    participant Kafka as 📨 Apache Kafka (Event Topic)
    participant Worker as ⚙️ Background Worker (Consumer)
    participant Postgres as 🐘 PostgreSQL (Database)

    Fabrika->>API: 1. POST /api/Allocations/request-allocation
    API->>MediatR: 2. CreateAllocationRequestCommand fırlatılır
    MediatR->>Redis: 3. Kota kontrolü ve anlık kilitleme (DecreaseQuotaAsync)
    
    alt Kota Yetersiz
        Redis-->>MediatR: False
        MediatR-->>API: 400 Bad Request (Kota Yetersiz)
        API-->>Fabrika: "Yıllık kotanızı aştınız!"
    else Kota Yeterli
        Redis-->>MediatR: True
        MediatR->>Kafka: 4. Publish Event ("allocation-requests-topic")
        MediatR-->>API: 200 OK (İstek Kuyruğa Alındı)
        API-->>Fabrika: "Talebiniz başarıyla sıraya alındı! (Milisaniyeler içinde)"
    end

    Note over Worker,Kafka: Asenkron Arka Plan İşleme (Consumer)
    Worker->>Kafka: 5. Kuyruktan yeni tahsis talebini oku (Consume)
    Worker->>Postgres: 6. AllocationRequests tablosuna kalıcı INSERT yap
    Worker->>Postgres: 7. SalesAnnouncement kalan tonajını güncelle