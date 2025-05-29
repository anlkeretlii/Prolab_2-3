using System;
using System.Security.Cryptography;
using System.Text;
using wpf_prolab.Models;
using wpf_prolab.Dbprolab;

namespace wpf_prolab.Services
{
    public class AuthService
    {
        private static User _currentUser;

        // Property to access the current logged-in user
        public static User CurrentUser 
        { 
            get { return _currentUser; }
            private set { _currentUser = value; }
        }

        private readonly UserService _userService;

        public AuthService()
        {
            _userService = new UserService();
        }

        // Log in with TC ID and password
        public bool Login(string tcId, string password)
        {
            User user = _userService.GetUserByTcId(tcId);
            
            if (user != null)
            {
                // Verify the password using BCrypt
                if (DbConnection.VerifyPassword(password, user.Password))
                {
                    // Set as current user
                    CurrentUser = user;
                    return true;
                }
            }
            
            return false;
        }

        // Log out the current user
        public void Logout()
        {
            CurrentUser = null;
        }

        // Check if the current user is a doctor
        public bool IsDoctor()
        {
            return CurrentUser != null && CurrentUser.UserType == UserType.Doctor;
        }

        // Check if the current user is a patient
        public bool IsPatient()
        {
            return CurrentUser != null && CurrentUser.UserType == UserType.Patient;
        }

        // Hash a password before storing it in the database
        public string HashPassword(string password)
        {
            return DbConnection.HashPassword(password);
        }
        
        // Validate password
        public bool ValidatePassword(string tcId, string password)
        {
            User user = _userService.GetUserByTcId(tcId);
            
            if (user != null)
            {
                return DbConnection.VerifyPassword(password, user.Password);
            }
            
            return false;
        }
    }
} 