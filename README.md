# 🍽️ QR Menüm SaaS Platform

Modern, multi-tenant (çoklu kiracı) mimariye sahip yüksek performanslı Dijital QR Menü platformu. İşletmelerin saniyeler içinde kendi markalarına özel dijital menülerini oluşturmasını, ürünlerini yönetmesini ve müşterilerine benzersiz bir kullanıcı deneyimi sunmasını sağlar.

## ✨ Özellikler

- **Multi-Tenant Mimari:** Tek bir platform üzerinden yüzlerce restoranı izole bir şekilde yönetin.
- **Premium Müşteri Arayüzü (UI):** Tamamen donanım hızlandırmalı, Glassmorphism (cam tasarımı) odaklı, 60fps çalışan Vanilla CSS / Native JS tasarımı.
- **Çoklu Dil Desteği:** Türkçe, İngilizce ve Arapça (Sağdan Sola / RTL) destekleyen esnek altyapı.
- **Dinamik Markalama:** Her restorana özel tema renkleri ve font destekleri, anında güncellenebilen CSS değişkenleri.
- **Dahili QR Kod Üretici:** Restoran, masa numarası üzerinden kolay tarama için otomatik QR oluşturma sistemi.
- **Karanlık Mod (Dark Mode):** Müşterilerin cihaz tercihlerine anında saygı duyan otomatik akıllı karanlık mod tasarımı.
- **Wolvox ERP Entegrasyonu:** Stok ve sipariş altyapısı için harici sisteme entegrasyon servisi.

## 🛠️ Teknolojiler

- **Backend:** C# (.NET 9) - ASP.NET Core API & Razor Pages
- **ORM & Database:** Entity Framework Core v9 (MSSQL & SQLite Support)
- **Frontend / Menü:** HTML5, Modern SSR (Razor), Vanilla CSS & JS
- **Barkod Üretici:** QRCoder

## 🚀 Başlangıç

### Geliştirme Ortamı (Development)
Projeyi lokal bilgisayarınızda çalıştırmak için:

1. **Gereksinimler:**
   - [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
   - MSSQL LocalDB (veya SQLite)

2. **Veritabanının Hazırlanması:**
```bash
dotnet ef database update --project src/QrMenu.Infrastructure --startup-project src/QrMenu.API
```

3. **Uygulamanın Çalıştırılması:**
```bash
# Otomatik yenileme ile başlatır
dotnet watch run --project src/QrMenu.API/QrMenu.API.csproj --launch-profile http
```

API ve Yönetim Paneli default olarak `http://localhost:5126` üzerinden hizmet verir. Süper Admin hesabı veritabanı oluştuğu anda `admin@qrmenu.com` / `qrmenu2024` olarak otomatik kurulur.

## 📂 Proje Mimarisi (Clean Architecture)

- **QrMenu.API**: Uygulamanın giriş noktası (Entry point). Razor Pages (yönetim ve müşteri ekranları), Controller'lar ve konfigürasyon.
- **QrMenu.Application**: İş mantığı (Business logic). DTO'lar, servis arayüzleri.
- **QrMenu.Domain**: Çekirdek katman. Varlıklar (Entities) ve enum yapıları (`Tenant`, `Restaurant`, `Category`, `MenuItem`, `User`).
- **QrMenu.Infrastructure**: Veri erişimi (Entity Framework DBContext, Migrations), Dış servis bağlantıları (WolvoxSdkService) ve JWT yetkilendirme altyapısı.

## 🌐 Müşteri Arayüzü URL Yapısı

Digital menü arayüzü `http://localhost:5126/m/{restoran-slug}` adresinden host edilir. URL üzerinden dil parametreleri `?lang=en` ile tetiklenebilir. Masa numarası eklentisi mevcuttur.

## 📬 İletişim & Geliştirici

**Geliştirici:** Yusuf  
**Email:** [yusufunkisiseli@icloud.com](mailto:yusufunkisiseli@icloud.com)

---
*Powered by Yusuf Çukurlu*
