## 🎯 Amaç  
Kodu sadece **“çalışır”** hale getirmek değil, **production-ready**, **sürdürülebilir**, **güvenli** ve **ölçeklenebilir** bir API’ye dönüştürmek.

---

## 📘 README.md İçin Beklentiler

Her düzeltme için aşağıdaki yapıyı kullanabilirsiniz.  
Ancak bu şablonu **kendi tarzınıza göre genişletmek serbesttir.**

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | |
| ⚠️ **Neden problemdi?** | |
| ✅ **Nasıl çözdünüz?** | |
| 🔁 **Alternatifler?** | |

> “Düzelttim” demek yeterli değildir — **neden** ve **nasıl** düzeltildiğini görmek istiyoruz.  
> Ek olarak, **kendi ek geliştirmelerinizi ve fikirlerinizi** de belirtin.
> yorum satırlarında hataların görüntülenmesi kafanızı karıştırmak için koyulmuş olabilir..

---

## 💬 Özet

Bu hackathon'un amacı sadece **hataları düzeltmek** değil;
kodunuzu **daha iyi hale getirme vizyonunuzu göstermenizdir.**

> Aynı hatayı herkes görebilir,
> ama onu **farklı bir şekilde çözmek fark yaratır.** 🚀

---

## 🔧 Yapılan Düzeltmeler

### 🟢 Kolay Seviye Hatalar (Build/Derleme Hataları)

#### 1. Program.cs - MapContrllers Hatası

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | `Program.cs` dosyasının 65. satırında `app.MapContrllers()` yazım hatası vardı. |
| ⚠️ **Neden problemdi?** | `MapControllers()` metodunun adı yanlış yazıldığı için (Controllers yerine Contrllers), ASP.NET Core bu metodu bulamıyordu ve derleme hatası veriyordu. Bu hata, API endpoint'lerinin hiç çalışmamasına neden olacaktı. |
| ✅ **Nasıl çözdünüz?** | `app.MapContrllers()` → `app.MapControllers()` olarak düzeltildi. |
| 🔁 **Alternatifler?** | Bu bir yazım hatası olduğu için alternatif yok, doğru yazılması gerekiyor. |

#### 2. Tüm Controller'larda Success Property Hatası

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | Tüm controller'larda `result.Success` şeklinde erişim vardı, ancak `IResult` ve `IDataResult` interface'lerinde property adı `IsSuccess` idi. |
| ⚠️ **Neden problemdi?** | Property adı yanlış olduğu için derleyici bu property'i bulamıyordu ve 40+ derleme hatası üretiyordu. Bu, tüm API endpoint'lerinin çalışmamasına neden oluyordu. |
| ✅ **Nasıl çözdünüz?** | Tüm controller'larda (CoursesController, StudentsController, InstructorsController, LessonsController, RegistrationsController, ExamsController, ExamResultsController) `result.Success` ifadeleri `result.IsSuccess` olarak değiştirildi. Toplamda 43 yerde düzeltme yapıldı. |
| 🔁 **Alternatifler?** | Interface'lerdeki property adını `Success` olarak değiştirmek mümkün olsa da, `IsSuccess` naming convention'a daha uygun (boolean property'ler Is- prefix'i ile başlar). |

#### 3. CoursesController.cs - GetByIdAsnc Hatası

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | `CoursesController.cs` dosyasının 33. satırında `_courseService.GetByIdAsnc(id)` yazım hatası vardı. |
| ⚠️ **Neden problemdi?** | Metod adı `GetByIdAsync` olması gerekirken `GetByIdAsnc` yazıldığı için (Async yerine Asnc), servis katmanında bu metod bulunamıyordu ve derleme hatası veriyordu. |
| ✅ **Nasıl çözdünüz?** | `GetByIdAsnc()` → `GetByIdAsync()` olarak düzeltildi. |
| 🔁 **Alternatifler?** | Servis interface'inde metodun adını değiştirmek mümkün ama bu standart async naming pattern'ına aykırı olur. |

#### 4. LessonsController.cs - CreatAsync Hatası

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | `LessonsController.cs` dosyasının 72. satırında `_lessonService.CreatAsync()` yazım hatası vardı. |
| ⚠️ **Neden problemdi?** | Metod adı `CreateAsync` olması gerekirken `CreatAsync` yazıldığı için (Create yerine Creat), servis katmanında bu metod bulunamıyordu. Lesson oluşturma endpoint'i çalışmıyordu. |
| ✅ **Nasıl çözdünüz?** | `CreatAsync()` → `CreateAsync()` olarak düzeltildi. |
| 🔁 **Alternatifler?** | Yazım hatası düzeltilmesi gerekiyor, alternatif yok. |

#### 5. LessonsController.cs - CreateLessonDto Property Hatası

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | `LessonsController.cs` dosyasının 66. satırında `createLessonDto.Name` kullanılıyordu, ancak `CreateLessonDto` sınıfında `Name` property'si yoktu. |
| ⚠️ **Neden problemdi?** | `CreateLessonDto` sınıfında `Name` property'si yerine `Title` property'si vardı. Yanlış property adı kullanıldığı için derleme hatası oluşuyordu. |
| ✅ **Nasıl çözdünüz?** | `createLessonDto.Name` → `createLessonDto.Title` olarak değiştirildi. |
| 🔁 **Alternatifler?** | DTO sınıfını değiştirmek yerine controller'ı düzeltmek daha mantıklı, çünkü DTO başka yerlerde de kullanılıyor olabilir. |

#### 6. RegistrationsController.cs - rsult Değişken Hatası

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | `RegistrationsController.cs` dosyasının 71. satırında `rsult.IsSuccess` yazım hatası vardı. |
| ⚠️ **Neden problemdi?** | Değişken adı `result` olması gerekirken `rsult` yazıldığı için derleyici bu değişkeni bulamıyordu. Registration oluşturma işleminin sonuç kontrolü yapılamıyordu. |
| ✅ **Nasıl çözdünüz?** | `rsult.IsSuccess` → `result.IsSuccess` olarak düzeltildi. |
| 🔁 **Alternatifler?** | Yazım hatası, düzeltilmesi gerekiyor. |

#### 7. ExamResultsController.cs - BadReqest Hatası

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | `ExamResultsController.cs` dosyasının 36. satırında `BadReqest(result)` yazım hatası vardı. |
| ⚠️ **Neden problemdi?** | Metod adı `BadRequest` olması gerekirken `BadReqest` yazıldığı için (Request yerine Reqest), ASP.NET Core bu metodu bulamıyordu. HTTP 400 response döndürülemiyordu. |
| ✅ **Nasıl çözdünüz?** | `BadReqest()` → `BadRequest()` olarak düzeltildi. |
| 🔁 **Alternatifler?** | ASP.NET Core'un built-in metodu, doğru yazılması gerekiyor. |

#### 8. StudentsController.cs - Property İsimlendirme Hataları

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | `StudentsController.cs` dosyasında 4 farklı yerde (51, 64, 69, 85. satırlar) küçük harfle `.name` kullanılmıştı. |
| ⚠️ **Neden problemdi?** | C# büyük-küçük harf duyarlıdır ve property adı `Name` (büyük N) olması gerekirken `name` (küçük n) kullanıldığı için property bulunamıyordu. Student CRUD işlemleri çalışmıyordu. |
| ✅ **Nasıl çözdünüz?** | Tüm `.name` kullanımları `.Name` olarak değiştirildi: <br>- `result.Data.name` → `result.Data.Name` (satır 51)<br>- `createStudentDto.name` → `createStudentDto.Name` (satır 64, 69)<br>- `updateStudentDto.name` → `updateStudentDto.Name` (satır 85) |
| 🔁 **Alternatifler?** | C# naming convention'ına göre public property'ler PascalCase olmalı, bu standard'a uyulmalı. |

#### 9. Invalid Cast Derleme Hataları

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | `StudentsController.cs` (satır 64) ve `InstructorsController.cs` (satır 50) dosyalarında `string` değerler `int`'e direkt cast edilmeye çalışılıyordu: `(int)createStudentDto.Name` |
| ⚠️ **Neden problemdi?** | C#'ta `string` türü `int` türüne direkt cast edilemez, derleme hatası verir. Bu, medium seviye bir hata olmasına rağmen derlemeyi engellediği için düzeltilmesi gerekiyordu. |
| ✅ **Nasıl çözdünüz?** | Her iki satır da yoruma alındı (comment out) çünkü bu satırlar zaten yanlış tip dönüşümü örnekleri olarak kasıtlı konulmuş hatalardı ve projenin çalışması için gerekli değillerdi. |
| 🔁 **Alternatifler?** | Eğer gerçekten string'i int'e çevirmek gerekse `int.Parse()`, `int.TryParse()` veya `Convert.ToInt32()` kullanılmalı. Ancak burada mantıklı bir kullanım olmadığı için yoruma almak en doğru çözümdü. |

---

### 📊 Düzeltme Özeti

- **Toplam Düzeltilen Hata Sayısı:** 51 build error → 0 error
- **Etkilenen Dosyalar:** 8 controller + 1 Program.cs
- **Hata Kategorileri:**
  - Yazım hataları (typo): 7 adet
  - Property/field adı hataları: 44 adet
  - Invalid cast hataları: 2 adet (yoruma alındı)

**Sonuç:** Proje artık başarıyla derleniyor ve çalışır durumda! ✅

---

### 🟡 Orta Seviye Hatalar (Runtime ve Mantıksal Hatalar)

## Toplam Düzeltilen Hata: 28 adet

### Manager Sınıfları Düzeltmeleri (21 hata)

#### 1. InstructorManager.cs - 4 Hata

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | 1) `GetByIdAsync` metodunda `id[5]` kullanımı (null/length check yok)<br>2) `hasInstructor` null olabilir ama kontrol edilmiyor<br>3) `Update` metodunda `entity` null kontrolü yok<br>4) `Update` hata durumunda `SuccessResult` döndürüyor (mantıksal hata) |
| ⚠️ **Neden problemdi?** | 1) ID 6 karakterden kısa ise `IndexOutOfRangeException` fırlatır<br>2) Veritabanında kayıt bulunamazsa `NullReferenceException` oluşur<br>3-4) Hatalı sonuç döndürülür, loglama ve hata takibi zorlaşır |
| ✅ **Nasıl çözdünüz?** | 1) `string.IsNullOrEmpty(id) \|\| id.Length < 6` kontrolü eklendi<br>2) `hasInstructor == null` kontrolü ve `ErrorDataResult` dönüşü eklendi<br>3) `entity == null` kontrolü eklendi<br>4) Satır 86'da `SuccessResult` → `ErrorResult` değiştirildi |
| 🔁 **Alternatifler?** | Guard clause pattern, FluentValidation kullanılabilir. Null-conditional operator (`?.`) kullanımı da alternatif. |

#### 2. LessonsManager.cs - 6 Hata

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | 1) `GetByIdAsync` null check eksik, yanlış mesaj<br>2) `CreateAsync` entity ve mapping null kontrolü yok<br>3) `Update` null check ve index out of range<br>4) `Update` hata durumunda `SuccessResult`<br>5) `GetAllLessonDetailAsync` boş liste kontrolü yok |
| ⚠️ **Neden problemdi?** | Runtime'da `NullReferenceException` ve `InvalidOperationException` riski. Yanlış mesajlar UI'da kafa karışıklığı yaratır. |
| ✅ **Nasıl çözdünüz?** | 1) Null kontrolü + doğru mesaj (`LessonGetByIdSuccessMessage`)<br>2-3) `entity == null` ve `string.IsNullOrEmpty` kontrolleri<br>4) `ErrorResult` döndürülmesi sağlandı<br>5) `lessonsListMapping.Any()` kontrolü eklendi |
| 🔁 **Alternatifler?** | Result pattern, Option monad (C# 8+ nullable reference types) |

#### 3. CourseManager.cs - 3 Hata

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | 1) `GetAllAsync` boş liste `result[0]` erişimi<br>2) `GetByIdAsync` null check eksik<br>3) `GetAllCourseDetail` boş `courseDetailDtoList.First()` |
| ⚠️ **Neden problemdi?** | Boş koleksiyonlarda `IndexOutOfRangeException` ve `InvalidOperationException` fırlatır. |
| ✅ **Nasıl çözdünüz?** | 1-3) Tüm liste işlemlerinden önce `== null \|\| Count == 0 \|\| !Any()` kontrolleri eklendi ve `ErrorDataResult` döndürüldü |
| 🔁 **Alternatifler?** | `FirstOrDefault()` kullanımı + null check, `ElementAtOrDefault()` metodu |

#### 4. ExamManager.cs - 2 Hata

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | 1) `GetAllAsync` boş `examtListMapping.ToList()[0]`<br>2) `CreateAsync` entity null kontrolü yok |
| ⚠️ **Neden problemdi?** | Boş koleksiyonda exception, null entity mapping hatası |
| ✅ **Nasıl çözdünüz?** | 1) `examtListMapping.Any()` kontrolü eklendi<br>2) `entity == null` ve `addedExamMapping == null` kontrolleri |
| 🔁 **Alternatifler?** | Repository pattern ile null object pattern kombinasyonu |

#### 5. ExamResultManager.cs - 1 Hata

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | `CreateAsync` metodunda `entity` ve mapping null kontrolü eksik |
| ⚠️ **Neden problemdi?** | Null entity veya başarısız mapping `NullReferenceException` fırlatır |
| ✅ **Nasıl çözdünüz?** | `entity == null` ve `addedExamResultMapping == null` kontrolleri eklendi |
| 🔁 **Alternatifler?** | AutoMapper'da null handling konfigürasyonu |

#### 6. RegistrationManager.cs - 4 Hata

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | 1) `CreateAsync` null check yok<br>2) `Update` invalid cast: `(int)decimal`<br>3) `Update` mantıksal hata: `SuccessResult` yerine `ErrorResult`<br>4) `GetAllRegistrationDetailAsync` boş liste kontrolü yok |
| ⚠️ **Neden problemdi?** | 1) Null reference exception<br>2) Ondalık veri kaybı, `OverflowException` riski<br>3) Yanlış sonuç döner<br>4) `IndexOutOfRangeException` |
| ✅ **Nasıl çözdünüz?** | 1) Null kontrolleri eklendi<br>2) `Convert.ToInt32()` güvenli dönüşüm kullanıldı<br>3) `ErrorResult` döndürüldü<br>4) `Any()` kontrolü eklendi |
| 🔁 **Alternatifler?** | 2) `Math.Round()` + explicit cast, `decimal.ToInt32()` |

#### 7. StudentManager.cs - 1 Hata

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | `GetByIdAsync` metodunda `hasStudent` ve `hasStudentMapping` null kontrolü eksik |
| ⚠️ **Neden problemdi?** | Kayıt bulunamazsa `NullReferenceException` fırlatır |
| ✅ **Nasıl çözdünüz?** | `string.IsNullOrEmpty(id)`, `hasStudent == null`, `hasStudentMapping == null` kontrolleri eklendi |
| 🔁 **Alternatifler?** | Result<T> generic wrapper pattern |

---

### Controller Düzeltmeleri (7 hata)

#### 8. CoursesController.cs - 2 Hata

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | `Create` metodunda `createCourseDto` ve `courseName` null/empty kontrolü yok, `courseName[0]` kullanımı |
| ⚠️ **Neden problemdi?** | Null veya boş string'de `IndexOutOfRangeException` fırlatır |
| ✅ **Nasıl çözdünüz?** | `createCourseDto == null \|\| string.IsNullOrEmpty(createCourseDto.CourseName)` kontrolü eklendi |
| 🔁 **Alternatifler?** | Data annotations (`[Required]`), FluentValidation |

#### 9. ExamsController.cs - 1 Hata

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | `GetAll` metodunda `result.Data` null olabilir ama kontrol edilmeden `ToList()` çağrılıyor |
| ⚠️ **Neden problemdi?** | `result.Data == null` ise `NullReferenceException` |
| ✅ **Nasıl çözdünüz?** | `result.Data == null` kontrolü eklendi, null ise `BadRequest` döndürülüyor |
| 🔁 **Alternatifler?** | Null-conditional operator: `result.Data?.ToList()` |

#### 10. LessonsController.cs - 2 Hata

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | `Create` metodunda `createLessonDto` ve `Title` null kontrolü yok, `lessonName[0]` kullanımı |
| ⚠️ **Neden problemdi?** | Null/empty string'de `IndexOutOfRangeException` |
| ✅ **Nasıl çözdünüz?** | `createLessonDto == null \|\| string.IsNullOrEmpty(createLessonDto.Title)` kontrolü |
| 🔁 **Alternatifler?** | Model validation attributes |

#### 11. RegistrationsController.cs - 1 Hata

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | `Create` metodunda invalid cast: `(int)createRegistrationDto.Price` + null check eksik |
| ⚠️ **Neden problemdi?** | Decimal'den int'e direkt cast veri kaybına neden olur, null DTO exception fırlatır |
| ✅ **Nasıl çözdünüz?** | Null kontrolü + `Convert.ToInt32()` güvenli dönüşümü |
| 🔁 **Alternatifler?** | `Math.Truncate()`, `Math.Ceiling()` |

#### 12. StudentsController.cs - 4 Hata

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | 1) `_cachedStudents` null (hiç initialize edilmemiş)<br>2) `GetById` id[10] index out of range<br>3) `result.Data.Name` null check yok<br>4) `Delete` metodunda `deleteStudentDto` null kontrolü yok |
| ⚠️ **Neden problemdi?** | 1) Satır 28'de `_cachedStudents.Count` null reference<br>2) ID 11 karakterden kısa ise exception<br>3-4) Null object access |
| ✅ **Nasıl çözdünüz?** | 1) `= new List<GetAllStudentDto>()` ile initialize edildi<br>2) `id.Length < 11` kontrolü<br>3) `result != null && result.Data != null` kontrolü<br>4) `deleteStudentDto == null` kontrolü |
| 🔁 **Alternatifler?** | 1) Lazy initialization, dependency injection |

#### 13. InstructorsController.cs - 2 Hata

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | `Create` metodunda `createdInstructorDto` ve `Name` null kontrolü yok, `instructorName[0]` kullanımı |
| ⚠️ **Neden problemdi?** | Null/empty string'de `IndexOutOfRangeException` |
| ✅ **Nasıl çözdünüz?** | `createdInstructorDto == null \|\| string.IsNullOrEmpty(createdInstructorDto.Name)` kontrolü |
| 🔁 **Alternatifler?** | Middleware-level validation |

---

### 📊 Orta Seviye Düzeltme Özeti

- **Toplam Düzeltilen Hata:** 28 adet
- **Hata Türleri:**
  - Null Reference Exception: 16 adet
  - Index Out of Range Exception: 10 adet
  - Invalid Cast Exception: 2 adet
  - Mantıksal Hatalar (Yanlış Result Tipi): 4 adet
- **Etkilenen Dosyalar:**
  - Manager sınıfları: 7 dosya (21 hata)
  - Controller sınıfları: 6 dosya (7 hata)

**Sonuç:** Runtime hataları ve mantıksal hatalar düzeltildi, uygulama artık daha güvenli ve stabil! ✅

---

### 🔴 Zor Seviye Hatalar (Performans, Güvenlik ve Mimari)

#### 1. Performans Problemi: N+1 Sorgu Hatası

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | `CourseManager` içindeki `GetAllAsync` ve `GetAllCourseDetail` metotları, ilişkili verileri (Instructor, Lessons) verimli bir şekilde çekmiyordu. `GetAllAsync`, her bir kurs için ayrı bir veritabanı sorgusu yaparak eğitmen bilgilerini alıyordu (Lazy Loading). `GetAllCourseDetail` ise potansiyel bir N+1 sorununa sahipti çünkü kurslara bağlı `Lessons` (dersleri) sorguya dahil etmiyordu. |
| ⚠️ **Neden problemdi?** | Bu, "N+1 sorgu problemi" olarak bilinen ciddi bir performans sorunudur. Örneğin, 100 kurs varsa, tüm kursları listelemek için 1 (kurslar için) + 100 (her kursun eğitmeni için) = 101 veritabanı sorgusu yapılıyordu. Bu durum, veri miktarı arttıkça API'nin yavaşlamasına ve kaynakların verimsiz kullanılmasına neden olur. |
| ✅ **Nasıl çözdünüz?** | 1. **Repository Katmanı:** `CourseRepository`'deki `GetAllCourseDetail` metodu, `Include(c => c.Instructor)` ifadesine ek olarak `Include(c => c.Lessons)` ifadesini de içerecek şekilde güncellendi. Artık tek bir sorguda bir kursun hem eğitmeni hem de tüm dersleri getiriliyor.<br>2. **Service Katmanı:** `CourseManager`'daki `GetAllAsync` metodu, verimsiz `GetAll()` yerine artık verileri tek seferde çeken `GetAllCourseDetail()` metodunu kullanacak şekilde değiştirildi.<br>3. **DTO ve Mapping:** `GetAllCourseDetailDto` içerisine `Lessons` koleksiyonu eklendi ve `CourseManager`'daki mapping (haritalama) işlemi, bu yeni verileri DTO'ya doğru bir şekilde aktaracak şekilde güncellendi. |
| 🔁 **Alternatifler?** | Projeksiyon (Projection) kullanılabilirdi. `Select` ifadesi içinde doğrudan istenen DTO'ya dönüşüm yapılarak sadece gerekli kolonlar çekilebilirdi. Bu, `Include`'dan daha performanslı olabilir ancak daha karmaşık sorgular gerektirir. Diğer bir alternatif ise `Dapper` gibi micro-ORM'ler ile optimize edilmiş SQL sorguları yazmaktır. |

#### 2. Mimari Hata ve Güvenlik Zafiyeti: Katman İhlali (Layer Violation)

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | `StudentsController`, `AppDbContext`'i doğrudan inject ederek kullanıyordu. `Create` ve `Delete` metotları içinde veritabanına doğrudan erişim sağlanıyordu. Ayrıca `Delete` metodunda manuel olarak oluşturulan `DbContext` dispose edilmiyordu. |
| ⚠️ **Neden problemdi?** | 1. **Güvenlik:** Bu durum, Service (iş mantığı) katmanını tamamen bypass ederek, validasyon ve iş kurallarının atlanmasına neden oluyordu. Herhangi bir kötü niyetli kullanıcı, bu zafiyeti kullanarak sisteme istenmeyen veriler ekleyebilirdi.<br>2. **Mimari Bozukluk:** Sunum katmanının (API Controller) veri erişim katmanına (`DbContext`) doğrudan erişmesi, n-tier mimarinin temel prensiplerini ihlal eder. Bu, kodun bakımını zorlaştırır ve katmanlar arası sıkı bir bağımlılık (tight coupling) yaratır.<br>3. **Memory Leak:** Dispose edilmeyen `DbContext`, veritabanı bağlantılarının ve hafızanın sızdırılmasına, bir süre sonra uygulamanın tamamen yanıt vermemesine neden olur. |
| ✅ **Nasıl çözdünüz?** | `StudentsController` içerisinden `AppDbContext` bağımlılığı tamamen kaldırıldı. Veritabanına doğrudan erişen (`_dbContext.Students.Add(...)` ve `new AppDbContext(...)`) tüm kod blokları silindi. Artık Controller, tüm veritabanı işlemleri için sadece `IStudentService` arayüzünü kullanıyor ve mimari bütünlük sağlandı. |
| 🔁 **Alternatifler?** | Bu bir mimari tasarım hatası olduğu için tek doğru çözüm, katmanlar arasındaki sorumlulukları doğru bir şekilde ayırmaktır. Alternatif bir yaklaşım yoktur; sunum katmanı, iş katmanını atlamamalıdır. |

#### 3. Kritik Güvenlik Zafiyeti: Yetkilendirme Eksikliği

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | Projedeki hiçbir Controller (`Courses`, `Students`, `Exams` vb.) veya endpoint üzerinde yetkilendirme (`Authorization`) mekanizması bulunmuyordu. |
| ⚠️ **Neden problemdi?** | Bu, sistemdeki en kritik güvenlik açıklarından biridir. API endpoint'lerine erişim tamamen halka açıktı. Bu, internete erişimi olan herhangi bir anonim kullanıcının sistemdeki tüm verileri (kurs, öğrenci, sınav vb.) oluşturabileceği, güncelleyebileceği ve silebileceği anlamına geliyordu. |
| ✅ **Nasıl çözdünüz?** | Tüm Controller sınıflarının üzerine `[Authorize]` attribute'u eklendi. Bu sayede, bu controller'lar altındaki tüm endpoint'lere (GET, POST, PUT, DELETE) erişim, sadece kimliği doğrulanmış (authenticated) kullanıcılarla sınırlandırıldı. Ayrıca ilgili `using Microsoft.AspNetCore.Authorization;` ifadesi controller'lara eklendi. |
| 🔁 **Alternatifler?** | Daha granüler bir yetkilendirme yapılabilirdi. Örneğin, `[AllowAnonymous]` attribute'u ile sadece `GET` metotlarına anonim erişim izni verilebilir, ancak veri değiştiren `POST`, `PUT`, `DELETE` metotları `[Authorize]` ile korunabilirdi. Ayrıca, `[Authorize(Roles = "Admin")]` gibi rol bazlı yetkilendirme ile sadece belirli rollerdeki kullanıcıların erişimi sağlanabilirdi. |

#### 4. Performans ve Stabilite Problemi: Async/Await Anti-Pattern'leri

| Soru | Açıklama |
|------|-----------|
| ❌ **Sorun neydi?** | `ExamManager` sınıfı içerisinde `async` programlama prensiplerine aykırı iki önemli hata vardı:<br>1. `GetAllAsync` metodu `async` olmasına rağmen veritabanından verileri çekerken `ToList()` gibi senkron bir metot kullanıyordu.<br>2. `CreateAsync` metodu içerisinde, asenkron `CreateAsync` çağrısı `await` ile beklenmek yerine `.Wait()` ile bloklanıyordu. |
| ⚠️ **Neden problemdi?** | 1. **Async over Sync:** `ToList()` kullanımı, asenkron olması gereken bir operasyonu senkron hale getirerek ilgili thread'i bloklar. Bu, uygulamanın ölçeklenebilirliğini ciddi şekilde düşürür ve "thread pool starvation" (thread havuzunun tükenmesi) riskine yol açar.<br>2. **Deadlock Riski:** `.Wait()` kullanımı, özellikle ASP.NET Core gibi bir senkronizasyon bağlamı olan ortamlarda `deadlock` (kilitlenme) riskini beraberinde getirir. Uygulama tamamen donabilir ve yanıt vermez hale gelebilirdi. |
| ✅ **Nasıl çözdünüz?** | 1. `ExamManager.GetAllAsync` içerisindeki `ToList()` çağrısı, `await _unitOfWork.Exams.GetAll(false).ToListAsync()` şeklinde asenkron versiyonuyla değiştirildi.<br>2. `ExamManager.CreateAsync` içerisindeki `.Wait()` çağrısı kaldırılarak yerine `await _unitOfWork.Exams.CreateAsync(addedExamMapping)` ifadesi kullanıldı. Bu sayede operasyonlar non-blocking (engellemeyen) hale getirildi. |
| 🔁 **Alternatifler?** | Bu anti-pattern'lerin tek doğru çözümü, `async/await`'i baştan sona doğru bir şekilde kullanmaktır. Senkron metotlar (`.Result`, `.Wait()`) yerine `await` anahtar kelimesi tercih edilmelidir. Alternatif bir yaklaşım, bu operasyonların tamamen senkron tasarlanması olabilirdi, ancak bu da modern web uygulamalarının ölçeklenebilirlik hedeflerine aykırı olurdu. |
