using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;
using NpgsqlTypes;
using BCrypt.Net;
using wpf_prolab.Models;

namespace wpf_prolab.Dbprolab
{
    // Enumların dotnet ve postgre arası dönüşümler
    public class EnumNameTranslator : INpgsqlNameTranslator
    {
        public string TranslateMemberName(string clrName) => clrName;
        public string TranslateTypeName(string clrName) => clrName.ToLower();
    }

    public static class DbConnection
    {
        private static string connectionString = "Host=localhost;Port=5432;Database=Prolab3;Username=postgres;Password=120494";
        private static bool _mappingsInitialized = false;
        private static readonly EnumNameTranslator nameTranslator = new EnumNameTranslator();

        static DbConnection()
        {
            SetupNpgsqlTypeMapping();
        }

        public static NpgsqlConnection GetConnection()
        {
            if (!_mappingsInitialized)
            {
                SetupNpgsqlTypeMapping();
            }
            return new NpgsqlConnection(connectionString);
        }

        private static void SetupNpgsqlTypeMapping()
        {
            try
            {
                NpgsqlConnection.GlobalTypeMapper.Reset();

                // Map PostgreSQL enum types to .NET enum types
                NpgsqlConnection.GlobalTypeMapper.MapEnum<UserType>("user_type", nameTranslator);
                NpgsqlConnection.GlobalTypeMapper.MapEnum<DietType>("diet_type", nameTranslator);
                NpgsqlConnection.GlobalTypeMapper.MapEnum<ExerciseType>("exercise_type", nameTranslator);
                NpgsqlConnection.GlobalTypeMapper.MapEnum<SymptomType>("symptom_type", nameTranslator);
                NpgsqlConnection.GlobalTypeMapper.MapEnum<MeasurementType>("measurement_type", nameTranslator);
                
                _mappingsInitialized = true;
                Console.WriteLine("Enum mappings initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting up Npgsql type mappings: {ex.Message}");
            }
        }

        public static void InitializeDatabase()
        {
            try
            {
                // Ensure type mappings are configured before any database operations
                if (!_mappingsInitialized)
                {
                    SetupNpgsqlTypeMapping();
                }
                
                // First check if the database exists, if not create it
                using (var adminConn = new NpgsqlConnection("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=120494"))
                {
                    adminConn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = adminConn;
                        cmd.CommandText = "SELECT 1 FROM pg_database WHERE datname='Prolab3'";
                        bool dbExists = cmd.ExecuteScalar() != null;

                        if (!dbExists)
                        {
                            cmd.CommandText = "CREATE DATABASE \"Prolab3\" WITH ENCODING='UTF8';";
                            cmd.ExecuteNonQuery();
                            Console.WriteLine("Database created successfully.");
                            
                            // Only if we create a new database, create all the schema
                            CreateDatabaseSchema();
                        }
                        else
                        {
                            Console.WriteLine("Database already exists.");
                            
                            // Check if we need to update schema, for example ensuring all tables exist
                            EnsureTablesExist();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
        
        private static void CreateDatabaseSchema()
        {
                using (var conn = GetConnection())
                {
                    conn.Open();
                Console.WriteLine("Connected to database. Creating schema...");

                try
                {
                    // Create enums first
                    ExecuteNonQuery(@"
                    DO $$
                    BEGIN
                        -- Create enum types if they don't exist
                        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'user_type') THEN
                            CREATE TYPE user_type AS ENUM ('Doctor', 'Patient');
                        END IF;
                        
                        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'diet_type') THEN
                            CREATE TYPE diet_type AS ENUM ('LowSugar', 'SugarFree', 'BalancedDiet');
                        END IF;
                        
                        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'exercise_type') THEN
                            CREATE TYPE exercise_type AS ENUM ('Walking', 'Cycling', 'ClinicalExercise');
                        END IF;
                        
                        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'symptom_type') THEN
                            CREATE TYPE symptom_type AS ENUM ('Polyuria', 'Polyphagia', 'Polydipsia', 'Neuropathy', 'WeightLoss', 'Fatigue', 'SlowHealingWounds', 'BlurredVision');
                        END IF;
                        
                        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'measurement_type') THEN
                            CREATE TYPE measurement_type AS ENUM ('Morning', 'Noon', 'Afternoon', 'Evening', 'Night');
                        END IF;
                    END
                    $$;", conn);

                    // Create all tables
                    CreateTables(conn);
                    
                    // Insert default admin
                    InsertDefaultAdmin(conn);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating database schema: {ex.Message}");
                    throw;
                }
            }
        }
        
        private static void EnsureTablesExist()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                Console.WriteLine("Checking database schema...");
                
                try
                {
                    // Check if main tables exist
                    bool usersExist = CheckTableExists(conn, "users");
                    
                    if (!usersExist)
                    {
                        Console.WriteLine("Tables don't exist, creating them...");
                        CreateTables(conn);
                        InsertDefaultAdmin(conn);
                    }
                    else
                    {
                        Console.WriteLine("Tables already exist, no need to create them.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking database schema: {ex.Message}");
                }
            }
        }
        
        private static bool CheckTableExists(NpgsqlConnection conn, string tableName)
        {
            using (var cmd = new NpgsqlCommand($"SELECT 1 FROM information_schema.tables WHERE table_name = '{tableName}' AND table_schema = 'public'", conn))
            {
                var result = cmd.ExecuteScalar();
                return result != null;
            }
        }
        
        private static void CreateTables(NpgsqlConnection conn)
        {
                    // Create Users table
                    ExecuteNonQuery(@"
                    CREATE TABLE IF NOT EXISTS users (
                        id SERIAL PRIMARY KEY,
                        tc_id VARCHAR(11) NOT NULL UNIQUE,
                        password VARCHAR(100) NOT NULL,
                        email VARCHAR(100) NOT NULL,
                        first_name VARCHAR(50) NOT NULL,
                        last_name VARCHAR(50) NOT NULL,
                        birth_date DATE NOT NULL,
                        gender CHAR(1) NOT NULL CHECK (gender IN ('M', 'F')),
                        user_type user_type NOT NULL,
                        profile_picture BYTEA,
                        created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
                    );", conn);

                    // Create DoctorPatient relationship table
                    ExecuteNonQuery(@"
                    CREATE TABLE IF NOT EXISTS doctor_patient (
                        id SERIAL PRIMARY KEY,
                        doctor_id INTEGER NOT NULL REFERENCES users(id),
                        patient_id INTEGER NOT NULL REFERENCES users(id),
                        assigned_date TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                        UNIQUE(doctor_id, patient_id)
                    );", conn);

                    // Kan Şekeri tablosu
                    ExecuteNonQuery(@"
                    CREATE TABLE IF NOT EXISTS blood_glucose (
                        id SERIAL PRIMARY KEY,
                        patient_id INTEGER NOT NULL REFERENCES users(id),
                        measurement_value DECIMAL(5,1) NOT NULL CHECK (measurement_value > 0),
                        measurement_time TIMESTAMP WITH TIME ZONE NOT NULL,
                measurement_type measurement_type NOT NULL,
                insulin_dose DECIMAL(3,1),
                notes TEXT,
                        created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
                    );", conn);

                    // Diyerler tablosu
                    ExecuteNonQuery(@"
                    CREATE TABLE IF NOT EXISTS diets (
                        id SERIAL PRIMARY KEY,
                        patient_id INTEGER NOT NULL REFERENCES users(id),
                        diet_type diet_type NOT NULL,
                        start_date DATE NOT NULL,
                        end_date DATE,
                        doctor_notes TEXT,
                doctor_id INTEGER NOT NULL REFERENCES users(id),
                        created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
                    );", conn);

                    // Egzersiz tablosu
                    ExecuteNonQuery(@"
                    CREATE TABLE IF NOT EXISTS exercises (
                        id SERIAL PRIMARY KEY,
                        patient_id INTEGER NOT NULL REFERENCES users(id),
                        exercise_type exercise_type NOT NULL,
                        start_date DATE NOT NULL,
                        end_date DATE,
                        doctor_notes TEXT,
                doctor_id INTEGER NOT NULL REFERENCES users(id),
                        created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
                    );", conn);

                    // Hasta logları takibi
                    ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS daily_trackers (
                        id SERIAL PRIMARY KEY,
                        patient_id INTEGER NOT NULL REFERENCES users(id),
                        tracking_date DATE NOT NULL,
                        diet_followed BOOLEAN,
                        exercise_done BOOLEAN,
                        created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                        UNIQUE(patient_id, tracking_date)
                    );", conn);

                    // semptom tablosu
                    ExecuteNonQuery(@"
                    CREATE TABLE IF NOT EXISTS patient_symptoms (
                        id SERIAL PRIMARY KEY,
                        patient_id INTEGER NOT NULL REFERENCES users(id),
                        symptom symptom_type NOT NULL,
                        symptom_date DATE NOT NULL,
                        intensity INTEGER NOT NULL CHECK (intensity BETWEEN 1 AND 5),
                        notes TEXT,
                        created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
                    );", conn);

                    // Uyarı tablosu
                    ExecuteNonQuery(@"
                    CREATE TABLE IF NOT EXISTS alerts (
                        id SERIAL PRIMARY KEY,
                        patient_id INTEGER NOT NULL REFERENCES users(id),
                        alert_type VARCHAR(50) NOT NULL,
                        alert_message TEXT NOT NULL,
                        is_read BOOLEAN DEFAULT FALSE,
                        created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
                    );", conn);

                    // insülin tablosu
                    ExecuteNonQuery(@"
                    CREATE TABLE IF NOT EXISTS insulin_recommendations (
                        id SERIAL PRIMARY KEY,
                        patient_id INTEGER NOT NULL REFERENCES users(id),
                        recommendation_date DATE NOT NULL,
                        measurement_type VARCHAR(20) NOT NULL,
                        average_glucose DECIMAL(5,1) NOT NULL,
                        insulin_dose DECIMAL(3,1) NOT NULL,
                        applied BOOLEAN DEFAULT FALSE,
                        created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
                    );", conn);

                    // uyarı için fonksiyonlar
                    ExecuteNonQuery(@"
                    CREATE OR REPLACE FUNCTION create_glucose_alert()
                    RETURNS TRIGGER AS $$
                    BEGIN
                        IF NEW.measurement_value < 70 THEN
                            INSERT INTO alerts (patient_id, alert_type, alert_message)
                            VALUES (NEW.patient_id, 'EmergencyAlert', 'Hastanın kan şekeri seviyesi 70 mg/dL''nin altına düştü. Hipoglisemi riski! Hızlı müdahale gerekebilir.');
                        ELSIF NEW.measurement_value > 200 THEN
                            INSERT INTO alerts (patient_id, alert_type, alert_message)
                            VALUES (NEW.patient_id, 'EmergencyAlert', 'Hastanın kan şekeri 200 mg/dL''nin üzerinde. Hiperglisemi durumu. Acil müdahale gerekebilir.');
                        ELSIF NEW.measurement_value BETWEEN 151 AND 200 THEN
                            INSERT INTO alerts (patient_id, alert_type, alert_message)
                            VALUES (NEW.patient_id, 'WarningAlert', 'Hastanın kan şekeri 151-200 mg/dL arasında. Diyabet kontrolü gereklidir.');
                        ELSIF NEW.measurement_value BETWEEN 111 AND 150 THEN
                            INSERT INTO alerts (patient_id, alert_type, alert_message)
                            VALUES (NEW.patient_id, 'InfoAlert', 'Hastanın kan şekeri 111-150 mg/dL arasında. Durum izlenmeli.');
                        END IF;
                        RETURN NEW;
                    END;
                    $$ LANGUAGE plpgsql;
                    
                    DROP TRIGGER IF EXISTS glucose_alert_trigger ON blood_glucose;
                    CREATE TRIGGER glucose_alert_trigger
                    AFTER INSERT ON blood_glucose
                    FOR EACH ROW
                    EXECUTE FUNCTION create_glucose_alert();
                    ", conn);

                    // Create index for performance optimization
                    ExecuteNonQuery(@"
                    -- Indexes for blood glucose measurements
                    CREATE INDEX IF NOT EXISTS idx_blood_glucose_patient_id ON blood_glucose(patient_id);
                    CREATE INDEX IF NOT EXISTS idx_blood_glucose_measurement_time ON blood_glucose(measurement_time);
                    
                    -- Indexes for doctor-patient relationship
                    CREATE INDEX IF NOT EXISTS idx_doctor_patient_doctor_id ON doctor_patient(doctor_id);
                    CREATE INDEX IF NOT EXISTS idx_doctor_patient_patient_id ON doctor_patient(patient_id);
                    
                    -- Indexes for alerts
                    CREATE INDEX IF NOT EXISTS idx_alerts_patient_id ON alerts(patient_id);
                    CREATE INDEX IF NOT EXISTS idx_alerts_is_read ON alerts(is_read);
                    ", conn);
        }

        private static void InsertDefaultAdmin(NpgsqlConnection conn)
        {
                    // Insert default admin doctor
                    var hashedPassword = HashPassword("admin123");
                    ExecuteNonQuery($@"
                    INSERT INTO users (tc_id, password, email, first_name, last_name, birth_date, gender, user_type)
                    SELECT '11111111111', '{hashedPassword}', 'admin@diabetes.com', 'Admin', 'Doctor', '1980-01-01', 'M', 'Doctor'
                    WHERE NOT EXISTS (SELECT 1 FROM users WHERE tc_id = '11111111111');
                    ", conn);
        }

        private static void ExecuteNonQuery(string sql, NpgsqlConnection connection)
        {
            using (var cmd = new NpgsqlCommand(sql, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
