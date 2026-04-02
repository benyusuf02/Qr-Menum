using QrMenu.Domain.Entities;
using QrMenu.Infrastructure.Persistence;

namespace QrMenu.Infrastructure.Persistence;

public static class DbInitializer
{
    public static void SeedData(AppDbContext db)
    {
        // Eğer veritabanında hiç Tenant yoksa seed işlemini başlat
        if (db.Tenants.Any())
        {
            return;
        }

        var tenantId = Guid.NewGuid();
        var restaurantId = Guid.NewGuid();

        // 1. Tenant (Müşteri Hesabı)
        var tenant = new Tenant
        {
            Id = tenantId,
            Slug = "demo-cafe",
            Name = "Demo Cafe & Burger",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        db.Tenants.Add(tenant);

        // 2. Restaurant (Restoran Bilgileri / Şube)
        var restaurant = new Restaurant
        {
            Id = restaurantId,
            TenantId = tenantId,
            Name = "The Premium Burger & Cafe",
            BrandColor = "#E11D48", // Rose Red
            IsActive = true
        };
        db.Restaurants.Add(restaurant);

        // 3. Kategoriler
        var catMainsId = Guid.NewGuid();
        var catSidesId = Guid.NewGuid();
        var catDessertsId = Guid.NewGuid();
        var catDrinksId = Guid.NewGuid();

        var categories = new List<Category>
        {
            new Category { Id = catMainsId, RestaurantId = restaurantId, Name = "Ana Yemekler", SortOrder = 1, IsActive = true },
            new Category { Id = catSidesId, RestaurantId = restaurantId, Name = "Atıştırmalıklar", SortOrder = 2, IsActive = true },
            new Category { Id = catDessertsId, RestaurantId = restaurantId, Name = "Tatlılar", SortOrder = 3, IsActive = true },
            new Category { Id = catDrinksId, RestaurantId = restaurantId, Name = "İçecekler", SortOrder = 4, IsActive = true }
        };
        db.Categories.AddRange(categories);

        // 4. Yemekler ve Çevirileri
        var menuItems = new List<MenuItem>();
        var itemTranslations = new List<MenuItemTranslation>();

        void AddItem(Guid catId, string trName, string trDesc, decimal price, string imageUrl, string badges, string enName, string enDesc, string arName, string arDesc, bool isAvail = true)
        {
            var itemId = Guid.NewGuid();
            menuItems.Add(new MenuItem
            {
                Id = itemId,
                CategoryId = catId,
                Name = trName,
                Description = trDesc,
                Price = price,
                ImageUrl = imageUrl,
                Badges = string.IsNullOrEmpty(badges) ? "[]" : System.Text.Json.JsonSerializer.Serialize(badges.Split(',')),
                SortOrder = menuItems.Count,
                IsAvailable = isAvail
            });

            itemTranslations.Add(new MenuItemTranslation { MenuItemId = itemId, LanguageCode = "en", Name = enName, Description = enDesc });
            itemTranslations.Add(new MenuItemTranslation { MenuItemId = itemId, LanguageCode = "ar", Name = arName, Description = arDesc });
        }

        // Ana Yemekler
        AddItem(catMainsId,
            "Signature Truffle Burger", "150gr dana köfte, taze trüf mantarı sosu, karamelize soğan, cheddar", 320,
            "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?q=80&w=600&auto=format&fit=crop", "popular,new",
            "Signature Truffle Burger", "150g beef patty, fresh truffle sauce, caramelized onions, cheddar",
            "برجر ترافل مميز", "شريحة لحم 150 جرام، صلصة ترافل طازجة، بصل مكرمل، شيدر");

        AddItem(catMainsId,
            "Classic Smokey Cheeseburger", "150gr dana köfte, füme et, çifte cheddar peyniri, ev yapımı barbekü sos", 280,
            "https://images.unsplash.com/photo-1550547660-d9450f859349?q=80&w=600&auto=format&fit=crop", "popular",
            "Classic Smokey Cheeseburger", "150g beef patty, smoked meat, double cheddar, homemade BBQ sauce",
            "برجر الجبن المدخن", "شريحة لحم 150 جرام، لحم مدخن، شيدر مزدوج، صلصة باربيكيو منزلية");

        AddItem(catMainsId,
            "Beyond Vegan Burger", "Bitkisel bazlı köfte, vegan peynir, taze yeşillikler, glutensiz ekmek", 350,
            "https://images.unsplash.com/photo-1520072959219-c595dc870360?q=80&w=600&auto=format&fit=crop", "vegan",
            "Beyond Vegan Burger", "Plant-based patty, vegan cheese, fresh greens, gluten-free bun",
            "برجر نباتي بیوند", "شريحة نباتية، جبن نباتي، خضروات طازجة، خبز خالي من الغلوتين");

        AddItem(catMainsId,
            "Crispy Chicken Burger", "Panelenmiş çıtır tavuk, marul, turşu, mayonez", 250,
            "https://images.unsplash.com/photo-1615865487701-44754a1ce24a?q=80&w=600&auto=format&fit=crop", "",
            "Crispy Chicken Burger", "Breaded crispy chicken, lettuce, pickles, mayonnaise",
            "برجر دجاج مقرمش", "دجاج مقرمش بالبقسماط، خس، مخلل، مايونيز");

        // Atıştırmalıklar (Yancılar)
        AddItem(catSidesId,
            "Trüflü ve Parmesanlı Patates", "Taze kesim patates kızartması, trüf yağı ve parmesan peyniri rendesi ile", 140,
            "https://images.unsplash.com/photo-1573080496181-08db3dd7eb57?q=80&w=600&auto=format&fit=crop", "popular",
            "Truffle Parmesan Fries", "Fresh cut fries tossed in truffle oil and freshly grated parmesan",
            "بطاطا مقلية بالكمأة والبارميزان", "بطاطا مقلية طازجة ممزوجة بزيت الكمأة والبارميزان");

        AddItem(catSidesId,
            "Çıtır Soğan Halkaları (6'lı)", "Özel baharatlarla panelenmiş kalın soğan halkaları, ranch sos ile", 110,
            "https://images.unsplash.com/photo-1639024471283-03518883512d?q=80&w=600&auto=format&fit=crop", "",
            "Crispy Onion Rings", "Thick cut onion rings in a special spice blend, served with ranch",
            "حلقات بصل مقرمشة", "حلقات بصل سميكة بتوابل خاصة، تقدم مع صلصة الرانش");

        AddItem(catSidesId,
            "Mac & Cheese Topları", "Krema ve peynir dolgulu, dışı çıtır pane harçlı enfes toplar", 160,
            "https://images.unsplash.com/photo-1543339308-43e59d6b73a6?q=80&w=600&auto=format&fit=crop", "new",
            "Mac & Cheese Bites", "Crispy on the outside, creamy and cheesy on the inside",
            "كرات ماك آند تشيز", "مقرمشة من الخارج، بالكريمة والجبن من الداخل");

        // Tatlılar
        AddItem(catDessertsId,
            "San Sebastian Cheesecake", "Üzeri yanık, içi akışkan kremamsı meşhur İspanyol peynir pastası", 210,
            "https://images.unsplash.com/photo-1692298642999-ad9e1cb46505?q=80&w=600&auto=format&fit=crop", "popular",
            "San Sebastian Cheesecake", "Burnt top, gooey and creamy center famous Spanish cheesecake",
            "تشيز كيك سان سيباستيان", "الجزء العلوي محروق، وسط كريمي، كعكة الجبن الإسبانية الشهيرة");

        AddItem(catDessertsId,
            "Lotus Biscoff Tiramisu", "Klasik tiramisu lezzetinin Lotus bisküvi kreması ile eşsiz uyumu", 240,
            "https://images.unsplash.com/photo-1571115177098-24ec42ed204d?q=80&w=600&auto=format&fit=crop", "new",
            "Lotus Biscoff Tiramisu", "Classic tiramisu flavor perfectly paired with Lotus biscuit cream",
            "تيراميسو لوتس بسكوف", "نكهة التيراميسو الكلاسيكية مقترنة تماماً بكريمة بسكويت اللوتس");
            
        AddItem(catDessertsId,
            "Sıcak Çikolatalı Sufle", "Yanında vanilyalı dondurma ile servis edilen Belçika çikolatalı sufle", 190,
            "https://images.unsplash.com/photo-1615822368943-e4d6a666eec6?q=80&w=600&auto=format&fit=crop", "",
            "Hot Chocolate Souffle", "Belgian chocolate souffle served with vanilla ice cream",
            "سوفليه شوكولاتة ساخن", "سوفليه الشوكولاتة البلجيكية تقدم مع آيس كريم الفانيليا");

        // İçecekler
        AddItem(catDrinksId,
            "El Yapımı Limonata (Orman Meyveli)", "Taze sıkım limon suyu, nane ve mevsim orman meyveleri aroması ile", 95,
            "https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd?q=80&w=600&auto=format&fit=crop", "",
            "Homemade Mixed Berry Lemonade", "Fresh squeezed lemon juice with mint and seasonal mixed berries",
            "عصير ليمون محلي الصنع", "عصير ليمون طازج مع نعناع وتوت غابة موسمي");

        AddItem(catDrinksId,
            "Coca-Cola Şişe (250ml)", "Soğuk cam şişe", 60,
            "https://images.unsplash.com/photo-1554866585-cd94860890b7?q=80&w=600&auto=format&fit=crop", "",
            "Coca-Cola Bottle", "Ice cold glass bottle",
            "كوكاكولا זجاجة", "زجاجة باردة جداً");
            
        AddItem(catDrinksId,
            "Kombucha Çayı (Zencefilli)", "Fermante edilmiş serinletici, sindirim dostu organik kombucha", 120,
            "https://images.unsplash.com/photo-1556881286-fc6915169721?q=80&w=600&auto=format&fit=crop", "vegan",
            "Ginger Kombucha", "Refreshing fermented, digestion-friendly organic kombucha",
            "شاي كومبوتشا الزنجبيل", "مشروب منعش ومفيد للهضم");

        AddItem(catDrinksId,
            "Soğuk Demleme (Cold Brew) Kahve", "24 saat ağır demlenmiş 100% Arabica çekirdeklerinden özel kahve", 110,
            "https://images.unsplash.com/photo-1461023058943-07fcbe16d735?q=80&w=600&auto=format&fit=crop", "",
            "Cold Brew Coffee", "24-hour slow steeped specialty coffee from 100% Arabica beans",
            "قهوة كولد برو", "قهوة منقوعة ببطء لمدة 24 ساعة من حبوب أرابيكا 100٪");

        // Tükendi örneği
        AddItem(catDessertsId,
            "Limitli Sürüm: Altın Yapraklı Profiterol", "Sadece hafta sonlarına özel, yenilebilir altın kaplamalı profiterol", 550,
            "https://images.unsplash.com/photo-1602462615438-bbcd385d3049?q=80&w=600&auto=format&fit=crop", "",
            "Limited Edition: Gold Leaf Profiterole", "Weekend special, gold leaf topped profiterole",
            "إصدار محدود: بروفيتيرول بورق الذهب", "خاص بعطلة نهاية الأسبوع", false);

        db.MenuItems.AddRange(menuItems);
        db.MenuItemTranslations.AddRange(itemTranslations);
        db.SaveChanges();
    }
}
