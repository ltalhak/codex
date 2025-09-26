# SolarOrderQuiz

SolarOrderQuiz, kullanıcıların Güneş etrafındaki 8 gezegeni doğru yörünge sırasına göre yerleştirerek bilgilerini pekiştirmelerini sağlayan .NET 8 tabanlı bir WPF masaüstü uygulamasıdır. Gezegen doğru konuma bırakıldığında o gezegenle ilgili bir soru sorulur ve doğru cevaplar puan kazandırır.

## Özellikler

- **Sürükle-bırak gezegen yerleştirme:** Gezegen paletinden doğru yörüngeye bıraktığınız gezegenler kilitlenir ve soru turu başlar.
- **Dinamik soru havuzu:** Her gezegen için en az üç çoktan seçmeli soru, yanlış cevaplarda açıklama.
- **Skorlama ve süre takibi:** Doğru yerleştirme ve cevaplar puan kazandırır, toplam süreye göre bonus uygulanır.
- **Erişilebilirlik:** Klavye odağı, yüksek kontrastlı tema, açıklayıcı tooltip'ler.
- **Kaydet/Yükle:** %AppData%\SolarOrderQuiz klasörüne oyun durumunu JSON olarak kaydedip geri yükleyin.
- **Özet ekranı:** Tüm gezegenler yerleştirildiğinde skor, doğru/yanlış sayıları ve süre raporlanır.

## Proje Yapısı

```
SolarOrderQuiz.sln
├── SolarOrderQuiz/              # WPF uygulaması
│   ├── Assets/Images/           # Vektör tabanlı gezegen çizimleri
│   ├── Behaviors/               # Sürükle-bırak davranışları
│   ├── Converters/              # Görünürlük dönüştürücüleri
│   ├── Models/                  # Planet, Question, QuizState, PlanetCatalog
│   ├── Resources/               # Yerelleştirme metinleri ve soru havuzu (Questions.json)
│   ├── Services/                # Soru, kaydet/yükle ve diyalog servisleri
│   ├── ViewModels/              # MVVM view model katmanı
│   └── Views/                   # XAML görünümleri ve diyaloglar
└── SolarOrderQuiz.Tests/        # xUnit testleri
```

## Gereksinimler

- .NET SDK 8.0 (Windows)
- WPF çalıştırabilmek için Windows 10 veya üstü önerilir.

## Derleme ve Çalıştırma

```bash
# Çözümü derleyin
dotnet build

# Uygulamayı başlatın
dotnet run --project SolarOrderQuiz
```

## Testler

```bash
dotnet test
```

## Yayınlama

Kendi kendine çalışır bir Windows x64 paketi üretmek için:

```bash
dotnet publish SolarOrderQuiz -c Release -r win-x64 --self-contained true
```

## Soru Havuzunu Güncelleme

`Resources/Questions.json` dosyasında her gezegen için en az üç soru bulunur. Yeni bir soru eklerken `planetId` alanının gezegen kimliğiyle (ör. `mars`) eşleştiğinden emin olun.

## Lisans

Bu örnek eğitim amaçlıdır.
