using System;

namespace wpf_prolab.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public ExerciseType ExerciseType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DoctorNotes { get; set; }
        public int DoctorId { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation properties
        public User Patient { get; set; }
        public User Doctor { get; set; }
        
        // Helper property to check if the exercise is active
        public bool IsActive => !EndDate.HasValue || EndDate.Value >= DateTime.Today;
    }
} 