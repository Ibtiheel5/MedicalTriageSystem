using MedicalTriageSystem.Models;

namespace MedicalTriageSystem.Models.ViewModels
{
    public class PatientDashboardViewModel
    {
        public Patient Patient { get; set; }
        public List<TriageResult> TriageHistory { get; set; }
        public TriageResult? LastTriage { get; set; }

        // Add symptoms list for the view
        public List<Symptom> Symptoms { get; set; }

        // Add appointments list
        public List<Appointment> Appointments { get; set; }
    }
}