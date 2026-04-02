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

## 🚀 Kurulum & Başlangıç Rehberi

Projeyi yerel cihazınızda (lokal ortamda) sorunsuz bir şekilde çalıştırmak ve geliştirmek için aşağıdaki adımları sırasıyla uygulayın.

### 1. Sistem Gereksinimleri
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) (Core Framework)
- MsSQL LocalDB (Visual Studio ile birlikte gelir) veya SQLite
- Git

### 2. Projeyi Bilgisayarınıza İndirin (Clone)
Terminalinizi açın ve Github deposunu bilgisayarınıza indirin:
```bash
git clone https://github.com/benyusuf02/Qr-Menum.git
cd Qr-Menum
```

### 3. Bağımlılıkları Yükleyin (Restore)
.NET paketlerini (Entity Framework vb.) indirmek için aşağıdaki komutu çalıştırın:
```bash
dotnet restore QrMenu.sln
```

### 4. Veritabanının Hazırlanması ve Demo Verilerin Yüklenmesi
Uygulama arka planda bir SQL veritabanına ihtiyaç duyar. Tabloları oluşturmak için Entity Framework Core CLI aracıyla aşağıdaki Migration işlemini çalıştırın:
```bash
dotnet ef database update --project src/QrMenu.Infrastructure --startup-project src/QrMenu.API
```
*Not: Eğer yukarıdaki komut hata verirse önce `dotnet tool install --global dotnet-ef` komutuyla EF Core araçlarını sisteminize kurduğunuzdan emin olun.*

Sistem ayağa kalktığında tablolarda hiç işlem yapılmamışsa, "Gurme Cafe & Burger" üzerinden **otomatik olarak resimli, Türkçe-İngilizce ve Arapça dillerinde eksiksiz demo verisi eklenecektir.**

### 5. Uygulamanın Başlatılması
Projeyi Hot Reload (Anında Yenileme) özelliğiyle başlatıp kod yazarken anlık test etmek için API klasöründe şu komutu çalıştırın:
```bash
cd src/QrMenu.API
dotnet watch run --launch-profile http
```

### 6. Test ve Yönetim
- **Müşteri Demo Menüsü:** `http://localhost:5126/m/demo-cafe` adresinden yeni tasarlanan harika "Glassmorphism" menüyü deneyimleyebilirsiniz.
- **Süper Admin Hesabı:** Proje kurulduğunda otomatik olarak bir yönetici hesabı oluşturulur.
  - E-Posta: `admin@qrmenu.com`
  - Şifre: `qrmenu2024`


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
