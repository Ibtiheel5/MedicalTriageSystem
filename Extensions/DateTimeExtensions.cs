using System;

namespace MedicalTriageSystem.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToMedicalFormat(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy HH:mm");
        }

        public static string ToRelativeTime(this DateTime date)
        {
            var now = DateTime.UtcNow;
            var diff = now - date;

            if (diff.TotalMinutes < 1)
                return "À l'instant";
            if (diff.TotalMinutes < 60)
                return $"Il y a {Math.Floor(diff.TotalMinutes)} min";
            if (diff.TotalHours < 24)
                return $"Il y a {Math.Floor(diff.TotalHours)} h";
            if (diff.TotalDays < 30)
                return $"Il y a {Math.Floor(diff.TotalDays)} j";

            return date.ToString("dd/MM/yyyy");
        }

        public static int GetAge(this DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}