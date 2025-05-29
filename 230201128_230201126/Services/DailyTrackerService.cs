using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using wpf_prolab.Dbprolab;
using wpf_prolab.Models;

namespace wpf_prolab.Services
{
    public class DailyTrackerService
    {
        // Get daily trackers by patient ID
        public List<DailyTracker> GetDailyTrackersByPatientId(int patientId)
        {
            List<DailyTracker> trackers = new List<DailyTracker>();
            string sql = "SELECT * FROM daily_trackers WHERE patient_id = @patientId ORDER BY tracking_date DESC";

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
                            trackers.Add(MapTrackerFromReader(reader));
                        }
                    }
                }
            }
            
            return trackers;
        }

        // Get daily trackers by date range
        public List<DailyTracker> GetDailyTrackersByDateRange(int patientId, DateTime startDate, DateTime endDate)
        {
            List<DailyTracker> trackers = new List<DailyTracker>();
            string sql = @"
                SELECT * FROM daily_trackers 
                WHERE patient_id = @patientId 
                AND tracking_date BETWEEN @startDate AND @endDate
                ORDER BY tracking_date DESC";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", patientId);
                    cmd.Parameters.AddWithValue("@startDate", startDate.Date);
                    cmd.Parameters.AddWithValue("@endDate", endDate.Date.AddDays(1).AddSeconds(-1)); // End of the day
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            trackers.Add(MapTrackerFromReader(reader));
                        }
                    }
                }
            }
            
            return trackers;
        }

        // Create a new daily tracker entry
        public int CreateDailyTracker(DailyTracker tracker)
        {
            string sql = @"
                INSERT INTO daily_trackers (patient_id, tracking_date, diet_followed, exercise_done)
                VALUES (@patientId, @trackingDate, @dietFollowed, @exerciseDone)
                RETURNING id";

            int newTrackerId = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", tracker.PatientId);
                    cmd.Parameters.AddWithValue("@trackingDate", tracker.TrackingDate.Date);
                    cmd.Parameters.AddWithValue("@dietFollowed", tracker.DietFollowed);
                    cmd.Parameters.AddWithValue("@exerciseDone", tracker.ExerciseDone);

                    newTrackerId = (int)cmd.ExecuteScalar();
                }
            }

            return newTrackerId;
        }

        // Update an existing daily tracker
        public bool UpdateDailyTracker(DailyTracker tracker)
        {
            string sql = @"
                UPDATE daily_trackers 
                SET diet_followed = @dietFollowed, exercise_done = @exerciseDone
                WHERE id = @id";

            int rowsAffected = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", tracker.Id);
                    cmd.Parameters.AddWithValue("@dietFollowed", tracker.DietFollowed);
                    cmd.Parameters.AddWithValue("@exerciseDone", tracker.ExerciseDone);

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            return rowsAffected > 0;
        }

        // Get a specific daily tracker by ID
        public DailyTracker GetDailyTrackerById(int id)
        {
            DailyTracker tracker = null;
            string sql = "SELECT * FROM daily_trackers WHERE id = @id";

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
                            tracker = MapTrackerFromReader(reader);
                        }
                    }
                }
            }
            
            return tracker;
        }

        // Get or create a daily tracker for a specific date
        public DailyTracker GetOrCreateTrackerForDate(int patientId, DateTime date)
        {
            string sql = "SELECT * FROM daily_trackers WHERE patient_id = @patientId AND tracking_date = @trackingDate";
            DailyTracker tracker = null;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", patientId);
                    cmd.Parameters.AddWithValue("@trackingDate", date.Date);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tracker = MapTrackerFromReader(reader);
                        }
                    }
                }

                // If no tracker found for this date, create a new one
                if (tracker == null)
                {
                    tracker = new DailyTracker
                    {
                        PatientId = patientId,
                        TrackingDate = date.Date,
                        DietFollowed = false,
                        ExerciseDone = false
                    };

                    // Save the new tracker
                    using (var cmd = new NpgsqlCommand(@"
                        INSERT INTO daily_trackers (patient_id, tracking_date, diet_followed, exercise_done)
                        VALUES (@patientId, @trackingDate, @dietFollowed, @exerciseDone)
                        RETURNING id, created_at", conn))
                    {
                        cmd.Parameters.AddWithValue("@patientId", tracker.PatientId);
                        cmd.Parameters.AddWithValue("@trackingDate", tracker.TrackingDate);
                        cmd.Parameters.AddWithValue("@dietFollowed", tracker.DietFollowed);
                        cmd.Parameters.AddWithValue("@exerciseDone", tracker.ExerciseDone);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                tracker.Id = Convert.ToInt32(reader["id"]);
                                tracker.CreatedAt = Convert.ToDateTime(reader["created_at"]);
                            }
                        }
                    }
                }
            }
            
            return tracker;
        }

        // Helper method to map a database record to a DailyTracker object
        private DailyTracker MapTrackerFromReader(IDataReader reader)
        {
            return new DailyTracker
            {
                Id = Convert.ToInt32(reader["id"]),
                PatientId = Convert.ToInt32(reader["patient_id"]),
                TrackingDate = Convert.ToDateTime(reader["tracking_date"]),
                DietFollowed = Convert.ToBoolean(reader["diet_followed"]),
                ExerciseDone = Convert.ToBoolean(reader["exercise_done"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"])
            };
        }
    }
} 