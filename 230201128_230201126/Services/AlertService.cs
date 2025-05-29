using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using wpf_prolab.Dbprolab;
using wpf_prolab.Models;

namespace wpf_prolab.Services
{
    public class AlertService
    {
        private readonly UserService _userService;
        private readonly BloodGlucoseService _bloodGlucoseService;

        public AlertService()
        {
            _userService = new UserService();
            _bloodGlucoseService = new BloodGlucoseService();
        }

        // Get all alerts for a patient
        public List<Alert> GetAlertsByPatientId(int patientId)
        {
            List<Alert> alerts = new List<Alert>();
            string sql = @"
                SELECT * FROM alerts 
                WHERE patient_id = @patientId 
                ORDER BY created_at DESC";

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
                            alerts.Add(MapAlertFromReader(reader));
                        }
                    }
                }
            }
            
            return alerts;
        }

        // Get unread alerts for a patient
        public List<Alert> GetUnreadAlerts(int patientId)
        {
            List<Alert> unreadAlerts = new List<Alert>();
            string sql = @"
                SELECT * FROM alerts 
                WHERE patient_id = @patientId AND is_read = false
                ORDER BY created_at DESC";

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
                            unreadAlerts.Add(MapAlertFromReader(reader));
                        }
                    }
                }
            }
            
            return unreadAlerts;
        }

        // Create a new alert
        public int CreateAlert(Alert alert)
        {
            string sql = @"
                INSERT INTO alerts (patient_id, alert_type, alert_message, is_read)
                VALUES (@patientId, @alertType, @alertMessage, @isRead)
                RETURNING id";

            int newAlertId = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", alert.PatientId);
                    cmd.Parameters.AddWithValue("@alertType", alert.AlertType);
                    cmd.Parameters.AddWithValue("@alertMessage", alert.AlertMessage);
                    cmd.Parameters.AddWithValue("@isRead", alert.IsRead);

                    newAlertId = (int)cmd.ExecuteScalar();
                }
            }

            return newAlertId;
        }

        // Mark an alert as read
        public bool MarkAlertAsRead(int alertId)
        {
            string sql = "UPDATE alerts SET is_read = true WHERE id = @id";
            int rowsAffected = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", alertId);
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            return rowsAffected > 0;
        }

        // Mark all alerts for a patient as read
        public bool MarkAllAlertsAsRead(int patientId)
        {
            string sql = "UPDATE alerts SET is_read = true WHERE patient_id = @patientId AND is_read = false";
            int rowsAffected = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", patientId);
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            return rowsAffected > 0;
        }

        // Get alert by ID
        public Alert GetAlertById(int alertId)
        {
            Alert alert = null;
            string sql = "SELECT * FROM alerts WHERE id = @id";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", alertId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            alert = MapAlertFromReader(reader);
                        }
                    }
                }
            }

            return alert;
        }

        // Get all unread alerts for a doctor's patients
        public List<Alert> GetUnreadAlertsForDoctor(int doctorId)
        {
            List<Alert> alerts = new List<Alert>();
            string sql = @"
                SELECT a.* 
                FROM alerts a
                JOIN doctor_patient dp ON a.patient_id = dp.patient_id
                WHERE dp.doctor_id = @doctorId AND a.is_read = false
                ORDER BY a.created_at DESC";

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
                            Alert alert = MapAlertFromReader(reader);
                            alert.Patient = _userService.GetUserById(alert.PatientId);
                            alerts.Add(alert);
                        }
                    }
                }
            }

            return alerts;
        }

        // Generate alerts based on blood glucose measurements
        public void GenerateGlucoseAlerts(BloodGlucose measurement)
        {
            // This is now handled by the database trigger, but we'll keep this method for other custom alerts
            // Refer to the create_glucose_alert() function in the database initialization
        }

        // Generate alert for missing measurements
        public void GenerateMissingMeasurementsAlert(int patientId, DateTime date)
        {
            // Check if there are any measurements for the day
            var measurements = _bloodGlucoseService.GetMeasurementsByDateRange(patientId, date.Date, date.Date.AddDays(1));
            
            if (measurements.Count == 0)
            {
                // Create alert for missing measurements
                var alert = new Alert
                {
                    PatientId = patientId,
                    AlertType = AlertTypes.MeasurementMissingAlert,
                    AlertMessage = "Hasta gün boyunca kan şekeri ölçümü yapmamıştır. Acil takip önerilir.",
                    IsRead = false
                };
                
                CreateAlert(alert);
            }
            else if (measurements.Count < 3)
            {
                // Create alert for insufficient measurements
                var alert = new Alert
                {
                    PatientId = patientId,
                    AlertType = AlertTypes.MeasurementInsufficientAlert,
                    AlertMessage = "Hastanın günlük kan şekeri ölçüm sayısı yetersiz (<3). Durum izlenmelidir.",
                    IsRead = false
                };
                
                CreateAlert(alert);
            }
        }

        // Helper method to map a database record to an Alert object
        private Alert MapAlertFromReader(IDataReader reader)
        {
            var alert = new Alert
            {
                Id = Convert.ToInt32(reader["id"]),
                PatientId = Convert.ToInt32(reader["patient_id"]),
                AlertType = reader["alert_type"].ToString(),
                AlertMessage = reader["alert_message"].ToString(),
                IsRead = Convert.ToBoolean(reader["is_read"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"])
            };

            return alert;
        }
    }
} 