using MedicalTriageSystem.Models;

namespace MedicalTriageSystem.Extensions
{
    public static class PatientExtensions
    {
        // Méthode pour obtenir le dernier triage
        public static TriageResult? GetLastTriage(this Patient patient)
        {
            if (patient.TriageResults == null || !patient.TriageResults.Any())
                return null;

            return patient.TriageResults
                .OrderByDescending(t => t.CreatedAt) // ⬅️ CHANGÉ: Utilisez CreatedAt au lieu de Date
                .FirstOrDefault();
        }

        // Méthode pour obtenir le niveau
        public static string GetTriageLevel(this Patient patient)
        {
            var lastTriage = patient.GetLastTriage();
            return lastTriage?.Level ?? "Non évalué";
        }

        // Méthode pour obtenir le score
        public static int GetTriageScore(this Patient patient)
        {
            var lastTriage = patient.GetLastTriage();
            return lastTriage?.Score ?? 0;
        }

        // Méthode pour obtenir la classe CSS du badge
        public static string GetTriageBadgeClass(this Patient patient)
        {
            var level = patient.GetTriageLevel();

            return level switch
            {
                "Urgent" => "bg-danger",
                "High" => "bg-warning",
                "Normal" => "bg-info",
                "Low" => "bg-success",
                _ => "bg-secondary"
            };
        }
    }
}