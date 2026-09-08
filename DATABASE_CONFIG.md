# 🔌 AgroRegulate Veritabanı ve Altyapı Bağlantı Dokümantasyonu

Bu doküman, AgroRegulate projesinin kullandığı veritabanı (PostgreSQL), önbellek (Redis) ve mesaj kuyruğu (Apache Kafka) servislerinin bağlantı parametrelerini, Docker yapılandırmalarını ve bağlantı dizelerini içerir.

---

## 1. Bağlantı Dizeleri (Connection Strings)

### A. AgroRegulate.API (`appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=AgroRegulateDb;Username=postgres;Password=postgres"
  },
  "Redis": {
    "Configuration": "localhost:6379"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "Topic": "allocation-requests-topic"
  }
}