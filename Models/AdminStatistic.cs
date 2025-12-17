namespace MedicalTriageSystem.Models
{
    public class AdminStatistic
    {
        public int Id { get; set; }
        public int TotalTriages { get; set; }
        public int UrgentCases { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}