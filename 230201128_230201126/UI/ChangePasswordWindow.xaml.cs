using System;
using System.Windows;
using wpf_prolab.Models;
using wpf_prolab.Services;
using BCrypt.Net;

namespace wpf_prolab.UI
{
    /// <summary>
    /// Interaction logic for ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        private readonly UserService _userService;
        private readonly User _currentUser;

        public ChangePasswordWindow(User currentUser)
        {
            InitializeComponent();
            _userService = new UserService();
            _currentUser = currentUser;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidateForm();
        }

        private void ValidateForm()
        {
            if (btnSave == null) return;

            bool isValid = !string.IsNullOrWhiteSpace(txtCurrentPassword.Password) &&
                          !string.IsNullOrWhiteSpace(txtNewPassword.Password) &&
                          !string.IsNullOrWhiteSpace(txtConfirmPassword.Password) &&
                          txtNewPassword.Password == txtConfirmPassword.Password &&
                          txtNewPassword.Password.Length >= 6;

            // Şifre eşleşme kontrolü
            bool passwordsMatch = txtNewPassword.Password == txtConfirmPassword.Password;
            txtPasswordError.Visibility = passwordsMatch || string.IsNullOrWhiteSpace(txtConfirmPassword.Password) 
                ? Visibility.Collapsed 
                : Visibility.Visible;

            btnSave.IsEnabled = isValid;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Mevcut şifreyi kontrol et
                if (!BCrypt.Net.BCrypt.Verify(txtCurrentPassword.Password, _currentUser.Password))
                {
                    MessageBox.Show("Mevcut şifre yanlış!", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Yeni şifre uzunluk kontrolü
                if (txtNewPassword.Password.Length < 6)
                {
                    MessageBox.Show("Yeni şifre en az 6 karakter olmalıdır!", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Yeni şifreyi hash'le ve güncelle
                string hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(txtNewPassword.Password);
                bool passwordUpdated = _userService.UpdatePassword(_currentUser.Id, hashedNewPassword);

                if (passwordUpdated)
                {
                    // Geçici şifre durumunu false yap
                    _userService.SetTemporaryPasswordStatus(_currentUser.Id, false);

                    MessageBox.Show("Şifreniz başarıyla değiştirildi!", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Şifre değiştirme işlemi başarısız!", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 