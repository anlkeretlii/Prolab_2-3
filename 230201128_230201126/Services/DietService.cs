using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using wpf_prolab.Dbprolab;
using wpf_prolab.Models;

namespace wpf_prolab.Services
{
    public class DietService
    {
        private readonly UserService _userService;

        public DietService()
        {
            _userService = new UserService();
        }

        // Create a new diet plan for a patient
        public int CreateDiet(Diet diet)
        {
            string sql = @"
                INSERT INTO diets (patient_id, diet_type, start_date, end_date, doctor_notes, doctor_id)
                VALUES (@patientId, @dietType, @startDate, @endDate, @doctorNotes, @doctorId)
                RETURNING id";

            int newDietId = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", diet.PatientId);
                    cmd.Parameters.AddWithValue("@dietType", diet.DietType);
                    cmd.Parameters.AddWithValue("@startDate", diet.StartDate);
                    
                    if (diet.EndDate.HasValue)
                        cmd.Parameters.AddWithValue("@endDate", diet.EndDate.Value);
                    else
                        cmd.Parameters.AddWithValue("@endDate", DBNull.Value);
                        
                    if (string.IsNullOrEmpty(diet.DoctorNotes))
                        cmd.Parameters.AddWithValue("@doctorNotes", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@doctorNotes", diet.DoctorNotes);
                    
                    cmd.Parameters.AddWithValue("@doctorId", diet.DoctorId);

                    newDietId = (int)cmd.ExecuteScalar();
                }
            }

            return newDietId;
        }

        // Update an existing diet plan
        public bool UpdateDiet(Diet diet)
        {
            string sql = @"
                UPDATE diets 
                SET diet_type = @dietType, 
                    start_date = @startDate, 
                    end_date = @endDate,
                    doctor_notes = @doctorNotes
                WHERE id = @id";

            int rowsAffected = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", diet.Id);
                    cmd.Parameters.AddWithValue("@dietType", diet.DietType);
                    cmd.Parameters.AddWithValue("@startDate", diet.StartDate);
                    
                    if (diet.EndDate.HasValue)
                        cmd.Parameters.AddWithValue("@endDate", diet.EndDate.Value);
                    else
                        cmd.Parameters.AddWithValue("@endDate", DBNull.Value);
                        
                    if (string.IsNullOrEmpty(diet.DoctorNotes))
                        cmd.Parameters.AddWithValue("@doctorNotes", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@doctorNotes", diet.DoctorNotes);

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            return rowsAffected > 0;
        }

        // End a diet plan (set end date to today)
        public bool EndDiet(int dietId)
        {
            string sql = "UPDATE diets SET end_date = @endDate WHERE id = @id";
            int rowsAffected = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", dietId);
                    cmd.Parameters.AddWithValue("@endDate", DateTime.Today);
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            return rowsAffected > 0;
        }

        // Get diet by ID
        public Diet GetDietById(int dietId)
        {
            Diet diet = null;
            string sql = "SELECT * FROM diets WHERE id = @id";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", dietId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            diet = MapDietFromReader(reader);
                        }
                    }
                }
            }

            return diet;
        }

        // Get all diets for a patient
        public List<Diet> GetDietsByPatientId(int patientId)
        {
            List<Diet> diets = new List<Diet>();
            string sql = "SELECT * FROM diets WHERE patient_id = @patientId ORDER BY start_date DESC";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", patientId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            diets.Add(MapDietFromReader(reader));
                        }
                    }
                }
            }

            return diets;
        }

        // Get active diet for a patient (most recent diet without end date or with future end date)
        public Diet GetActiveDiet(int patientId)
        {
            Diet diet = null;
            string sql = @"
                SELECT * FROM diets 
                WHERE patient_id = @patientId 
                  AND (end_date IS NULL OR end_date >= @today)
                ORDER BY start_date DESC
                LIMIT 1";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", patientId);
                    cmd.Parameters.AddWithValue("@today", DateTime.Today);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            diet = MapDietFromReader(reader);
                        }
                    }
                }
            }

            return diet;
        }

        // Get all diets for a specific patient
        public List<Diet> GetDietsForPatient(int patientId)
        {
            List<Diet> diets = new List<Diet>();
            string sql = @"
                SELECT * FROM diets 
                WHERE patient_id = @patientId 
                ORDER BY start_date DESC";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", patientId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            diets.Add(MapDietFromReader(reader));
                        }
                    }
                }
            }
            
            return diets;
        }

        // Get current diet for a patient
        public Diet GetCurrentDietForPatient(int patientId)
        {
            Diet diet = null;
            DateTime today = DateTime.Today;
            
            string sql = @"
                SELECT * FROM diets 
                WHERE patient_id = @patientId 
                AND start_date <= @today
                AND (end_date IS NULL OR end_date >= @today)
                ORDER BY start_date DESC 
                LIMIT 1";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", patientId);
                    cmd.Parameters.AddWithValue("@today", today);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            diet = MapDietFromReader(reader);
                        }
                    }
                }
            }
            
            return diet;
        }

        // Helper method to map a database record to a Diet object
        private Diet MapDietFromReader(IDataReader reader)
        {
            var diet = new Diet
            {
                Id = Convert.ToInt32(reader["id"]),
                PatientId = Convert.ToInt32(reader["patient_id"]),
                DietType = (DietType)Enum.Parse(typeof(DietType), reader["diet_type"].ToString()),
                StartDate = Convert.ToDateTime(reader["start_date"]),
                DoctorId = Convert.ToInt32(reader["doctor_id"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"])
            };

            if (reader["end_date"] != DBNull.Value)
                diet.EndDate = Convert.ToDateTime(reader["end_date"]);

            if (reader["doctor_notes"] != DBNull.Value)
                diet.DoctorNotes = reader["doctor_notes"].ToString();

            return diet;
        }

        // Get diet recommendation based on blood glucose level and symptoms
        public DietType GetRecommendedDietType(decimal glucoseLevel, List<SymptomType> symptoms)
        {
            // Very low glucose level
            if (glucoseLevel < 70)
            {
                return DietType.BalancedDiet;
            }
            
            // Normal to low-normal glucose level (70-110)
            if (glucoseLevel >= 70 && glucoseLevel <= 110)
            {
                if (symptoms.Contains(SymptomType.Fatigue) || symptoms.Contains(SymptomType.WeightLoss))
                {
                    return DietType.LowSugar;
                }
                
                if (symptoms.Contains(SymptomType.Polyphagia) || symptoms.Contains(SymptomType.Polydipsia))
                {
                    return DietType.BalancedDiet;
                }
            }
            
            // Elevated glucose level (110-180)
            if (glucoseLevel > 110 && glucoseLevel <= 180)
            {
                if (symptoms.Contains(SymptomType.BlurredVision) || symptoms.Contains(SymptomType.Neuropathy))
                {
                    return DietType.LowSugar;
                }
                
                if (symptoms.Contains(SymptomType.Polyuria) || symptoms.Contains(SymptomType.Polydipsia))
                {
                    return DietType.SugarFree;
                }
                
                if (symptoms.Contains(SymptomType.Fatigue) || symptoms.Contains(SymptomType.Neuropathy) || 
                    symptoms.Contains(SymptomType.BlurredVision))
                {
                    return DietType.LowSugar;
                }
            }
            
            // High glucose level (>180)
            if (glucoseLevel > 180)
            {
                if (symptoms.Contains(SymptomType.SlowHealingWounds) || 
                    (symptoms.Contains(SymptomType.Polyphagia) && symptoms.Contains(SymptomType.Polydipsia)))
                {
                    return DietType.SugarFree;
                }
                
                if (symptoms.Contains(SymptomType.SlowHealingWounds) || symptoms.Contains(SymptomType.WeightLoss))
                {
                    return DietType.SugarFree;
                }
            }
            
            // Default recommendation if no specific rules match
            return DietType.BalancedDiet;
        }
    }
} 