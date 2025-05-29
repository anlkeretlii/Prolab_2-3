using System;

namespace wpf_prolab.Models
{
    public class Diet
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DietType DietType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DoctorNotes { get; set; }
        public int DoctorId { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation properties
        public User Patient { get; set; }
        public User Doctor { get; set; }
        
        // Helper property to check if the diet is active
        public bool IsActive => !EndDate.HasValue || EndDate.Value >= DateTime.Today;
    }
} 