using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using wpf_prolab.Dbprolab;
using wpf_prolab.Models;
using System.Linq;

namespace wpf_prolab.Services
{
    public class UserService
    {
        // Get user by TC ID (for login)
        public User GetUserByTcId(string tcId)
        {
            User user = null;
            string sql = "SELECT * FROM users WHERE tc_id = @tcId";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@tcId", tcId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = MapUserFromReader(reader);
                        }
                    }
                }
                
                // Load patient profile if this is a patient
                if (user != null && user.UserType == UserType.Patient)
                {
                    LoadPatientProfile(conn, user);
                }
            }
            
            return user;
        }

        // Get user by ID
        public User GetUserById(int id)
        {
            User user = null;
            string sql = "SELECT * FROM users WHERE id = @id";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = MapUserFromReader(reader);
                        }
                    }
                }
                
                // Load patient profile if this is a patient
                if (user != null && user.UserType == UserType.Patient)
                {
                    LoadPatientProfile(conn, user);
                }
            }
            
            return user;
        }

        // Create a new user
        public int CreateUser(User user)
        {
            // Doktor ise geçici şifre hiçbir zaman olmasın
            if (user.UserType == UserType.Doctor)
            {
                user.IsTemporaryPassword = false;
            }

            string userSql = @"
                INSERT INTO users (tc_id, password, email, first_name, last_name, birth_date, gender, user_type, profile_picture)
                VALUES (@tcId, @password, @email, @firstName, @lastName, @birthDate, @gender, @userType, @profilePicture)
                RETURNING id";

            int newUserId = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Insert user
                        using (var cmd = new NpgsqlCommand(userSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@tcId", user.TcId);
                            cmd.Parameters.AddWithValue("@password", user.Password); // Should be already hashed
                            cmd.Parameters.AddWithValue("@email", user.Email);
                            cmd.Parameters.AddWithValue("@firstName", user.FirstName);
                            cmd.Parameters.AddWithValue("@lastName", user.LastName);
                            cmd.Parameters.AddWithValue("@birthDate", user.BirthDate);
                            cmd.Parameters.AddWithValue("@gender", user.Gender);
                            cmd.Parameters.AddWithValue("@userType", user.UserType);
                            
                            if (user.ProfilePicture != null)
                                cmd.Parameters.AddWithValue("@profilePicture", user.ProfilePicture);
                            else
                                cmd.Parameters.AddWithValue("@profilePicture", DBNull.Value);

                            newUserId = (int)cmd.ExecuteScalar();
                        }

                        // If this is a patient, create patient profile
                        if (user.UserType == UserType.Patient && user.PatientProfile != null)
                        {
                            string profileSql = @"
                                INSERT INTO patient_profiles (patient_id, diabetes_type, diagnosis_date, doctor_notes)
                                VALUES (@patientId, @diabetesType, @diagnosisDate, @doctorNotes)";

                            using (var cmd = new NpgsqlCommand(profileSql, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@patientId", newUserId);
                                cmd.Parameters.AddWithValue("@diabetesType", user.PatientProfile.DiabetesType);
                                cmd.Parameters.AddWithValue("@diagnosisDate", user.PatientProfile.DiagnosisDate);
                                
                                // Fix for C# 7.3 compatibility - avoid conditional expressions with DBNull
                                if (string.IsNullOrEmpty(user.PatientProfile.DoctorNotes))
                                {
                                    cmd.Parameters.AddWithValue("@doctorNotes", DBNull.Value);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@doctorNotes", user.PatientProfile.DoctorNotes);
                                }

                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return newUserId;
        }

        // Update an existing user
        public bool UpdateUser(User user)
        {
            string sql = @"
                UPDATE users 
                SET email = @email, first_name = @firstName, last_name = @lastName, 
                    birth_date = @birthDate, gender = @gender
                WHERE id = @id";

            int rowsAffected = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", user.Id);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                    cmd.Parameters.AddWithValue("@firstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", user.LastName);
                    cmd.Parameters.AddWithValue("@birthDate", user.BirthDate);
                    cmd.Parameters.AddWithValue("@gender", user.Gender);

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            return rowsAffected > 0;
        }

        // Update user profile picture
        public bool UpdateProfilePicture(int userId, byte[] profilePicture)
        {
            string sql = "UPDATE users SET profile_picture = @profilePicture WHERE id = @id";
            int rowsAffected = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    
                    // Fix for C# 7.3 compatibility - avoid conditional expressions with DBNull
                    if (profilePicture != null)
                    {
                        cmd.Parameters.AddWithValue("@profilePicture", profilePicture);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@profilePicture", DBNull.Value);
                    }

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            return rowsAffected > 0;
        }

        // Update user password
        public bool UpdatePassword(int userId, string hashedPassword)
        {
            string sql = "UPDATE users SET password = @password WHERE id = @id";
            int rowsAffected = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.Parameters.AddWithValue("@password", hashedPassword);

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            return rowsAffected > 0;
        }

        // Set temporary password status for a user
        public bool SetTemporaryPasswordStatus(int userId, bool isTemporary)
        {
            // Bu metod sadece hasta kayıtları için çalışsın
            // Doktor kayıtları için hiçbir şey yapmasın
            try
            {
                string checkUserTypeSql = "SELECT user_type FROM users WHERE id = @id";
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(checkUserTypeSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", userId);
                        var userType = cmd.ExecuteScalar();
                        
                        if (userType != null && (UserType)userType == UserType.Doctor)
                        {
                            return true; // Doktor için işlem başarılı sayılsın ama hiçbir şey yapılmasın
                        }
                    }
                }
                
                // is_temporary_password alanı henüz yoksa, işlem başarılı sayılsın
                string sql = "UPDATE users SET is_temporary_password = @isTemporary WHERE id = @id";
                int rowsAffected = 0;

                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", userId);
                        cmd.Parameters.AddWithValue("@isTemporary", isTemporary);

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                }

                return rowsAffected > 0;
            }
            catch
            {
                // Eğer is_temporary_password alanı yoksa, işlem başarılı sayılsın
                return true;
            }
        }

        // Get all patients for a doctor
        public List<User> GetPatientsByDoctorId(int doctorId)
        {
            List<User> patients = new List<User>();
            string sql = @"
                SELECT u.* 
                FROM users u
                JOIN doctor_patient dp ON u.id = dp.patient_id
                WHERE dp.doctor_id = @doctorId
                ORDER BY u.last_name, u.first_name";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@doctorId", doctorId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            patients.Add(MapUserFromReader(reader));
                        }
                    }
                }
                
                // Load patient profiles for all patients
                LoadPatientProfiles(conn, patients);
            }

            return patients;
        }
        
        // Load patient profiles for a list of patients
        private void LoadPatientProfiles(NpgsqlConnection conn, List<User> patients)
        {
            if (patients.Count == 0) return;
            
            var patientIds = string.Join(",", patients.Select(p => p.Id));
            string sql = $"SELECT * FROM patient_profiles WHERE patient_id IN ({patientIds})";
            
            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int patientId = Convert.ToInt32(reader["patient_id"]);
                        var profile = MapPatientProfileFromReader(reader);
                        
                        // Find the patient and assign the profile
                        var patient = patients.FirstOrDefault(p => p.Id == patientId);
                        if (patient != null)
                        {
                            patient.PatientProfile = profile;
                        }
                    }
                }
            }
        }
        
        // Helper method to map a database record to a PatientProfile object
        private PatientProfile MapPatientProfileFromReader(IDataReader reader)
        {
            var profile = new PatientProfile
            {
                Id = Convert.ToInt32(reader["id"]),
                PatientId = Convert.ToInt32(reader["patient_id"]),
                DiabetesType = Convert.ToInt32(reader["diabetes_type"]),
                DiagnosisDate = Convert.ToDateTime(reader["diagnosis_date"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"])
            };
            
            // Fix for C# 7.3 compatibility - avoid conditional expressions with DBNull
            if (reader["doctor_notes"] != DBNull.Value)
            {
                profile.DoctorNotes = reader["doctor_notes"].ToString();
            }
            
            return profile;
        }

        // Helper method to map a database record to a User object
        private User MapUserFromReader(IDataReader reader)
        {
            var user = new User
            {
                Id = Convert.ToInt32(reader["id"]),
                TcId = reader["tc_id"].ToString(),
                Password = reader["password"].ToString(),
                Email = reader["email"].ToString(),
                FirstName = reader["first_name"].ToString(),
                LastName = reader["last_name"].ToString(),
                BirthDate = Convert.ToDateTime(reader["birth_date"]),
                Gender = Convert.ToChar(reader["gender"]),
                UserType = (UserType)reader["user_type"],
                CreatedAt = Convert.ToDateTime(reader["created_at"])
            };

            // is_temporary_password alanını güvenli bir şekilde oku
            try
            {
                if (reader.FieldCount > 0)
                {
                    // Alanın var olup olmadığını kontrol et
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (reader.GetName(i) == "is_temporary_password")
                        {
                            user.IsTemporaryPassword = reader["is_temporary_password"] != DBNull.Value ? 
                                Convert.ToBoolean(reader["is_temporary_password"]) : false;
                            break;
                        }
                    }
                    
                    // Eğer alan bulunamadıysa, kullanıcı tipine göre varsayılan değer ata
                    if (user.UserType == UserType.Doctor)
                    {
                        user.IsTemporaryPassword = false; // Doktorlar için hiçbir zaman geçici şifre yok
                    }
                    else if (user.UserType == UserType.Patient)
                    {
                        user.IsTemporaryPassword = true; // Hastalar için varsayılan olarak geçici şifre
                    }
                }
            }
            catch
            {
                // Alanı okuyamazsa, kullanıcı tipine göre varsayılan değer ata
                user.IsTemporaryPassword = user.UserType == UserType.Patient;
            }

            // Fix for C# 7.3 compatibility - avoid conditional expressions with DBNull
            if (reader["profile_picture"] != DBNull.Value)
            {
                user.ProfilePicture = (byte[])reader["profile_picture"];
            }

            return user;
        }

        // Assign a patient to a doctor
        public bool AssignPatientToDoctor(int doctorId, int patientId)
        {
            string sql = @"
                INSERT INTO doctor_patient (doctor_id, patient_id)
                VALUES (@doctorId, @patientId)
                ON CONFLICT (doctor_id, patient_id) DO NOTHING";

            int rowsAffected = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@doctorId", doctorId);
                    cmd.Parameters.AddWithValue("@patientId", patientId);

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            return rowsAffected > 0;
        }

        // Remove a patient from a doctor
        public bool RemovePatientFromDoctor(int doctorId, int patientId)
        {
            string sql = "DELETE FROM doctor_patient WHERE doctor_id = @doctorId AND patient_id = @patientId";
            int rowsAffected = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@doctorId", doctorId);
                    cmd.Parameters.AddWithValue("@patientId", patientId);

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            return rowsAffected > 0;
        }

        // Load patient profile for a single user
        private void LoadPatientProfile(NpgsqlConnection conn, User patient)
        {
            string sql = "SELECT * FROM patient_profiles WHERE patient_id = @patientId";
            
            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@patientId", patient.Id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        patient.PatientProfile = MapPatientProfileFromReader(reader);
                    }
                }
            }
        }
    }
} 