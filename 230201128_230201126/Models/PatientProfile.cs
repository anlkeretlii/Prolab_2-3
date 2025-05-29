using System;

namespace wpf_prolab.Models
{
    public class PatientProfile
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DiabetesType { get; set; } // 0: Type 1, 1: Type 2, 2: Gestational, 3: Other
        public DateTime DiagnosisDate { get; set; }
        public string DoctorNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation property
        public User Patient { get; set; }
        
        public string DiabetesTypeDisplay
        {
            get
            {
                switch (DiabetesType)
                {
                    case 0:
                        return "Tip 1 Diyabet";
                    case 1:
                        return "Tip 2 Diyabet";
                    case 2:
                        return "Gestasyonel Diyabet";
                    case 3:
                        return "Diğer";
                    default:
                        return "Belirtilmemiş";
                }
            }
        }
    }
} 