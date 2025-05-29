using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using wpf_prolab.Dbprolab;
using wpf_prolab.Models;

namespace wpf_prolab.Services
{
    public class SymptomService
    {
        private readonly UserService _userService;

        public SymptomService()
        {
            _userService = new UserService();
        }

        // Create a new symptom record
        public int CreateSymptom(Symptom symptom)
        {
            try
            {
                // Önce tablo varlığını kontrol edelim
                int tableExists = CheckTableExists("patient_symptoms");
                if (tableExists <= 0)
                {
                    // Tablo yok, symptoms tablosunu kontrol edelim
                    tableExists = CheckTableExists("symptoms");
                    if (tableExists > 0)
                    {
                        // symptoms tablosu var, onu kullanalım
                        Console.WriteLine("'patient_symptoms' tablosu bulunamadı, 'symptoms' tablosu kullanılıyor.");
                        return CreateSymptomInTable("symptoms", symptom);
                    }
                    else
                    {
                        throw new Exception("Veritabanında 'patient_symptoms' veya 'symptoms' tablosu bulunamadı!");
                    }
                }
                
                // Tablo var, normal şekilde devam edelim
                return CreateSymptomInTable("patient_symptoms", symptom);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateSymptom içinde hata: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"İç hata: {ex.InnerException.Message}");
                }
                throw; // Hata bilgilerini üst metoda ilet
            }
        }
        
        // Tablo kontrolü için yardımcı metod
        private int CheckTableExists(string tableName)
        {
            int exists = 0;
            try
        {
            string sql = @"
                    SELECT count(*) 
                    FROM information_schema.tables 
                    WHERE table_schema = 'public' 
                    AND table_name = @tableName";
                    
                using (var conn = DbConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@tableName", tableName);
                        exists = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Tablo kontrolü sırasında hata: {ex.Message}");
                exists = 0;
            }
            
            return exists;
        }
        
        // Belirtilen tabloya kayıt eklemek için yardımcı metod
        private int CreateSymptomInTable(string tableName, Symptom symptom)
        {
            // Doğrudan SQL sorgusu içinde enum değerini kullanarak cast işlemi yapalım
            string sql = $@"
                INSERT INTO {tableName} (patient_id, symptom, symptom_date, intensity, notes)
                VALUES ({symptom.PatientId}, '{symptom.SymptomType.ToString()}'::symptom_type, '{symptom.SymptomDate:yyyy-MM-dd}', {symptom.Intensity}, {(string.IsNullOrEmpty(symptom.Notes) ? "NULL" : $"'{symptom.Notes}'")})
                RETURNING id";

            int newSymptomId = 0;

            try
            {
            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                        // Debug bilgisi
                        Console.WriteLine($"SQL: {cmd.CommandText}");

                    newSymptomId = (int)cmd.ExecuteScalar();
                        Console.WriteLine($"Yeni semptom başarıyla eklendi, ID: {newSymptomId}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{tableName} tablosuna kayıt eklerken hata: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"İç hata: {ex.InnerException.Message}");
                }
                throw; // Hata bilgilerini üst metoda ilet
            }

            return newSymptomId;
        }

        // Update an existing symptom record
        public bool UpdateSymptom(Symptom symptom)
        {
            string sql = @"
                UPDATE patient_symptoms 
                SET symptom = @symptom, symptom_date = @symptomDate, intensity = @intensity, notes = @notes
                WHERE id = @id";

            int rowsAffected = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", symptom.Id);
                    cmd.Parameters.AddWithValue("@symptom", symptom.SymptomType.ToString());
                    cmd.Parameters.AddWithValue("@symptomDate", symptom.SymptomDate);
                    cmd.Parameters.AddWithValue("@intensity", symptom.Intensity);
                    
                    if (!string.IsNullOrEmpty(symptom.Notes))
                        cmd.Parameters.AddWithValue("@notes", symptom.Notes);
                    else
                        cmd.Parameters.AddWithValue("@notes", DBNull.Value);

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            return rowsAffected > 0;
        }

        // Delete a symptom record
        public bool DeleteSymptom(int symptomId)
        {
            string sql = "DELETE FROM patient_symptoms WHERE id = @id";
            int rowsAffected = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", symptomId);
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            return rowsAffected > 0;
        }

        // Get symptom by ID
        public Symptom GetSymptomById(int symptomId)
        {
            Symptom symptom = null;
            string sql = "SELECT * FROM patient_symptoms WHERE id = @id";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", symptomId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            symptom = MapSymptomFromReader(reader);
                        }
                    }
                }
            }

            return symptom;
        }

        // Get all symptoms for a patient
        public List<Symptom> GetSymptomsByPatientId(int patientId)
        {
            List<Symptom> symptoms = new List<Symptom>();
            string sql = "SELECT * FROM patient_symptoms WHERE patient_id = @patientId ORDER BY symptom_date DESC";

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
                            symptoms.Add(MapSymptomFromReader(reader));
                        }
                    }
                }
            }

            return symptoms;
        }

        // Get all symptoms for a patient by date range
        public List<Symptom> GetSymptomsByDateRange(int patientId, DateTime startDate, DateTime endDate)
        {
            List<Symptom> symptoms = new List<Symptom>();
            string sql = @"
                SELECT * FROM patient_symptoms 
                WHERE patient_id = @patientId 
                AND symptom_date BETWEEN @startDate AND @endDate
                ORDER BY symptom_date DESC";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", patientId);
                    cmd.Parameters.AddWithValue("@startDate", startDate.Date);
                    cmd.Parameters.AddWithValue("@endDate", endDate.Date.AddDays(1).AddSeconds(-1)); // End of day
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            symptoms.Add(MapSymptomFromReader(reader));
                        }
                    }
                }
            }

            return symptoms;
        }

        // Get today's symptoms for a patient
        public List<Symptom> GetTodaysSymptoms(int patientId)
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);
            return GetSymptomsByDateRange(patientId, today, tomorrow);
        }

        // Get recent symptoms (last 7 days) for a patient
        public List<Symptom> GetRecentSymptoms(int patientId)
        {
            DateTime today = DateTime.Today;
            DateTime sevenDaysAgo = today.AddDays(-7);
            return GetSymptomsByDateRange(patientId, sevenDaysAgo, today);
        }

        // Get active symptoms (has intensity >= 3) for a patient within the last 3 days
        public List<Symptom> GetActiveSymptoms(int patientId)
        {
            List<Symptom> activeSymptoms = new List<Symptom>();
            string sql = @"
                SELECT * FROM patient_symptoms 
                WHERE patient_id = @patientId 
                  AND symptom_date >= @threeDaysAgo
                  AND intensity >= 3
                ORDER BY intensity DESC, symptom_date DESC";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@patientId", patientId);
                    cmd.Parameters.AddWithValue("@threeDaysAgo", DateTime.Today.AddDays(-3));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            activeSymptoms.Add(MapSymptomFromReader(reader));
                        }
                    }
                }
            }

            return activeSymptoms;
        }

        // Helper method to map a database record to a Symptom object
        private Symptom MapSymptomFromReader(IDataReader reader)
        {
            var symptom = new Symptom
            {
                Id = Convert.ToInt32(reader["id"]),
                PatientId = Convert.ToInt32(reader["patient_id"]),
                SymptomType = (SymptomType)Enum.Parse(typeof(SymptomType), reader["symptom"].ToString()),
                SymptomDate = Convert.ToDateTime(reader["symptom_date"]),
                Intensity = Convert.ToInt32(reader["intensity"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"])
            };

            if (reader["notes"] != DBNull.Value)
                symptom.Notes = reader["notes"].ToString();

            return symptom;
        }

        // Get list of current symptoms by type for a patient (for recommendation engine)
        public List<SymptomType> GetCurrentSymptomTypes(int patientId)
        {
            var activeSymptoms = GetActiveSymptoms(patientId);
            List<SymptomType> symptomTypes = new List<SymptomType>();
            
            foreach (var symptom in activeSymptoms)
            {
                if (!symptomTypes.Contains(symptom.SymptomType))
                {
                    symptomTypes.Add(symptom.SymptomType);
                }
            }
            
            return symptomTypes;
        }

        // Add a symptom (wrapper for CreateSymptom)
        public bool AddSymptom(Symptom symptom)
        {
            try
            {
                int newId = CreateSymptom(symptom);
                Console.WriteLine($"AddSymptom başarılı: SymptomId={newId}");
                return newId > 0;
            }
            catch (Exception ex)
            {
                // Hata mesajını yazdıralım
                Console.WriteLine($"Semptom eklenirken hata oluştu: {ex.Message}");
                
                // İç içe geçmiş hatayı da kontrol edelim
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"İç hata: {ex.InnerException.Message}");
                }
                
                // Tüm hata yığınını yazdıralım
                Console.WriteLine($"Hata yığını: {ex.StackTrace}");
                
                // MessageBox üzerinden hata gösterelim (bu satır normalde istemcide olurdu)
                System.Windows.MessageBox.Show($"Semptom kaydedilirken bir hata oluştu: {ex.Message}", "Veritabanı Hatası", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                
                return false;
            }
        }
    }
} 