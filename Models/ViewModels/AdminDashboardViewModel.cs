using System;
using System.Collections.Generic;

namespace MedicalTriageSystem.Models.ViewModels
{
    public class AdminDashboardSimpleViewModel
    {
        public GlobalStatistics Statistics { get; set; } = new();
        public List<PatientInfo> RecentPatients { get; set; } = new();
        public List<DoctorInfo> DoctorAvailability { get; set; } = new();
        public List<TriageResultInfo> UrgentCasesList { get; set; } = new();
    }
}