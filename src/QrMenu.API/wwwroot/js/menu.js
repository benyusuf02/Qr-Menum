(function () {
    const params = new URLSearchParams(location.search);
    const slug = params.get('slug');
    const table = params.get('table');

    if (!slug) {
        document.getElementById('items').innerHTML =
            '<div id="error">Geçersiz menü bağlantısı.</div>';
        return;
    }

    const API_BASE = '/api/menu';
    const LANGS = ['tr', 'en', 'ar'];
    const BADGE_LABELS = {
        popular: { tr: 'Popüler', en: 'Popular', ar: 'الأكثر طلباً' },
        new: { tr: 'Yeni', en: 'New', ar: 'جديد' },
        vegan: { tr: 'Vegan', en: 'Vegan', ar: 'نباتي' },
    };
    const BADGE_CLASS = {
        popular: 'badge-popular',
        new: 'badge-new',
        vegan: 'badge-vegan',
    };

    let menuData = null;
    let currentLang = 'tr';
    let currentCat = null;

    async function load(lang) {
        try {
            const res = await fetch(`${API_BASE}/${slug}?lang=${lang}`);
            if (!res.ok) throw new Error();
            menuData = await res.json();
            currentCat = menuData.categories[0]?.id ?? null;
            renderHeader();
            renderLangSwitcher();
            renderCats();
            renderItems();
        } catch {
            document.getElementById('items').innerHTML =
                '<div id="error">Menü bulunamadı.</div>';
        }
    }

    function renderHeader() {
        document.getElementById('rest-name').textContent = menuData.restaurantName;
        document.getElementById('table-info').textContent = table ? `Masa ${table}` : '';
        document.getElementById('header').style.background = menuData.brandColor || '#0F6E56';
        document.title = menuData.restaurantName;
    }

    function renderLangSwitcher() {
        document.getElementById('lang-switcher').innerHTML = LANGS.map(l =>
            `<button class="lang-btn${l === currentLang ? ' active' : ''}"
        onclick="setLang('${l}')">${l.toUpperCase()}</button>`
        ).join('');
    }

    function renderCats() {
        document.getElementById('cats').innerHTML = menuData.categories.map(c =>
            `<button class="cat-btn${c.id === currentCat ? ' active' : ''}"
        onclick="setCat('${c.id}')">${c.name}</button>`
        ).join('');
    }

    function renderItems() {
        const cat = menuData.categories.find(c => c.id === currentCat);
        if (!cat) return;

        if (cat.items.length === 0) {
            document.getElementById('items').innerHTML =
                '<div id="loading">Bu kategoride ürün yok.</div>';
            return;
        }

        document.getElementById('items').innerHTML = cat.items.map(item => {
            const badges = (item.badges || []).map(b =>
                `<span class="badge ${BADGE_CLASS[b] || ''}">${BADGE_LABELS[b]?.[currentLang] || b}</span>`
            ).join('');

            const img = item.imageUrl
                ? `<img class="item-img" src="${item.imageUrl}" alt="${item.name}" loading="lazy">`
                : `<div class="item-img no-img">🍽</div>`;

            return `
        <div class="item">
          ${img}
          <div style="flex:1;min-width:0">
            <div class="item-name">${item.name}</div>
            ${badges ? `<div class="badges">${badges}</div>` : ''}
            <div class="item-desc">${item.description || ''}</div>
            <div class="item-footer">
              <span class="item-price">₺${Number(item.price).toFixed(2)}</span>
            </div>
          </div>
        </div>`;
        }).join('');
    }

    window.setLang = function (l) {
        currentLang = l;
        load(l);
    };

    window.setCat = function (id) {
        currentCat = id;
        renderCats();
        renderItems();
    };

    load(currentLang);
})();