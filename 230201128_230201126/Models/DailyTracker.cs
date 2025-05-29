using System;

namespace wpf_prolab.Models
{
    public class DailyTracker
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime TrackingDate { get; set; }
        public bool DietFollowed { get; set; }
        public bool ExerciseDone { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation property
        public User Patient { get; set; }
    }
} 