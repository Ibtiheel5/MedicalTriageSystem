namespace MedicalTriageSystem.Models.ViewModels
{
    public class PatientDashboardViewModel
    {
        public Patient Patient { get; set; } = null!;
        public TriageResult? LastTriage { get; set; }
        public List<Appointment> UpcomingAppointments { get; set; } = new();
        public List<Symptom> RecentSymptoms { get; set; } = new();

        // Ajouté pour matcher l'admin : Stats personnelles
        public DashboardStatistics Statistics { get; set; } = new DashboardStatistics();
        public List<TriageResult> UrgentCases { get; set; } = new();  // Cas urgents personnels
        public List<Doctor> DoctorAvailability { get; set; } = new();  // Médecins disponibles (général, comme admin)
    }
}