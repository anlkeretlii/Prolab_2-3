using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using wpf_prolab.Models;
using wpf_prolab.Services;
using BCrypt.Net;
using System.Linq;

namespace wpf_prolab.UI
{
    /// <summary>
    /// Interaction logic for AddPatientWindow.xaml
    /// </summary>
    public partial class AddPatientWindow : Window
    {
        private readonly UserService _userService;
        private readonly EmailService _emailService;
        private readonly int _doctorId;

        public User NewPatient { get; private set; }

        public AddPatientWindow(int doctorId)
        {
            InitializeComponent();
            
            _userService = new UserService();
            _emailService = new EmailService();
            _doctorId = doctorId;
            
            // Set default values
            dpBirthDate.SelectedDate = DateTime.Today.AddYears(-30);
            dpDiagnosisDate.SelectedDate = DateTime.Today;
            cmbDiabetesType.SelectedIndex = 0;
            
            // Attach Loaded event
            this.Loaded += Window_Loaded;
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize form after UI is fully loaded
            UpdateFormValidity();
        }

        #region Validation Methods

        private void txtTcId_TextChanged(object sender, TextChangedEventArgs e)
        {
            string tcId = txtTcId.Text;
            bool isValid = ValidateTcId(tcId);
            txtTcIdError.Visibility = isValid ? Visibility.Collapsed : Visibility.Visible;
            UpdateFormValidity();
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            string email = txtEmail.Text;
            bool isValid = ValidateEmail(email);
            txtEmailError.Visibility = isValid || string.IsNullOrWhiteSpace(email) ? Visibility.Collapsed : Visibility.Visible;
            UpdateFormValidity();
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidatePassword();
            UpdateFormValidity();
        }

        private void txtConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidatePassword();
            UpdateFormValidity();
        }

        // TextChanged event handler for the form 
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFormValidity();
        }

        // DatePicker SelectedDateChanged event handler
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFormValidity();
        }

        // RadioButton Checked event handler
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateFormValidity();
        }

        // ComboBox SelectionChanged event handler
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFormValidity();
        }

        // The main form validation logic, now a regular method not an event handler
        private void UpdateFormValidity()
        {
            // Avoid NullReferenceException by checking if btnSave is null
            if (btnSave == null) return;
            
            try
            {
                // Check if all required fields are filled and valid
                // Artık e-posta zorunlu, şifre alanları kaldırıldı
                bool isValid = !string.IsNullOrWhiteSpace(txtTcId.Text) && 
                              ValidateTcId(txtTcId.Text) &&
                              !string.IsNullOrWhiteSpace(txtFirstName.Text) &&
                              !string.IsNullOrWhiteSpace(txtLastName.Text) &&
                              dpBirthDate.SelectedDate.HasValue &&
                              !string.IsNullOrWhiteSpace(txtEmail.Text) &&
                              ValidateEmail(txtEmail.Text);

                btnSave.IsEnabled = isValid;
            }
            catch (Exception ex)
            {
                // Handle any exceptions during validation
                System.Diagnostics.Debug.WriteLine($"UpdateFormValidity error: {ex.Message}");
            }
        }

        private bool ValidateTcId(string tcId)
        {
            // TC ID should be 11 digits
            return !string.IsNullOrWhiteSpace(tcId) && tcId.Length == 11 && tcId.All(char.IsDigit);
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false; // Artık e-posta zorunlu

            // Simple email validation
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        private void ValidatePassword()
        {
            bool passwordsMatch = txtPassword.Password == txtConfirmPassword.Password;
            txtPasswordError.Visibility = passwordsMatch || string.IsNullOrWhiteSpace(txtConfirmPassword.Password) 
                ? Visibility.Collapsed 
                : Visibility.Visible;
        }

        #endregion

        #region Button Event Handlers

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if TC ID already exists
                var existingUser = _userService.GetUserByTcId(txtTcId.Text);
                if (existingUser != null)
                {
                    MessageBox.Show("Bu TC Kimlik numarası ile kayıtlı bir kullanıcı zaten var.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // E-posta adresi kontrolü
                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("E-posta adresi gereklidir. Hasta giriş bilgileri e-posta ile gönderilecektir.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Geçici şifre oluştur
                string temporaryPassword = _emailService.GenerateTemporaryPassword(10);

                // Create new patient user
                var patient = new User
                {
                    TcId = txtTcId.Text,
                    FirstName = txtFirstName.Text,
                    LastName = txtLastName.Text,
                    BirthDate = dpBirthDate.SelectedDate.Value,
                    Gender = rbMale.IsChecked.Value ? 'M' : 'F',
                    Email = txtEmail.Text,
                    // Hash the temporary password
                    Password = BCrypt.Net.BCrypt.HashPassword(temporaryPassword),
                    UserType = UserType.Patient,
                    IsTemporaryPassword = true, // Geçici şifre ile oluşturuluyor
                    // Additional patient-specific fields in PatientProfile
                    PatientProfile = new PatientProfile
                    {
                        DiabetesType = cmbDiabetesType.SelectedIndex,
                        DiagnosisDate = dpDiagnosisDate.SelectedDate ?? DateTime.Today,
                        DoctorNotes = txtNotes.Text
                    }
                };

                // Kaydetme butonunu devre dışı bırak
                btnSave.IsEnabled = false;
                btnSave.Content = "Kaydediliyor...";

                // Save to database
                int newPatientId = _userService.CreateUser(patient);

                if (newPatientId > 0)
                {
                    // Assign to doctor
                    bool assigned = _userService.AssignPatientToDoctor(_doctorId, newPatientId);
                    
                    if (assigned)
                    {
                        // Set the NewPatient property so the parent window can access it
                        patient.Id = newPatientId;
                        NewPatient = patient;

                        // E-posta gönder
                        bool emailSent = await _emailService.SendLoginCredentialsAsync(patient.Email, patient.FullName, patient.TcId, temporaryPassword);

                        if (emailSent)
                        {
                            MessageBox.Show(
                                $"Hasta başarıyla eklendi!\n\n" +
                                $"Giriş bilgileri {patient.Email} adresine gönderildi.\n" +
                                $"Hasta geçici şifresi ile sisteme giriş yapabilir.",
                                "Başarılı", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show(
                                $"Hasta başarıyla eklendi ancak e-posta gönderilemedi.\n\n" +
                                $"Giriş bilgileri:\n" +
                                $"Kullanıcı Adı: {patient.TcId}\n" +
                                $"Geçici Şifre: {temporaryPassword}\n\n" +
                                $"Bu bilgileri hastaya manuel olarak iletebilirsiniz.",
                                "Uyarı", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Warning);
                        }
                        
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Hasta eklendi ancak doktora atanamadı.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                        DialogResult = true;
                        Close();
                    }
                }
                else
                {
                    MessageBox.Show("Hasta eklenirken bir hata oluştu.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Butonu tekrar aktif et
                btnSave.IsEnabled = true;
                btnSave.Content = "Kaydet";
            }
        }

        #endregion
    }
} 