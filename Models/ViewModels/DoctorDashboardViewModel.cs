namespace MedicalTriageSystem.Models.ViewModels
{
    public class DoctorDashboardSimpleViewModel
    {
        public DoctorInfo DoctorInfo { get; set; } = new();
        public DoctorStatisticsInfo DoctorStatisticsInfo { get; set; } = new();
        public GlobalStatistics GlobalStatistics { get; set; } = new();
    }

    public class DoctorStatisticsInfo
    {
        public int MyPatientsCount { get; set; }
        public int MyAppointmentsToday { get; set; }
        public int PatientsSeenToday { get; set; }
        public int MyUrgentCasesCount { get; set; }
        public int AverageConsultationTime { get; set; }
        public double PatientRating { get; set; }
        public int CompletedAppointmentsCount { get; set; }
        public int MyAppointmentsThisWeek { get; set; }
    }
}