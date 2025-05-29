using System;
using System.Windows;
using System.Windows.Threading;
using wpf_prolab.Dbprolab;
using wpf_prolab.UI;

namespace wpf_prolab
{
    /// <summary>
    /// Application Entry Point.
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Global exception handling
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var exception = args.ExceptionObject as Exception;
                HandleGlobalException(exception, "AppDomain.UnhandledException", args.IsTerminating);
            };
            
            // UI thread exception handling
            Current.DispatcherUnhandledException += (sender, args) =>
            {
                HandleGlobalException(args.Exception, "DispatcherUnhandledException", false);
                args.Handled = true; // Prevent application from crashing
            };
            
            try
            {
                // Initialize the database
                DbConnection.InitializeDatabase();
                DbInitializer.InitializeDatabase();
                
                // Create and show login window
                var loginWindow = new LoginWindow();
                loginWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Uygulama başlatılırken bir hata oluştu: {ex.Message}\n\nStackTrace: {ex.StackTrace}", 
                    "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                
                // Exit the application if initialization fails
                Current.Shutdown();
            }
        }
        
        private void HandleGlobalException(Exception exception, string source, bool isTerminating)
        {
            if (exception == null) return;
            
            // Check if it's a LiveCharts error
            bool isLiveChartsError = exception.ToString().Contains("LiveCharts") || 
                                    (exception.StackTrace != null && exception.StackTrace.Contains("LiveCharts"));
            
            if (isLiveChartsError)
            {
                string message = $"Grafik oluşturulurken bir hata oluştu. Bu genellikle veri eksikliğinden kaynaklanır veya " +
                               $"geçersiz değerlerden olabilir.\n\nDetay: {exception.Message}";
                
                // Show message on UI thread
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                    MessageBox.Show(message, "Grafik Hatası", MessageBoxButton.OK, MessageBoxImage.Warning);
                }));
                
                // If application is terminating due to this error, start login instead
                if (isTerminating)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                        try
                        {
                            var exceptionLoginWindow = new LoginWindow();
                            exceptionLoginWindow.Show();
                        }
                        catch {}
                    }));
                }
            }
            else if (isTerminating)
            {
                // For other fatal errors
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                    MessageBox.Show($"Beklenmeyen bir hata oluştu: {exception.Message}", 
                        "Kritik Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }));
            }
        }
    }
}
