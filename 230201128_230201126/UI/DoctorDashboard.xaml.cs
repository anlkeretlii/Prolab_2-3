using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Data;
using wpf_prolab.Models;
using wpf_prolab.Services;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;
using System.IO;
using System.Windows.Media.Imaging;
using Npgsql;

namespace wpf_prolab.UI
{
    /// <summary>
    /// Interaction logic for DoctorDashboard.xaml
    /// </summary>
    public partial class DoctorDashboard : Window, INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private readonly BloodGlucoseService _bloodGlucoseService;
        private readonly DailyTrackerService _dailyTrackerService;
        private readonly DietService _dietService;
        private readonly ExerciseService _exerciseService;
        private readonly SymptomService _symptomService;
        private readonly AlertService _alertService;
        private readonly RecommendationService _recommendationService;

        private List<User> _allPatients;
        private User _selectedPatient;

        // For Chart binding
        private SeriesCollection _glucoseSeries;
        public SeriesCollection GlucoseSeries
        {
            get { return _glucoseSeries; }
            set
            {
                _glucoseSeries = value;
                OnPropertyChanged(nameof(GlucoseSeries));
            }
        }

        private Func<double, string> _dateTimeFormatter;
        public Func<double, string> DateTimeFormatter
        {
            get { return _dateTimeFormatter; }
            set
            {
                _dateTimeFormatter = value;
                OnPropertyChanged(nameof(DateTimeFormatter));
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DoctorDashboard()
        {
            InitializeComponent();

            try
            {
            // Initialize services
            _userService = new UserService();
            _bloodGlucoseService = new BloodGlucoseService();
                _dailyTrackerService = new DailyTrackerService();
            _dietService = new DietService();
            _exerciseService = new ExerciseService();
            _symptomService = new SymptomService();
            _alertService = new AlertService();
            _recommendationService = new RecommendationService();

            // Set doctor name
            txtDoctorName.Text = $"Dr. {AuthService.CurrentUser.FirstName} {AuthService.CurrentUser.LastName}";

                // Load profile information
                LoadDoctorProfileInfo();

            // Set default date filters
            DateTime today = DateTime.Today;
            DateTime monthAgo = today.AddDays(-30);
            
            dpGlucoseStart.SelectedDate = monthAgo;
            dpGlucoseEnd.SelectedDate = today;
                dpGlucoseDate.SelectedDate = today;
            
            dpSymptomsStart.SelectedDate = monthAgo;
            dpSymptomsEnd.SelectedDate = today;
                
                // Initialize chart
                InitializeChart();
                
                // Set DataContext for binding
                DataContext = this;

            // Load data
            LoadAllPatients();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Doktor paneli yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeChart()
        {
            try
            {
                // Create new SeriesCollection if null
                if (GlucoseSeries == null)
                {
                    GlucoseSeries = new SeriesCollection();
                }
                
                // IMPORTANT: Configure the DoctorMeasurementPoint mapping for LiveCharts globally
                // This tells LiveCharts how to plot our custom point type
                var mapper = Mappers.Xy<DoctorMeasurementPoint>()
                    .X(point => point.DateTime.Ticks)    // Use DateTime ticks as X value
                    .Y(point => (double)point.Value);    // Use blood glucose value as Y value
                
                // Configure the mapper globally
                LiveCharts.Charting.For<DoctorMeasurementPoint>(mapper);
                
                // Set up the date formatter
                DateTimeFormatter = value => new DateTime((long)value).ToString("dd.MM.yy HH:mm");
                
                // Check if chart control exists
                if (chartBloodGlucose == null)
                {
                    MessageBox.Show("Grafik bileşeni bulunamadı.", "Grafik Hatası", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // Clear any existing axes
                if (chartBloodGlucose.AxisX != null)
                {
                    chartBloodGlucose.AxisX.Clear();
                }
                else
                {
                    // If AxisX is null, cannot continue
                    MessageBox.Show("Grafik X ekseni oluşturulamadı.", "Grafik Hatası", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                if (chartBloodGlucose.AxisY != null)
                {
                    chartBloodGlucose.AxisY.Clear();
                }
                else
                {
                    // If AxisY is null, cannot continue
                    MessageBox.Show("Grafik Y ekseni oluşturulamadı.", "Grafik Hatası", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // Add default X axis with date formatter
                chartBloodGlucose.AxisX.Add(new LiveCharts.Wpf.Axis
                {
                    LabelFormatter = DateTimeFormatter,
                    Title = "Tarih/Saat"
                });
                
                // Add default Y axis
                chartBloodGlucose.AxisY.Add(new LiveCharts.Wpf.Axis
                {
                    MinValue = 40,
                    MaxValue = 200,
                    Title = "Kan Şekeri (mg/dL)"
                });
                
                // Set DataContext for chart binding
                chartBloodGlucose.DataContext = this;
            }
            catch (Exception ex)
            {
                // Grafik başlatma hatası normal işlemi bloke etmesin
                MessageBox.Show($"Grafik başlatılırken hata oluştu: {ex.Message}", "Grafik Hatası", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadDoctorProfileInfo()
        {
            // Load basic profile information
            txtDoctorProfileTcId.Text = AuthService.CurrentUser.TcId;
            txtDoctorProfileName.Text = $"{AuthService.CurrentUser.FirstName} {AuthService.CurrentUser.LastName}";
            txtDoctorProfileBirthDate.Text = AuthService.CurrentUser.BirthDate.ToString("dd.MM.yyyy");
            txtDoctorProfileGender.Text = AuthService.CurrentUser.Gender == 'M' ? "Erkek" : "Kadın";
            txtDoctorProfileEmail.Text = AuthService.CurrentUser.Email;
            
            // Load profile picture if exists
            if (AuthService.CurrentUser.ProfilePicture != null)
            {
                LoadDoctorProfilePicture(AuthService.CurrentUser.ProfilePicture);
            }
        }
        
        private void LoadDoctorProfilePicture(byte[] imageData)
        {
            if (imageData != null && imageData.Length > 0)
            {
                try
                {
                    // Convert byte array to image
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new MemoryStream(imageData);
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    
                    // Display image
                    imgDoctorProfilePicture.Source = bitmapImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Profil resmi yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        private void btnUploadDoctorProfilePicture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create OpenFileDialog
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                
                // Set filter for file extension and default file extension
                dlg.DefaultExt = ".jpg";
                dlg.Filter = "Resim Dosyaları (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
                
                // Display OpenFileDialog
                bool? result = dlg.ShowDialog();
                
                // Process selected file
                if (result == true)
                {
                    // Get selected file path
                    string filename = dlg.FileName;
                    
                    // Read file as byte array
                    byte[] imageData = File.ReadAllBytes(filename);
                    
                    // Update profile picture in database
                    bool success = _userService.UpdateProfilePicture(AuthService.CurrentUser.Id, imageData);
                    
                    if (success)
                    {
                        // Update current user object
                        AuthService.CurrentUser.ProfilePicture = imageData;
                        
                        // Display profile picture
                        LoadDoctorProfilePicture(imageData);
                        
                        MessageBox.Show("Profil resmi başarıyla güncellendi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Profil resmi güncellenirken bir hata oluştu.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void btnChangeDoctorPassword_Click(object sender, RoutedEventArgs e)
        {
            string currentPassword = txtDoctorCurrentPassword.Password;
            string newPassword = txtDoctorNewPassword.Password;
            string confirmPassword = txtDoctorConfirmPassword.Password;
            
            // Validate inputs
            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Yeni şifre ve tekrarı eşleşmiyor.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (newPassword.Length < 6)
            {
                MessageBox.Show("Yeni şifre en az 6 karakter olmalıdır.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            // Check current password
            AuthService authService = new AuthService();
            bool isValidPassword = authService.ValidatePassword(AuthService.CurrentUser.TcId, currentPassword);
            
            if (!isValidPassword)
            {
                MessageBox.Show("Mevcut şifreniz hatalı.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            // Update password
            string hashedPassword = authService.HashPassword(newPassword);
            bool success = _userService.UpdatePassword(AuthService.CurrentUser.Id, hashedPassword);
            
            if (success)
            {
                MessageBox.Show("Şifreniz başarıyla değiştirildi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                txtDoctorCurrentPassword.Clear();
                txtDoctorNewPassword.Clear();
                txtDoctorConfirmPassword.Clear();
            }
            else
            {
                MessageBox.Show("Şifre değiştirme işlemi sırasında bir hata oluştu.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Data Loading Methods

        private void LoadAllPatients()
        {
            // Get all patients assigned to the current doctor
            _allPatients = _userService.GetPatientsByDoctorId(AuthService.CurrentUser.Id);
            
            // Display in the list
            lstPatients.ItemsSource = _allPatients;
            
            // Fill recommendation patient combobox
            cmbRecommendationPatient.ItemsSource = _allPatients;
            
            // Select the first patient if available
            if (_allPatients.Count > 0)
            {
                lstPatients.SelectedIndex = 0;
            }
            else
            {
                // No patients assigned
                MessageBox.Show("Henüz size atanmış bir hasta bulunmamaktadır.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadPatientDetails(User patient)
        {
            if (patient == null) return;

            _selectedPatient = patient;

            // Display basic patient info
            txtPatientName.Text = patient.FullName;
            txtPatientInitials.Text = $"{patient.FirstName[0]}{patient.LastName[0]}";
            txtPatientDetails.Text = $"TC: {patient.TcId} | {patient.Age} yaş | {(patient.Gender == 'M' ? "Erkek" : "Kadın")}";

            // Load blood glucose data
            LoadBloodGlucoseData();

            // Load diet and exercise adherence
            LoadDietExerciseAdherence();

            // Load diet plans
            LoadDietPlans();

            // Load exercise plans
            LoadExercisePlans();

            // Load symptoms
            LoadSymptoms();

            // Load alerts
            LoadAlerts();
            
            // Initialize blood glucose chart with default date range (last 30 days)
            DateTime today = DateTime.Today;
            DateTime monthAgo = today.AddDays(-30);
            LoadBloodGlucoseChart(monthAgo, today);
        }

        private void LoadBloodGlucoseData()
        {
            // Get the most recent blood glucose reading
            var recentMeasurement = _bloodGlucoseService.GetLatestMeasurement(_selectedPatient.Id);
            
            if (recentMeasurement != null)
            {
                txtLastGlucose.Text = $"{recentMeasurement.MeasurementValue} mg/dL";
                
                // Format time display
                DateTime measurementTime = recentMeasurement.MeasurementTime;
                if (measurementTime.Date == DateTime.Today)
                {
                    txtLastGlucoseTime.Text = $"Bugün {measurementTime:HH:mm}";
                }
                else if (measurementTime.Date == DateTime.Today.AddDays(-1))
                {
                    txtLastGlucoseTime.Text = $"Dün {measurementTime:HH:mm}";
                }
                else
                {
                    txtLastGlucoseTime.Text = measurementTime.ToString("dd.MM.yyyy HH:mm");
                }
                
                // Set color based on value
                if (recentMeasurement.MeasurementValue < 70)
                {
                    txtLastGlucose.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if (recentMeasurement.MeasurementValue > 180)
                {
                    txtLastGlucose.Foreground = new SolidColorBrush(Colors.DarkRed);
                }
                else if (recentMeasurement.MeasurementValue > 110)
                {
                    txtLastGlucose.Foreground = new SolidColorBrush(Colors.Orange);
                }
                else
                {
                    txtLastGlucose.Foreground = new SolidColorBrush(Colors.Green);
                }
            }
            else
            {
                txtLastGlucose.Text = "Veri yok";
                txtLastGlucoseTime.Text = "-";
            }

            // Get recent measurements
            var recentMeasurements = _bloodGlucoseService.GetMeasurementsByDateRange(
                _selectedPatient.Id, 
                DateTime.Today.AddDays(-7), 
                DateTime.Today.AddDays(1)
            );

            // Convert to view model with status indicators
            var measurementsWithStatus = recentMeasurements.Select(m => new DoctorBloodGlucoseViewModel
            {
                Id = m.Id,
                MeasurementValue = m.MeasurementValue,
                MeasurementTime = m.MeasurementTime,
                MeasurementType = m.MeasurementType.ToString(),
                StatusColor = GetStatusColorForGlucose(m.MeasurementValue),
                StatusText = GetStatusTextForGlucose(m.MeasurementValue)
            }).ToList();

            gridRecentMeasurements.ItemsSource = measurementsWithStatus;
        }

        private void LoadDietExerciseAdherence()
        {
            // Calculate diet adherence percentage for the last 30 days
            DateTime start = DateTime.Today.AddDays(-30);
            DateTime end = DateTime.Today;
            
            // Get daily tracking records for the selected patient in date range
            var dailyRecords = _dailyTrackerService.GetDailyTrackersByDateRange(_selectedPatient.Id, start, end);
            
            if (dailyRecords.Count > 0)
            {
                // Calculate diet adherence
                int dietFollowedCount = dailyRecords.Count(r => r.DietFollowed);
                decimal dietAdherencePercentage = (decimal)dietFollowedCount / dailyRecords.Count * 100;
                txtDietAdherence.Text = $"%{Math.Round(dietAdherencePercentage)}";
                
                // Calculate exercise adherence
                int exerciseDoneCount = dailyRecords.Count(r => r.ExerciseDone);
                decimal exerciseAdherencePercentage = (decimal)exerciseDoneCount / dailyRecords.Count * 100;
                txtExerciseAdherence.Text = $"%{Math.Round(exerciseAdherencePercentage)}";
            }
            else
            {
                txtDietAdherence.Text = "Veri yok";
                txtExerciseAdherence.Text = "Veri yok";
            }
        }

        private void LoadDietPlans()
        {
            // Get diet plans for the patient
            var dietPlans = _dietService.GetDietsForPatient(_selectedPatient.Id);
            
            // Convert to view model
            var dietPlanViewModels = dietPlans.Select(d => new DoctorDietViewModel
            {
                Id = d.Id,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                EndDateDisplay = d.EndDate.HasValue ? d.EndDate.Value.ToString("dd.MM.yyyy") : "Süresiz",
                DietType = d.DietType,
                DietTypeDisplay = GetDietTypeDisplay(d.DietType),
                DoctorNotes = d.DoctorNotes
            }).ToList();
            
            // Display in the grid
            dgDietPlans.ItemsSource = dietPlanViewModels;
        }

        private void LoadExercisePlans()
        {
            // Get exercise plans for the patient
            var exercisePlans = _exerciseService.GetExercisesForPatient(_selectedPatient.Id);
            
            // Convert to view model
            var exercisePlanViewModels = exercisePlans.Select(e => new DoctorExerciseViewModel
            {
                Id = e.Id,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                EndDateDisplay = e.EndDate.HasValue ? e.EndDate.Value.ToString("dd.MM.yyyy") : "Süresiz",
                ExerciseType = e.ExerciseType,
                ExerciseTypeDisplay = GetExerciseTypeDisplay(e.ExerciseType),
                DoctorNotes = e.DoctorNotes
            }).ToList();
            
            // Display in the grid
            dgExercisePlans.ItemsSource = exercisePlanViewModels;
        }

        private void LoadSymptoms()
        {
            // Get date range from UI
            DateTime start = dpSymptomsStart.SelectedDate ?? DateTime.Today.AddDays(-30);
            DateTime end = dpSymptomsEnd.SelectedDate ?? DateTime.Today;
            
            // Get symptoms for the patient in date range
            var symptoms = _symptomService.GetSymptomsByDateRange(_selectedPatient.Id, start, end);
            
            // Convert to view model
            var symptomViewModels = symptoms.Select(s => new DoctorSymptomViewModel
            {
                Id = s.Id,
                SymptomDate = s.SymptomDate,
                Symptom = s.SymptomType,
                SymptomDisplay = GetSymptomDisplay(s.SymptomType),
                Intensity = s.Intensity,
                Notes = s.Notes
            }).ToList();
            
            gridSymptoms.ItemsSource = symptomViewModels;
            
            // Ensure grid is visible and entry panel is hidden
            panelAddDoctorSymptom.Visibility = Visibility.Collapsed;
            gridSymptoms.Visibility = Visibility.Visible;
        }

        private void LoadAlerts()
        {
            // Get alerts for the patient
            var allAlerts = _alertService.GetAlertsByPatientId(_selectedPatient.Id);
            
            // Convert to view model
            var alertViewModels = allAlerts.Select(a => new DoctorAlertViewModel
            {
                Id = a.Id,
                AlertType = GetAlertTypeDisplay(a.AlertType),
                AlertMessage = a.AlertMessage,
                CreatedAt = a.CreatedAt,
                IsRead = a.IsRead,
                IsReadText = a.IsRead ? "Okundu" : "Okunmadı",
                IsReadColor = a.IsRead ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red),
                IsReadButtonVisibility = a.IsRead ? Visibility.Collapsed : Visibility.Visible
            }).ToList();
            
            // Show recent alerts in the overview tab
            gridRecentAlerts.ItemsSource = alertViewModels.Take(5).ToList();
            
            // Show all alerts in the alerts tab
            gridAllAlerts.ItemsSource = alertViewModels;
        }

        #endregion

        #region Helper Methods

        private string GetDietTypeDisplay(DietType dietType)
        {
            switch (dietType)
            {
                case DietType.LowSugar:
                    return "Az Şekerli Diyet";
                case DietType.SugarFree:
                    return "Şekersiz Diyet";
                case DietType.BalancedDiet:
                    return "Dengeli Beslenme";
                default:
                    return dietType.ToString();
            }
        }

        private string GetExerciseTypeDisplay(ExerciseType exerciseType)
        {
            switch (exerciseType)
            {
                case ExerciseType.Walking:
                    return "Yürüyüş";
                case ExerciseType.Cycling:
                    return "Bisiklet";
                case ExerciseType.ClinicalExercise:
                    return "Klinik Egzersiz";
                default:
                    return exerciseType.ToString();
            }
        }

        private string GetSymptomDisplay(SymptomType symptom)
        {
            switch (symptom)
            {
                case SymptomType.Polyuria:
                    return "Poliüri (Sık idrara çıkma)";
                case SymptomType.Polyphagia:
                    return "Polifaji (Aşırı açlık hissi)";
                case SymptomType.Polydipsia:
                    return "Polidipsi (Aşırı susama hissi)";
                case SymptomType.Neuropathy:
                    return "Nöropati";
                case SymptomType.WeightLoss:
                    return "Kilo kaybı";
                case SymptomType.Fatigue:
                    return "Yorgunluk";
                case SymptomType.SlowHealingWounds:
                    return "Yaraların yavaş iyileşmesi";
                case SymptomType.BlurredVision:
                    return "Bulanık görme";
                default:
                    return symptom.ToString();
            }
        }

        private string GetAlertTypeDisplay(string alertType)
        {
            switch (alertType)
            {
                case "EmergencyAlert":
                    return "Acil Uyarı";
                case "WarningAlert":
                    return "Uyarı";
                case "InfoAlert":
                    return "Bilgi";
                default:
                    return alertType;
            }
        }

        private string GetStatusTextForGlucose(decimal value)
        {
            if (value < 70)
                return "Düşük";
            if (value <= 110)
                return "Normal";
            if (value <= 180)
                return "Yüksek";
            return "Çok Yüksek";
        }

        private SolidColorBrush GetStatusColorForGlucose(decimal value)
        {
            if (value < 70)
                return new SolidColorBrush(Colors.Red);
            if (value <= 110)
                return new SolidColorBrush(Colors.Green);
            if (value <= 180)
                return new SolidColorBrush(Colors.Orange);
            return new SolidColorBrush(Colors.DarkRed);
        }

        private void FilterPatientList(string searchText = null, int? glucoseFilterIndex = null)
        {
            if (_allPatients == null) return;

            IEnumerable<User> filteredPatients = _allPatients;

            // Apply text search filter
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string searchLower = searchText.ToLower();
                filteredPatients = filteredPatients.Where(p => 
                    p.FirstName.ToLower().Contains(searchLower) || 
                    p.LastName.ToLower().Contains(searchLower) || 
                    p.TcId.Contains(searchLower));
            }

            // Apply glucose level filter
            if (glucoseFilterIndex.HasValue && glucoseFilterIndex.Value > 0)
            {
                filteredPatients = filteredPatients.Where(p => {
                    var latestReading = _bloodGlucoseService.GetLatestMeasurement(p.Id);
                    if (latestReading == null) return false;

                    decimal value = latestReading.MeasurementValue;
                    switch (glucoseFilterIndex)
                    {
                        case 1: // Düşük
                            return value < 70;
                        case 2: // Normal
                            return value >= 70 && value <= 110;
                        case 3: // Yüksek
                            return value > 110 && value <= 180;
                        case 4: // Çok Yüksek
                            return value > 180;
                        default:
                            return true;
                    }
                });
            }

            // Update list
            lstPatients.ItemsSource = filteredPatients.ToList();
        }

        #endregion


        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Log out current user
            AuthService authService = new AuthService();
            authService.Logout();

            // Show login window
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();

            // Close this window
            this.Close();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterPatientList(txtSearch.Text, cmbGlucoseFilter.SelectedIndex);
        }

        private void cmbGlucoseFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterPatientList(txtSearch.Text, cmbGlucoseFilter.SelectedIndex);
        }

        private void lstPatients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstPatients.SelectedItem is User selectedPatient)
            {
                LoadPatientDetails(selectedPatient);
            }
        }

        private void btnAddPatient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addPatientWindow = new AddPatientWindow(AuthService.CurrentUser.Id);
                bool? result = addPatientWindow.ShowDialog();
                
                if (result == true && addPatientWindow.NewPatient != null)
                {
                    // Reload patient list
                    LoadAllPatients();
                    
                    // Find and select the newly added patient
                    int newPatientId = addPatientWindow.NewPatient.Id;
                    var newPatient = _allPatients.FirstOrDefault(p => p.Id == newPatientId);
                    if (newPatient != null)
                    {
                        lstPatients.SelectedItem = newPatient;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hasta ekleme işleminde hata: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dpGlucoseRange_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // No auto-refresh to avoid too many API calls
        }

        private void btnApplyGlucoseFilter_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPatient == null) return;

            // Get date range from UI
            DateTime startDate = dpGlucoseStart.SelectedDate ?? DateTime.Today.AddDays(-30);
            DateTime endDate = dpGlucoseEnd.SelectedDate ?? DateTime.Today;

            // Load and display blood glucose chart
            LoadBloodGlucoseChart(startDate, endDate);
        }

        private void LoadBloodGlucoseChart(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Before clearing, check if objects are null
                if (GlucoseSeries == null)
                {
                    GlucoseSeries = new SeriesCollection();
                }
                else
                {
                    // Clear existing series 
                    GlucoseSeries.Clear();
                }
                
                // Ensure chartBloodGlucose and its axes aren't null
                if (chartBloodGlucose != null && chartBloodGlucose.AxisY != null)
                {
                    chartBloodGlucose.AxisY.Clear();
                }
                else if (chartBloodGlucose == null)
                {
                    // Critical error - chart control not found
                    MessageBox.Show("Grafik bileşeni bulunamadı.", "Grafik Hatası", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // Check if _selectedPatient is null
                if (_selectedPatient == null)
                {
                    // Show default empty chart
                    ShowDefaultChart();
                    return;
                }
                
                // Get measurements for the selected date range
                var measurements = _bloodGlucoseService?.GetMeasurementsByDateRange(
                    _selectedPatient.Id,
                    startDate,
                    endDate.AddDays(1)  // Include the end date
                );

                // Eğer hiç ölçüm yoksa veya seçili hasta yoksa, varsayılan bir değer seti ekleyin
                if (measurements == null || measurements.Count == 0)
                {
                    // Varsayılan veri noktaları ekleyerek grafik hatasını önle
                    ShowDefaultChart();
                    return; // Boş veri durumunda diğer işlemleri atlayarak metodu sonlandır
                }

                // Group by measurement type for line series
                var dataByType = measurements
                    .GroupBy(m => m.MeasurementType)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Define colors for different measurement types
                var typeColors = new Dictionary<MeasurementType, SolidColorBrush>
                {
                    { MeasurementType.Morning, new SolidColorBrush(Colors.DodgerBlue) },
                    { MeasurementType.Noon, new SolidColorBrush(Colors.Orange) },
                    { MeasurementType.Afternoon, new SolidColorBrush(Colors.Green) },
                    { MeasurementType.Evening, new SolidColorBrush(Colors.Purple) },
                    { MeasurementType.Night, new SolidColorBrush(Colors.Red) }
                };

                // Add a series for each measurement type
                foreach (var type in Enum.GetValues(typeof(MeasurementType)).Cast<MeasurementType>())
                {
                    if (dataByType.ContainsKey(type) && dataByType[type].Count > 0)
                    {
                        // Convert the measurements to chart points
                        var points = dataByType[type]
                            .Select(m => new DoctorMeasurementPoint
                            {
                                DateTime = m.MeasurementTime,
                                Value = m.MeasurementValue
                            })
                            .OrderBy(p => p.DateTime)
                            .ToList();
                            
                            // En az iki farklı değer olduğundan emin ol
                            if (points.Count == 1)
                            {
                                // Tek nokta varsa, yakın bir değer ekle
                                points.Add(new DoctorMeasurementPoint { 
                                    DateTime = points[0].DateTime.AddHours(1),
                                    Value = points[0].Value + 10 
                                });
                            }

                            // Add the series
                            GlucoseSeries.Add(new LineSeries
                            {
                                Title = GetMeasurementTypeDisplay(type),
                                Values = new ChartValues<DoctorMeasurementPoint>(points),
                                PointGeometry = DefaultGeometries.Diamond,
                                PointGeometrySize = 8,
                                Stroke = typeColors[type],
                                Fill = new SolidColorBrush(Colors.Transparent)
                            });
                    }
                }

                // Tüm ölçümleri birleştir
                var allValues = measurements.Select(m => (double)m.MeasurementValue).ToList();
                
                // Veri sayısı çok az ise veya tüm değerler aynıysa, aralığı açıkça ayarla
                if (allValues.Count <= 2 || allValues.Max() - allValues.Min() < 10)
                {
                    double midValue = allValues.Count > 0 ? allValues.Average() : 100;
                    chartBloodGlucose.AxisY.Add(new Axis
                    {
                        MinValue = Math.Max(0, midValue - 40),
                        MaxValue = midValue + 40,
                        Title = "Kan Şekeri (mg/dL)"
                    });
                }
                else
                {
                    // Normal durumda da eksen başlığını ayarla
                    chartBloodGlucose.AxisY.Add(new Axis
                    {
                        MinValue = Math.Max(0, allValues.Min() - 20),
                        MaxValue = allValues.Max() + 20,
                        Title = "Kan Şekeri (mg/dL)"
                    });
                }

                // Set the chart as DataContext
                chartBloodGlucose.DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kan şekeri grafiği yüklenirken hata oluştu: {ex.Message}", "Grafik Hatası", MessageBoxButton.OK, MessageBoxImage.Warning);
                ShowDefaultChart();
            }
        }

        // Helper method to show a default chart when there's an error
        private void ShowDefaultChart()
        {
            try
            {
                // Ensure SeriesCollection is initialized
                if (GlucoseSeries == null)
                {
                    GlucoseSeries = new SeriesCollection();
                }
                else
                {
                    GlucoseSeries.Clear();
                }
                
                // Ensure axes are initialized
                if (chartBloodGlucose != null && chartBloodGlucose.AxisY != null)
                {
                    chartBloodGlucose.AxisY.Clear();
                }
                
                // Varsayılan veri ekle
                var now = DateTime.Now;
                var defaultPoints = new List<DoctorMeasurementPoint>
                {
                    new DoctorMeasurementPoint { DateTime = now.AddDays(-2), Value = 80m },
                    new DoctorMeasurementPoint { DateTime = now.AddDays(-1), Value = 100m },
                    new DoctorMeasurementPoint { DateTime = now, Value = 120m }
                };
                
                GlucoseSeries.Add(new LineSeries
                {
                    Title = "Örnek Veriler",
                    Values = new ChartValues<DoctorMeasurementPoint>(defaultPoints),
                    PointGeometry = DefaultGeometries.Circle,
                    Stroke = new SolidColorBrush(Colors.Gray),
                    Fill = new SolidColorBrush(Colors.Transparent)
                });
                
                // Güvenli eksen aralığı
                if (chartBloodGlucose != null)
                {
                    chartBloodGlucose.AxisY.Add(new Axis
                    {
                        MinValue = 40,
                        MaxValue = 200,
                        Title = "Kan Şekeri (mg/dL)"
                    });
                    
                    // Set the chart as DataContext
                    chartBloodGlucose.DataContext = this;
                }
            }
            catch
            {
                // En kötü durumda bile uygulamanın çökmesini engelle
            }
        }

        private string GetMeasurementTypeDisplay(MeasurementType type)
        {
            switch (type)
            {
                case MeasurementType.Morning:
                    return "Sabah";
                case MeasurementType.Noon:
                    return "Öğle";
                case MeasurementType.Afternoon:
                    return "İkindi";
                case MeasurementType.Evening:
                    return "Akşam";
                case MeasurementType.Night:
                    return "Gece";
                default:
                    return type.ToString();
            }
        }

        private void dpSymptomsRange_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // No auto-refresh to avoid too many API calls
        }

        private void btnApplySymptomsFilter_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPatient != null)
            {
                LoadSymptoms();
            }
        }

        private void btnAddDoctorSymptom_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPatient == null)
            {
                MessageBox.Show("Lütfen önce bir hasta seçin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Veritabanı tablolarını kontrol et
            CheckDatabaseTables();

            // Initial panel setup
            if (cmbDoctorSymptomType.Items.Count > 0)
                cmbDoctorSymptomType.SelectedIndex = 0;
            
            if (cmbDoctorSymptomIntensity.Items.Count > 0)
                cmbDoctorSymptomIntensity.SelectedIndex = 0;
            
            // Set default date to today
            dpDoctorSymptomDate.SelectedDate = DateTime.Today;
            
            txtDoctorSymptomNotes.Text = string.Empty;
            
            // Toggle visibility
            gridSymptoms.Visibility = Visibility.Collapsed;
            panelAddDoctorSymptom.Visibility = Visibility.Visible;
        }

        private void CheckDatabaseTables()
        {
            try
            {
                using (var conn = Dbprolab.DbConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        
                        // Tablo varlığını kontrol et
                        cmd.CommandText = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'";
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("Veritabanında var olan tablolar:");
                            while (reader.Read())
                            {
                                string tableName = reader["table_name"].ToString();
                                Console.WriteLine($"- {tableName}");
                            }
                        }
                    }
                    
                    // Semptom tablosunun sütunlarını kontrol et
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = @"
                            SELECT column_name, data_type 
                            FROM information_schema.columns 
                            WHERE table_schema = 'public' 
                              AND (table_name = 'symptoms' OR table_name = 'patient_symptoms')";
                              
                        using (var reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("Semptom tablolarının sütunları:");
                            while (reader.Read())
                            {
                                string columnName = reader["column_name"].ToString();
                                string dataType = reader["data_type"].ToString();
                                Console.WriteLine($"- {columnName} ({dataType})");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veritabanı şeması kontrolü sırasında hata: {ex.Message}");
            }
        }

        private void btnCancelDoctorSymptom_Click(object sender, RoutedEventArgs e)
        {
            // Hide entry panel and show grid
            panelAddDoctorSymptom.Visibility = Visibility.Collapsed;
            gridSymptoms.Visibility = Visibility.Visible;
        }

        private void btnSaveDoctorSymptom_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPatient == null)
            {
                MessageBox.Show("Hasta seçili değil.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (cmbDoctorSymptomType.SelectedItem == null || cmbDoctorSymptomIntensity.SelectedItem == null)
            {
                MessageBox.Show("Lütfen semptom tipi ve şiddetini seçin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Semptom tipini belirleme
                int selectedIndex = cmbDoctorSymptomType.SelectedIndex;
                
                // Enum değerini kontrol et
                if (!Enum.IsDefined(typeof(SymptomType), selectedIndex))
                {
                    MessageBox.Show($"Geçersiz semptom tipi: {selectedIndex}. Lütfen semptom tipini doğru seçin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                SymptomType symptomType = (SymptomType)selectedIndex;

                // Seçilen tarihi alma
                DateTime symptomDate = dpDoctorSymptomDate.SelectedDate ?? DateTime.Today;

                // Şiddet değerini güvenli bir şekilde alma
                ComboBoxItem selectedIntensityItem = cmbDoctorSymptomIntensity.SelectedItem as ComboBoxItem;
                if (selectedIntensityItem == null || selectedIntensityItem.Tag == null)
                {
                    MessageBox.Show("Şiddet değeri seçiminde hata. Lütfen tekrar deneyin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int intensityValue;
                if (!int.TryParse(selectedIntensityItem.Tag.ToString(), out intensityValue))
                {
                    MessageBox.Show("Şiddet değeri sayısal bir değer değil.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Hasta ID'sini yazdıralım
                Console.WriteLine($"Hasta ID: {_selectedPatient.Id}");
                
                // Yeni semptom oluşturma
                var symptom = new Symptom
                {
                    PatientId = _selectedPatient.Id,
                    SymptomDate = symptomDate,
                    SymptomType = symptomType,
                    Intensity = intensityValue,
                    Notes = txtDoctorSymptomNotes.Text
                };

                // Debug bilgisi göster
                string debugInfo = $"PatientId: {symptom.PatientId}, Date: {symptom.SymptomDate}, Type: {symptom.SymptomType}, Intensity: {symptom.Intensity}";
                Console.WriteLine(debugInfo);
                
                // Database bağlantısını test edelim
                try {
                    var conn = Dbprolab.DbConnection.GetConnection();
                    conn.Open();
                    Console.WriteLine("Veritabanı bağlantısı başarılı!");
                    conn.Close();
                } catch (Exception dbEx) {
                    Console.WriteLine($"Veritabanı bağlantı hatası: {dbEx.Message}");
                    MessageBox.Show($"Veritabanı bağlantısı kurulamadı: {dbEx.Message}", "Bağlantı Hatası", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Semptom verilerini kaydetme
                bool success = _symptomService.AddSymptom(symptom);

                if (success)
                {
                    MessageBox.Show("Semptom başarıyla kaydedildi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Giriş panelini gizle ve semptom listesini yenile
                    panelAddDoctorSymptom.Visibility = Visibility.Collapsed;
                    gridSymptoms.Visibility = Visibility.Visible;
                    
                    // Semptomları yeniden yükleme
                    LoadSymptoms();
                }
                else
                {
                    MessageBox.Show("Semptom kaydedilirken bir hata oluştu.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Semptom kaydedilirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DoctorSymptomTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Seçim yapıldıysa arka plan rengini değiştirme
            if (cmbDoctorSymptomType.SelectedItem != null)
            {
                // ComboBox'ı seçildiğinde rengini değiştirme
                ComboBoxItem selectedItem = cmbDoctorSymptomType.SelectedItem as ComboBoxItem;
                cmbDoctorSymptomType.Background = new SolidColorBrush(Color.FromRgb(173, 216, 230)); // LightBlue
            }
            else
            {
                // Seçim yapılmadıysa default arka plana dönme
                cmbDoctorSymptomType.Background = null;
            }
        }

        private void DoctorSymptomIntensitySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Şiddet seviyesine göre renk değişimi
            if (cmbDoctorSymptomIntensity.SelectedItem != null)
            {
                ComboBoxItem selectedItem = cmbDoctorSymptomIntensity.SelectedItem as ComboBoxItem;
                int intensity = int.Parse(selectedItem.Tag.ToString());
                
                // Şiddet seviyesine göre renk değiştirme
                switch (intensity)
                {
                    case 1:
                        cmbDoctorSymptomIntensity.Background = new SolidColorBrush(Color.FromRgb(144, 238, 144)); // LightGreen
                        break;
                    case 2:
                        cmbDoctorSymptomIntensity.Background = new SolidColorBrush(Color.FromRgb(255, 255, 0)); // Yellow
                        break;
                    case 3:
                        cmbDoctorSymptomIntensity.Background = new SolidColorBrush(Color.FromRgb(255, 165, 0)); // Orange
                        break;
                    case 4:
                        cmbDoctorSymptomIntensity.Background = new SolidColorBrush(Color.FromRgb(255, 69, 0)); // OrangeRed
                        break;
                    case 5:
                        cmbDoctorSymptomIntensity.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0)); // Red
                        break;
                    default:
                        cmbDoctorSymptomIntensity.Background = null;
                        break;
                }
            }
            else
            {
                cmbDoctorSymptomIntensity.Background = null;
            }
        }

        private void btnAddDiet_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPatient == null)
            {
                MessageBox.Show("Lütfen önce bir hasta seçin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            // Show diet plan entry panel
            gridDietPlans.Visibility = Visibility.Collapsed;
            panelAddDiet.Visibility = Visibility.Visible;
            
            // Set default values
            dpDietStartDate.SelectedDate = DateTime.Today;
            dpDietEndDate.SelectedDate = null;
            if (cmbDietType.Items.Count > 0)
                cmbDietType.SelectedIndex = 0;
            txtDietNotes.Text = string.Empty;
        }

        private void btnCancelDiet_Click(object sender, RoutedEventArgs e)
        {
            // Hide entry panel and show grid
            panelAddDiet.Visibility = Visibility.Collapsed;
            gridDietPlans.Visibility = Visibility.Visible;
        }

        private void btnSaveDiet_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPatient == null)
            {
                MessageBox.Show("Hasta seçili değil.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (cmbDietType.SelectedItem == null)
            {
                MessageBox.Show("Lütfen diyet tipini seçin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Diet type determination
                ComboBoxItem selectedItem = cmbDietType.SelectedItem as ComboBoxItem;
                if (selectedItem == null || selectedItem.Tag == null)
                {
                    MessageBox.Show("Diyet tipi seçiminde hata. Lütfen tekrar deneyin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int dietTypeValue;
                if (!int.TryParse(selectedItem.Tag.ToString(), out dietTypeValue))
                {
                    MessageBox.Show("Diyet tipi değeri sayısal bir değer değil.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DietType dietType = (DietType)dietTypeValue;
                
                // Get selected dates
                DateTime startDate = dpDietStartDate.SelectedDate ?? DateTime.Today;
                DateTime? endDate = dpDietEndDate.SelectedDate;
                
                // Create new diet plan
                var diet = new Diet
                {
                    PatientId = _selectedPatient.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    DietType = dietType,
                    DoctorNotes = txtDietNotes.Text,
                    DoctorId = AuthService.CurrentUser.Id  // Set current doctor as the creator
                };

                // Save diet plan
                int dietId = _dietService.CreateDiet(diet);

                if (dietId > 0)
                {
                    MessageBox.Show("Diyet planı başarıyla kaydedildi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Hide entry panel and show grid
                    panelAddDiet.Visibility = Visibility.Collapsed;
                    gridDietPlans.Visibility = Visibility.Visible;
                    
                    // Reload diet plans
                    LoadDietPlans();
                }
                else
                {
                    MessageBox.Show("Diyet planı kaydedilirken bir hata oluştu.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Diyet planı kaydedilirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditDiet_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int dietId = Convert.ToInt32(button.Tag);
            
            // To be implemented: Show edit diet dialog
            MessageBox.Show($"Diyet planı düzenleme ekranı henüz uygulanmamıştır. Diyet ID: {dietId}", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnAddExercise_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPatient == null)
            {
                MessageBox.Show("Lütfen önce bir hasta seçin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            // Show exercise plan entry panel
            gridExercisePlans.Visibility = Visibility.Collapsed;
            panelAddExercise.Visibility = Visibility.Visible;
            
            // Set default values
            dpExerciseStartDate.SelectedDate = DateTime.Today;
            dpExerciseEndDate.SelectedDate = null;
            if (cmbExerciseType.Items.Count > 0)
                cmbExerciseType.SelectedIndex = 0;
            txtExerciseNotes.Text = string.Empty;
        }

        private void btnCancelExercise_Click(object sender, RoutedEventArgs e)
        {
            // Hide entry panel and show grid
            panelAddExercise.Visibility = Visibility.Collapsed;
            gridExercisePlans.Visibility = Visibility.Visible;
        }

        private void btnSaveExercise_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPatient == null)
            {
                MessageBox.Show("Hasta seçili değil.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (cmbExerciseType.SelectedItem == null)
            {
                MessageBox.Show("Lütfen egzersiz tipini seçin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Exercise type determination
                ComboBoxItem selectedItem = cmbExerciseType.SelectedItem as ComboBoxItem;
                if (selectedItem == null || selectedItem.Tag == null)
                {
                    MessageBox.Show("Egzersiz tipi seçiminde hata. Lütfen tekrar deneyin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int exerciseTypeValue;
                if (!int.TryParse(selectedItem.Tag.ToString(), out exerciseTypeValue))
                {
                    MessageBox.Show("Egzersiz tipi değeri sayısal bir değer değil.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ExerciseType exerciseType = (ExerciseType)exerciseTypeValue;
                
                // Get selected dates
                DateTime startDate = dpExerciseStartDate.SelectedDate ?? DateTime.Today;
                DateTime? endDate = dpExerciseEndDate.SelectedDate;
                
                // Create new exercise plan
                var exercise = new Exercise
                {
                    PatientId = _selectedPatient.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    ExerciseType = exerciseType,
                    DoctorNotes = txtExerciseNotes.Text,
                    DoctorId = AuthService.CurrentUser.Id  // Set current doctor as the creator
                };

                // Save exercise plan
                int exerciseId = _exerciseService.CreateExercise(exercise);

                if (exerciseId > 0)
                {
                    MessageBox.Show("Egzersiz planı başarıyla kaydedildi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Hide entry panel and show grid
                    panelAddExercise.Visibility = Visibility.Collapsed;
                    gridExercisePlans.Visibility = Visibility.Visible;
                    
                    // Reload exercise plans
                    LoadExercisePlans();
                }
                else
                {
                    MessageBox.Show("Egzersiz planı kaydedilirken bir hata oluştu.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Egzersiz planı kaydedilirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditExercise_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int exerciseId = Convert.ToInt32(button.Tag);
            
            // To be implemented: Show edit exercise dialog
            MessageBox.Show($"Egzersiz planı düzenleme ekranı henüz uygulanmamıştır. Egzersiz ID: {exerciseId}", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void cmbAlertFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectedPatient == null || gridAllAlerts.ItemsSource == null) return;

            ComboBox comboBox = sender as ComboBox;
            int selectedIndex = comboBox.SelectedIndex;
            
            ICollectionView view = CollectionViewSource.GetDefaultView(gridAllAlerts.ItemsSource);
            
            switch (selectedIndex)
            {
                case 1: // Okunmamış Uyarılar
                    view.Filter = item => !(item as DoctorAlertViewModel).IsRead;
                    break;
                case 2: // Acil Uyarılar
                    view.Filter = item => (item as DoctorAlertViewModel).AlertType == "Acil Uyarı";
                    break;
                default: // Tüm Uyarılar
                    view.Filter = null;
                    break;
            }
        }

        private void btnMarkAlertRead_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int alertId = Convert.ToInt32(button.Tag);
            
            // Mark alert as read
            bool success = _alertService.MarkAlertAsRead(alertId);
            
            if (success)
            {
                // Reload alerts
                LoadAlerts();
            }
        }

        private void btnMarkAllAlertsRead_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPatient == null) return;
            
            // Mark all alerts as read
            bool success = _alertService.MarkAllAlertsAsRead(_selectedPatient.Id);
            
            if (success)
            {
                // Reload alerts
                LoadAlerts();
            }
        }

        // Number validation for TextBox inputs
        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[^0-9,.]");
            e.Handled = regex.IsMatch(e.Text);
        }

        #region Recommendation System

        private void cmbRecommendationPatient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRecommendationPatient.SelectedItem is User selectedPatient)
            {
                LoadDoctorRecommendations(selectedPatient.Id);
            }
        }

        private void btnRefreshDoctorRecommendations_Click(object sender, RoutedEventArgs e)
        {
            if (cmbRecommendationPatient.SelectedItem is User selectedPatient)
            {
                LoadDoctorRecommendations(selectedPatient.Id);
            }
        }

        private void LoadDoctorRecommendations(int patientId)
        {
            try
            {
                // Hasta bilgilerini göster
                ShowPatientRecommendationInfo(patientId);
                
                // Önerileri generate et
                var recommendations = _recommendationService.GenerateRecommendations(patientId);
                
                // ViewModel'e dönüştür
                var recommendationViewModels = recommendations.Select(r => new DoctorRecommendationViewModel
                {
                    Title = r.Title,
                    Message = r.Message,
                    Category = r.Category,
                    Actions = r.Actions,
                    CreatedAt = r.CreatedAt,
                    Priority = r.Priority,
                    Type = r.Type,
                    PriorityBrush = GetPriorityBrush(r.Priority),
                    PriorityText = GetPriorityText(r.Priority),
                    HasActions = r.Actions != null && r.Actions.Any()
                }).ToList();

                // Önceliğe göre sırala
                recommendationViewModels = recommendationViewModels
                    .OrderByDescending(r => r.Priority)
                    .ThenByDescending(r => r.CreatedAt)
                    .ToList();

                // UI'ya bind et
                lstDoctorRecommendations.ItemsSource = recommendationViewModels;
                
                // Panel görünürlüklerini ayarla
                if (recommendationViewModels.Any())
                {
                    panelEmptyRecommendations.Visibility = Visibility.Collapsed;
                    scrollRecommendations.Visibility = Visibility.Visible;
                    panelRecommendationPatientInfo.Visibility = Visibility.Visible;
                }
                else
                {
                    panelEmptyRecommendations.Visibility = Visibility.Visible;
                    scrollRecommendations.Visibility = Visibility.Collapsed;
                    panelRecommendationPatientInfo.Visibility = Visibility.Collapsed;
                }
                
                // Son güncelleme zamanını göster
                txtRecommendationsLastUpdated.Text = $"Son güncelleme: {DateTime.Now:dd.MM.yyyy HH:mm}";
                
                // Yüksek öncelikli önerileri alert olarak da ekle
                _recommendationService.CreateAlertsFromRecommendations(
                    patientId, 
                    recommendations.Where(r => r.Priority >= RecommendationPriority.High).ToList()
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Öneriler yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                
                // Hata durumunda empty state göster
                panelEmptyRecommendations.Visibility = Visibility.Visible;
                scrollRecommendations.Visibility = Visibility.Collapsed;
                panelRecommendationPatientInfo.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowPatientRecommendationInfo(int patientId)
        {
            try
            {
                // Son ölçüm
                var latestMeasurement = _bloodGlucoseService.GetLatestMeasurement(patientId);
                if (latestMeasurement != null)
                {
                    txtRecommendationLastGlucose.Text = $"{latestMeasurement.MeasurementValue} mg/dL";
                    txtRecommendationLastGlucose.Foreground = GetStatusColorForGlucose(latestMeasurement.MeasurementValue);
                }
                else
                {
                    txtRecommendationLastGlucose.Text = "Veri yok";
                    txtRecommendationLastGlucose.Foreground = new SolidColorBrush(Colors.Gray);
                }

                // Günlük ortalama
                var dailyAverage = _bloodGlucoseService.CalculateDailyAverage(patientId, DateTime.Today);
                if (dailyAverage > 0)
                {
                    txtRecommendationDailyAvg.Text = $"{Math.Round(dailyAverage)} mg/dL";
                    txtRecommendationDailyAvg.Foreground = GetStatusColorForGlucose(dailyAverage);
                }
                else
                {
                    txtRecommendationDailyAvg.Text = "Veri yok";
                    txtRecommendationDailyAvg.Foreground = new SolidColorBrush(Colors.Gray);
                }

                // Toplam ölçüm sayısı
                var totalMeasurements = _bloodGlucoseService.GetMeasurementsByDateRange(
                    patientId, 
                    DateTime.Today.AddDays(-30), 
                    DateTime.Today.AddDays(1)
                ).Count;
                txtRecommendationTotalMeasurements.Text = totalMeasurements.ToString();

                // Risk seviyesi hesaplama
                string riskLevel = "Düşük";
                var riskColor = Colors.Green;

                if (latestMeasurement != null)
                {
                    if (latestMeasurement.MeasurementValue < 70 || latestMeasurement.MeasurementValue > 180)
                    {
                        riskLevel = "Yüksek";
                        riskColor = Colors.Red;
                    }
                    else if (latestMeasurement.MeasurementValue > 110)
                    {
                        riskLevel = "Orta";
                        riskColor = Colors.Orange;
                    }
                }

                txtRecommendationRiskLevel.Text = riskLevel;
                txtRecommendationRiskLevel.Foreground = new SolidColorBrush(riskColor);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hasta bilgileri yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private SolidColorBrush GetPriorityBrush(RecommendationPriority priority)
        {
            switch (priority)
            {
                case RecommendationPriority.Critical:
                    return new SolidColorBrush(Color.FromRgb(220, 20, 60));  // Crimson
                case RecommendationPriority.High:
                    return new SolidColorBrush(Color.FromRgb(255, 69, 0));   // Red-Orange
                case RecommendationPriority.Normal:
                    return new SolidColorBrush(Color.FromRgb(30, 144, 255)); // Dodger Blue
                case RecommendationPriority.Low:
                    return new SolidColorBrush(Color.FromRgb(50, 205, 50));  // Lime Green
                default:
                    return new SolidColorBrush(Color.FromRgb(128, 128, 128)); // Gray
            }
        }

        private string GetPriorityText(RecommendationPriority priority)
        {
            switch (priority)
            {
                case RecommendationPriority.Critical:
                    return "KRİTİK";
                case RecommendationPriority.High:
                    return "YÜKSEK";
                case RecommendationPriority.Normal:
                    return "NORMAL";
                case RecommendationPriority.Low:
                    return "DÜŞÜK";
                default:
                    return "BİLİNMEYEN";
            }
        }

        #endregion

        // Save blood glucose measurement
        private void btnSaveGlucose_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPatient == null)
            {
                MessageBox.Show("Lütfen önce bir hasta seçin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtGlucoseValue.Text))
            {
                MessageBox.Show("Lütfen kan şekeri değerini girin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtGlucoseValue.Text.Replace(',', '.'), out decimal glucoseValue))
            {
                MessageBox.Show("Kan şekeri değeri geçerli bir sayı değil.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kan şekeri değerinin makul aralıkta olduğunu kontrol et (0-1000 mg/dL)
            if (glucoseValue < 0 || glucoseValue > 1000)
            {
                MessageBox.Show("Kan şekeri değeri 0-1000 mg/dL aralığında olmalıdır.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpGlucoseDate.SelectedDate == null)
            {
                MessageBox.Show("Lütfen bir tarih seçin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Parse time
            string timeText = txtGlucoseTime.Text;
            if (!TimeSpan.TryParse(timeText, out TimeSpan timeSpan))
            {
                MessageBox.Show("Geçersiz saat formatı. Lütfen HH:mm formatında girin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get measurement type
            if (cmbMeasurementType.SelectedItem == null)
            {
                MessageBox.Show("Lütfen ölçüm türünü seçin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ComboBoxItem selectedItem = (ComboBoxItem)cmbMeasurementType.SelectedItem;
            MeasurementType measurementType = (MeasurementType)Enum.Parse(typeof(MeasurementType), selectedItem.Tag.ToString());

            // Parse insulin dose if provided
            decimal? insulinDose = null;
            if (!string.IsNullOrWhiteSpace(txtInsulinDose.Text))
            {
                if (decimal.TryParse(txtInsulinDose.Text.Replace(',', '.'), out decimal doseValue))
                {
                    // İnsülin dozu makul aralıkta mı kontrol et (0-100 ünite)
                    if (doseValue < 0 || doseValue > 100)
                    {
                        MessageBox.Show("İnsülin dozu 0-100 aralığında olmalıdır.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    
                    insulinDose = doseValue;
                }
                else
                {
                    MessageBox.Show("İnsülin dozu geçerli bir sayı değil.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            try
            {
                // Create measurement datetime
                DateTime selectedDate = dpGlucoseDate.SelectedDate.Value.Date;
                DateTime measurementDateTime = selectedDate.Add(timeSpan);

                // Create the blood glucose object
                BloodGlucose measurement = new BloodGlucose
                {
                    PatientId = _selectedPatient.Id,
                    MeasurementValue = glucoseValue,
                    MeasurementTime = measurementDateTime,
                    MeasurementType = measurementType,
                    InsulinDose = insulinDose,
                    Notes = txtGlucoseNotes.Text
                };

                // Save to database
                int newId = _bloodGlucoseService.CreateMeasurement(measurement);
                
                if (newId > 0)
                {
                    MessageBox.Show("Kan şekeri ölçümü başarıyla kaydedildi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Clear form
                    txtGlucoseValue.Clear();
                    txtInsulinDose.Clear();
                    txtGlucoseNotes.Clear();
                    
                    // Reload data
                    LoadBloodGlucoseData();
                    
                    // Refresh chart
                    DateTime startDate = dpGlucoseStart.SelectedDate ?? DateTime.Today.AddDays(-30);
                    DateTime endDate = dpGlucoseEnd.SelectedDate ?? DateTime.Today;
                    LoadBloodGlucoseChart(startDate, endDate);
                }
                else
                {
                    MessageBox.Show("Kan şekeri ölçümü kaydedilirken bir hata oluştu.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    #region View Models

    public class DoctorBloodGlucoseViewModel
    {
        public int Id { get; set; }
        public decimal MeasurementValue { get; set; }
        public DateTime MeasurementTime { get; set; }
        public string MeasurementType { get; set; }
        public SolidColorBrush StatusColor { get; set; }
        public string StatusText { get; set; }
    }

    public class DoctorDietViewModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string EndDateDisplay { get; set; }
        public DietType DietType { get; set; }
        public string DietTypeDisplay { get; set; }
        public string DoctorNotes { get; set; }
    }

    public class DoctorExerciseViewModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string EndDateDisplay { get; set; }
        public ExerciseType ExerciseType { get; set; }
        public string ExerciseTypeDisplay { get; set; }
        public string DoctorNotes { get; set; }
    }

    public class DoctorSymptomViewModel
    {
        public int Id { get; set; }
        public DateTime SymptomDate { get; set; }
        public SymptomType Symptom { get; set; }
        public string SymptomDisplay { get; set; }
        public int Intensity { get; set; }
        public string Notes { get; set; }
    }

    public class DoctorAlertViewModel
    {
        public int Id { get; set; }
        public string AlertType { get; set; }
        public string AlertMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string IsReadText { get; set; }
        public SolidColorBrush IsReadColor { get; set; }
        public Visibility IsReadButtonVisibility { get; set; }
    }

    // This class is used to map blood glucose measurements to chart points
    public class DoctorMeasurementPoint
    {
        public DateTime DateTime { get; set; }
        public decimal Value { get; set; }
    }

    public class DoctorRecommendationViewModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Category { get; set; }
        public List<string> Actions { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        public RecommendationPriority Priority { get; set; }
        public RecommendationType Type { get; set; }
        public SolidColorBrush PriorityBrush { get; set; }
        public string PriorityText { get; set; }
        public bool HasActions { get; set; }
    }

    #endregion
} 