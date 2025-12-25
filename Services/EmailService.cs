using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MedicalTriageSystem.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);
        Task SendAppointmentConfirmationAsync(string to, string patientName, DateTime appointmentDate, string doctorName);
        Task SendTriageResultAsync(string to, string patientName, string triageLevel, string recommendation);
        Task SendPasswordResetAsync(string to, string resetLink);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderName = _configuration["EmailSettings:SenderName"];
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];

                // For development, just log the email
                if (string.IsNullOrEmpty(smtpServer) || _configuration["Environment"] == "Development")
                {
                    _logger.LogInformation($"Development Email: To: {to}, Subject: {subject}");
                    Console.WriteLine($"Email to {to}: {subject}");
                    return;
                }

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to {to}");
                // Don't throw, just log in development
                Console.WriteLine($"Email error (simulated in dev): {ex.Message}");
            }
        }

        public async Task SendAppointmentConfirmationAsync(string to, string patientName, DateTime appointmentDate, string doctorName)
        {
            var subject = "Confirmation de rendez-vous médical - MedicalTriageSystem";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                    <div style='text-align: center; background: linear-gradient(135deg, #1a73e8 0%, #0d6efd 100%); color: white; padding: 20px; border-radius: 10px 10px 0 0;'>
                        <h1 style='margin: 0;'>MedicalTriageSystem</h1>
                        <p style='margin: 5px 0 0 0; opacity: 0.9;'>Votre santé, notre priorité</p>
                    </div>
                    
                    <div style='padding: 30px 20px;'>
                        <h2 style='color: #1a73e8;'>Cher(e) {patientName},</h2>
                        <p>Votre rendez-vous médical a été confirmé avec succès.</p>
                        
                        <div style='background: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                            <h3 style='color: #198754; margin-top: 0;'>Détails du rendez-vous :</h3>
                            <ul style='list-style: none; padding: 0;'>
                                <li style='margin-bottom: 10px;'>
                                    <strong style='color: #495057;'>📅 Date :</strong> {appointmentDate:dddd dd MMMM yyyy}
                                </li>
                                <li style='margin-bottom: 10px;'>
                                    <strong style='color: #495057;'>👨‍⚕️ Médecin :</strong> Dr. {doctorName}
                                </li>
                                <li style='margin-bottom: 10px;'>
                                    <strong style='color: #495057;'>📍 Statut :</strong> <span style='color: #198754; font-weight: bold;'>Confirmé</span>
                                </li>
                            </ul>
                        </div>
                        
                        <p style='color: #6c757d; font-size: 14px;'>
                            <strong>Rappel :</strong> Veuillez arriver 15 minutes avant votre rendez-vous.
                            Pensez à apporter vos documents médicaux et votre carte vitale.
                        </p>
                        
                        <div style='text-align: center; margin-top: 30px;'>
                            <a href='{_configuration["AppSettings:BaseUrl"]}/PatientDashboard/Dashboard' 
                               style='background: #1a73e8; color: white; padding: 12px 30px; text-decoration: none; 
                                      border-radius: 5px; display: inline-block; font-weight: bold;'>
                               📋 Voir mes rendez-vous
                            </a>
                        </div>
                    </div>
                    
                    <div style='text-align: center; padding: 20px; background: #f8f9fa; border-radius: 0 0 10px 10px; 
                                 border-top: 1px solid #ddd; font-size: 12px; color: #6c757d;'>
                        <p style='margin: 0;'>
                            Cet email a été envoyé automatiquement. Merci de ne pas y répondre.<br>
                            © {DateTime.Now.Year} MedicalTriageSystem. Tous droits réservés.
                        </p>
                    </div>
                </div>
            </body>
            </html>";

            await SendEmailAsync(to, subject, body, true);
        }

        public async Task SendTriageResultAsync(string to, string patientName, string triageLevel, string recommendation)
        {
            var levelColor = triageLevel switch
            {
                "Urgent" => "#dc3545",
                "Élevé" => "#fd7e14",
                "Moyen" => "#ffc107",
                _ => "#198754"
            };

            var subject = $"Résultat de votre triage médical - Niveau: {triageLevel}";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                    <div style='text-align: center; background: linear-gradient(135deg, {levelColor} 0%, {levelColor}80 100%); 
                                 color: white; padding: 20px; border-radius: 10px 10px 0 0;'>
                        <h1 style='margin: 0;'>MedicalTriageSystem</h1>
                        <p style='margin: 5px 0 0 0; opacity: 0.9;'>Résultat de votre évaluation médicale</p>
                    </div>
                    
                    <div style='padding: 30px 20px;'>
                        <h2 style='color: {levelColor};'>Cher(e) {patientName},</h2>
                        <p>Voici les résultats de votre évaluation médicale réalisée le {DateTime.Now:dd/MM/yyyy} :</p>
                        
                        <div style='background: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0; text-align: center;'>
                            <div style='display: inline-block; padding: 15px 30px; background: {levelColor}; 
                                        color: white; border-radius: 50px; font-size: 24px; font-weight: bold;'>
                                Niveau {triageLevel}
                            </div>
                        </div>
                        
                        <div style='background: white; border-left: 4px solid {levelColor}; padding: 15px; margin: 20px 0;'>
                            <h4 style='color: {levelColor}; margin-top: 0;'>📋 Recommandation :</h4>
                            <p style='font-size: 16px;'>{recommendation}</p>
                        </div>
                        
                        <div style='background: #e7f3ff; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                            <h4 style='color: #1a73e8; margin-top: 0;'>ℹ️ Informations importantes :</h4>
                            <ul style='padding-left: 20px;'>
                                <li>Cette évaluation ne remplace pas une consultation médicale</li>
                                <li>Si votre état s'aggrave, consultez immédiatement un médecin</li>
                                <li>Gardez une trace de vos symptômes</li>
                            </ul>
                        </div>
                        
                        <div style='text-align: center; margin-top: 30px;'>
                            <a href='{_configuration["AppSettings:BaseUrl"]}/PatientDashboard/Dashboard' 
                               style='background: {levelColor}; color: white; padding: 12px 30px; text-decoration: none; 
                                      border-radius: 5px; display: inline-block; font-weight: bold; margin: 5px;'>
                               📊 Voir mon tableau de bord
                            </a>
                            <a href='{_configuration["AppSettings:BaseUrl"]}/Appointments/Create' 
                               style='background: #198754; color: white; padding: 12px 30px; text-decoration: none; 
                                      border-radius: 5px; display: inline-block; font-weight: bold; margin: 5px;'>
                               📅 Prendre un rendez-vous
                            </a>
                        </div>
                    </div>
                    
                    <div style='text-align: center; padding: 20px; background: #f8f9fa; border-radius: 0 0 10px 10px; 
                                 border-top: 1px solid #ddd; font-size: 12px; color: #6c757d;'>
                        <p style='margin: 0;'>
                            <strong>⚠️ Urgence médicale ?</strong> Composez le 15 (SAMU) ou le 112 (urgence européenne)<br>
                            © {DateTime.Now.Year} MedicalTriageSystem. Cet email est informatif.
                        </p>
                    </div>
                </div>
            </body>
            </html>";

            await SendEmailAsync(to, subject, body, true);
        }

        public async Task SendPasswordResetAsync(string to, string resetLink)
        {
            var subject = "Réinitialisation de votre mot de passe - MedicalTriageSystem";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                    <div style='text-align: center; background: linear-gradient(135deg, #6c757d 0%, #495057 100%); 
                                 color: white; padding: 20px; border-radius: 10px 10px 0 0;'>
                        <h1 style='margin: 0;'>MedicalTriageSystem</h1>
                        <p style='margin: 5px 0 0 0; opacity: 0.9;'>Réinitialisation de mot de passe</p>
                    </div>
                    
                    <div style='padding: 30px 20px;'>
                        <h2 style='color: #495057;'>Réinitialisation de mot de passe</h2>
                        <p>Vous avez demandé la réinitialisation de votre mot de passe.</p>
                        
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{resetLink}' 
                               style='background: #dc3545; color: white; padding: 15px 40px; text-decoration: none; 
                                      border-radius: 5px; display: inline-block; font-weight: bold; font-size: 16px;'>
                               🔒 Réinitialiser mon mot de passe
                            </a>
                        </div>
                        
                        <div style='background: #f8f9fa; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                            <p style='margin: 0; color: #6c757d; font-size: 14px;'>
                                <strong>⚠️ Important :</strong><br>
                                • Ce lien expirera dans 24 heures<br>
                                • Si vous n'avez pas demandé cette réinitialisation, ignorez cet email<br>
                                • Pour votre sécurité, ne partagez jamais ce lien
                            </p>
                        </div>
                        
                        <p style='color: #6c757d; font-size: 14px;'>
                            Si le bouton ne fonctionne pas, copiez-collez ce lien dans votre navigateur :<br>
                            <code style='background: #f1f3f5; padding: 5px 10px; border-radius: 3px; word-break: break-all;'>{resetLink}</code>
                        </p>
                    </div>
                    
                    <div style='text-align: center; padding: 20px; background: #f8f9fa; border-radius: 0 0 10px 10px; 
                                 border-top: 1px solid #ddd; font-size: 12px; color: #6c757d;'>
                        <p style='margin: 0;'>
                            Cet email a été envoyé à {to}<br>
                            © {DateTime.Now.Year} MedicalTriageSystem. Service de sécurité.
                        </p>
                    </div>
                </div>
            </body>
            </html>";

            await SendEmailAsync(to, subject, body, true);
        }
    }
}