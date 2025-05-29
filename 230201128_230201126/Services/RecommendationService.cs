using System;
using System.Collections.Generic;
using System.Linq;
using wpf_prolab.Models;

namespace wpf_prolab.Services
{
    public class RecommendationService
    {
        private readonly BloodGlucoseService _bloodGlucoseService;
        private readonly AlertService _alertService;

        public RecommendationService()
        {
            _bloodGlucoseService = new BloodGlucoseService();
            _alertService = new AlertService();
        }

        /// <summary>
        /// Kan şekeri seviyesine göre öneriler oluşturur
        /// </summary>
        public List<Recommendation> GenerateRecommendations(int patientId)
        {
            var recommendations = new List<Recommendation>();
            
            // Son 3 gündeki ölçümleri al
            var recentMeasurements = _bloodGlucoseService.GetMeasurementsByDateRange(
                patientId, 
                DateTime.Today.AddDays(-3), 
                DateTime.Today.AddDays(1)
            );

            if (recentMeasurements.Any())
            {
                // Son ölçüm
                var latestMeasurement = recentMeasurements.OrderByDescending(m => m.MeasurementTime).First();
                
                // Günlük ortalama
                var dailyAverage = _bloodGlucoseService.CalculateDailyAverage(patientId, DateTime.Today);
                
                // 3 günlük ortalama
                var threedayAverage = recentMeasurements.Average(m => m.MeasurementValue);

                // Son ölçüme göre öneriler
                recommendations.AddRange(GetRecommendationsForGlucoseLevel(latestMeasurement.MeasurementValue, "Son ölçüm"));
                
                // Günlük ortalamaya göre öneriler
                if (dailyAverage > 0)
                {
                    recommendations.AddRange(GetRecommendationsForGlucoseLevel(dailyAverage, "Günlük ortalama"));
                }
                
                // Trend analizi
                recommendations.AddRange(AnalyzeTrend(recentMeasurements));
                
                // Ölçüm sıklığı kontrolü
                recommendations.AddRange(CheckMeasurementFrequency(patientId));
            }
            else
            {
                // Hiç ölçüm yoksa
                recommendations.Add(new Recommendation
                {
                    Type = RecommendationType.Measurement,
                    Priority = RecommendationPriority.High,
                    Title = "Kan Şekeri Ölçümü Gerekli",
                    Message = "Henüz kan şekeri ölçümü yapmamışsınız. Lütfen ilk ölçümünüzü yapın.",
                    Category = "Ölçüm"
                });
            }

            return recommendations.OrderByDescending(r => r.Priority).ToList();
        }

        /// <summary>
        /// Kan şekeri seviyesine göre spesifik öneriler
        /// </summary>
        private List<Recommendation> GetRecommendationsForGlucoseLevel(decimal glucoseLevel, string context)
        {
            var recommendations = new List<Recommendation>();

            if (glucoseLevel < 70) // Hipoglisemi
            {
                recommendations.Add(new Recommendation
                {
                    Type = RecommendationType.Emergency,
                    Priority = RecommendationPriority.Critical,
                    Title = "ACİL: Hipoglisemi",
                    Message = $"{context}: {glucoseLevel} mg/dL - Acil müdahale gerekli! Hemen şekerli gıda tüketin.",
                    Category = "Acil Durum",
                    Actions = new List<string> 
                    { 
                        "15g şeker alın (3 şeker küpü)",
                        "15 dakika bekleyin",
                        "Tekrar ölçüm yapın",
                        "Doktorunuzu arayın"
                    }
                });

                recommendations.Add(new Recommendation
                {
                    Type = RecommendationType.Diet,
                    Priority = RecommendationPriority.High,
                    Title = "Acil Beslenme",
                    Message = "Hipoglisemi için hızlı karbonhidrat alın",
                    Category = "Diyet",
                    Actions = new List<string>
                    {
                        "Meyve suyu (1 bardak)",
                        "Bal (1 yemek kaşığı)",
                        "Şekerli içecek (150ml)"
                    }
                });
            }
            else if (glucoseLevel <= 110) // Normal
            {
                recommendations.Add(new Recommendation
                {
                    Type = RecommendationType.Diet,
                    Priority = RecommendationPriority.Normal,
                    Title = "Dengeli Beslenme",
                    Message = $"{context}: {glucoseLevel} mg/dL - Normal seviye. Dengeli beslenmene devam et.",
                    Category = "Diyet",
                    Actions = new List<string>
                    {
                        "Az şekerli diyet",
                        "Düzenli öğünler",
                        "Bol su tüketimi"
                    }
                });

                recommendations.Add(new Recommendation
                {
                    Type = RecommendationType.Exercise,
                    Priority = RecommendationPriority.Normal,
                    Title = "Düzenli Egzersiz",
                    Message = "Yürüyüş veya hafif egzersiz yapabilirsiniz",
                    Category = "Egzersiz",
                    Actions = new List<string>
                    {
                        "30 dakika yürüyüş",
                        "Hafif tempolu bisiklet",
                        "Günlük aktiviteleri artırın"
                    }
                });
            }
            else if (glucoseLevel <= 180) // Hafif yüksek
            {
                recommendations.Add(new Recommendation
                {
                    Type = RecommendationType.Diet,
                    Priority = RecommendationPriority.High,
                    Title = "Karbonhidrat Kontrolü",
                    Message = $"{context}: {glucoseLevel} mg/dL - Yüksek seviye. Karbonhidrat alımını azaltın.",
                    Category = "Diyet",
                    Actions = new List<string>
                    {
                        "Şekerli gıdalardan kaçının",
                        "Porsiyon miktarını azaltın",
                        "Lifli gıdaları tercih edin",
                        "Bol su için"
                    }
                });

                recommendations.Add(new Recommendation
                {
                    Type = RecommendationType.Exercise,
                    Priority = RecommendationPriority.High,
                    Title = "Aktif Egzersiz",
                    Message = "Kan şekerini düşürmek için egzersiz yapın",
                    Category = "Egzersiz",
                    Actions = new List<string>
                    {
                        "45 dakika tempolu yürüyüş",
                        "Merdiven çıkma",
                        "Hafif tempolu jogging"
                    }
                });
            }
            else // Çok yüksek (>180)
            {
                recommendations.Add(new Recommendation
                {
                    Type = RecommendationType.Warning,
                    Priority = RecommendationPriority.Critical,
                    Title = "UYARI: Çok Yüksek Kan Şekeri",
                    Message = $"{context}: {glucoseLevel} mg/dL - Kritik seviye! Doktorunuzla iletişime geçin.",
                    Category = "Uyarı",
                    Actions = new List<string>
                    {
                        "Doktorunuzu arayın",
                        "İnsülin dozunu kontrol edin",
                        "Şekerli gıdaları kesin",
                        "Çok su için"
                    }
                });

                recommendations.Add(new Recommendation
                {
                    Type = RecommendationType.Diet,
                    Priority = RecommendationPriority.Critical,
                    Title = "Acil Diyet Değişikliği",
                    Message = "Şekerli gıdaları tamamen kesin",
                    Category = "Diyet",
                    Actions = new List<string>
                    {
                        "Sadece su ve çay",
                        "Yeşil yapraklı sebzeler",
                        "Protein ağırlıklı beslenme"
                    }
                });

                recommendations.Add(new Recommendation
                {
                    Type = RecommendationType.Exercise,
                    Priority = RecommendationPriority.Normal,
                    Title = "Hafif Egzersiz",
                    Message = "Yoğun egzersizden kaçının, hafif aktivite yapın",
                    Category = "Egzersiz",
                    Actions = new List<string>
                    {
                        "Yavaş tempolu yürüyüş",
                        "Nefes egzersizleri",
                        "Germe hareketleri"
                    }
                });
            }

            return recommendations;
        }

        /// <summary>
        /// Kan şekeri trendini analiz eder
        /// </summary>
        private List<Recommendation> AnalyzeTrend(List<BloodGlucose> measurements)
        {
            var recommendations = new List<Recommendation>();
            
            if (measurements.Count < 3) return recommendations;

            var sortedMeasurements = measurements.OrderBy(m => m.MeasurementTime).ToList();
            var lastThree = sortedMeasurements.Skip(sortedMeasurements.Count - 3).Take(3).ToList();

            // Sürekli artış trendi
            if (lastThree[0].MeasurementValue < lastThree[1].MeasurementValue && 
                lastThree[1].MeasurementValue < lastThree[2].MeasurementValue)
            {
                recommendations.Add(new Recommendation
                {
                    Type = RecommendationType.Warning,
                    Priority = RecommendationPriority.High,
                    Title = "Artan Trend Tespit Edildi",
                    Message = "Son 3 ölçümde sürekli artış var. Dikkatli olun!",
                    Category = "Trend Analizi",
                    Actions = new List<string>
                    {
                        "Diyet planınızı gözden geçirin",
                        "İlaç alımını kontrol edin",
                        "Doktor kontrolü planlayın"
                    }
                });
            }

            // Sürekli azalış trendi
            if (lastThree[0].MeasurementValue > lastThree[1].MeasurementValue && 
                lastThree[1].MeasurementValue > lastThree[2].MeasurementValue)
            {
                if (lastThree[2].MeasurementValue < 80)
                {
                    recommendations.Add(new Recommendation
                    {
                        Type = RecommendationType.Warning,
                        Priority = RecommendationPriority.High,
                        Title = "Azalan Trend - Hipoglisemi Riski",
                        Message = "Kan şekeriniz sürekli düşüyor. Hipoglisemi riski!",
                        Category = "Trend Analizi",
                        Actions = new List<string>
                        {
                            "Daha sık ölçüm yapın",
                            "Yanınızda şeker bulundurun",
                            "İlaç dozlarını kontrol edin"
                        }
                    });
                }
                else
                {
                    recommendations.Add(new Recommendation
                    {
                        Type = RecommendationType.Info,
                        Priority = RecommendationPriority.Normal,
                        Title = "İyileşme Trendi",
                        Message = "Kan şeker değerleriniz düşüyor. Bu olumlu!",
                        Category = "Trend Analizi"
                    });
                }
            }

            return recommendations;
        }

        /// <summary>
        /// Ölçüm sıklığını kontrol eder
        /// </summary>
        private List<Recommendation> CheckMeasurementFrequency(int patientId)
        {
            var recommendations = new List<Recommendation>();
            var todayMeasurements = _bloodGlucoseService.GetTodaysMeasurements(patientId);

            if (todayMeasurements.Count == 0)
            {
                recommendations.Add(new Recommendation
                {
                    Type = RecommendationType.Measurement,
                    Priority = RecommendationPriority.High,
                    Title = "Bugün Ölçüm Yapılmadı",
                    Message = "Bugün henüz kan şekeri ölçümü yapmadınız.",
                    Category = "Ölçüm",
                    Actions = new List<string>
                    {
                        "Sabah açlık ölçümü yapın",
                        "Öğün öncesi ölçümler",
                        "Yatmadan önce ölçüm"
                    }
                });
            }
            else if (todayMeasurements.Count < 3)
            {
                recommendations.Add(new Recommendation
                {
                    Type = RecommendationType.Measurement,
                    Priority = RecommendationPriority.Normal,
                    Title = "Ölçüm Sıklığını Artırın",
                    Message = $"Bugün {todayMeasurements.Count} ölçüm yaptınız. Günde en az 3 ölçüm önerilir.",
                    Category = "Ölçüm"
                });
            }

            return recommendations;
        }

        /// <summary>
        /// Önerileri uyarı olarak sisteme ekler
        /// </summary>
        public void CreateAlertsFromRecommendations(int patientId, List<Recommendation> recommendations)
        {
            foreach (var recommendation in recommendations.Where(r => r.Priority >= RecommendationPriority.High))
            {
                string alertType;
                switch (recommendation.Type)
                {
                    case RecommendationType.Emergency:
                        alertType = AlertTypes.EmergencyAlert;
                        break;
                    case RecommendationType.Warning:
                        alertType = AlertTypes.WarningAlert;
                        break;
                    default:
                        alertType = AlertTypes.InfoAlert;
                        break;
                }

                var alert = new Alert
                {
                    PatientId = patientId,
                    AlertType = alertType,
                    AlertMessage = $"{recommendation.Title}: {recommendation.Message}",
                    IsRead = false
                };

                _alertService.CreateAlert(alert);
            }
        }
    }

    /// <summary>
    /// Öneri modeli
    /// </summary>
    public class Recommendation
    {
        public RecommendationType Type { get; set; }
        public RecommendationPriority Priority { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Category { get; set; }
        public List<string> Actions { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public enum RecommendationType
    {
        Diet,
        Exercise,
        Measurement,
        Warning,
        Emergency,
        Info
    }

    public enum RecommendationPriority
    {
        Low = 1,
        Normal = 2,
        High = 3,
        Critical = 4
    }
} 