using MedicalTriageSystem.Data;
using MedicalTriageSystem.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MedicalTriageSystem.Services
{
    public interface INotificationService
    {
        Task SendWelcomeNotification(int patientId);
        Task SendAppointmentConfirmation(int patientId, int appointmentId);
        Task SendEmergencyAlert(int patientId, string level, string recommendation);
        Task SendDoctorStatusUpdate(int doctorId, bool isAvailable);
        Task SendAppointmentCompletion(int patientId, int appointmentId);
        Task SendPrescriptionNotification(int patientId, int appointmentId);
    }

    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IEmailService _emailService;

        public NotificationService(
            ApplicationDbContext context,
            IHubContext<NotificationHub> hubContext,
            IEmailService emailService)
        {
            _context = context;
            _hubContext = hubContext;
            _emailService = emailService;
        }

        public async Task SendWelcomeNotification(int patientId)
        {
            try
            {
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == patientId);

                if (patient == null) return;

                await _hubContext.Clients.User(patient.UserId.ToString())
                    .SendAsync("ReceiveNotification", new
                    {
                        Title = "Bienvenue sur MedicalTriageSystem",
                        Message = "Votre compte a été créé avec succès",
                        Type = "success",
                        Time = DateTime.Now.ToString("HH:mm")
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendWelcomeNotification: {ex.Message}");
            }
        }

        public async Task SendAppointmentConfirmation(int patientId, int appointmentId)
        {
            try
            {
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .FirstOrDefaultAsync(a => a.Id == appointmentId);

                if (appointment == null) return;

                await _hubContext.Clients.User(appointment.Patient.UserId.ToString())
                    .SendAsync("ReceiveAppointmentNotification", new
                    {
                        Title = "Rendez-vous confirmé",
                        Message = $"Votre RDV avec {appointment.Doctor?.Name} est confirmé pour le {appointment.Date:dd/MM/yyyy}",
                        AppointmentId = appointment.Id,
                        Time = DateTime.Now.ToString("HH:mm")
                    });

                // Send email
                await _emailService.SendAppointmentConfirmationAsync(
                    appointment.Patient.Email,
                    appointment.Patient.Name,
                    appointment.Date,
                    appointment.Doctor?.Name ?? "Médecin"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendAppointmentConfirmation: {ex.Message}");
            }
        }

        public async Task SendEmergencyAlert(int patientId, string level, string recommendation)
        {
            try
            {
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == patientId);

                if (patient == null) return;

                await _hubContext.Clients.User(patient.UserId.ToString())
                    .SendAsync("ReceiveEmergencyAlert", new
                    {
                        Title = "Alerte médicale",
                        Message = $"Niveau {level}: {recommendation}",
                        Level = level,
                        Time = DateTime.Now.ToString("HH:mm")
                    });

                // Notify available doctors
                var doctors = await _context.Doctors
                    .Where(d => d.IsAvailable && d.UserId != null)
                    .ToListAsync();

                foreach (var doctor in doctors)
                {
                    if (doctor.UserId.HasValue)
                    {
                        await _hubContext.Clients.User(doctor.UserId.Value.ToString())
                            .SendAsync("ReceiveUrgentCase", new
                            {
                                PatientName = patient.Name,
                                Level = level,
                                Time = DateTime.Now.ToString("HH:mm")
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendEmergencyAlert: {ex.Message}");
            }
        }

        public async Task SendDoctorStatusUpdate(int doctorId, bool isAvailable)
        {
            try
            {
                var doctor = await _context.Doctors.FindAsync(doctorId);
                if (doctor == null) return;

                await _hubContext.Clients.All
                    .SendAsync("DoctorStatusChanged", new
                    {
                        DoctorId = doctor.Id,
                        DoctorName = doctor.Name,
                        IsAvailable = isAvailable,
                        Time = DateTime.Now.ToString("HH:mm")
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendDoctorStatusUpdate: {ex.Message}");
            }
        }

        public async Task SendAppointmentCompletion(int patientId, int appointmentId)
        {
            try
            {
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .FirstOrDefaultAsync(a => a.Id == appointmentId);

                if (appointment == null) return;

                await _hubContext.Clients.User(appointment.Patient.UserId.ToString())
                    .SendAsync("ReceiveNotification", new
                    {
                        Title = "Rendez-vous terminé",
                        Message = "Votre consultation est terminée. Retrouvez votre ordonnance dans votre espace.",
                        Type = "info",
                        Time = DateTime.Now.ToString("HH:mm")
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendAppointmentCompletion: {ex.Message}");
            }
        }

        public async Task SendPrescriptionNotification(int patientId, int appointmentId)
        {
            try
            {
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .FirstOrDefaultAsync(a => a.Id == appointmentId);

                if (appointment == null) return;

                await _hubContext.Clients.User(appointment.Patient.UserId.ToString())
                    .SendAsync("ReceivePrescription", new
                    {
                        Title = "Nouvelle ordonnance",
                        Message = "Votre médecin a ajouté une nouvelle ordonnance",
                        AppointmentId = appointment.Id,
                        Time = DateTime.Now.ToString("HH:mm")
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendPrescriptionNotification: {ex.Message}");
            }
        }
    }
}