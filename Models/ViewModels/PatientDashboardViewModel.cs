namespace MedicalTriageSystem.Models.ViewModels
{
    public class PatientDashboardSimpleViewModel
    {
        public PatientInfo? PatientInfo { get; set; }
        public TriageResultInfo? LastTriageResult { get; set; }
        public HealthMetrics? HealthMetrics { get; set; }
        public PatientStatistics? PatientStats { get; set; }
        public List<AppointmentInfo>? UpcomingAppointments { get; set; }
        public List<SymptomInfo>? RecentSymptoms { get; set; }
        public List<DoctorInfo>? DoctorAvailability { get; set; }
        public List<TriageResultInfo>? UrgentCasesList { get; set; }
    }
}