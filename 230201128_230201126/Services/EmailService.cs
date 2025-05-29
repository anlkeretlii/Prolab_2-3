using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace wpf_prolab.Services
{
    public class EmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderPassword;
        private readonly bool _enableSsl;

        public EmailService()
        {
            // E-posta ayarlarını App.config dosyasından al
            _smtpHost = ConfigurationManager.AppSettings["SmtpHost"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
            _senderEmail = ConfigurationManager.AppSettings["SenderEmail"] ?? "diabetestrack.system@gmail.com";
            _senderPassword = ConfigurationManager.AppSettings["SenderPassword"] ?? "your-app-password";
            _enableSsl = bool.Parse(ConfigurationManager.AppSettings["EnableSsl"] ?? "true");
        }

        /// <summary>
        /// Hastaya giriş bilgilerini e-posta ile gönderir
        /// </summary>
        /// <param name="patientEmail">Hastanın e-posta adresi</param>
        /// <param name="patientName">Hastanın adı soyadı</param>
        /// <param name="tcId">TC Kimlik numarası (kullanıcı adı)</param>
        /// <param name="temporaryPassword">Geçici şifre</param>
        /// <returns>E-posta gönderim durumu</returns>
        public async Task<bool> SendLoginCredentialsAsync(string patientEmail, string patientName, string tcId, string temporaryPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(patientEmail))
                {
                    throw new ArgumentException("E-posta adresi boş olamaz");
                }

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail, "DiabetesTrack Sistemi"),
                    Subject = "DiabetesTrack - Giriş Bilgileriniz",
                    IsBodyHtml = true,
                    Body = GenerateEmailBody(patientName, tcId, temporaryPassword)
                };

                mailMessage.To.Add(patientEmail);

                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    // Gmail için özel ayarlar
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
                    smtpClient.EnableSsl = _enableSsl;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.Timeout = 30000; // 30 saniye timeout

                    await smtpClient.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (SmtpException smtpEx)
            {
                // SMTP özel hatası
                System.Diagnostics.Debug.WriteLine($"SMTP Hatası: {smtpEx.Message}");
                System.Windows.MessageBox.Show(
                    $"E-posta gönderim hatası: {smtpEx.Message}\n\n" +
                    "Lütfen e-posta ayarlarınızı kontrol edin:\n" +
                    "• Yahoo Mail için 'Daha az güvenli uygulamalar' seçeneğini etkinleştirin\n" +
                    "• Veya uygulama şifresi kullanın",
                    "E-posta Hatası", 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Warning);
                return false;
            }
            catch (Exception ex)
            {
                // Diğer hatalar
                System.Diagnostics.Debug.WriteLine($"E-posta gönderim hatası: {ex.Message}");
                System.Windows.MessageBox.Show(
                    $"E-posta gönderim hatası: {ex.Message}",
                    "Hata", 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// E-posta içeriğini oluşturur
        /// </summary>
        private string GenerateEmailBody(string patientName, string tcId, string temporaryPassword)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #2c5aa0; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .credentials {{ background-color: #e8f4fd; padding: 15px; margin: 20px 0; border-left: 4px solid #2c5aa0; }}
        .warning {{ background-color: #fff3cd; padding: 15px; margin: 20px 0; border-left: 4px solid #ffc107; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>💉 DiabetesTrack</h1>
            <p>Diyabet Yönetim Sistemi</p>
        </div>
        
        <div class='content'>
            <h2>Merhaba {patientName},</h2>
            
            <p>DiabetesTrack sistemine hoş geldiniz! Doktorunuz tarafından sisteme kaydınız oluşturulmuştur.</p>
            
            <div class='credentials'>
                <h3>🔐 Giriş Bilgileriniz:</h3>
                <p><strong>Kullanıcı Adı:</strong> {tcId}</p>
                <p><strong>Geçici Şifre:</strong> {temporaryPassword}</p>
            </div>
            
            <div class='warning'>
                <h3>⚠️ Güvenlik Uyarısı:</h3>
                <p>Bu geçici şifre ile ilk girişinizde mutlaka şifrenizi değiştirin. Bu e-postayı güvenli bir yerde saklayın ve başkalarıyla paylaşmayın.</p>
            </div>
            
            <h3>📱 Sisteme Nasıl Giriş Yapabilirsiniz:</h3>
            <ol>
                <li>DiabetesTrack uygulamasını açın</li>
                <li>TC Kimlik numaranızı (kullanıcı adı) girin</li>
                <li>Yukarıda belirtilen geçici şifreyi girin</li>
                <li>İlk girişte yeni bir şifre belirleyin</li>
            </ol>
            
            <h3>📋 Sistemde Neler Yapabilirsiniz:</h3>
            <ul>
                <li>Egzersiz planlarınızı görüntüleyebilirsiniz</li>
                <li>Kan şekeri ölçümlerinizi kaydedebilirsiniz</li>
                <li>Semptomlarınızı raporlayabilirsiniz</li>
                <li>İlerleme durumunuzu takip edebilirsiniz</li>
                <li>Doktorunuzla iletişim kurabilirsiniz</li>
            </ul>
            
            <p>Herhangi bir sorunuz olursa doktorunuzla veya sistem yöneticisiyle iletişime geçebilirsiniz.</p>
            
            <p>Sağlıklı günler dileriz!</p>
        </div>
        
        <div class='footer'>
            <p>Bu e-posta DiabetesTrack sistemi tarafından otomatik olarak gönderilmiştir.</p>
            <p>© 2024 DiabetesTrack - Tüm hakları saklıdır.</p>
        </div>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// E-posta adresinin geçerli olup olmadığını kontrol eder
        /// </summary>
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Rastgele güvenli şifre üretir
        /// </summary>
        public string GenerateTemporaryPassword(int length = 8)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@#$%";
            var random = new Random();
            var password = new char[length];

            for (int i = 0; i < length; i++)
            {
                password[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(password);
        }
    }
} 