using System;

namespace wpf_prolab.Models
{
    public class User
    {
        public int Id { get; set; }
        public string TcId { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public char Gender { get; set; }
        public UserType UserType { get; set; }
        public byte[] ProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsTemporaryPassword { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public int Age => DateTime.Now.Year - BirthDate.Year - (DateTime.Now.DayOfYear < BirthDate.DayOfYear ? 1 : 0);
        
        // Navigation properties
        public PatientProfile PatientProfile { get; set; }
    }
} 