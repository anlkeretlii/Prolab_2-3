using System;

namespace wpf_prolab.Models
{
    public class BloodGlucose
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public decimal MeasurementValue { get; set; } // mg/dL
        public DateTime MeasurementTime { get; set; }
        public MeasurementType MeasurementType { get; set; }
        public decimal? InsulinDose { get; set; } // Optional insulin dose in ml
        public string Notes { get; set; } // Optional notes
        public DateTime CreatedAt { get; set; }
        
        // Navigation properties
        public User Patient { get; set; }
        
        public bool IsHypoglycemia => MeasurementValue < 70;
        public bool IsNormal => MeasurementValue >= 70 && MeasurementValue <= 110;
        public bool IsPrediabetic => MeasurementValue > 110 && MeasurementValue <= 125;
        public bool IsDiabetic => MeasurementValue > 125;
        
        public bool IsCritical => IsHypoglycemia || MeasurementValue > 200;
        
        public string GetStatusText()
        {
            if (IsHypoglycemia) return "Düşük";
            if (IsNormal) return "Normal";
            if (IsPrediabetic) return "Yüksek";
            return "Çok Yüksek";
        }
        
        // Helper property to determine glucose level category
        public GlucoseLevelCategory Category
        {
            get
            {
                if (MeasurementValue < 70) return GlucoseLevelCategory.Low;
                if (MeasurementValue <= 99) return GlucoseLevelCategory.Normal;
                if (MeasurementValue <= 125) return GlucoseLevelCategory.Prediabetes;
                return GlucoseLevelCategory.Diabetes;
            }
        }
    }
    
    public enum GlucoseLevelCategory
    {
        Low,       // < 70 mg/dL (Hypoglycemia)
        Normal,    // 70-99 mg/dL
        Prediabetes, // 100-125 mg/dL
        Diabetes   // >= 126 mg/dL
    }
} 