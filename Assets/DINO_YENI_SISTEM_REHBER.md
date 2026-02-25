# 🦕 Dino Yeni Sistem — Unity Kurulum Rehberi

Bu rehber, **sıralı dino çarpan sistemi** için Unity Editor'da yapmanız gereken TÜM işlemleri içerir.

> **Sistem:** 40 dino, sırayla satın alınır. Her biri kalıcı çarpan verir. Level yok — al ve devam et.
> Sadece **4 template SO** oluşturup ayarlamanız yeterli, kod 40 dinoyu otomatik üretir.

---

## 📋 İÇİNDEKİLER

1. [Save Sil](#adım-1-save-sil)
2. [Eski Objeleri Sil](#adım-2-eski-objeleri-sil)
3. [4 Template SO Oluştur](#adım-3-template-soları-oluştur)
4. [DinoManager Inspector](#adım-4-dinomanager-inspector)
5. [DinoPanel UI Oluştur](#adım-5-dinopanel-ui-oluştur)
6. [DinoPanel Inspector Bağlantıları](#adım-6-dinopanel-inspector-bağlantıları)
7. [Test](#adım-7-test)
8. [Sistem Nasıl Çalışır?](#sistem-nasıl-çalışır)
9. [Sorun Giderme](#sorun-giderme)

---

## ADIM 1: Save Sil

Unity Editor → **Edit → Clear All PlayerPrefs**

---

## ADIM 2: Eski Objeleri Sil

### Hierarchy'den (DinoPanel altındaki child'lar):
- `ShopTabButton` → ❌ Delete
- `AssignTabButton` → ❌ Delete
- `ShopPanel` → ❌ Delete
- `AssignPanel` → ❌ Delete
- `DinoTitle` → ❌ Delete

> ⚠️ **DinoPanel objesinin kendisini SİLMEYİN!**

### Project'ten:
- `Assets/Prefabs/UI/DinoShopItem.prefab` → ❌ Delete
- `Assets/Prefabs/UI/DinoAssignItem.prefab` → ❌ Delete
- `Assets/ScriptableObjects/Dinos/` içindeki eski SO dosyaları (1.asset ... 10.asset) → ❌ Hepsini Delete

---

## ADIM 3: Template SO'ları Oluştur

`Assets/ScriptableObjects/Dinos/` klasöründe **4 adet** SO oluşturun:
Sağ tık → **Create → DinoCore → Dino Data**

### Template_RoomIncome

| Alan | Değer |
|------|-------|
| **Dino Id** | `room_income` |
| **Display Name** | `Madenci Rex` |
| **Description** | `Hedef odanın gelirini artırır` |
| **Dino Type** | `RoomIncome` |
| **Target Room Index** | `0` |
| **Base Cost** | `100` |
| **Cost Exponent** | `1.15` |
| **Base Bonus** | `0.50` |
| **Bonus Per Level** | `0` |
| **Rarity** | `Common` |

### Template_RoomSpeed

| Alan | Değer |
|------|-------|
| **Dino Id** | `room_speed` |
| **Display Name** | `Hızlı Raptor` |
| **Description** | `Hedef odanın üretim hızını artırır` |
| **Dino Type** | `RoomSpeed` |
| **Target Room Index** | `0` |
| **Base Cost** | `100` |
| **Cost Exponent** | `1.15` |
| **Base Bonus** | `0.25` |
| **Bonus Per Level** | `0` |
| **Rarity** | `Common` |

### Template_TechMultiplier

| Alan | Değer |
|------|-------|
| **Dino Id** | `tech_mult` |
| **Display Name** | `Bilgin Stego` |
| **Description** | `Rebirth tech kazancını artırır` |
| **Dino Type** | `TechIncome` |
| **Target Room Index** | `-1` |
| **Base Cost** | `100` |
| **Cost Exponent** | `1.15` |
| **Base Bonus** | `0.03` |
| **Bonus Per Level** | `0` |
| **Rarity** | `Common` |

> **Not:** Base Bonus `0.03` = her satın almada rebirth tech kazancı +3% artar. Çok az çok az artıyor.

### Template_GlobalIncome

| Alan | Değer |
|------|-------|
| **Dino Id** | `global_income` |
| **Display Name** | `Kral Braki` |
| **Description** | `Tüm odaların gelirini artırır` |
| **Dino Type** | `GlobalIncome` |
| **Target Room Index** | `-1` |
| **Base Cost** | `100` |
| **Cost Exponent** | `1.15` |
| **Base Bonus** | `0.15` |
| **Bonus Per Level** | `0` |
| **Rarity** | `Common` |

---

## ADIM 4: DinoManager Inspector

1. Hierarchy → `[GAME_MANAGER]` seçin → Inspector → **DinoManager**
2. Template alanlarını bağlayın:

| Alan | SO |
|------|-----|
| **Room Income Template** | `Template_RoomIncome` |
| **Room Speed Template** | `Template_RoomSpeed` |
| **Tech Multiplier Template** | `Template_TechMultiplier` |
| **Global Income Template** | `Template_GlobalIncome` |

3. Generation Settings (varsayılanlar iyi, isteğe bağlı):

| Alan | Varsayılan | Açıklama |
|------|-----------|----------|
| **Starting Cost** | `1000` | İlk dinonun fiyatı |
| **Cost Multiplier Per Tier** | `3.5` | Her tier'da fiyat 3.5× artar |
| **Bonus Growth Per Tier** | `0.03` | Her tier'da bonus +3% daha fazla |

---

## ADIM 5: DinoPanel UI Oluştur

### 5.1 — DinoPanel Ayarları
1. `DinoPanel` seçin → RectTransform: Stretch All, L/R/T/B: `0`
2. Image Color: `(20, 15, 10, 240)`
3. Script: `DinoPanel` bileşeni ekliyse kontrol edin, yoksa Add Component
4. **Tik kutusu kapalı** (inactive)

### 5.2 — ProgressText
1. `DinoPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `ProgressText`
2. Anchor: Top-Center, Pos: `(0, -30)`, W: `500`, H: `40`
3. TMP: Font 20, Center+Middle, Color: `(200,200,200)`
4. Text: `"Dino: 0/40"`

### 5.3 — CardPanel (büyük kart container)
1. `DinoPanel` sağ tık → **Create Empty** → Adı: `CardPanel`
2. Anchor: **Stretch All**
3. Left: `30`, Right: `30`, Top: `80`, Bottom: `80`
4. Add Component → **Image** → Color: `(40, 35, 30, 220)`
5. Add Component → **Vertical Layout Group**:
   - Padding: L=30, R=30, T=20, B=20
   - Spacing: `12`
   - Child Alignment: `Upper Center`
   - Control Child Size → Width ✓, Height ✗
   - Child Force Expand → Width ✓, Height ✗

### 5.4 — CardPanel İçine Sırayla Ekle

**a) RarityBorder:**
- `CardPanel` sağ tık → **UI → Image** → Adı: `RarityBorder`
- Stretch All + L/R/T/B: `0`
- Color: beyaz Alpha=60
- Layout Element → **Ignore Layout** ✓
- Raycast Target ✗

**b) DinoIcon:**
- `CardPanel` sağ tık → **UI → Image** → Adı: `DinoIcon`
- Layout Element: MinW=`150`, MinH=`150`
- (sprite atadıktan sonra Image → Preserve Aspect ✓)

**c) DinoName:**
- `CardPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `DinoName`
- Layout Element: PrefH=`50`
- TMP: Font `32`, **Bold**, Center+Middle, beyaz
- Text: `"Madenci Rex"`

**d) DinoBonusDesc:**
- `CardPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `DinoBonusDesc`
- Layout Element: PrefH=`40`
- TMP: Font `22`, Center+Middle, Color: `(170,255,170)` açık yeşil
- Text: `"Taş Madeni Gelir Çarpanı +50%"`

**e) DinoRarity:**
- `CardPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `DinoRarity`
- Layout Element: PrefH=`35`
- TMP: Font `18`, Center+Middle, Color: `(180,180,180)` gri
- Text: `"Sıradan"`

**f) DinoCost:**
- `CardPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `DinoCost`
- Layout Element: PrefH=`35`
- TMP: Font `24`, Center+Middle, Color: `(255,215,0)` altın
- Text: `"1.00K"`

**g) BuyButton:**
- `CardPanel` sağ tık → **UI → Button - TextMeshPro** → Adı: `BuyButton`
- Layout Element: PrefH=`65`
- Button Image Color: `(34,170,34)` yeşil
- Altındaki Text → Adını `BuyButtonText` yap
- TMP: Font `22`, **Bold**, Center+Middle, beyaz
- Text: `"Satın Al: 1.00K"`

### 5.5 — AllDonePanel
1. `DinoPanel` sağ tık → **Create Empty** → Adı: `AllDonePanel`
2. Anchor: Middle-Center, W: `500`, H: `200`
3. **Tik kutusu kapalı (inactive)**
4. İçine TMP_Text: Font 28, Bold, Center, altın rengi
5. Text: `"Tüm dinozorlar alındı! 🎉"`

### 5.6 — Son Hierarchy

```
DinoPanel  (script bağlı, tik kapalı)
├── ProgressText
├── CardPanel
│   ├── RarityBorder (Stretch All, Ignore Layout)
│   ├── DinoIcon (150×150)
│   ├── DinoName
│   ├── DinoBonusDesc
│   ├── DinoRarity
│   ├── DinoCost
│   └── BuyButton
│       └── BuyButtonText
└── AllDonePanel (tik kapalı)
    └── Text
```

---

## ADIM 6: DinoPanel Inspector Bağlantıları

`DinoPanel` seç → Inspector → `DinoPanel (Script)`:

| Alan | Obje |
|------|------|
| **Panel** | `DinoPanel` kendisi |
| **Dino Icon** | `CardPanel → DinoIcon` |
| **Dino Name** | `CardPanel → DinoName` |
| **Dino Bonus Desc** | `CardPanel → DinoBonusDesc` |
| **Dino Cost** | `CardPanel → DinoCost` |
| **Dino Rarity** | `CardPanel → DinoRarity` |
| **Rarity Border** | `CardPanel → RarityBorder` |
| **Buy Button** | `CardPanel → BuyButton` |
| **Buy Button Text** | `CardPanel → BuyButton → BuyButtonText` |
| **Progress Text** | `ProgressText` |
| **All Done Panel** | `AllDonePanel` |
| **Card Panel** | `CardPanel` |

---

## ADIM 7: Test

1. ▶ Play moduna girin
2. Alt navigasyondan **Dino** sekmesine tıklayın

### Kontrol Listesi:
- [ ] Büyük kart paneli kaplıyor
- [ ] İlk dino: "Madenci Rex" — "Taş Madeni Gelir Çarpanı +50%"
- [ ] Fiyat: 1.00K
- [ ] Satın al → ikinci dino geldi: "Hızlı Raptor"
- [ ] Tekrar al → "Bilgin Stego" geldi — "Tech Çarpanı +3%"
- [ ] Sonraki: "Kral Braki" — "Genel Gelir Çarpanı +15%"
- [ ] 5. dino: "Madenci Rex II" — fiyat artmış, yine oda gelir çarpanı
- [ ] Level up butonu YOK — sadece satın al
- [ ] İlerleme: "Dino: 4/40" gösteriyor
- [ ] Rebirth yaptığında daha fazla tech kazanıyor (Bilgin Stego etkisi)

---

## Sistem Nasıl Çalışır?

### 4 Tip × 10 Oda = 40 Dino

Her oda tier'ı için 4 dino üretilir:

| # | Dino | Tip | Hedef | Etki |
|---|------|-----|-------|------|
| 1 | Madenci Rex | RoomIncome | Oda 0 | O odanın gelirini çarpanlar |
| 2 | Hızlı Raptor | RoomSpeed | Oda 0 | O odanın üretim hızını artırır |
| 3 | Bilgin Stego | TechIncome | Global | Rebirth tech kazancını artırır |
| 4 | Kral Braki | GlobalIncome | Global | TÜM odaların gelirini çarpanlar |
| 5 | Madenci Rex II | RoomIncome | Oda 1 | 2. odanın gelirini çarpanlar |
| ... | ... | ... | ... | *fiyat ve bonus artarak devam eder* |

### Fiyat Ölçeklemesi
- Başlangıç: **1.000** (1K)
- Oda 1 tier'ı: ~3.500 (3.5K)
- Oda 2 tier'ı: ~12.250
- Sonraki tier'lar üstel olarak büyür

### İsim Formatı
Madenci Rex → Madenci Rex II → Madenci Rex III → ... → Madenci Rex X

### Rarity Dağılımı (otomatik)
Oda 0-1: Common → Oda 2-3: Uncommon → Oda 4-5: Rare → Oda 6-7: Epic → Oda 8-9: Legendary

---

## Sorun Giderme

| Sorun | Çözüm |
|-------|-------|
| DinoPanel açılmıyor | MainNavigation'da Dino tab bağlı mı? |
| 0/0 dino gösteriyor | DinoManager'da 4 template slot dolu mu? |
| Buton çalışmıyor | BuyButton Inspector'da bağlı mı? |
| NullReferenceException | Inspector'daki tüm alanlar dolu mu? |
| Compile error | Save silip Unity restart |

---

## Geçersiz Eski Bölümler (UNITY_SETUP_GUIDE)

| Bölüm | Durum |
|-------|-------|
| 2.3 Dino Data (10 SO) | ❌ Geçersiz — 4 template kullan |
| 6.6.1 Dino Panel | ❌ Geçersiz — bu rehber geçerli |
| 6.6.2 Dino Shop Item | ❌ Silindi |
| 6.6.3 Dino Assign Item | ❌ Silindi |
| Diğer bölümler | ✅ Geçerli |
