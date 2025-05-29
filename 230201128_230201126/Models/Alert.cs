using System;

namespace wpf_prolab.Models
{
    public class Alert
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string AlertType { get; set; }
        public string AlertMessage { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation property
        public User Patient { get; set; }
    }
    
    public static class AlertTypes
    {
        public const string EmergencyAlert = "EmergencyAlert";
        public const string WarningAlert = "WarningAlert";
        public const string InfoAlert = "InfoAlert";
        public const string MeasurementMissingAlert = "MeasurementMissingAlert";
        public const string MeasurementInsufficientAlert = "MeasurementInsufficientAlert";
    }
} 