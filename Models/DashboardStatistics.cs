namespace MedicalTriageSystem.Models.ViewModels
{
    public class DashboardStatistics
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalTriages { get; set; }
        public int UrgentCases { get; set; }
        public int TodayTriages { get; set; }
        public int WeekTriages { get; set; }
        public int MonthlyTriages { get; set; }
        public int MonthlyAppointments { get; set; }
        public TimeSpan AvgTriageTime { get; set; }
        public decimal AvgResponseTime { get; set; }
        public decimal PatientSatisfaction { get; set; }
    }
}