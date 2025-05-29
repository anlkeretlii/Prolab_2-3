using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using wpf_prolab.Dbprolab;
using wpf_prolab.Models;

namespace wpf_prolab.Services
{
    public class ExerciseService
    {
        private readonly UserService _userService;

        public ExerciseService()
        {
            _userService = new UserService();
        }

        // Create a new exercise plan for a patient
        public int CreateExercise(Exercise exercise)
        {
            string sql = @"
                INSERT INTO exercises (patient_id, exercise_type, start_date, end_date, doctor_notes, doctor_id)
                VALUES (@patientId, @exerciseType, @startDate, @endDate, @doctorNotes, @doctorId)
                RETURNING id";

            int newExerciseId = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", exercise.PatientId);
                    cmd.Parameters.AddWithValue("@exerciseType", exercise.ExerciseType);
                    cmd.Parameters.AddWithValue("@startDate", exercise.StartDate);
                    
                    if (exercise.EndDate.HasValue)
                        cmd.Parameters.AddWithValue("@endDate", exercise.EndDate.Value);
                    else
                        cmd.Parameters.AddWithValue("@endDate", DBNull.Value);
                        
                    if (string.IsNullOrEmpty(exercise.DoctorNotes))
                        cmd.Parameters.AddWithValue("@doctorNotes", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@doctorNotes", exercise.DoctorNotes);
                    
                    cmd.Parameters.AddWithValue("@doctorId", exercise.DoctorId);

                    newExerciseId = (int)cmd.ExecuteScalar();
                }
            }

            return newExerciseId;
        }

        // Update an existing exercise plan
        public bool UpdateExercise(Exercise exercise)
        {
            string sql = @"
                UPDATE exercises 
                SET exercise_type = @exerciseType, 
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
                    cmd.Parameters.AddWithValue("@id", exercise.Id);
                    cmd.Parameters.AddWithValue("@exerciseType", exercise.ExerciseType);
                    cmd.Parameters.AddWithValue("@startDate", exercise.StartDate);
                    
                    if (exercise.EndDate.HasValue)
                        cmd.Parameters.AddWithValue("@endDate", exercise.EndDate.Value);
                    else
                        cmd.Parameters.AddWithValue("@endDate", DBNull.Value);
                        
                    if (string.IsNullOrEmpty(exercise.DoctorNotes))
                        cmd.Parameters.AddWithValue("@doctorNotes", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@doctorNotes", exercise.DoctorNotes);

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            return rowsAffected > 0;
        }

        // End an exercise plan (set end date to today)
        public bool EndExercise(int exerciseId)
        {
            string sql = "UPDATE exercises SET end_date = @endDate WHERE id = @id";
            int rowsAffected = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", exerciseId);
                    cmd.Parameters.AddWithValue("@endDate", DateTime.Today);
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            return rowsAffected > 0;
        }

        // Get exercise by ID
        public Exercise GetExerciseById(int exerciseId)
        {
            Exercise exercise = null;
            string sql = "SELECT * FROM exercises WHERE id = @id";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", exerciseId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            exercise = MapExerciseFromReader(reader);
                        }
                    }
                }
            }

            return exercise;
        }

        // Get all exercises for a patient
        public List<Exercise> GetExercisesByPatientId(int patientId)
        {
            List<Exercise> exercises = new List<Exercise>();
            string sql = "SELECT * FROM exercises WHERE patient_id = @patientId ORDER BY start_date DESC";

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
                            exercises.Add(MapExerciseFromReader(reader));
                        }
                    }
                }
            }

            return exercises;
        }

        // Get active exercise for a patient (most recent exercise without end date or with future end date)
        public Exercise GetActiveExercise(int patientId)
        {
            Exercise exercise = null;
            string sql = @"
                SELECT * FROM exercises 
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
                            exercise = MapExerciseFromReader(reader);
                        }
                    }
                }
            }

            return exercise;
        }

        // Get all exercises for a specific patient
        public List<Exercise> GetExercisesForPatient(int patientId)
        {
            List<Exercise> exercises = new List<Exercise>();
            string sql = @"
                SELECT * FROM exercises 
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
                            exercises.Add(MapExerciseFromReader(reader));
                        }
                    }
                }
            }
            
            return exercises;
        }

        // Get current exercise for a patient
        public Exercise GetCurrentExerciseForPatient(int patientId)
        {
            Exercise exercise = null;
            DateTime today = DateTime.Today;
            
            string sql = @"
                SELECT * FROM exercises 
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
                            exercise = MapExerciseFromReader(reader);
                        }
                    }
                }
            }
            
            return exercise;
        }

        // Helper method to map a database record to an Exercise object
        private Exercise MapExerciseFromReader(IDataReader reader)
        {
            var exercise = new Exercise
            {
                Id = Convert.ToInt32(reader["id"]),
                PatientId = Convert.ToInt32(reader["patient_id"]),
                ExerciseType = (ExerciseType)Enum.Parse(typeof(ExerciseType), reader["exercise_type"].ToString()),
                StartDate = Convert.ToDateTime(reader["start_date"]),
                DoctorId = Convert.ToInt32(reader["doctor_id"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"])
            };

            if (reader["end_date"] != DBNull.Value)
                exercise.EndDate = Convert.ToDateTime(reader["end_date"]);

            if (reader["doctor_notes"] != DBNull.Value)
                exercise.DoctorNotes = reader["doctor_notes"].ToString();

            return exercise;
        }

        // Get exercise recommendation based on blood glucose level and symptoms
        public ExerciseType GetRecommendedExerciseType(decimal glucoseLevel, List<SymptomType> symptoms)
        {
            // Very low glucose level - no exercise recommended
            if (glucoseLevel < 70)
            {
                // Return null or a special value to indicate no exercise is recommended
                // For simplicity, we'll return Walking as the lightest exercise
                return ExerciseType.Walking;
            }
            
            // Normal to low-normal glucose level (70-110)
            if (glucoseLevel >= 70 && glucoseLevel <= 110)
            {
                if (symptoms.Contains(SymptomType.Fatigue) || symptoms.Contains(SymptomType.WeightLoss))
                {
                    return ExerciseType.Walking;
                }
                
                if (symptoms.Contains(SymptomType.Polyphagia) || symptoms.Contains(SymptomType.Polydipsia))
                {
                    return ExerciseType.Walking;
                }
            }
            
            // Elevated glucose level (110-180)
            if (glucoseLevel > 110 && glucoseLevel <= 180)
            {
                if (symptoms.Contains(SymptomType.BlurredVision) || symptoms.Contains(SymptomType.Neuropathy))
                {
                    return ExerciseType.ClinicalExercise;
                }
                
                if (symptoms.Contains(SymptomType.Polyuria) || symptoms.Contains(SymptomType.Polydipsia))
                {
                    return ExerciseType.ClinicalExercise;
                }
                
                if (symptoms.Contains(SymptomType.Fatigue) || symptoms.Contains(SymptomType.Neuropathy) || 
                    symptoms.Contains(SymptomType.BlurredVision))
                {
                    return ExerciseType.Walking;
                }
            }
            
            // High glucose level (>180)
            if (glucoseLevel > 180)
            {
                if (symptoms.Contains(SymptomType.SlowHealingWounds) || 
                    (symptoms.Contains(SymptomType.Polyphagia) && symptoms.Contains(SymptomType.Polydipsia)))
                {
                    return ExerciseType.ClinicalExercise;
                }
                
                if (symptoms.Contains(SymptomType.SlowHealingWounds) || symptoms.Contains(SymptomType.WeightLoss))
                {
                    return ExerciseType.Walking;
                }
            }
            
            // Default recommendation if no specific rules match
            return ExerciseType.Walking;
        }
    }
} 