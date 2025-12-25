namespace MedicalTriageSystem.Models.ViewModels
{
    // Statistiques globales
    public class GlobalStatistics
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalTriages { get; set; }
        public int UrgentCasesCount { get; set; }
        public int TodayTriagesCount { get; set; }
        public TimeSpan AvgTriageTime { get; set; }
    }

    // Informations patient
    public class PatientInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string BloodType { get; set; } = string.Empty;
        public string Allergies { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // Informations médecin
    public class DoctorInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public string? Qualifications { get; set; }
        public string? Availability { get; set; }
        public string? ConsultationFee { get; set; }
    }

    // Résultat de triage
    public class TriageResultInfo
    {
        public PatientInfo? Patient { get; set; }
        public string Level { get; set; } = string.Empty;
        public int Score { get; set; }
        public string Recommendation { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // Métriques de santé
    public class HealthMetrics
    {
        public int HeartRate { get; set; }
        public int BloodPressure { get; set; }
        public double Temperature { get; set; }
        public int OxygenSaturation { get; set; }
    }

    // Statistiques patient
    public class PatientStatistics
    {
        public int TotalTriages { get; set; }
        public int UpcomingAppointmentsCount { get; set; }
        public int MedicalRecordsCount { get; set; }
        public int PrescriptionsCount { get; set; }
        public int TotalSymptoms { get; set; }
        public int UrgentCasesCount { get; set; }
    }

    // Rendez-vous
    public class AppointmentInfo
    {
        public DateTime Date { get; set; }
        public TimeSpan? StartTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DoctorInfo? Doctor { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    // Symptômes
    public class SymptomInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int Severity { get; set; }
    }
}