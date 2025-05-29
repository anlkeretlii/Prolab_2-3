using System;

namespace wpf_prolab.Models
{
    public class Symptom
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public SymptomType SymptomType { get; set; } // Renamed property
        public DateTime SymptomDate { get; set; }
        public int Intensity { get; set; } // 1-5 scale
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation property
        public User Patient { get; set; }
    }
} 