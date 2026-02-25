# 🎮 DinoCore – Evolution Idle: Unity 6.3 Kurulum ve Kullanım Rehberi

Bu rehber, projeyi sıfırdan Unity'de ayağa kaldırmanız için her adımı açıklar.
Unity 6.3 ve 2D URP projesine özeldir.

---

## 📋 İÇİNDEKİLER

1. [Proje Yapısı](#1-proje-yapısı)
2. [ScriptableObject'leri Oluşturma](#2-scriptableobjectleri-oluşturma)
3. [Sahne Kurulumu](#3-sahne-kurulumu)
4. [Manager GameObject'leri](#4-manager-gameobjectleri)
5. [Room Prefab Oluşturma](#5-room-prefab-oluşturma)
6. [UI Kurulumu](#6-ui-kurulumu)
7. [Prefab Bağlantıları](#7-prefab-bağlantıları)
8. [Görselleri Import Etme](#8-görselleri-import-etme)
9. [Test ve Debug](#9-test-ve-debug)
10. [Android/iOS Build](#10-androidios-build)
11. [Sık Karşılaşılan Hatalar](#11-sık-karşılaşılan-hatalar)
12. [Genişletme Rehberi](#12-genişletme-rehberi)

---

## 1. PROJE YAPISI

Projeniz şu şekilde organize edilmiştir:

```
Assets/
├── Art/
│   ├── Backgrounds/       ← Arka plan görselleri
│   ├── Dinos/             ← Dinozor sprite'ları
│   ├── Effects/           ← Efekt sprite sheet'leri
│   ├── Planets/           ← Gezegen görselleri
│   ├── Rooms/             ← Oda arka planları
│   ├── UI/                ← UI ikonları, butonlar
│   └── AI_ART_PROMPTS.md  ← Görsel üretim promptları
├── Prefabs/
│   ├── Dinos/             ← Dinozor prefab'ları
│   ├── Rooms/             ← Oda prefab'ı
│   └── UI/                ← UI prefab'ları
├── ScriptableObjects/
│   ├── Dinos/             ← DinoData SO dosyaları
│   ├── Rooms/             ← RoomData SO dosyaları
│   └── Tech/              ← TechData SO dosyaları
├── Scripts/
│   ├── Core/              ← Event sistemi
│   ├── Data/              ← ScriptableObject tanımları, SaveData
│   ├── Managers/          ← GameManager, CurrencyManager
│   ├── Systems/           ← Room, Dino, Tech, Rebirth, Planet, Save, Offline, Click
│   ├── UI/                ← Tüm UI scriptleri
│   └── Utils/             ← BigNumberFormatter
└── Scenes/
    └── SampleScene.unity  ← Ana oyun sahnesi
```

---

## 2. SCRIPTABLEOBJECT'LERİ OLUŞTURMA

ScriptableObject'ler oyunun veri tabanıdır. Inspector'dan her şeyi ayarlarsınız.

### 2.1 GameConfig Oluşturma

1. **Project** penceresinde `Assets/ScriptableObjects` klasörüne sağ tıklayın
2. **Create → DinoCore → Game Config** seçin
3. Adını `GameConfig` yapın
4. Inspector'da değerleri ayarlayın:

| Alan | Değer | Açıklama |
|------|-------|----------|
| Base Click Income | 1 | Tıklama başına gelir |
| Click Income Growth Per Level | 0.10 | Her seviye başına artış |
| Rebirth Min Total Income | 1000000 | Rebirth için minimum toplam kazanç |
| Rebirth Currency Formula Divisor | 1000000 | RC hesaplama böleni |
| Rebirth Multiplier Per Currency | 0.05 | Her RC başına çarpan artışı |
| Planet Min Rebirth Count | 5 | Gezegen açmak için min rebirth |
| Planet Multiplier Per Planet | 0.25 | Her gezegen başına çarpan |
| Planet Dino Sacrifice Percent | 0.20 | Feda edilecek dino yüzdesi |
| Auto Collector Base Cost | 500 | İlk auto collector maliyeti |
| Auto Collector Cost Exponent | 2.0 | Maliyet artış üssü |
| Max Offline Hours | 8 | Maksimum offline süre (saat) |
| Offline Efficiency | 0.50 | Offline kazanç verimliliği |
| Auto Save Interval Seconds | 30 | Otomatik kayıt aralığı |
| Min Production Time | 0.5 | Minimum üretim süresi (saniye) |

### 2.2 Room Data'ları Oluşturma

Her oda için bir RoomData SO oluşturun:

1. `Assets/ScriptableObjects/Rooms` klasörüne sağ tıklayın
2. **Create → DinoCore → Room Data** seçin

**10 Oda Verileri:**

| Oda | roomId | displayName | baseIncome | baseCost | unlockCost | baseProductionTime | baseDinoSlotCount | sortOrder |
|-----|--------|-------------|------------|----------|------------|-------------------|-------------------|-----------|
| 1 | room_01 | Taş Madeni   | 2          | 10       | 100        | 5                 | 1                 | 0 |
| 2 | room_02 | Kristal Mağarası | 8      | 50       | 500        | 7                 | 1                 | 1 |
| 3 | room_03 | Fosil Kazı Alanı | 35     | 300      | 2500       | 9                 | 2                 | 2 |
| 4 | room_04 | Magma Odası | 150         | 1500     | 12000      | 10                | 2                 | 3 |
| 5 | room_05 | Buzul Mağarası | 700      | 9000     | 60000      | 12                | 2                 | 4 |
| 6 | room_06 | Mantar Ormanı | 3500      | 50000    | 300000     | 14                | 3                 | 5 |
| 7 | room_07 | Antik Tapınak | 18000     | 300000   | 1500000    | 16                | 3                 | 6 |
| 8 | room_08 | Teknoloji Lab | 90000     | 1800000  | 8000000    | 18                | 3                 | 7 |
| 9 | room_09 | Yeraltı Gölü | 450000     | 10000000 | 40000000   | 20                | 4                 | 8 |
| 10 | room_10 | Elmas Madeni | 2500000   | 60000000 | 200000000  | 22                | 4                 | 9 |

Diğer değerler (tümü için aynı bırakabilirsiniz):
- costExponent: 1.18
- levelMultiplierStep: 0.15
- baseStorageCapacity: 100 (her oda için baseIncome × 100 yapabilirsiniz)
- storageGrowthPerLevel: 0.10
- maxDinoSlots: 5

### 2.3 Dino Data'ları Oluşturma

`Assets/ScriptableObjects/Dinos` klasöründe oluşturun:

| Dino | dinoId      | displayName | dinoType | baseCost | baseBonus | bonusPerLevel | rarity |
|------|--------     |-------------|----------|----------|-----------|---------------|--------|
| 1 | dino_worker_01 | Madenci Rex | Worker   | 50       | 0.10      | 0.02       | Common |
| 2 | dino_worker_02 | Kazıcı Rex  | Worker   | 300      | 0.15      | 0.03       | Uncommon |
| 3 | dino_speed_01 | Hızlı Raptor | Speed    | 100      | 0.08      | 0.015      | Common |
| 4 | dino_speed_02 | Yıldırım Raptor | Speed | 600      | 0.12      | 0.02       | Uncommon |
| 5 | dino_power_01 | Güçlü Trike  | Power    | 200      | 0.05      | 0.01       | Common |
| 6 | dino_power_02 | Yıkıcı Trike | Power    | 1200     | 0.08      | 0.015      | Rare |
| 7 | dino_leader_01 | Kral Braki  | Leader   | 500      | 0.20      | 0.03       | Rare |
| 8 | dino_leader_02| İmparator Braki | Leader | 5000    | 0.30      | 0.05       | Epic |
| 9 | dino_tech_01 | Bilgin Stego  | Tech     | 1000     | 0.10      | 0.02       | Rare |
| 10 | dino_tech_02 | Deha Stego   | Tech     | 10000    | 0.20      | 0.04       | Legendary |

Tüm dinolar için costExponent: 1.15

### 2.4 Tech Data'ları Oluşturma

`Assets/ScriptableObjects/Tech` klasöründe oluşturun:

| Tech | techId | displayName | category | baseCost | bonusPerLevel | maxLevel | costExponent |
|------|--------|-------------|----------|----------|---------------|----------|--------------|
|1|tech_incom   | Gelir Artışı| Income   | 10       | 0.05          | 50          | 1.25 |
|2|tech_speed   | Hız Artışı  | Speed    | 15       | 0.03          | 30          | 1.30 |
|3|tech_storage| Depo Genişletme| Storage| 10       | 0.08          | 40          | 1.20 |
|4|tech_rebirth| Rebirth Güçlendirme| RebirthBoost| 25 | 0.10       | 20          | 1.40 |
|5|tech_click  | Tıklama Gücü | ClickPower     | 20 | 0.05          | 50          | 1.25 |
|6|tech_dino|Dino Verimliliği | DinoEfficiency | 30 | 0.04          | 30          | 1.35 |

### 2.5 Planet Data'ları Oluşturma

`Assets/ScriptableObjects` klasöründe yeni bir `Planets` klasörü oluşturun:

| Gezegen | planetId | displayName | requiredRebirthCount | dinoSacrificePercent | multiplierBonus | sortOrder |
|---------|----------|-------------|---------------------|---------------------|-----------------|-----------|
| 1 | planet_pangea  | Pangea      | 5 | 0.20 | 0.25 | 0 |
| 2 | planet_kryo    | Kryo        | 8 | 0.20 | 0.25 | 1 |
| 3 | planet_ignis   | Ignis       | 12 | 0.25 | 0.30 | 2 |
| 4 | planet_flora   | Flora       | 18 | 0.25 | 0.30 | 3 |
| 5 | planet_nexus   | Nexus       | 25 | 0.30 | 0.35 | 4 |

---

## 3. SAHNE KURULUMU

### 3.1 Sahneyi Açın

1. `Assets/Scenes/SampleScene` sahnesini açın (çift tıklayın)
2. Hierarchy'de mevcut objeleri temizleyin (Main Camera ve Directional Light hariç)

### 3.2 Canvas Oluşturma

1. **Hierarchy** → Sağ tık → **UI → Canvas**
2. Canvas'ı seçin, Inspector'da:
   - **Canvas Scaler** bileşenini bulun
   - **UI Scale Mode**: `Scale With Screen Size` seçin
   - **Reference Resolution**: `1080 x 1920` yazın (mobil dikey)
   - **Screen Match Mode**: `Match Width Or Height`
   - **Match**: `0.5` (slider'ı ortaya getirin)
3. Canvas altında **UI → EventSystem** otomatik oluşur, yoksa oluşturun

### 3.3 Camera Ayarları

1. **Main Camera** seçin
2. Inspector'da:
   - **Clear Flags**: `Solid Color`
   - **Background**: Koyu kahverengi (#3D2817)
   - **Projection**: `Orthographic`

---

## 4. MANAGER GAMEOBJECT'LERİ

Tüm manager'lar tek bir root GameObject altında olacak.

### 4.1 GameManager Oluşturma

1. **Hierarchy** → Sağ tık → **Create Empty**
2. Adını `[GAME_MANAGER]` yapın
3. Pozisyonunu (0, 0, 0) yapın

### 4.2 Manager Bileşenlerini Ekleme

`[GAME_MANAGER]` objesini seçin, **Add Component** ile şu scriptleri ekleyin:

1. **GameManager** (Scripts/Managers/)
2. **CurrencyManager** (Scripts/Managers/)
3. **RoomManager** (Scripts/Systems/)
4. **DinoManager** (Scripts/Systems/)
5. **TechManager** (Scripts/Systems/)
6. **RebirthManager** (Scripts/Systems/)
7. **PlanetManager** (Scripts/Systems/)
8. **SaveManager** (Scripts/Systems/)
9. **ClickManager** (Scripts/Systems/)
10. **OfflineIncomeSystem** (Scripts/Systems/)

### 4.3 Inspector'da Referansları Bağlama

`[GAME_MANAGER]` seçili iken:

**GameManager bileşeni:**
- `Config` → GameConfig SO'yu sürükleyin

**RoomManager bileşeni:**
- `Room Templates` → 10 adet RoomData SO'yu sürükleyin (sortOrder sırasına göre)
- `Room Container` → (6. adımda oluşturacağız)
- `Room Prefab` → (5. adımda oluşturacağız)

**DinoManager bileşeni:**
- `Dino Templates` → 10 adet DinoData SO'yu sürükleyin

**TechManager bileşeni:**
- `Tech Templates` → 6 adet TechData SO'yu sürükleyin

**RebirthManager bileşeni:**
- `Config` → GameConfig SO'yu sürükleyin

**PlanetManager bileşeni:**
- `Planet Templates` → 5 adet PlanetData SO'yu sürükleyin
- `Config` → GameConfig SO'yu sürükleyin

**SaveManager bileşeni:**
- `Config` → GameConfig SO'yu sürükleyin

**ClickManager bileşeni:**
- `Config` → GameConfig SO'yu sürükleyin

**OfflineIncomeSystem bileşeni:**
- `Config` → GameConfig SO'yu sürükleyin

---

## 5. ROOM PREFAB OLUŞTURMA

Her oda bir prefab'dır. Scroll view'da çoğaltılır.
Referans çözünürlük 1080×1920'dir. Tüm koordinatlar buna göredir.

> **ÖNEMLİ KAVRAM – Anchor Preset Seçimi:**
> Inspector'da RectTransform bölümünde sol üstteki kare ikona tıklayın.
> Açılan pencerede:
> - **Stretch Horizontal** = üst satırdaki en sağdaki ikon (yatay uzar)
> - **Stretch All** = sağ alt köşedeki ikon (her yöne uzar)
> - **Top-Left / Top-Right / Center** vb. = ilgili konumdaki ikonlar
>
> **Anchor seçimine göre Inspector'da hangi alanlar görünür:**
>
> | Anchor Tipi | Yatay Alanlar | Dikey Alanlar |
> |-------------|---------------|---------------|
> | Sabit (ör: Top-Left, Center) | Pos X, Width | Pos Y, Height |
> | Stretch Horizontal + Top/Bottom | Left, Right | Pos Y, Height |
> | Stretch Vertical + Left/Right | Pos X, Width | Top, Bottom |
> | Stretch All | Left, Right | Top, Bottom |
>
> Özetle: Hangi eksende "stretch" varsa o eksende Left/Right veya Top/Bottom görünür.
> Hangi eksende sabit anchor varsa o eksende Pos X/Pos Y ve Width/Height görünür.

### 5.1 Room GameObject Oluşturma

1. **Hierarchy** → Canvas altında sağ tık → **Create Empty**
2. Adını `RoomPrefab` yapın

> ⚠️ **Uyarı:** RoomPrefab şu an Canvas'ın altında doğrudan durduğu için
> Game view'da ekranın en tepesinde küçük bir çubuk olarak görünecek.
> **Bu tamamen normal!** Prefab olarak kaydedip Hierarchy'den sildikten sonra
> (Bölüm 5.5), runtime'da ScrollView Content içine Instantiate edilecek
> ve Vertical Layout Group otomatik olarak düzgün konumlandıracak.
3. Inspector → RectTransform → Sol üst kare ikona tıklayın → **Stretch Horizontal, Top** seçin
   (üst satır, en sağdaki yatay uzayan ikon)
4. Değerleri girin:
   - **Left**: `0`
   - **Right**: `0`
   - **Pos Y**: `0` (Layout Group bu değeri otomatik yönetir, bu yüzden 0 bırakın)
   - **Height**: `300`
   > **Not:** "Stretch Horizontal + Top" seçildiğinde dikey eksen için
   > Inspector'da "Top" yerine "Pos Y" alanı görünür. Bu normaldir.
   > Bu prefab, Scroll View içindeki Vertical Layout Group tarafından
   > konumlandırılacağı için Pos Y değeri otomatik override edilir.
5. **Add Component** → **Layout Element**
   - **Preferred Height**: `300` (Scroll View layout'un doğru çalışması için)

### 5.2 Room Prefab'ın İç Yapısı – LockedPanel

`RoomPrefab` seçili → sağ tık → **UI → Panel** → Adı: `LockedPanel`

**LockedPanel RectTransform:**
- Anchor: **Stretch All** (sağ alt köşedeki ikon)
- Left: `0`, Right: `0`, Top: `0`, Bottom: `0`
- (Yani parent'ın tamamını kaplar)
- **Image** bileşeni → Color: `(80, 80, 80, 200)` (koyu gri yarı-saydam)

`LockedPanel` içine:

**a) UnlockCostText:**
1. `LockedPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `UnlockCostText`
2. Anchor: **Middle-Center** (ortadaki ikon)
3. Pos X: `0`, Pos Y: `30`
4. Width: `400`, Height: `60`
5. TMP ayarları:
   - Font Size: `32`
   - Alignment: **Center + Middle** (orta hizalı)
   - Color: beyaz
   - Text: `"100"` (placeholder)

**b) UnlockButton:**
1. `LockedPanel` sağ tık → **UI → Button - TextMeshPro** → Adı: `UnlockButton`
2. Anchor: **Middle-Center**
3. Pos X: `0`, Pos Y: `-40`
4. Width: `250`, Height: `70`
5. Button → Image Color: `(76, 175, 80, 255)` (yeşil)
6. Altındaki **Text (TMP)** objesini seçin:
   - Text: `"Aç"`
   - Font Size: `28`
   - Alignment: Center + Middle
   - Color: beyaz

### 5.3 Room Prefab'ın İç Yapısı – UnlockedPanel

`RoomPrefab` seçili → sağ tık → **UI → Panel** → Adı: `UnlockedPanel`

**UnlockedPanel RectTransform:**
- Anchor: **Stretch All**
- Left: `0`, Right: `0`, Top: `0`, Bottom: `0`
- **Image** bileşeni → Color: `(50, 40, 30, 220)` (koyu kahverengi yarı-saydam)

`UnlockedPanel` içine sırasıyla şunları ekleyin:

```
┌──────────────────────────────────────────────────────────┐
│ [RoomIcon]  RoomNameText              LevelText          │  ← Üst satır
│             IncomeText           ProductionTimeText      │  ← Orta satır
│             ██████ProgressBar██████   DinoSlotText       │  ← Progress bar
│             StoredText                                   │  ← Stored bilgi
│ [CollectBtn] [UpgradeBtn]         [AutoCollectorBtn]     │  ← Alt butonlar
└──────────────────────────────────────────────────────────┘
```

**a) RoomIcon:**
1. `UnlockedPanel` sağ tık → **UI → Image** → Adı: `RoomIcon`
2. Anchor: **Top-Left** (sol üst ikon)
3. Pivot: `(0.5, 0.5)`
4. Pos X: `50`, Pos Y: `-50`
5. Width: `80`, Height: `80`
6. Image → Color: beyaz (sprite atanınca görünür)

**b) RoomNameText:**
1. `UnlockedPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `RoomNameText`
2. Anchor: **Top-Left**
3. Pos X: `210`, Pos Y: `-20`
4. Width: `350`, Height: `40`
5. TMP: Font Size `26`, Alignment: **Left + Middle**, Color: beyaz, Bold
6. Text: `"Taş Madeni"` (placeholder)

**c) LevelText:**
1. `UnlockedPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `LevelText`
2. Anchor: **Top-Right** (sağ üst ikon)
3. Pos X: `-80`, Pos Y: `-20`
4. Width: `150`, Height: `40`
5. TMP: Font Size `22`, Alignment: **Right + Middle**, Color: `(255, 215, 0)` (altın sarı)
6. Text: `"Lv. 1"` (placeholder)

**d) IncomeText:**
1. `UnlockedPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `IncomeText`
2. Anchor: **Top-Left**
3. Pos X: `210`, Pos Y: `-60`
4. Width: `300`, Height: `35`
5. TMP: Font Size `20`, Alignment: **Left + Middle**, Color: `(144, 238, 144)` (açık yeşil)
6. Text: `"1.00/cycle"` (placeholder)

**e) ProductionTimeText:**
1. `UnlockedPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `ProductionTimeText`
2. Anchor: **Top-Right**
3. Pos X: `-80`, Pos Y: `-60`
4. Width: `150`, Height: `35`
5. TMP: Font Size `18`, Alignment: **Right + Middle**, Color: `(173, 216, 230)` (açık mavi)
6. Text: `"10s"` (placeholder)

**f) ProgressBar:**
1. `UnlockedPanel` sağ tık → **UI → Image** → Adı: `ProgressBarBG`
2. Anchor: **Top-Left**
3. Pos X: `310`, Pos Y: `-110`
4. Width: `450`, Height: `20`
5. Image → Color: `(60, 60, 60, 255)` (koyu gri arka plan)
6. `ProgressBarBG` sağ tık → **UI → Image** → Adı: `ProgressBar`
7. Anchor: **Stretch All** (parent'ın tamamını kapla)
8. Left: `0`, Right: `0`, Top: `0`, Bottom: `0`
9. Image bileşeninde:
   - **Source Image** alanının sağındaki küçük **⊙** (daire) ikona tıklayın
   - Açılan pencede arama kutusuna `Background` yazın
   - **Background** sprite'ını seçin (Unity'nin built-in beyaz kare sprite'ı)
   > **Not:** `Image Type` alanı ancak bir Source Image atandığında görünür.
   > Sprite olmadan bu alan gizlidir.
10. Şimdi görünen **Image Type** alanını `Filled` yapın
11. **Fill Method**: `Horizontal`
12. **Fill Origin**: `Left`
13. Image → Color: `(76, 175, 80, 255)` (yeşil)

**g) DinoSlotText:**
1. `UnlockedPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `DinoSlotText`
2. Anchor: **Top-Right**
3. Pos X: `-40`, Pos Y: `-110`
4. Width: `80`, Height: `30`
5. TMP: Font Size `18`, Alignment: **Right + Middle**, Color: `(255, 165, 0)` (turuncu)
6. Text: `"0/1"` (placeholder)

**h) StoredText:**
1. `UnlockedPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `StoredText`
2. Anchor: **Top-Left**
3. Pos X: `210`, Pos Y: `-145`
4. Width: `400`, Height: `30`
5. TMP: Font Size `18`, Alignment: **Left + Middle**, Color: `(200, 200, 200)` (açık gri)
6. Text: `"0 / 100"` (placeholder)

**i) CollectButton:**
1. `UnlockedPanel` sağ tık → **UI → Button - TextMeshPro** → Adı: `CollectButton`
2. Anchor: **Bottom-Left** (sol alt ikon)
3. Pos X: `100`, Pos Y: `40`
4. Width: `160`, Height: `55`
5. Button Image Color: `(255, 193, 7, 255)` (altın sarı)
6. Altındaki Text (TMP): Text `"Topla"`, Font Size `20`, Color: siyah, Bold

**j) UpgradeButton:**
1. `UnlockedPanel` sağ tık → **UI → Button - TextMeshPro** → Adı: `UpgradeButton`
2. Anchor: **Bottom-Center** (alt orta ikon)
3. Pos X: `0`, Pos Y: `40`
4. Width: `200`, Height: `55`
5. Button Image Color: `(33, 150, 243, 255)` (mavi)
6. Altındaki Text (TMP): Silin veya boşaltın
7. `UpgradeButton` sağ tık → **UI → Text - TextMeshPro** → Adı: `UpgradeCostText`
   - Anchor: **Stretch All**, Left/Right/Top/Bottom: `5`
   - TMP: Font Size `16`, Alignment: **Center + Middle**, Color: beyaz
   - Text: `"⬆ 10"` (placeholder)

**k) AutoCollectorButton:**
1. `UnlockedPanel` sağ tık → **UI → Button - TextMeshPro** → Adı: `AutoCollectorButton`
2. Anchor: **Bottom-Right** (sağ alt ikon)
3. Pos X: `-100`, Pos Y: `40`
4. Width: `160`, Height: `55`
5. Button Image Color: `(156, 39, 176, 255)` (mor)
6. Altındaki Text (TMP): Silin veya boşaltın
7. `AutoCollectorButton` sağ tık → **UI → Text - TextMeshPro** → Adı: `AutoCollectorCostText`
   - Anchor: **Stretch All**, Left/Right/Top/Bottom: `5`
   - TMP: Font Size `14`, Alignment: **Center + Middle**, Color: beyaz
   - Text: `"🤖 500"` (placeholder)

### 5.4 Script Ekleme

1. `RoomPrefab` objesini Hierarchy'de seçin
2. **Add Component** → `Room` yazın → **Room** (DinoCore.Systems) ekleyin
3. **Add Component** → `RoomPanel` yazın → **RoomPanel** (DinoCore.UI) ekleyin
4. **RoomPanel** Inspector'ında referansları sürükle-bırak ile bağlayın:

| RoomPanel Alanı | Sürüklenecek Obje |
|----------------|-------------------|
| Room | Aynı objedeki Room bileşeni (RoomPrefab üzerinde) |
| Room Name Text | `UnlockedPanel/RoomNameText` |
| Level Text | `UnlockedPanel/LevelText` |
| Income Text | `UnlockedPanel/IncomeText` |
| Stored Text | `UnlockedPanel/StoredText` |
| Upgrade Cost Text | `UnlockedPanel/UpgradeButton/UpgradeCostText` |
| Production Time Text | `UnlockedPanel/ProductionTimeText` |
| Progress Bar | `UnlockedPanel/ProgressBarBG/ProgressBar` |
| Room Icon | `UnlockedPanel/RoomIcon` |
| Collect Button | `UnlockedPanel/CollectButton` |
| Upgrade Button | `UnlockedPanel/UpgradeButton` |
| Auto Collector Button | `UnlockedPanel/AutoCollectorButton` |
| Unlock Button | `LockedPanel/UnlockButton` |
| Locked Panel | `LockedPanel` |
| Unlocked Panel | `UnlockedPanel` |
| Unlock Cost Text | `LockedPanel/UnlockCostText` |
| Auto Collector Cost Text | `UnlockedPanel/AutoCollectorButton/AutoCollectorCostText` |
| Dino Slot Text | `UnlockedPanel/DinoSlotText` |

### 5.5 Prefab Kaydetme

1. `RoomPrefab` objesini **Hierarchy**'den sürükleyip `Assets/Prefabs/Rooms/` klasörüne bırakın
   - "Original Prefab" seçeneğini seçin
2. **Hierarchy**'deki orijinalini silin (sağ tık → Delete)
3. `[GAME_MANAGER]` seçin → **RoomManager** bileşeni → `Room Prefab` alanına
   `Assets/Prefabs/Rooms/RoomPrefab` prefab'ını sürükleyin

---

## 6. UI KURULUMU

> **📌 Kaldığınız yer burası (Bölüm 6)**
> Aşağıda her eleman için tam Anchor, Pos X, Pos Y, Width, Height, renk ve font değerleri var.

### 6.1 Scroll View (Oda Listesi)

1. **Canvas** seçili → sağ tık → **UI → Scroll View**
2. Adını `RoomScrollView` yapın
3. RectTransform → Anchor Preset: **Stretch All** (sağ alt köşe ikonu)
4. Değerleri girin:
   - **Left**: `0`
   - **Right**: `0`
   - **Top**: `450` (üst HUD + Click Button için boşluk bırakır)
   - **Bottom**: `140` (alt navigasyon için boşluk bırakır)
5. **Scroll Rect** bileşeninde:
   - **Horizontal** kutusunun tikini kaldırın ✗
   - **Vertical** tikli kalsın ✓
   - **Movement Type**: `Elastic`
   - **Scrollbar Horizontal** alanını boşaltın (sağ tıklayıp None yapın)
6. Hierarchy'de `RoomScrollView → Viewport → Content` objesini bulun
7. `Content` seçin, Inspector'da:
   - **Add Component** → `Vertical Layout Group` arayın, ekleyin:
     - **Padding**: Left `10`, Right `10`, Top `10`, Bottom `10`
     - **Spacing**: `15`
     - **Child Alignment**: `Upper Center`
     - **Control Child Size** → Width ✓, Height ✗
     - **Use Child Scale** → Width ✗, Height ✗
     - **Child Force Expand** → Width ✓, Height ✓
   - **Add Component** → `Content Size Fitter` arayın, ekleyin:
     - **Horizontal Fit**: `Unconstrained`
     - **Vertical Fit**: `Preferred Size`
8. `[GAME_MANAGER]` seçin → **RoomManager** → `Room Container` alanına bu `Content` objesini sürükleyin

### 6.2 Üst HUD (Para Göstergesi)

```
┌────────────────────────────────────────────────────────┐
│  🪙 1.50K                              💎 0   🔮 0    │  ← Üst satır
│            +2.30/s                                     │  ← Alt satır
└────────────────────────────────────────────────────────┘
```

1. **Canvas** sağ tık → **UI → Panel** → Adı: `TopHUD`
2. Anchor: **Stretch Horizontal, Top** (üst satır, yatay uzayan ikon)
3. Değerler:
   - **Left**: `0`, **Right**: `0`, **Top**: `0`
   - **Height**: `180`
4. **Image** bileşeni → Color: `(30, 25, 20, 230)` (koyu kahverengi yarı-saydam)

`TopHUD` içine sırasıyla:

**a) MoneyIcon:**
1. `TopHUD` sağ tık → **UI → Image** → Adı: `MoneyIcon`
2. Anchor: **Top-Left**
3. Pos X: `40`, Pos Y: `-45`
4. Width: `48`, Height: `48`
5. Image → Color: `(255, 215, 0)` (altın, sprite atanana kadar)

**b) MoneyText:**
1. `TopHUD` sağ tık → **UI → Text - TextMeshPro** → Adı: `MoneyText`
2. Anchor: **Top-Left**
3. Pos X: `230`, Pos Y: `-45`
4. Width: `300`, Height: `50`
5. TMP: Font Size `38`, Alignment: **Left + Middle**, Color: `(255, 215, 0)` (altın sarı), **Bold**
6. Text: `"0"` (placeholder)

**c) IncomePerSecondText:**
1. `TopHUD` sağ tık → **UI → Text - TextMeshPro** → Adı: `IncomePerSecondText`
2. Anchor: **Top-Left**
3. Pos X: `230`, Pos Y: `-95`
4. Width: `300`, Height: `35`
5. TMP: Font Size `20`, Alignment: **Left + Middle**, Color: `(144, 238, 144)` (açık yeşil)
6. Text: `"+0/s"` (placeholder)

**d) TechIcon:**
1. `TopHUD` sağ tık → **UI → Image** → Adı: `TechIcon`
2. Anchor: **Top-Right**
3. Pos X: `-250`, Pos Y: `-45`
4. Width: `32`, Height: `32`
5. Image → Color: `(0, 188, 212)` (teal)

**e) TechCurrencyText:**
1. `TopHUD` sağ tık → **UI → Text - TextMeshPro** → Adı: `TechCurrencyText`
2. Anchor: **Top-Right**
3. Pos X: `-170`, Pos Y: `-45`
4. Width: `120`, Height: `35`
5. TMP: Font Size `22`, Alignment: **Left + Middle**, Color: `(0, 188, 212)` (teal)
6. Text: `"0"` (placeholder)

**f) RebirthIcon:**
1. `TopHUD` sağ tık → **UI → Image** → Adı: `RebirthIcon`
2. Anchor: **Top-Right**
3. Pos X: `-100`, Pos Y: `-45`
4. Width: `32`, Height: `32`
5. Image → Color: `(156, 39, 176)` (mor)

**g) RebirthCurrencyText:**
1. `TopHUD` sağ tık → **UI → Text - TextMeshPro** → Adı: `RebirthCurrencyText`
2. Anchor: **Top-Right**
3. Pos X: `-40`, Pos Y: `-45`
4. Width: `80`, Height: `35`
5. TMP: Font Size `22`, Alignment: **Left + Middle**, Color: `(186, 104, 200)` (açık mor)
6. Text: `"0"` (placeholder)

**h) Script bağlama:**
1. `TopHUD` seçin → **Add Component** → `CurrencyHUD` ekleyin
2. Inspector'da referansları sürükleyin:
   - Money Text → `MoneyText`
   - Income Per Second Text → `IncomePerSecondText`
   - Tech Currency Text → `TechCurrencyText`
   - Rebirth Currency Text → `RebirthCurrencyText`

### 6.3 Click Butonu

1. **Canvas** sağ tık → **UI → Button - TextMeshPro** → Adı: `ClickButton`
2. Anchor: **Top-Center** (üst orta ikon)
3. Pos X: `0`, Pos Y: `-330`
   (TopHUD'un hemen altında, ScrollView'ın biraz üstünde)
4. Width: `200`, Height: `200`
5. Button → **Image** → Color: `(255, 152, 0, 255)` (turuncu)
6. (İsteğe bağlı) Image → sprite olarak daire resmi atayabilirsiniz

**ClickButton içindeki Text:**
1. Button oluşturulunca altında otomatik bir `Text (TMP)` oluşur
2. Onu silin (sağ tık → Delete)
3. `ClickButton` sağ tık → **UI → Text - TextMeshPro** → Adı: `ClickIncomeText`
4. Anchor: **Stretch All**, Left/Right/Top/Bottom: `10`
5. TMP: Font Size `24`, Alignment: **Center + Middle**, Color: beyaz, Bold
6. Text: `"+1"` (placeholder)

**FeedbackParent (floating text konteyneri):**
1. `ClickButton` sağ tık → **Create Empty** → Adı: `FeedbackParent`
2. RectTransform → Anchor: **Middle-Center**
3. Pos X: `0`, Pos Y: `50` (butonun biraz üstünde)
4. Width: `200`, Height: `100`

**Script bağlama:**
1. `ClickButton` seçin → **Add Component** → `ClickButton` (DinoCore.UI) ekleyin
2. Inspector:
   - Button → `ClickButton` objesindeki Button bileşeni
   - Click Income Text → `ClickIncomeText`
   - Feedback Parent → `FeedbackParent`
   - Floating Text Prefab → (6.4'te oluşturacağız, şimdilik boş bırakın)

### 6.4 Floating Text Prefab

1. **Canvas** sağ tık → **UI → Text - TextMeshPro** → Adı: `FloatingTextPrefab`
2. Anchor: **Middle-Center**
3. Pos X: `0`, Pos Y: `0`
4. Width: `200`, Height: `50`
5. TMP: Font Size `28`, Alignment: **Center + Middle**, Color: `(255, 215, 0)` (altın sarı), Bold
6. Text: `"+1"` (placeholder)
7. `FloatingTextPrefab` seçin → **Add Component** → `FloatingText` (DinoCore.UI) ekleyin
   - Move Speed: `100`
   - Fade Speed: `2`
   - Lifetime: `1`
8. `FloatingTextPrefab` objesini **Hierarchy'den** sürükleyip `Assets/Prefabs/UI/` klasörüne bırakın
   - "Original Prefab" seçin
9. **Hierarchy'deki orijinalini silin** (sağ tık → Delete)
10. Şimdi `ClickButton` objesini seçin → **ClickButton** script'inde:
    - `Floating Text Prefab` → `Assets/Prefabs/UI/FloatingTextPrefab` prefab'ını sürükleyin

### 6.5 Alt Navigasyon Çubuğu

```
┌──────────────────────────────────────────────────────────┐
│  [Maden]  [Dino]  [Teknoloji]  [Evrim]  [Gezegen]       │
└──────────────────────────────────────────────────────────┘
```

1. **Canvas** sağ tık → **UI → Panel** → Adı: `BottomNav`
2. Anchor: **Stretch Horizontal, Bottom** (alt satır, yatay uzayan ikon)
3. Değerler:
   - **Left**: `0`, **Right**: `0`, **Bottom**: `0`
   - **Height**: `130`
4. **Image** → Color: `(30, 25, 20, 240)` (koyu kahverengi)
5. **Add Component** → `Horizontal Layout Group`:
   - **Padding**: Left `10`, Right `10`, Top `10`, Bottom `10`
   - **Spacing**: `8`
   - **Child Alignment**: `Middle Center`
   - **Control Child Size** → Width ✓, Height ✓
   - **Child Force Expand** → Width ✓, Height ✗

6. `BottomNav` içine 5 adet buton ekleyin (her biri için `BottomNav` sağ tık → **UI → Button - TextMeshPro**):

| Sıra | Obje Adı | Buton Metni | Button Image Color (RGBA) |
|------|----------|-------------|---------------------------|
| 1 | `MineTabButton` | Maden | `(121, 85, 72, 255)` kahverengi |
| 2 | `DinoTabButton` | Dino | `(76, 175, 80, 255)` yeşil |
| 3 | `TechTabButton` | Teknoloji | `(0, 188, 212, 255)` teal |
| 4 | `RebirthTabButton` | Evrim | `(156, 39, 176, 255)` mor |
| 5 | `PlanetTabButton` | Gezegen | `(33, 150, 243, 255)` mavi |

Her butonun altındaki **Text (TMP)** objesinde:
- Font Size: `18`
- Alignment: **Center + Middle**
- Color: beyaz

7. **MainNavigation script'ini henüz eklemeyin** — paneller oluşturulduktan sonra 6.8'de ekleyeceğiz.

### 6.6 Panel'ler (Dino, Tech, Rebirth, Planet)

Her panel Canvas'ın tam üstünü kaplar. Bir tab'a basınca açılır, başka tab'a basınca kapanır.

---

#### 6.6.1 Dino Panel

1. **Canvas** sağ tık → **UI → Panel** → Adı: `DinoPanel`
2. Anchor: **Stretch All**
3. Left: `0`, Right: `0`, Top: `0`, Bottom: `0`
4. Image → Color: `(20, 15, 10, 240)` (koyu opak arka plan)

`DinoPanel` içine:

**Başlık:**
1. `DinoPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `DinoTitle`
2. Anchor: **Top-Center**
3. Pos X: `0`, Pos Y: `-40`
4. Width: `400`, Height: `60`
5. TMP: Font Size `36`, Center + Middle, Color: beyaz, Bold
6. Text: `"Dinozorlar"`

**Tab Butonları:**
1. `DinoPanel` sağ tık → **UI → Button - TextMeshPro** → Adı: `ShopTabButton`
   - Anchor: **Top-Left**, Pos X: `200`, Pos Y: `-110`
   - Width: `250`, Height: `60`
   - Button Color: `(76, 175, 80, 255)` (yeşil)
   - Text: `"Mağaza"`, Font Size `22`, beyaz

2. `DinoPanel` sağ tık → **UI → Button - TextMeshPro** → Adı: `AssignTabButton`
   - Anchor: **Top-Right**, Pos X: `-200`, Pos Y: `-110`
   - Width: `250`, Height: `60`
   - Button Color: `(33, 150, 243, 255)` (mavi)
   - Text: `"Ata"`, Font Size `22`, beyaz

**ShopPanel (Scroll View):**
1. `DinoPanel` sağ tık → **UI → Scroll View** → Adı: `ShopPanel`
2. Anchor: **Stretch All**
3. Left: `20`, Right: `20`, Top: `180`, Bottom: `160`
4. Scroll Rect: Horizontal ✗, Vertical ✓
5. İçindeki `Content` objesine:
   - **Vertical Layout Group**: Spacing `10`, Child Force Expand Width ✓, Height ✗
   - **Content Size Fitter**: Vertical Fit `Preferred Size`

**AssignPanel (Scroll View):**
1. `DinoPanel` sağ tık → **UI → Scroll View** → Adı: `AssignPanel`
2. (ShopPanel ile aynı RectTransform ayarları)
3. Anchor: **Stretch All**, Left: `20`, Right: `20`, Top: `180`, Bottom: `160`
4. Aynı Content ayarları

**DinoPanel Script:**
1. `DinoPanel` seçin → **Add Component** → `DinoPanel` (DinoCore.UI)
2. Inspector:
   - Panel → `DinoPanel` (kendisi)
   - Shop Content → `ShopPanel/Viewport/Content`
   - Assign Content → `AssignPanel/Viewport/Content`
   - Shop Tab Button → `ShopTabButton`
   - Assign Tab Button → `AssignTabButton`
   - Shop Panel → `ShopPanel`
   - Assign Panel → `AssignPanel`
   - Dino Shop Item Prefab → (6.6.2'de oluşturacağız)
   - Dino Assign Item Prefab → (6.6.3'te oluşturacağız)

3. `DinoPanel` objesini seçin → Inspector en üstte ismin solundaki **tik kutusunu kaldırın** ✗
   (Bu, başlangıçta SetActive(false) yapar)

---

#### 6.6.2 Dino Shop Item Prefab Burayı bir sor burda kaldın sayılır

```
┌──────────────────────────────────────────────────────────┐
│ [Icon]  Madenci Rex    Worker    x0     50      [Satın Al]│
└──────────────────────────────────────────────────────────┘
```

1. Geçici olarak **Canvas** sağ tık → **Create Empty** → Adı: `DinoShopItem`
2. RectTransform: Anchor **Stretch Horizontal, Top**, Height: `80`, Left: `0`, Right: `0`
3. **Add Component** → **Layout Element** → Preferred Height: `80`
4. **Add Component** → **Horizontal Layout Group**:
   - Padding: Left `10`, Right `10`, Top `5`, Bottom `5`
   - Spacing: `15`
   - Child Alignment: Middle Left
   - Control Child Size → Width ✗, Height ✓
   - Child Force Expand → Width ✗, Height ✗
5. Image bileşeni ekleyin → Color: `(60, 50, 40, 200)`

İçine sırasıyla:

| Sıra | Tür | Obje Adı | Width | Height | Font Size | Alignment | Renk |
|------|-----|----------|-------|--------|-----------|-----------|------|
| 1 | Image  | `Icon`   | 60    | 60     | -         | -         | beyaz |
| 2 |TMP Text| `NameText`| 180  | 40     | 20        | Left+Mid  | beyaz |
| 3 |TMP Text| `TypeText`| 100  | 30     | 16        | Left+Mid  | `(180,180,180)` gri |
| 4 |TMP Text| `CountText`| 50  | 30     | 18        | Center+Mid| `(255,215,0)` altın |
| 5 |TMP Text| `CostText`| 100  | 30     | 18        | Right+Mid | `(255,152,0)` turuncu |
| 6 | Button | `BuyButton`| 120 | 50     | 18        | Center+Mid| `(76,175,80)` yeşil, Text: "Satın Al" |

Her objeyi **Add Component** → **Layout Element** ile Width override yapın (Min Width = tablodaki Width).

6. Prefab yapın: `DinoShopItem` objesini `Assets/Prefabs/UI/` klasörüne sürükleyin
7. Hierarchy'deki orijinalini silin
8. `DinoPanel` script → `Dino Shop Item Prefab` → bu prefab'ı sürükleyin

---

#### 6.6.3 Dino Assign Item Prefab

```
┌──────────────────────────────────────────────────┐
│  Madenci Rex Lv.1       +10%          [Ata]      │
└──────────────────────────────────────────────────┘
```

1. **Canvas** sağ tık → **Create Empty** → Adı: `DinoAssignItem`
2. RectTransform: Stretch Horizontal + Top, Height: `70`, Left: `0`, Right: `0`
3. **Layout Element** → Preferred Height: `70`
4. **Horizontal Layout Group**: Padding 10, Spacing 15
5. Image → Color: `(50, 60, 50, 200)`

İçine:

| Sıra | Tür | Obje Adı | Width | Height | Font | Alignment | Renk |
|------|-----|----------|-------|--------|------|-----------|------|
| 1 | TMP Text | `NameText` | 300 | 40 | 20 | Left+Mid | beyaz |
| 2 | TMP Text | `BonusText` | 100 | 30 | 18 | Right+Mid | `(144,238,144)` yeşil |
| 3 | Button | `AssignButton` | 100 | 45 | 18 | Center+Mid | `(33,150,243)` mavi, Text: "Ata" |

6. Prefab yapın → `Assets/Prefabs/UI/DinoAssignItem`
7. Orijinalini silin
8. `DinoPanel` script → `Dino Assign Item Prefab` → bu prefab

---

#### 6.6.4 Tech Panel

1. **Canvas** sağ tık → **UI → Panel** → Adı: `TechPanel`
2. Anchor: **Stretch All**, Left/Right/Top/Bottom: `0`
3. Image → Color: `(15, 25, 35, 240)` (koyu lacivert)

İçine:

**Başlık:**
- TMP Text, Adı: `TechTitle`, Anchor Top-Center
- Pos X: `0`, Pos Y: `-40`, Width: `400`, Height: `60`
- Font Size `36`, Center, beyaz, Bold, Text: `"Teknoloji"`

**Scroll View:**
1. `TechPanel` sağ tık → **UI → Scroll View** → Adı: `TechScrollView`
2. Anchor: **Stretch All**, Left: `20`, Right: `20`, Top: `120`, Bottom: `160`
3. Horizontal ✗, Vertical ✓
4. Content: Vertical Layout Group (Spacing 10) + Content Size Fitter (Preferred Size)

**TechPanel Script:**
1. `TechPanel` seçin → **Add Component** → `TechPanel` (DinoCore.UI)
2. Inspector:
   - Panel → `TechPanel`
   - Content Parent → `TechScrollView/Viewport/Content`
   - Tech Item Prefab → (aşağıda oluşturacağız)
3. Tik kutusunu kaldırın (başlangıçta kapalı)

---

#### 6.6.5 Tech Item Prefab

```
┌──────────────────────────────────────────────────────────┐
│ [Icon]  Gelir Artışı    Lv. 0    +0%    10   [Yükselt]  │
└──────────────────────────────────────────────────────────┘
```

1. **Canvas** sağ tık → **Create Empty** → Adı: `TechItem`
2. Stretch Horizontal + Top, Height: `80`
3. Layout Element → Preferred Height: `80`
4. Horizontal Layout Group: Padding 10, Spacing 12
5. Image → Color: `(40, 55, 70, 220)`

İçine:

| Sıra | Tür | Obje Adı | Width | Font | Alignment | Renk |
|------|-----|----------|-------|------|-----------|------|
| 1 | Image | `Icon` | 50×50 | - | - | beyaz |
| 2 | TMP | `NameText` | 180 | 18 | Left+Mid | beyaz |
| 3 | TMP | `LevelText` | 80 | 18 | Center+Mid | `(255,215,0)` altın |
| 4 | TMP | `BonusText` | 80 | 16 | Center+Mid | `(144,238,144)` yeşil |
| 5 | TMP | `CostText` | 80 | 16 | Right+Mid | `(0,188,212)` teal |
| 6 | Button | `UpgradeButton` | 110×50 | 16 | Center+Mid | `(0,150,136)` koyu teal, Text: "Yükselt" |

6. `TechItem` objesine **Add Component** → `TechItemUI` (DinoCore.UI)
7. Inspector'da tüm referansları bağlayın (NameText, LevelText, CostText, BonusText, UpgradeButton, Icon)
8. Prefab yapın → `Assets/Prefabs/UI/TechItem`
9. Orijinalini silin
10. `TechPanel` script → `Tech Item Prefab` → bu prefab

---

#### 6.6.6 Rebirth Panel

> **Bu panelde toplam 7 obje oluşturacaksınız:**
> - 5 adet Text (1 başlık + 4 bilgi metni)
> - 2 adet Button (Evrimleş + İptal)

**Ana Panel:**
1. **Canvas** sağ tık → **UI → Panel** → Adı: `RebirthPanel`
2. Anchor: **Stretch All**, Left/Right/Top/Bottom: `0`
3. Image → Color: `(35, 15, 40, 240)` (koyu mor)

---

**Obje 1 — Başlık (sadece görsel, script'e bağlanmaz):**
1. `RebirthPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `RebirthTitle`
2. Anchor: **Top-Center**
3. Pos X: `0`, Pos Y: `-60`
4. Width: `500`, Height: `70`
5. TMP ayarları: Font Size `40`, Alignment **Center + Middle**, Color **beyaz**, **Bold** ✓
6. Text: `"Evrim (Rebirth)"`

---

**Obje 2 — CurrentMultiplierText (script'e bağlanacak):**
1. `RebirthPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `CurrentMultiplierText`
2. Anchor: **Top-Center**
3. Pos X: `0`, Pos Y: `-180`
4. Width: `600`, Height: `50`
5. TMP ayarları: Font Size `26`, Alignment **Center + Middle**, Color `(186, 104, 200)` (açık mor)
6. Text: `"Mevcut Çarpan: x1.00"`

---

**Obje 3 — GainPreviewText (script'e bağlanacak):**
1. `RebirthPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `GainPreviewText`
2. Anchor: **Top-Center**
3. Pos X: `0`, Pos Y: `-260`
4. Width: `600`, Height: `50`
5. TMP ayarları: Font Size `28`, Alignment **Center + Middle**, Color `(255, 215, 0)` (altın sarı), **Bold** ✓
6. Text: `"Kazanılacak: 0 RC"`

---

**Obje 4 — NewMultiplierText (script'e bağlanacak):**
1. `RebirthPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `NewMultiplierText`
2. Anchor: **Top-Center**
3. Pos X: `0`, Pos Y: `-340`
4. Width: `600`, Height: `50`
5. TMP ayarları: Font Size `26`, Alignment **Center + Middle**, Color `(144, 238, 144)` (açık yeşil)
6. Text: `"Yeni Çarpan: x1.00"`

---

**Obje 5 — RebirthCountText (script'e bağlanacak):**
1. `RebirthPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `RebirthCountText`
2. Anchor: **Top-Center**
3. Pos X: `0`, Pos Y: `-420`
4. Width: `600`, Height: `50`
5. TMP ayarları: Font Size `22`, Alignment **Center + Middle**, Color `(200, 200, 200)` (gri)
6. Text: `"Toplam Rebirth: 0"`

---

**Obje 6 — RebirthButton (script'e bağlanacak):**
1. `RebirthPanel` sağ tık → **UI → Button - TextMeshPro** → Adı: `RebirthButton`
2. Anchor: **Bottom-Center**
3. Pos X: `-130`, Pos Y: `200`
4. Width: `220`, Height: `70`
5. Button → Image → Color: `(156, 39, 176, 255)` (mor)
6. Alt obje `Text (TMP)`: Font Size `24`, **Bold** ✓, Center + Middle, beyaz, Text: `"Evrimleş!"`

---

**Obje 7 — CancelButton (script'e bağlanacak):**
1. `RebirthPanel` sağ tık → **UI → Button - TextMeshPro** → Adı: `CancelButton`
2. Anchor: **Bottom-Center**
3. Pos X: `130`, Pos Y: `200`
4. Width: `220`, Height: `70`
5. Button → Image → Color: `(97, 97, 97, 255)` (gri)
6. Alt obje `Text (TMP)`: Font Size `22`, Center + Middle, beyaz, Text: `"İptal"`

---

**Script bağlama (son adım):**
1. `RebirthPanel` seçin → **Add Component** → `RebirthPanel` (DinoCore.UI)
2. Inspector'da 6 alanı sürükleyerek doldurun:

| Inspector Alanı | Sürüklenecek Obje |
|----------------|-------------------|
| Panel | `RebirthPanel` (kendisi) |
| Rebirth Button | `RebirthButton` |
| Cancel Button | `CancelButton` |
| Current Multiplier Text | `CurrentMultiplierText` |
| Gain Preview Text | `GainPreviewText` |
| New Multiplier Text | `NewMultiplierText` |
| Rebirth Count Text | `RebirthCountText` |

3. `RebirthPanel` objesini seçin → Inspector en üstte ismin solundaki **tik kutusunu kaldırın** ✗
   (Bu, başlangıçta paneli kapalı yapar)

---

#### 6.6.7 Planet Panel

> **Bu panelde toplam 8 obje oluşturacaksınız:**
> - 1 adet Başlık Text (script'e bağlanmaz)
> - 1 adet Image (PlanetIcon)
> - 4 adet Text (bilgi metinleri)
> - 2 adet Button (Keşfet + İptal)

**Ana Panel:**
1. **Canvas** sağ tık → **UI → Panel** → Adı: `PlanetPanel`
2. Anchor: **Stretch All**, Left/Right/Top/Bottom: `0`
3. Image → Color: `(10, 15, 35, 240)` (koyu lacivert)

---

**Obje 1 — Başlık (sadece görsel, script'e bağlanmaz):**
1. `PlanetPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `PlanetTitle`
2. Anchor: **Top-Center**
3. Pos X: `0`, Pos Y: `-60`
4. Width: `500`, Height: `70`
5. TMP ayarları: Font Size `40`, Alignment **Center + Middle**, Color **beyaz**, **Bold** ✓
6. Text: `"Gezegenler"`

---

**Obje 2 — PlanetIcon (script'e bağlanacak):**
1. `PlanetPanel` sağ tık → **UI → Image** → Adı: `PlanetIcon`
2. Anchor: **Top-Center**
3. Pos X: `0`, Pos Y: `-200`
4. Width: `150`, Height: `150`
5. Image → Color: **beyaz** (sprite atanınca doğru renkte görünmesi için)
6. (Sprite şimdilik boş bırakılabilir, daha sonra atanır)

---

**Obje 3 — NextPlanetText (script'e bağlanacak):**
1. `PlanetPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `NextPlanetText`
2. Anchor: **Top-Center**
3. Pos X: `0`, Pos Y: `-320`
4. Width: `500`, Height: `50`
5. TMP ayarları: Font Size `30`, Alignment **Center + Middle**, Color `(100, 181, 246)` (açık mavi), **Bold** ✓
6. Text: `"Pangea"`

---

**Obje 4 — CurrentMultiplierText (script'e bağlanacak):**
1. `PlanetPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `CurrentMultiplierText`
2. Anchor: **Top-Center**
3. Pos X: `0`, Pos Y: `-400`
4. Width: `600`, Height: `45`
5. TMP ayarları: Font Size `24`, Alignment **Center + Middle**, Color `(144, 238, 144)` (açık yeşil)
6. Text: `"Gezegen Çarpanı: x1.00"`

---

**Obje 5 — RequirementText (script'e bağlanacak):**
1. `PlanetPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `RequirementText`
2. Anchor: **Top-Center**
3. Pos X: `0`, Pos Y: `-480`
4. Width: `600`, Height: `70`
5. TMP ayarları: Font Size `20`, Alignment **Center + Middle**, Color `(200, 200, 200)` (gri)
6. Text: `"Gerekli Rebirth: 5"` (placeholder, iki satırlı olabilir)

---

**Obje 6 — PlanetCountText (script'e bağlanacak):**
1. `PlanetPanel` sağ tık → **UI → Text - TextMeshPro** → Adı: `PlanetCountText`
2. Anchor: **Top-Center**
3. Pos X: `0`, Pos Y: `-560`
4. Width: `600`, Height: `45`
5. TMP ayarları: Font Size `22`, Alignment **Center + Middle**, Color `(255, 215, 0)` (altın sarı)
6. Text: `"Açılan Gezegen: 0"`

---

**Obje 7 — UnlockButton (script'e bağlanacak):**
1. `PlanetPanel` sağ tık → **UI → Button - TextMeshPro** → Adı: `UnlockButton`
2. Anchor: **Bottom-Center**
3. Pos X: `-130`, Pos Y: `200`
4. Width: `220`, Height: `70`
5. Button → Image → Color: `(33, 150, 243, 255)` (mavi)
6. Alt obje `Text (TMP)`: Font Size `24`, **Bold** ✓, Center + Middle, beyaz, Text: `"Keşfet!"`

---

**Obje 8 — CancelButton (script'e bağlanacak):**
1. `PlanetPanel` sağ tık → **UI → Button - TextMeshPro** → Adı: `CancelButton`
2. Anchor: **Bottom-Center**
3. Pos X: `130`, Pos Y: `200`
4. Width: `220`, Height: `70`
5. Button → Image → Color: `(97, 97, 97, 255)` (gri)
6. Alt obje `Text (TMP)`: Font Size `22`, Center + Middle, beyaz, Text: `"İptal"`

---

**Script bağlama (son adım):**
1. `PlanetPanel` seçin → **Add Component** → `PlanetPanel` (DinoCore.UI)
2. Inspector'da 7 alanı sürükleyerek doldurun:

| Inspector Alanı | Sürüklenecek Obje |
|----------------|-------------------|
| Panel | `PlanetPanel` (kendisi) |
| Unlock Button | `UnlockButton` |
| Cancel Button | `CancelButton` |
| Current Multiplier Text | `CurrentMultiplierText` |
| Next Planet Text | `NextPlanetText` |
| Requirement Text | `RequirementText` |
| Planet Count Text | `PlanetCountText` |
| Planet Icon | `PlanetIcon` |

3. `PlanetPanel` objesini seçin → Inspector en üstte ismin solundaki **tik kutusunu kaldırın** ✗
   (Bu, başlangıçta paneli kapalı yapar)

---

### 6.7 Offline Income Popup

```
┌────────────────────────────────────────┐
│                                        │
│      Tekrar hoş geldin!                │
│                                        │
│   Uzakta geçen süre: 2h 30m            │
│   Kazanılan: 1.50K                     │
│                                        │
│          [ Topla! ]                    │
│                                        │
└────────────────────────────────────────┘
```

1. **Canvas** sağ tık → **UI → Panel** → Adı: `OfflinePopupBG`
   - Anchor: **Stretch All**, Left/Right/Top/Bottom: `0`
   - Image → Color: `(0, 0, 0, 150)` (siyah yarı-saydam overlay)

2. `OfflinePopupBG` sağ tık → **UI → Panel** → Adı: `OfflinePopup`
   - Anchor: **Middle-Center**
   - Pos X: `0`, Pos Y: `0`
   - Width: `700`, Height: `500`
   - Image → Color: `(45, 35, 55, 250)` (koyu mor)

> **`OfflinePopup` içine toplam 4 obje oluşturacaksınız:**
> - 3 adet Text (1 başlık + 2 bilgi metni)
> - 1 adet Button (Topla)

---

**Obje 1 — Başlık (sadece görsel, script'e bağlanmaz):**
1. `OfflinePopup` sağ tık → **UI → Text - TextMeshPro** → Adı: `PopupTitle`
2. Anchor: **Top-Center**
3. Pos X: `0`, Pos Y: `-50`
4. Width: `500`, Height: `60`
5. TMP ayarları: Font Size `32`, Alignment **Center + Middle**, Color **beyaz**, **Bold** ✓
6. Text: `"Tekrar Hoş Geldin!"`

---

**Obje 2 — TimeAwayText (script'e bağlanacak):**
1. `OfflinePopup` sağ tık → **UI → Text - TextMeshPro** → Adı: `TimeAwayText`
2. Anchor: **Middle-Center**
3. Pos X: `0`, Pos Y: `40`
4. Width: `550`, Height: `45`
5. TMP ayarları: Font Size `24`, Alignment **Center + Middle**, Color `(200, 200, 200)` (gri)
6. Text: `"Uzakta geçen süre: ..."`

---

**Obje 3 — IncomeText (script'e bağlanacak):**
1. `OfflinePopup` sağ tık → **UI → Text - TextMeshPro** → Adı: `IncomeText`
2. Anchor: **Middle-Center**
3. Pos X: `0`, Pos Y: `-20`
4. Width: `550`, Height: `50`
5. TMP ayarları: Font Size `28`, Alignment **Center + Middle**, Color `(255, 215, 0)` (altın sarı), **Bold** ✓
6. Text: `"Kazanılan: 0"`

---

**Obje 4 — ToplaButton (onClick ile bağlanacak):**
1. `OfflinePopup` sağ tık → **UI → Button - TextMeshPro** → Adı: `ToplaButton`
2. Anchor: **Bottom-Center**
3. Pos X: `0`, Pos Y: `60`
4. Width: `250`, Height: `70`
5. Button → Image → Color: `(76, 175, 80, 255)` (yeşil)
6. Alt obje `Text (TMP)`: Font Size `26`, **Bold** ✓, Center + Middle, beyaz, Text: `"Topla!"`

---

**Script bağlama (ÖNCE bunu yapın):**
1. `OfflinePopupBG` seçin → **Add Component** → `OfflineIncomePopup` yazın ve ekleyin
   > ⚠️ Add Component'te bulamıyorsanız, Unity Console'da (Window → General → Console) kırmızı hata olup olmadığını kontrol edin. Derleme hatası varsa hiçbir script görünmez!
2. Inspector'da 3 alanı sürükleyerek doldurun:

| Inspector Alanı | Sürüklenecek Obje |
|----------------|-------------------|
| Popup Panel | `OfflinePopupBG` (kendisi — tüm overlay'i kapatmak için) |
| Time Away Text | `TimeAwayText` |
| Income Text | `IncomeText` |

3. `OfflinePopupBG` objesini seçin → Inspector en üstte ismin solundaki **tik kutusunu kaldırmayın** ✓
   > ⚠️ **ÖNEMLİ:** `OfflinePopupBG` **aktif (enabled) kalmalıdır!**
   > Script'in `Awake()` çalışabilmesi için objenin başlangıçta aktif olması gerekir.
   > Popup'ı gizlemek için tik kaldırmaya gerek yok — script zaten `Start()` içinde
   > otomatik olarak `popupPanel.SetActive(false)` çağırarak popup'ı gizler.
   > Tik kaldırırsanız script hiç başlamaz ve popup **asla gösterilmez**.

---

**ToplaButton onClick bağlama (script eklendikten SONRA yapın):**
1. `ToplaButton` seçin → Inspector'da **Button** bileşenini bulun
2. **On Click ()** bölümünde `+` butonuna tıklayın
3. Sol alttaki boş alana **`OfflinePopupBG`** objesini Hierarchy'den sürükleyin
4. Sağ üstteki dropdown'dan: **OfflineIncomePopup → ClosePopup** seçin
   > ⚠️ Dropdown'da `OfflineIncomePopup` görünmüyorsa, 3. adımda doğru objeyi (`OfflinePopupBG`) sürüklediğinizden emin olun. Script o objeye bağlı olmalıdır.

---

### 6.8 MainNavigation Bağlantıları

Şimdi tüm paneller hazır olduğuna göre, navigasyonu bağlayın:

1. `BottomNav` objesini seçin
2. **Add Component** → `MainNavigation` (DinoCore.UI)
3. Inspector'da şu referansları sürükleyin:

| MainNavigation Alanı | Sürüklenecek Obje |
|----------------------|-------------------|
| Mine Tab Button | `BottomNav/MineTabButton` |
| Dino Tab Button | `BottomNav/DinoTabButton` |
| Tech Tab Button | `BottomNav/TechTabButton` |
| Rebirth Tab Button | `BottomNav/RebirthTabButton` |
| Planet Tab Button | `BottomNav/PlanetTabButton` |
| Mine Panel | `RoomScrollView` |
| Dino Panel | `DinoPanel` (DinoPanel script bileşeni) |
| Tech Panel | `TechPanel` (TechPanel script bileşeni) |
| Rebirth Panel | `RebirthPanel` (RebirthPanel script bileşeni) |
| Planet Panel | `PlanetPanel` (PlanetPanel script bileşeni) |

### 6.9 Hierarchy Son Görünüm

İşlem bittiğinde Hierarchy şöyle görünmelidir:

```
SampleScene
├── Main Camera
├── Directional Light
├── [GAME_MANAGER]           ← 10 manager script
├── EventSystem
└── Canvas
    ├── TopHUD               ← CurrencyHUD script
    │   ├── MoneyIcon
    │   ├── MoneyText
    │   ├── IncomePerSecondText
    │   ├── TechIcon
    │   ├── TechCurrencyText
    │   ├── RebirthIcon
    │   └── RebirthCurrencyText
    ├── ClickButton           ← ClickButton script
    │   ├── ClickIncomeText
    │   └── FeedbackParent
    ├── RoomScrollView
    │   └── Viewport
    │       └── Content       ← RoomManager.roomContainer buraya bağlı
    ├── BottomNav              ← MainNavigation script
    │   ├── MineTabButton
    │   ├── DinoTabButton
    │   ├── TechTabButton
    │   ├── RebirthTabButton
    │   └── PlanetTabButton
    ├── DinoPanel (kapalı)     ← DinoPanel script
    │   ├── DinoTitle
    │   ├── ShopTabButton
    │   ├── AssignTabButton
    │   ├── ShopPanel (ScrollView)
    │   └── AssignPanel (ScrollView)
    ├── TechPanel (kapalı)     ← TechPanel script
    │   ├── TechTitle
    │   └── TechScrollView
    ├── RebirthPanel (kapalı)  ← RebirthPanel script
    │   ├── Başlık
    │   ├── CurrentMultiplierText
    │   ├── GainPreviewText
    │   ├── NewMultiplierText
    │   ├── RebirthCountText
    │   ├── RebirthButton
    │   └── CancelButton
    ├── PlanetPanel (kapalı)   ← PlanetPanel script
    │   ├── Başlık
    │   ├── PlanetIcon
    │   ├── NextPlanetText
    │   ├── CurrentMultiplierText
    │   ├── RequirementText
    │   ├── PlanetCountText
    │   ├── UnlockButton
    │   └── CancelButton
    └── OfflinePopupBG (kapalı) ← OfflineIncomePopup script
        └── OfflinePopup
            ├── Başlık
            ├── TimeAwayText
            ├── IncomeText
            └── ToplaButton
```

---

## 7. PREFAB BAĞLANTILARI

Tüm referansları bağladığınızdan emin olun:

### Kontrol Listesi:
- [ ] GameManager → Config (GameConfig SO)
- [ ] RoomManager → Room Templates (10 RoomData SO)
- [ ] RoomManager → Room Container (Scroll View Content)
- [ ] RoomManager → Room Prefab (Prefabs/Rooms/RoomPrefab)
- [ ] DinoManager → Dino Templates (10 DinoData SO)
- [ ] TechManager → Tech Templates (6 TechData SO)
- [ ] RebirthManager → Config
- [ ] PlanetManager → Planet Templates (5 PlanetData SO)
- [ ] PlanetManager → Config
- [ ] SaveManager → Config
- [ ] ClickManager → Config
- [ ] OfflineIncomeSystem → Config
- [ ] CurrencyHUD → 4 adet Text referansı
- [ ] ClickButton → Button, Text, FeedbackParent, FloatingTextPrefab
- [ ] MainNavigation → 5 buton + 5 panel
- [ ] RoomPrefab → RoomPanel içindeki tüm referanslar
- [ ] OfflineIncomePopup → Panel, 2 Text

---

## 8. GÖRSELLERİ IMPORT ETME

### 8.1 AI ile Görselleri Üretme
1. `Assets/Art/AI_ART_PROMPTS.md` dosyasını açın
2. Her prompt'u Midjourney, DALL-E 3 veya Leonardo AI'da kullanın
3. PNG formatında (şeffaf arka plan) kaydedin

### 8.2 Unity'ye Import

1. Görselleri ilgili klasörlere sürükleyin (Assets/Art/Dinos/ vb.)
2. Her görseli seçin, Inspector'da:
   - **Texture Type**: `Sprite (2D and UI)`
   - **Sprite Mode**: `Single` (sprite sheet'ler için `Multiple`)
   - **Max Size**: `512` (ikonlar `128`, arka planlar `1024`)
   - **Compression**: `ASTC 6x6` (mobil için en iyi)
   - **Generate Mip Maps**: ✗ (2D oyun için kapatın)
3. **Apply** butonuna tıklayın

### 8.3 Sprite Sheet'ler

Efekt sprite sheet'leri için:
1. Sprite Mode: `Multiple` seçin
2. **Sprite Editor** butonuna tıklayın
3. Sol üstte **Slice** menüsünü açın
4. Type: `Grid By Cell Count` veya `Grid By Cell Size`
5. **Slice** butonuna tıklayın
6. **Apply** butonuna tıklayın

---

## 9. TEST VE DEBUG

### 9.1 İlk Çalıştırma

1. Sahneyi kaydedin (Ctrl+S)
2. **Play** butonuna basın (▶ veya Ctrl+P)
3. Console penceresini açın (Window → General → Console)
4. `[GameManager] Game initialized successfully.` mesajını görmelisiniz

### 9.2 Debug Komutları

Inspector'da `[GAME_MANAGER]` objesini seçin:
- **GameManager** bileşeninde sağ tık → Context Menu:
  - `Delete Save Data` → Save'i siler
  - `Add 1M Money` → Test için para ekler
  - `Add 100 Tech Currency` → Tech para ekler
  - `Force Rebirth` → Zorla rebirth yapar

### 9.3 Yaygın Sorunlar ve Çözümleri

**"NullReferenceException" hatası:**
- Inspector'da bağlanmamış referans var. Console'da hangi script'te olduğunu kontrol edin.
- Genellikle SO (ScriptableObject) referansları veya UI text referansları eksiktir.

**Odalar görünmüyor:**
- RoomManager → Room Container alanının Content objesine bağlı olduğundan emin olun
- RoomManager → Room Prefab alanının atandığından emin olun
- Room Templates listesinin boş olmadığından emin olun

**Para artmıyor:**
- İlk oda otomatik olarak açılıyor mu kontrol edin (room_01 ID'si doğru mu?)
- ClickButton script'i bağlı mı?

---

## 10. ANDROID/iOS BUILD

### 10.1 Platform Değiştirme

1. **File → Build Profiles** (Unity 6.3'te bu şekilde)
2. Sol taraftan **Android** veya **iOS** seçin
3. **Switch Platform** butonuna tıklayın (biraz sürer)

### 10.2 Player Settings

**File → Build Profiles → Player Settings** veya **Edit → Project Settings → Player**

**Android Ayarları:**
- Company Name: `YourCompany`
- Product Name: `DinoCore`
- Default Icon: app_icon.png
- **Other Settings:**
  - Package Name: `com.yourcompany.dinocore`
  - Minimum API Level: `Android 7.0 (API Level 24)`
  - Target API Level: `Automatic (highest installed)`
  - Scripting Backend: `IL2CPP`
  - Target Architectures: `ARM64` ✓ (ARMv7 opsiyonel)
- **Resolution and Presentation:**
  - Default Orientation: `Portrait`
  - Allowed Orientations: Sadece `Portrait` ✓

**iOS Ayarları:**
- Bundle Identifier: `com.yourcompany.dinocore`
- Target minimum iOS Version: `14.0`
- Scripting Backend: `IL2CPP`
- Architecture: `ARM64`

### 10.3 Kalite Ayarları

**Edit → Project Settings → Quality:**
1. Mobil için `Medium` veya `Low` profili seçin
2. V Sync Count: `Don't Sync` (mobilde frame rate için)
3. Anti Aliasing: `Disabled` (2D oyun, gerekli değil)
4. Texture Quality: `Full Res`

**Application → Target Frame Rate:**
Kodda ekleyebilirsiniz veya şu script'i kullanabilirsiniz:

### 10.4 Build Alma

1. **File → Build Profiles**
2. Sahnenizi `Scenes in Build` listesine ekleyin
3. **Build** veya **Build And Run** butonuna tıklayın
4. Çıktı klasörünü seçin
5. Android: `.apk` veya `.aab` dosyası oluşur
6. iOS: Xcode projesi oluşur (Mac'te Xcode ile açıp build alın)

---

## 11. SIK KARŞILAŞILAN HATALAR

### TextMeshPro Package
Eğer TMP_Text bulunamıyor hatası alırsanız:
1. **Window → Package Manager**
2. **TextMeshPro** paketini arayın (genellikle Unity 6'da önceden yüklüdür)
3. Yüklü değilse **Install** butonuna tıklayın
4. İlk kullanımda **Window → TextMeshPro → Import TMP Essential Resources**

### Assembly Definition Uyarıları
Script'ler namespace kullanıyor ama Assembly Definition dosyası yok.
Bu bir sorun değil, ama isterseniz her Scripts alt klasörüne `.asmdef` ekleyebilirsiniz.

### Prefab Override Sorunları
Prefab'ı düzenlerken:
1. Hierarchy'de prefab instance'ını seçin
2. Değişiklik yapın
3. Inspector üstünde **Overrides → Apply All** yapın

---

## 12. GENİŞLETME REHBERİ

### Yeni Oda Eklemek
1. `Assets/ScriptableObjects/Rooms/` → Create → DinoCore → Room Data
2. Değerleri doldurun (benzersiz `roomId` verin)
3. `[GAME_MANAGER]` → RoomManager → Room Templates listesine ekleyin

### Yeni Dinozor Eklemek
1. `Assets/ScriptableObjects/Dinos/` → Create → DinoCore → Dino Data
2. Değerleri doldurun
3. DinoManager → Dino Templates listesine ekleyin

### Yeni Teknoloji Eklemek
1. `Assets/ScriptableObjects/Tech/` → Create → DinoCore → Tech Data
2. TechManager → Tech Templates listesine ekleyin

### Yeni Gezegen Eklemek
1. PlanetData SO oluşturun
2. PlanetManager → Planet Templates listesine ekleyin

### Yeni DinoType Eklemek
1. `DinoData.cs` → `DinoType` enum'ına yeni tip ekleyin
2. `DinoManager.cs` → bonus hesaplama metodlarına yeni tip için case ekleyin
3. `Room.cs` → gerekiyorsa production loop'a etki ekleyin

### Daily Reward Sistemi Eklemek
1. Yeni script: `DailyRewardSystem.cs`
2. SaveData'ya `lastDailyRewardTimestamp` ekleyin
3. GameEvents'e `OnDailyRewardClaimed` ekleyin
4. UI panel oluşturun

### Achievement Sistemi Eklemek
1. `AchievementData.cs` (ScriptableObject)
2. `AchievementManager.cs`
3. GameEvents'leri dinleyerek başarım kontrolü yapın

---

## 🎯 HIZLI BAŞLANGIÇ ÖZET

1. ✅ Scriptler zaten oluşturuldu
2. ⬜ ScriptableObject'leri oluşturun (Bölüm 2)
3. ⬜ Sahne'de Canvas kurun (Bölüm 3)
4. ⬜ Manager GameObject oluşturun (Bölüm 4)
5. ⬜ Room Prefab oluşturun (Bölüm 5)
6. ⬜ UI elementlerini kurun (Bölüm 6)
7. ⬜ Referansları bağlayın (Bölüm 7)
8. ⬜ AI ile görselleri üretin (Bölüm 8)
9. ⬜ Test edin (Bölüm 9)
10. ⬜ Build alın (Bölüm 10)

Bu rehberi takip ederek tamamen çalışan bir idle oyunu kurabilirsiniz. Herhangi bir adımda takılırsanız, o adımın detaylarını sorun!
