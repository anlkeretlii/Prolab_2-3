using System;
using Npgsql;

namespace wpf_prolab.Dbprolab
{
    public static class DbInitializer
    {
        public static void InitializeDatabase()
        {
            // Create all required tables if they don't exist
            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();

                // Create users table
                using (var cmd = new NpgsqlCommand(
                    @"CREATE TABLE IF NOT EXISTS users (
                        id SERIAL PRIMARY KEY,
                        tc_id VARCHAR(11) UNIQUE NOT NULL,
                        password VARCHAR(255) NOT NULL,
                        email VARCHAR(255),
                        first_name VARCHAR(50) NOT NULL,
                        last_name VARCHAR(50) NOT NULL,
                        birth_date DATE NOT NULL,
                        gender CHAR(1) NOT NULL,
                        user_type INTEGER NOT NULL,
                        profile_picture BYTEA,
                        created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
                    )", conn))
                {
                    cmd.ExecuteNonQuery();
                }
                
                // Create doctor_patient relation table
                using (var cmd = new NpgsqlCommand(
                    @"CREATE TABLE IF NOT EXISTS doctor_patient (
                        doctor_id INTEGER NOT NULL REFERENCES users(id),
                        patient_id INTEGER NOT NULL REFERENCES users(id),
                        assigned_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        PRIMARY KEY (doctor_id, patient_id)
                    )", conn))
                {
                    cmd.ExecuteNonQuery();
                }
                
                // Create patient_profiles table
                using (var cmd = new NpgsqlCommand(
                    @"CREATE TABLE IF NOT EXISTS patient_profiles (
                        id SERIAL PRIMARY KEY,
                        patient_id INTEGER NOT NULL REFERENCES users(id),
                        diabetes_type INTEGER NOT NULL,
                        diagnosis_date DATE NOT NULL,
                        doctor_notes TEXT,
                        created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        UNIQUE (patient_id)
                    )", conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Create blood_glucose table
                using (var cmd = new NpgsqlCommand(
                    @"CREATE TABLE IF NOT EXISTS blood_glucose (
                        id SERIAL PRIMARY KEY,
                        patient_id INTEGER NOT NULL REFERENCES users(id),
                        measurement_value DECIMAL(5,1) NOT NULL,
                        measurement_time TIMESTAMP NOT NULL,
                        measurement_type VARCHAR(50) NOT NULL,
                        insulin_dose DECIMAL(5,1),
                        notes TEXT,
                        created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
                    )", conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Create daily_tracker table
                using (var cmd = new NpgsqlCommand(
                    @"CREATE TABLE IF NOT EXISTS daily_tracker (
                        id SERIAL PRIMARY KEY,
                        patient_id INTEGER NOT NULL REFERENCES users(id),
                        tracker_date DATE NOT NULL,
                        diet_followed BOOLEAN NOT NULL DEFAULT false,
                        exercise_done BOOLEAN NOT NULL DEFAULT false,
                        notes TEXT,
                        created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        UNIQUE (patient_id, tracker_date)
                    )", conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Create symptoms table
                using (var cmd = new NpgsqlCommand(
                    @"CREATE TABLE IF NOT EXISTS symptoms (
                        id SERIAL PRIMARY KEY,
                        patient_id INTEGER NOT NULL REFERENCES users(id),
                        symptom_date DATE NOT NULL,
                        symptom_type VARCHAR(50) NOT NULL,
                        intensity INTEGER NOT NULL,
                        notes TEXT,
                        created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
                    )", conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Create diet table
                using (var cmd = new NpgsqlCommand(
                    @"CREATE TABLE IF NOT EXISTS diets (
                        id SERIAL PRIMARY KEY,
                        patient_id INTEGER NOT NULL REFERENCES users(id),
                        start_date DATE NOT NULL,
                        end_date DATE,
                        diet_type VARCHAR(50) NOT NULL,
                        doctor_notes TEXT,
                        created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
                    )", conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Create exercise table
                using (var cmd = new NpgsqlCommand(
                    @"CREATE TABLE IF NOT EXISTS exercises (
                        id SERIAL PRIMARY KEY,
                        patient_id INTEGER NOT NULL REFERENCES users(id),
                        start_date DATE NOT NULL,
                        end_date DATE,
                        exercise_type VARCHAR(50) NOT NULL,
                        doctor_notes TEXT,
                        created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
                    )", conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Create alerts table
                using (var cmd = new NpgsqlCommand(
                    @"CREATE TABLE IF NOT EXISTS alerts (
                        id SERIAL PRIMARY KEY,
                        patient_id INTEGER NOT NULL REFERENCES users(id),
                        alert_type VARCHAR(50) NOT NULL,
                        alert_message TEXT NOT NULL,
                        is_read BOOLEAN NOT NULL DEFAULT false,
                        created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
                    )", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
} 