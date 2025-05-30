# 💉 DiabetesTrack - Diyabet Yönetim Sistemi 💊

### 📋 Proje Hakkında
DiabetesTrack, diyabet hastalarının takibi ve tedavisi için geliştirilmiş kapsamlı bir WPF uygulamasıdır. Bu sistem, doktorların hastalarına özel egzersiz planları oluşturmasına, kan şekeri seviyelerini takip etmesine ve semptomlar doğrultusunda uygun egzersiz türlerini önermesine olanak sağlar. Kullanıcı dostu arayüzü ve güçlü veritabanı entegrasyonu ile sağlık profesyonellerine zaman kazandırır, hastaların tedaviye uyumunu artırır.

### ✨ Özellikler
- 👨‍⚕️ Hasta kaydı ve yönetimi
- 🏃‍♂️ Egzersiz planları oluşturma ve düzenleme
- 📊 Kan şekeri seviyesi takibi ve grafiksel analizi
- 🔍 Semptom bazlı egzersiz önerileri
- 📝 Doktor notları ve tedavi takibi
- 💾 PostgreSQL veritabanı entegrasyonu
- 📱 Kullanıcı dostu arayüz tasarımı
- 🔔 Hatırlatma ve bildirim sistemi

### 🛠️ Teknik Altyapı
- 💻 C# / WPF - Modern ve güçlü kullanıcı arayüzü
- 🗄️ PostgreSQL veritabanı - Güvenli ve ölçeklenebilir veri depolama
- 🔌 Npgsql kütüphanesi - Verimli veritabanı bağlantısı
- 🏗️ Entity modelleri ve servis mimarisi - Sürdürülebilir kod yapısı
- 📊 Grafik kütüphaneleri - Görsel veri sunumu

### 📥 Kurulum
1. 📂 Projeyi bilgisayarınıza indirin
2. 🗄️ PostgreSQL veritabanını kurun ve bağlantı ayarlarını yapılandırın
   - Veritabanı adı: `diabetesdb`
   - Kullanıcı adı ve şifrenizi `Dbconnection` dosyasında güncelleyin
3. 🧰 Visual Studio ile projeyi açın ve bağımlılıkları yükleyin
   - NuGet paketlerini geri yükleyin
4. ▶️ Uygulamayı derleyin ve çalıştırın

### 📱 Kullanım
Sistem doktor ve hasta kullanıcı türlerini destekler. 

#### 👨‍⚕️ Doktor Arayüzü:
- Hasta kayıtları oluşturma ve yönetme
- Egzersiz planları hazırlama
- Tedavi takibi yapma
- Hasta ilerlemesini izleme

#### 👨‍👩‍👧‍👦 Hasta Arayüzü:
- Egzersiz planlarını görüntüleme
- Kan şekeri ölçümlerini kaydetme
- Semptomları raporlama
- İlerleme durumunu takip etme

### 🏗️ Uygulama Mimarisi
Uygulama, servis tabanlı bir mimariye sahiptir. Her bir işlevsellik için ayrı servisler bulunmaktadır:
- 🏃‍♂️ **ExerciseService**: Egzersiz planlarının oluşturulması ve yönetimi
- 👥 **UserService**: Kullanıcı ve hasta yönetimi
- 📊 **GlucoseService**: Kan şekeri seviyesi takibi
- 🩺 **SymptomService**: Semptom kaydı ve analizi

### 📸 Ekran Görüntüleri
![image](https://github.com/user-attachments/assets/3defa83e-5e28-4e6a-b39f-11487c6b3c8a)

![image](https://github.com/user-attachments/assets/18d6b76d-5812-4efc-9b68-717d494dcf33)

![image](https://github.com/user-attachments/assets/a0f06ceb-1b4b-43c2-976c-4bc380110c5d)

![image](https://github.com/user-attachments/assets/bbdfa0c1-c293-4d43-8bb5-683c29e45033)




