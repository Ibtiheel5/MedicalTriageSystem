using MedicalTriageSystem.Models;

namespace MedicalTriageSystem.Models.ViewModels
{
    public class DashboardViewModel
    {
        public DashboardStatistics Statistics { get; set; } = new();
        public List<Patient> RecentPatients { get; set; } = new();
        public List<TriageResult> UrgentCases { get; set; } = new();
        public List<Doctor> DoctorAvailability { get; set; } = new();
        public List<Appointment> UpcomingAppointments { get; set; } = new(); // ✅ PROPRIÉTÉ AJOUTÉE
    }

    public class DashboardStatistics
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalTriages { get; set; }
        public int UrgentCases { get; set; }
        public int TodayTriages { get; set; }
        public int WeekTriages { get; set; }
        public TimeSpan AvgTriageTime { get; set; }
    }
}