using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Npgsql;
using wpf_prolab.Dbprolab;
using wpf_prolab.Models;

namespace wpf_prolab.Services
{
    public class BloodGlucoseService
    {
        private readonly UserService _userService;

        public BloodGlucoseService()
        {
                _userService = new UserService();
        }

        // Add a new blood glucose measurement
        public int AddMeasurement(BloodGlucose measurement)
        {
            string sql = @"
                INSERT INTO blood_glucose (patient_id, measurement_value, measurement_time, measurement_type, insulin_dose, notes)
                VALUES (@patientId, @measurementValue, @measurementTime, @measurementType, @insulinDose, @notes)
                RETURNING id";

            int newMeasurementId = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", measurement.PatientId);
                    cmd.Parameters.AddWithValue("@measurementValue", measurement.MeasurementValue);
                    cmd.Parameters.AddWithValue("@measurementTime", measurement.MeasurementTime);
                    cmd.Parameters.AddWithValue("@measurementType", measurement.MeasurementType);
                    
                    if (measurement.InsulinDose.HasValue)
                        cmd.Parameters.AddWithValue("@insulinDose", measurement.InsulinDose.Value);
                    else
                        cmd.Parameters.AddWithValue("@insulinDose", DBNull.Value);
                        
                    cmd.Parameters.AddWithValue("@notes", (object)measurement.Notes ?? DBNull.Value);

                    newMeasurementId = (int)cmd.ExecuteScalar();
                }
            }

            return newMeasurementId;
        }

        // Get all measurements for a patient
        public List<BloodGlucose> GetMeasurementsByPatientId(int patientId)
        {
            List<BloodGlucose> measurements = new List<BloodGlucose>();
            string sql = @"
                SELECT * FROM blood_glucose 
                WHERE patient_id = @patientId
                ORDER BY measurement_time DESC";

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
                            measurements.Add(MapBloodGlucoseFromReader(reader));
                        }
                    }
                }
            }

            return measurements;
        }

        // Get measurements for a patient within a date range
        public List<BloodGlucose> GetMeasurementsByDateRange(int patientId, DateTime startDate, DateTime endDate)
        {
            List<BloodGlucose> measurements = new List<BloodGlucose>();
            string sql = @"
                SELECT * FROM blood_glucose 
                WHERE patient_id = @patientId 
                AND measurement_time BETWEEN @startDate AND @endDate
                ORDER BY measurement_time DESC";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", patientId);
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            measurements.Add(MapBloodGlucoseFromReader(reader));
                        }
                    }
                }
            }

            return measurements;
        }

        // Get today's measurements for a patient
        public List<BloodGlucose> GetTodaysMeasurements(int patientId)
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);
            return GetMeasurementsByDateRange(patientId, today, tomorrow);
        }

        // Calculate daily average
        public decimal CalculateDailyAverage(int patientId, DateTime date)
        {
            DateTime nextDay = date.Date.AddDays(1);
            var measurements = GetMeasurementsByDateRange(patientId, date.Date, nextDay);
            
            if (measurements.Count == 0)
                return 0;
                
            return measurements.Average(m => m.MeasurementValue);
        }

        // Calculate average by measurement type for a specific day
        public decimal CalculateAverageByType(int patientId, DateTime date, MeasurementType type)
        {
            DateTime nextDay = date.Date.AddDays(1);
            var measurements = GetMeasurementsByDateRange(patientId, date.Date, nextDay)
                .Where(m => m.MeasurementType == type)
                .ToList();
            
            if (measurements.Count == 0)
                return 0;
                
            return measurements.Average(m => m.MeasurementValue);
        }

        // Calculate average based on all measurements up to a specific type for a day (for insulin recommendation)
        public decimal CalculateProgressiveAverage(int patientId, DateTime date, MeasurementType upToType)
        {
            DateTime nextDay = date.Date.AddDays(1);
            var allMeasurements = GetMeasurementsByDateRange(patientId, date.Date, nextDay);
            
            // Get all measurements up to and including the specified type
            var relevantMeasurements = new List<BloodGlucose>();
            
            foreach (MeasurementType type in Enum.GetValues(typeof(MeasurementType)))
            {
                var measurementsOfType = allMeasurements.Where(m => m.MeasurementType == type).ToList();
                relevantMeasurements.AddRange(measurementsOfType);
                
                if (type == upToType)
                    break;
            }
            
            if (relevantMeasurements.Count == 0)
                return 0;
                
            return relevantMeasurements.Average(m => m.MeasurementValue);
        }

        // Get recommended insulin dose based on average blood glucose
        public decimal GetRecommendedInsulinDose(decimal averageGlucose)
        {
            if (averageGlucose <= 110)
                return 0;
            if (averageGlucose <= 150)
                return 1;
            if (averageGlucose <= 200)
                return 2;
            return 3;
        }

        // Check if measurement time is within the expected time range for the given type
        public bool IsTimeWithinExpectedRange(DateTime measurementTime, MeasurementType type)
        {
            TimeSpan time = measurementTime.TimeOfDay;
            
            switch (type)
            {
                case MeasurementType.Morning:
                    return time >= new TimeSpan(7, 0, 0) && time <= new TimeSpan(8, 0, 0);
                case MeasurementType.Noon:
                    return time >= new TimeSpan(12, 0, 0) && time <= new TimeSpan(13, 0, 0);
                case MeasurementType.Afternoon:
                    return time >= new TimeSpan(15, 0, 0) && time <= new TimeSpan(16, 0, 0);
                case MeasurementType.Evening:
                    return time >= new TimeSpan(18, 0, 0) && time <= new TimeSpan(19, 0, 0);
                case MeasurementType.Night:
                    return time >= new TimeSpan(22, 0, 0) && time <= new TimeSpan(23, 0, 0);
                default:
                    return false;
            }
        }

        // Get measurements from patients with critical values
        public List<BloodGlucose> GetCriticalMeasurements(int doctorId)
        {
            List<BloodGlucose> criticalMeasurements = new List<BloodGlucose>();
            string sql = @"
                SELECT bg.* 
                FROM blood_glucose bg
                JOIN doctor_patient dp ON bg.patient_id = dp.patient_id
                WHERE dp.doctor_id = @doctorId
                  AND (bg.measurement_value < 70 OR bg.measurement_value > 200)
                ORDER BY bg.measurement_time DESC";

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
                            var measurement = MapBloodGlucoseFromReader(reader);
                            measurement.Patient = _userService.GetUserById(measurement.PatientId);
                            criticalMeasurements.Add(measurement);
                        }
                    }
                }
            }

            return criticalMeasurements;
        }

        // Get the latest measurement for a patient
        public BloodGlucose GetLatestMeasurement(int patientId)
        {
            BloodGlucose measurement = null;
            string sql = @"
                SELECT * FROM blood_glucose 
                WHERE patient_id = @patientId 
                ORDER BY measurement_time DESC 
                LIMIT 1";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", patientId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            measurement = MapBloodGlucoseFromReader(reader);
                        }
                    }
                }
            }
            
            return measurement;
        }

        // Create a new measurement
        public int CreateMeasurement(BloodGlucose measurement)
        {
            // Değerlerin makul aralıkta olduğunu kontrol et
            if (measurement.MeasurementValue < 0 || measurement.MeasurementValue > 1000)
            {
                throw new ArgumentOutOfRangeException("MeasurementValue", "Kan şekeri değeri 0-1000 mg/dL aralığında olmalıdır.");
            }

            if (measurement.InsulinDose.HasValue && (measurement.InsulinDose.Value < 0 || measurement.InsulinDose.Value > 100))
            {
                throw new ArgumentOutOfRangeException("InsulinDose", "İnsülin dozu 0-100 aralığında olmalıdır.");
            }

            string sql = @"
                INSERT INTO blood_glucose (patient_id, measurement_value, measurement_time, measurement_type, insulin_dose, notes)
                VALUES (@patientId, @measurementValue, @measurementTime, @measurementType, @insulinDose, @notes)
                RETURNING id";

            int newMeasurementId = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", measurement.PatientId);
                    cmd.Parameters.AddWithValue("@measurementValue", measurement.MeasurementValue);
                    cmd.Parameters.AddWithValue("@measurementTime", measurement.MeasurementTime);
                    cmd.Parameters.AddWithValue("@measurementType", measurement.MeasurementType);
                    
                    if (measurement.InsulinDose.HasValue)
                        cmd.Parameters.AddWithValue("@insulinDose", measurement.InsulinDose.Value);
                    else
                        cmd.Parameters.AddWithValue("@insulinDose", DBNull.Value);
                    
                    if (string.IsNullOrEmpty(measurement.Notes))
                        cmd.Parameters.AddWithValue("@notes", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@notes", measurement.Notes);

                    newMeasurementId = (int)cmd.ExecuteScalar();
                }
            }

            return newMeasurementId;
        }

        // Update an existing measurement
        public bool UpdateMeasurement(BloodGlucose measurement)
        {
            // Değerlerin makul aralıkta olduğunu kontrol et
            if (measurement.MeasurementValue < 0 || measurement.MeasurementValue > 1000)
            {
                throw new ArgumentOutOfRangeException("MeasurementValue", "Kan şekeri değeri 0-1000 mg/dL aralığında olmalıdır.");
            }

            if (measurement.InsulinDose.HasValue && (measurement.InsulinDose.Value < 0 || measurement.InsulinDose.Value > 100))
            {
                throw new ArgumentOutOfRangeException("InsulinDose", "İnsülin dozu 0-100 aralığında olmalıdır.");
            }

            string sql = @"
                UPDATE blood_glucose 
                SET measurement_value = @measurementValue, 
                    measurement_time = @measurementTime, 
                    measurement_type = @measurementType, 
                    insulin_dose = @insulinDose, 
                    notes = @notes
                WHERE id = @id";

            int rowsAffected = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", measurement.Id);
                    cmd.Parameters.AddWithValue("@measurementValue", measurement.MeasurementValue);
                    cmd.Parameters.AddWithValue("@measurementTime", measurement.MeasurementTime);
                    cmd.Parameters.AddWithValue("@measurementType", measurement.MeasurementType);
                    
                    if (measurement.InsulinDose.HasValue)
                        cmd.Parameters.AddWithValue("@insulinDose", measurement.InsulinDose.Value);
                    else
                        cmd.Parameters.AddWithValue("@insulinDose", DBNull.Value);
                    
                    if (string.IsNullOrEmpty(measurement.Notes))
                        cmd.Parameters.AddWithValue("@notes", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@notes", measurement.Notes);

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            return rowsAffected > 0;
        }

        // Get a specific measurement by ID
        public BloodGlucose GetMeasurementById(int id)
        {
            BloodGlucose measurement = null;
            string sql = "SELECT * FROM blood_glucose WHERE id = @id";

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
                            measurement = MapBloodGlucoseFromReader(reader);
                        }
                    }
                }
            }
            
            return measurement;
        }

        // Get daily average for a specific date
        public decimal? GetDailyAverage(int patientId, DateTime date)
        {
            decimal? averageValue = null;
            string sql = @"
                SELECT AVG(measurement_value) as average_value
                FROM blood_glucose 
                WHERE patient_id = @patientId 
                AND DATE(measurement_time) = @date";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", patientId);
                    cmd.Parameters.AddWithValue("@date", date.Date);
                    
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        averageValue = Convert.ToDecimal(result);
                    }
                }
            }
            
            return averageValue;
        }

        // Helper method to map a database record to a BloodGlucose object
        private BloodGlucose MapBloodGlucoseFromReader(IDataReader reader)
        {
            return new BloodGlucose
            {
                Id = Convert.ToInt32(reader["id"]),
                PatientId = Convert.ToInt32(reader["patient_id"]),
                MeasurementValue = Convert.ToDecimal(reader["measurement_value"]),
                MeasurementTime = Convert.ToDateTime(reader["measurement_time"]),
                MeasurementType = (MeasurementType)Enum.Parse(typeof(MeasurementType), reader["measurement_type"].ToString()),
                InsulinDose = reader["insulin_dose"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["insulin_dose"]) : null,
                Notes = reader["notes"] != DBNull.Value ? reader["notes"].ToString() : null,
                CreatedAt = Convert.ToDateTime(reader["created_at"])
            };
        }
    }
} 