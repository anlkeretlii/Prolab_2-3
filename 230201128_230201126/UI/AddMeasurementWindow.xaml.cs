using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpf_prolab.Models;
using wpf_prolab.Services;

namespace wpf_prolab.UI
{
    /// <summary>
    /// Interaction logic for AddMeasurementWindow.xaml
    /// </summary>
    public partial class AddMeasurementWindow : Window
    {
        private readonly BloodGlucoseService _bloodGlucoseService;
        
        public BloodGlucose NewMeasurement { get; private set; }

        public AddMeasurementWindow()
        {
            InitializeComponent();
            
            _bloodGlucoseService = new BloodGlucoseService();
            
            // Set default values
            dpMeasurementDate.SelectedDate = DateTime.Today;
            txtMeasurementTime.Text = DateTime.Now.ToString("HH:mm");
        }

        private void txtMeasurementValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Update the status indicator based on the entered value
            if (decimal.TryParse(txtMeasurementValue.Text, out decimal value))
            {
                borderStatus.Visibility = Visibility.Visible;
                
                if (value < 70)
                {
                    borderStatus.Background = new SolidColorBrush(Colors.Red);
                    txtStatus.Text = "Düşük";
                }
                else if (value <= 110)
                {
                    borderStatus.Background = new SolidColorBrush(Colors.Green);
                    txtStatus.Text = "Normal";
                }
                else if (value <= 180)
                {
                    borderStatus.Background = new SolidColorBrush(Colors.Orange);
                    txtStatus.Text = "Yüksek";
                }
                else
                {
                    borderStatus.Background = new SolidColorBrush(Colors.DarkRed);
                    txtStatus.Text = "Çok Yüksek";
                }
            }
            else
            {
                borderStatus.Visibility = Visibility.Collapsed;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    // Create blood glucose measurement
                    var measurement = new BloodGlucose
                    {
                        PatientId = AuthService.CurrentUser.Id,
                        MeasurementValue = decimal.Parse(txtMeasurementValue.Text),
                        MeasurementType = GetMeasurementTypeFromIndex(cmbMeasurementType.SelectedIndex),
                        MeasurementTime = CombineDateAndTime(dpMeasurementDate.SelectedDate.Value, txtMeasurementTime.Text)
                    };

                    // Set optional values
                    if (!string.IsNullOrWhiteSpace(txtInsulinDose.Text))
                    {
                        measurement.InsulinDose = decimal.Parse(txtInsulinDose.Text);
                    }

                    if (!string.IsNullOrWhiteSpace(txtNotes.Text))
                    {
                        measurement.Notes = txtNotes.Text;
                    }

                    // Save to database
                    int measurementId = _bloodGlucoseService.AddMeasurement(measurement);

                    if (measurementId > 0)
                    {
                        measurement.Id = measurementId;
                        NewMeasurement = measurement;
                        
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Ölçüm kaydedilirken bir hata oluştu.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateInput()
        {
            // Validate measurement value
            if (string.IsNullOrWhiteSpace(txtMeasurementValue.Text) || 
                !decimal.TryParse(txtMeasurementValue.Text, out decimal value))
            {
                MessageBox.Show("Lütfen geçerli bir ölçüm değeri girin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtMeasurementValue.Focus();
                return false;
            }

            // Validate date
            if (!dpMeasurementDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Lütfen bir tarih seçin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpMeasurementDate.Focus();
                return false;
            }

            // Validate time format
            string timePattern = @"^([01]?[0-9]|2[0-3]):[0-5][0-9]$";
            if (string.IsNullOrWhiteSpace(txtMeasurementTime.Text) || 
                !Regex.IsMatch(txtMeasurementTime.Text, timePattern))
            {
                MessageBox.Show("Lütfen geçerli bir saat girin (ÖR: 14:30).", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtMeasurementTime.Focus();
                return false;
            }

            // Optional: Validate insulin dose if provided
            if (!string.IsNullOrWhiteSpace(txtInsulinDose.Text) && 
                !decimal.TryParse(txtInsulinDose.Text, out _))
            {
                MessageBox.Show("Lütfen geçerli bir insülin dozu girin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtInsulinDose.Focus();
                return false;
            }

            return true;
        }
        
        private MeasurementType GetMeasurementTypeFromIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return MeasurementType.Morning;
                case 1:
                    return MeasurementType.Noon;
                case 2:
                    return MeasurementType.Afternoon;
                case 3:
                    return MeasurementType.Evening;
                case 4:
                    return MeasurementType.Night;
                default:
                    return MeasurementType.Morning;
            }
        }
        
        private DateTime CombineDateAndTime(DateTime date, string timeString)
        {
            string[] timeParts = timeString.Split(':');
            int hours = int.Parse(timeParts[0]);
            int minutes = int.Parse(timeParts[1]);
            
            return new DateTime(date.Year, date.Month, date.Day, hours, minutes, 0);
        }
    }
} 