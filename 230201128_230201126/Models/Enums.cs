namespace wpf_prolab.Models
{
    // Enum for user types
    public enum UserType
    {
        Doctor,
        Patient
    }

    // Enum for symptom types
    public enum SymptomType
    {
        Polyuria,        // Sık idrara çıkma
        Polyphagia,      // Aşırı açlık hissi
        Polydipsia,      // Aşırı susama hissi
        Neuropathy,      // El ve ayaklarda karıncalanma veya uyuşma hissi
        WeightLoss,      // Kilo kaybı
        Fatigue,         // Yorgunluk
        SlowHealingWounds, // Yaraların yavaş iyileşmesi
        BlurredVision    // Bulanık görme
    }

    // Enum for diet types
    public enum DietType
    {
        LowSugar,       // Az Şekerli Diyet
        SugarFree,      // Şekersiz Diyet
        BalancedDiet    // Dengeli Beslenme
    }

    // Enum for exercise types
    public enum ExerciseType
    {
        Walking,          // Yürüyüş
        Cycling,          // Bisiklet
        ClinicalExercise  // Klinik Egzersiz
    }

    // Enum for measurement types
    public enum MeasurementType
    {
        Morning,  // Sabah Ölçümü (07:00 - 08:00)
        Noon,     // Öğle Ölçümü (12:00 - 13:00)
        Afternoon, // İkindi Ölçümü (15:00 - 16:00)
        Evening,  // Akşam Ölçümü (18:00 - 19:00)
        Night     // Gece Ölçümü (22:00 - 23:00)
    }
}
