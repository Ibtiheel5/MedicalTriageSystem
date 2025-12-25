using MedicalTriageSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedicalTriageSystem.Models.ViewModels
{
    // ===== DASHBOARD STATISTICS =====
    public class DashboardStatsViewModel
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalTriages { get; set; }
        public int UrgentCasesCount { get; set; }
        public int TodayTriagesCount { get; set; }
        public int WeekTriagesCount { get; set; }
        public int MonthlyTriagesCount { get; set; }
        public int MonthlyAppointmentsCount { get; set; }
        public TimeSpan AvgTriageTime { get; set; }
        public decimal AvgResponseTime { get; set; }
        public decimal PatientSatisfaction { get; set; }
    }

    // ===== ADMIN DASHBOARD =====
    public class AdminDashboardViewModel
    {
        public DashboardStatsViewModel Statistics { get; set; } = new();
        public List<Patient> RecentPatients { get; set; } = new();
        public List<TriageResult> UrgentCasesList { get; set; } = new();
        public List<Doctor> DoctorAvailability { get; set; } = new();
        public List<Appointment> UpcomingAppointments { get; set; } = new();
    }

    // ===== DOCTOR DASHBOARD =====
    public class DoctorDashboardViewModel
    {
        public Doctor DoctorInfo { get; set; }
        public DashboardStatsViewModel GlobalStatistics { get; set; } = new();
        public DoctorStatsViewModel DoctorStatisticsInfo { get; set; } = new();
        public List<Appointment> TodayAppointmentsList { get; set; } = new();
        public List<Appointment> UpcomingAppointmentsList { get; set; } = new();
        public List<TriageResult> UrgentCasesList { get; set; } = new();
        public List<Patient> MyPatientsList { get; set; } = new();
        public List<Doctor> AvailableDoctorsList { get; set; } = new();
        public List<AppointmentRequestViewModel> AppointmentRequestsList { get; set; } = new();
    }

    public class DoctorStatsViewModel
    {
        public int MyPatientsCount { get; set; }
        public int MyAppointmentsToday { get; set; }
        public int MyAppointmentsThisWeek { get; set; }
        public int MyUrgentCasesCount { get; set; }
        public decimal AverageConsultationTime { get; set; }
        public int PatientsSeenToday { get; set; }
        public int CompletedAppointmentsCount { get; set; }
        public decimal PatientRating { get; set; }
    }

    public class AppointmentRequestViewModel
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime PreferredDate { get; set; }
        public string PreferredTime { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    // ===== PATIENT DASHBOARD =====
    public class PatientDashboardViewModel
    {
        public Patient PatientInfo { get; set; }
        public TriageResult LastTriageResult { get; set; }
        public List<Appointment> UpcomingAppointments { get; set; } = new();
        public List<Symptom> RecentSymptoms { get; set; } = new();
        public PatientDashboardStatsViewModel PatientStats { get; set; } = new();
        public List<TriageResult> UrgentCasesList { get; set; } = new();
        public List<Doctor> DoctorAvailability { get; set; } = new();
        public HealthMetricsViewModel HealthMetrics { get; set; } = new();
    }

    public class PatientDashboardStatsViewModel
    {
        public int TotalTriages { get; set; }
        public int UrgentCasesCount { get; set; }
        public int TodayTriagesCount { get; set; }
        public int TotalSymptoms { get; set; }
        public int TotalAppointments { get; set; }
        public int UpcomingAppointmentsCount { get; set; }
        public int MedicalRecordsCount { get; set; }
        public int PrescriptionsCount { get; set; }
        public TimeSpan AvgTriageTime { get; set; }
    }

    public class HealthMetricsViewModel
    {
        public double HeartRate { get; set; } = 75;
        public double BloodPressure { get; set; } = 120;
        public double Temperature { get; set; } = 36.6;
        public double OxygenSaturation { get; set; } = 98;
    }

    // ===== QUICK TRIAGE =====
    public class QuickTriageRequestViewModel
    {
        public List<string> Symptoms { get; set; } = new();
        public string Notes { get; set; } = string.Empty;
    }

    // ===== LOGIN/REGISTER =====
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire")]
        [Display(Name = "Nom d'utilisateur")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Se souvenir de moi")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire")]
        [Display(Name = "Nom d'utilisateur")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'adresse email est obligatoire")]
        [EmailAddress(ErrorMessage = "Veuillez entrer une adresse email valide")]
        [Display(Name = "Adresse email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmation du mot de passe est obligatoire")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas")]
        [Display(Name = "Confirmer le mot de passe")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Nom complet")]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Veuillez entrer un numéro de téléphone valide")]
        [Display(Name = "Téléphone")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le type de compte est obligatoire")]
        [Display(Name = "Type de compte")]
        public string Role { get; set; } = "Patient";
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "L'adresse email est obligatoire")]
        [EmailAddress(ErrorMessage = "Veuillez entrer une adresse email valide")]
        [Display(Name = "Adresse email")]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Les mots de passe ne correspondent pas")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    // ===== APPOINTMENT VIEWMODELS =====
    public class CreateAppointmentViewModel
    {
        public int PatientId { get; set; }
        public int? DoctorId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }

        [Required]
        [StringLength(200)]
        public string Reason { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;
        public List<Doctor> AvailableDoctors { get; set; } = new();
    }

    public class EditAppointmentViewModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int? DoctorId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        [Required]
        [StringLength(200)]
        public string Reason { get; set; } = string.Empty;

        public string Status { get; set; } = "Scheduled";
        public string Diagnosis { get; set; } = string.Empty;
        public string Prescription { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public List<Doctor> AvailableDoctors { get; set; } = new();
    }

    // ===== SYMPTOM VIEWMODELS =====
    public class CreateSymptomViewModel
    {
        public int PatientId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(1, 10)]
        public int Severity { get; set; } = 1;

        public DateTime Date { get; set; } = DateTime.Now;
        public string Location { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Triggers { get; set; } = string.Empty;
        public string ReliefFactors { get; set; } = string.Empty;
        public string AssociatedSymptoms { get; set; } = string.Empty;
    }

    // ===== TRIAGE VIEWMODELS =====
    public class TriageViewModel
    {
        public int PatientId { get; set; }
        public List<string> Symptoms { get; set; } = new();
        public string Notes { get; set; } = string.Empty;
        public int Age { get; set; }
        public bool HasFever { get; set; }
        public bool HasChestPain { get; set; }
        public bool HasBreathingDifficulty { get; set; }
        public bool HasSeverePain { get; set; }
        public bool HasBleeding { get; set; }
    }

    public class TriageResultViewModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int Score { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Symptoms { get; set; } = string.Empty;
    }

    // ===== PROFILE VIEWMODELS =====
    public class UserProfileViewModel
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe actuel")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nouveau mot de passe")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Les mots de passe ne correspondent pas")]
        [Display(Name = "Confirmer le nouveau mot de passe")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}