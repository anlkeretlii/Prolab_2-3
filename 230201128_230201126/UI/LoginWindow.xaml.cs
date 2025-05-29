using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using wpf_prolab.Services;
using wpf_prolab.Models;

namespace wpf_prolab.UI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;

        public LoginWindow()
        {
            InitializeComponent();
            _authService = new AuthService();
            
            // Set initial focus to TC ID field
            Loaded += (s, e) => txtTcId.Focus();
            
            // Allow pressing Enter in password field to login
            txtPassword.KeyDown += (s, e) => 
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                    AttemptLogin();
            };
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            AttemptLogin();
        }

        private void AttemptLogin()
        {
            // Clear previous error message
            HideError();

            // Validate input
            if (string.IsNullOrWhiteSpace(txtTcId.Text))
            {
                ShowError("Lütfen T.C. Kimlik No giriniz.");
                txtTcId.Focus();
                return;
            }

            if (txtTcId.Text.Length != 11 || !IsNumeric(txtTcId.Text))
            {
                ShowError("T.C. Kimlik No 11 haneli ve sadece rakamlardan oluşmalıdır.");
                txtTcId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                ShowError("Lütfen şifre giriniz.");
                txtPassword.Focus();
                return;
            }

            // Attempt login
            bool loginSuccess = _authService.Login(txtTcId.Text, txtPassword.Password);

            if (loginSuccess)
            {
                // Determine which form to show based on user type
                if (_authService.IsDoctor())
                {
                    // Open doctor dashboard
                    DoctorDashboard dashboard = new DoctorDashboard();
                    dashboard.Show();
                    this.Close();
                }
                else if (_authService.IsPatient())
                {
                    // Open patient dashboard
                    PatientDashboard dashboard = new PatientDashboard();
                    dashboard.Show();
                    this.Close();
                }
            }
            else
            {
                ShowError("T.C. Kimlik No veya şifre yanlış. Lütfen tekrar deneyiniz.");
                txtPassword.Password = string.Empty;
                txtPassword.Focus();
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            lblError.Text = string.Empty;
            lblError.Visibility = Visibility.Collapsed;
        }

        private bool IsNumeric(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }
    }
} 