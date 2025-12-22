using MedicalTriageSystem.Models;
using System.Collections.Generic;

namespace MedicalTriageSystem.Models.ViewModels
{
    public class DashboardViewModel
    {
        public DashboardStatistics Statistics { get; set; } = new DashboardStatistics();
        public List<Patient> RecentPatients { get; set; } = new List<Patient>();
        public List<TriageResult> UrgentCases { get; set; } = new List<TriageResult>();
        public List<Doctor> DoctorAvailability { get; set; } = new List<Doctor>();
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