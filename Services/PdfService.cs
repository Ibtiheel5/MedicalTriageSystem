using MedicalTriageSystem.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MedicalTriageSystem.Services
{
    public interface IPdfService
    {
        byte[] GeneratePatientReport(Patient patient);
        byte[] GenerateTriageReport(TriageResult triageResult);
        byte[] GenerateAppointmentSummary(Appointment appointment);
        byte[] GenerateMedicalCertificate(Patient patient, Doctor doctor, string diagnosis);
    }

    public class PdfService : IPdfService
    {
        public byte[] GeneratePatientReport(Patient patient)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("MedicalTriageSystem")
                                .SemiBold().FontSize(24).FontColor(Colors.Blue.Medium);

                            row.AutoItem().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                                .FontSize(10).FontColor(Colors.Grey.Medium);
                        });

                        col.Item().PaddingTop(10).Text("Rapport Patient")
                            .SemiBold().FontSize(18);
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        // Patient Information
                        col.Item().PaddingBottom(10).Text("Informations Patient")
                            .SemiBold().FontSize(14);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Text("Nom:").SemiBold();
                            table.Cell().Text(patient.Name);

                            table.Cell().Text("Âge:").SemiBold();
                            table.Cell().Text($"{patient.Age} ans");

                            table.Cell().Text("Email:").SemiBold();
                            table.Cell().Text(patient.Email);

                            table.Cell().Text("Téléphone:").SemiBold();
                            table.Cell().Text(patient.Phone);

                            table.Cell().Text("Groupe sanguin:").SemiBold();
                            table.Cell().Text(patient.BloodType);
                        });

                        // Medical Information
                        if (!string.IsNullOrEmpty(patient.Allergies) || !string.IsNullOrEmpty(patient.MedicalHistory))
                        {
                            col.Item().PaddingTop(20).PaddingBottom(10)
                                .Text("Informations Médicales")
                                .SemiBold().FontSize(14);

                            if (!string.IsNullOrEmpty(patient.Allergies))
                            {
                                col.Item().PaddingBottom(5).Text("Allergies:")
                                    .SemiBold();
                                col.Item().Text(patient.Allergies);
                            }

                            if (!string.IsNullOrEmpty(patient.MedicalHistory))
                            {
                                col.Item().PaddingBottom(5).Text("Antécédents:")
                                    .SemiBold();
                                col.Item().Text(patient.MedicalHistory);
                            }
                        }
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ").FontSize(10);
                        text.CurrentPageNumber().FontSize(10);
                        text.Span(" / ").FontSize(10);
                        text.TotalPages().FontSize(10);
                    });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateTriageReport(TriageResult triageResult)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var levelColor = triageResult.Level switch
            {
                "Urgent" => Colors.Red.Medium,
                "Élevé" => Colors.Orange.Medium,
                "Moyen" => Colors.Yellow.Medium,
                _ => Colors.Green.Medium
            };

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("MedicalTriageSystem")
                                .SemiBold().FontSize(24).FontColor(Colors.Blue.Medium);

                            row.AutoItem().Text("Rapport de Triage")
                                .FontSize(10).FontColor(Colors.Grey.Medium);
                        });

                        col.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Column(col2 =>
                            {
                                col2.Item().Text("Patient:").SemiBold().FontSize(12);
                                col2.Item().Text(triageResult.Patient?.Name ?? "N/A").FontSize(14);
                            });

                            row.AutoItem().Column(col2 =>
                            {
                                col2.Item().Text("Date:").SemiBold().FontSize(12);
                                col2.Item().Text(triageResult.CreatedAt.ToString("dd/MM/yyyy HH:mm")).FontSize(14);
                            });
                        });
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        // Score and Level
                        col.Item().AlignCenter().PaddingBottom(20).Column(col2 =>
                        {
                            col2.Item().Text($"Score: {triageResult.Score}/100")
                                .SemiBold().FontSize(20);

                            col2.Item().PaddingTop(5).PaddingHorizontal(30).PaddingVertical(10)
                                .Background(levelColor).AlignCenter()
                                .Text(triageResult.Level)
                                .FontColor(Colors.White).SemiBold().FontSize(24);
                        });

                        // Recommendation
                        col.Item().PaddingBottom(15).Text("Recommandation:")
                            .SemiBold().FontSize(14);
                        col.Item().PaddingBottom(20).Text(triageResult.Recommendation)
                            .FontSize(12);

                        // Symptoms
                        if (!string.IsNullOrEmpty(triageResult.Symptoms))
                        {
                            col.Item().PaddingBottom(10).Text("Symptômes rapportés:")
                                .SemiBold().FontSize(14);
                            col.Item().Text(triageResult.Symptoms).FontSize(12);
                        }

                        // Notes
                        if (!string.IsNullOrEmpty(triageResult.Notes))
                        {
                            col.Item().PaddingTop(10).PaddingBottom(10)
                                .Text("Notes supplémentaires:")
                                .SemiBold().FontSize(14);
                            col.Item().Text(triageResult.Notes).FontSize(12);
                        }
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Document généré le ").FontSize(8);
                        text.Span(DateTime.Now.ToString("dd/MM/yyyy à HH:mm")).FontSize(8);
                        text.Span(" - MedicalTriageSystem").FontSize(8);
                    });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateAppointmentSummary(Appointment appointment)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("MedicalTriageSystem")
                                .SemiBold().FontSize(24).FontColor(Colors.Blue.Medium);

                            row.AutoItem().Text("Résumé de Consultation")
                                .FontSize(10).FontColor(Colors.Grey.Medium);
                        });

                        col.Item().PaddingTop(10).Text($"Rendez-vous du {appointment.Date:dd/MM/yyyy}")
                            .SemiBold().FontSize(16);
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        // Patient Info
                        col.Item().PaddingBottom(15).Row(row =>
                        {
                            row.RelativeItem().Column(col2 =>
                            {
                                col2.Item().Text("Patient:").SemiBold();
                                col2.Item().Text(appointment.Patient?.Name ?? "N/A");
                                col2.Item().Text(appointment.Patient?.Email ?? "");
                            });

                            row.RelativeItem().Column(col2 =>
                            {
                                col2.Item().Text("Médecin:").SemiBold();
                                col2.Item().Text(appointment.Doctor?.Name ?? "N/A");
                                col2.Item().Text(appointment.Doctor?.Specialty ?? "");
                            });
                        });

                        // Appointment Details
                        col.Item().PaddingTop(10).PaddingBottom(10)
                            .Background(Colors.Grey.Lighten3).Padding(10)
                            .Column(col2 =>
                            {
                                col2.Item().Text("Détails du rendez-vous:")
                                    .SemiBold().FontSize(12);

                                col2.Item().PaddingTop(5).Row(row =>
                                {
                                    row.RelativeItem().Text("Date:");
                                    row.RelativeItem().Text(appointment.Date.ToString("dddd dd MMMM yyyy"));
                                });

                                col2.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Heure:");
                                    row.RelativeItem().Text(appointment.StartTime?.ToString(@"hh\:mm") ?? "N/A");
                                });

                                col2.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Raison:");
                                    row.RelativeItem().Text(appointment.Reason);
                                });

                                col2.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Statut:");
                                    row.RelativeItem().Text(appointment.Status);
                                });
                            });

                        // Prescription
                        if (!string.IsNullOrEmpty(appointment.Prescription))
                        {
                            col.Item().PaddingTop(15).PaddingBottom(10)
                                .Text("Ordonnance:")
                                .SemiBold().FontSize(14);
                            col.Item().Text(appointment.Prescription).FontSize(12);
                        }

                        // Diagnosis
                        if (!string.IsNullOrEmpty(appointment.Diagnosis))
                        {
                            col.Item().PaddingTop(10).PaddingBottom(10)
                                .Text("Diagnostic:")
                                .SemiBold().FontSize(14);
                            col.Item().Text(appointment.Diagnosis).FontSize(12);
                        }

                        // Follow-up Instructions
                        if (!string.IsNullOrEmpty(appointment.FollowUpInstructions))
                        {
                            col.Item().PaddingTop(10).PaddingBottom(10)
                                .Text("Instructions de suivi:")
                                .SemiBold().FontSize(14);
                            col.Item().Text(appointment.FollowUpInstructions).FontSize(12);
                        }

                        // Notes
                        if (!string.IsNullOrEmpty(appointment.Notes))
                        {
                            col.Item().PaddingTop(10).Text("Notes du médecin:")
                                .SemiBold().FontSize(14);
                            col.Item().Text(appointment.Notes).FontSize(12);
                        }
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Document confidentiel - ").FontSize(8);
                        text.Span("Généré le ").FontSize(8);
                        text.Span(DateTime.Now.ToString("dd/MM/yyyy")).FontSize(8);
                    });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateMedicalCertificate(Patient patient, Doctor doctor, string diagnosis)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(3, Unit.Centimetre);

                    page.Header().Column(col =>
                    {
                        col.Item().AlignCenter().Text("CERTIFICAT MÉDICAL")
                            .SemiBold().FontSize(20).FontColor(Colors.Blue.Darken3);

                        col.Item().AlignCenter().PaddingTop(10)
                            .Text("MedicalTriageSystem")
                            .FontSize(12).FontColor(Colors.Grey.Medium);

                        col.Item().PaddingTop(20).LineHorizontal(1);
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Item().PaddingBottom(20).AlignCenter().Text(text =>
                        {
                            text.Span("Je soussigné, Dr. ").FontSize(12);
                            text.Span(doctor.Name).SemiBold().FontSize(12);
                            text.Span(", exerçant à ").FontSize(12);
                            text.Span("MedicalTriageSystem").SemiBold().FontSize(12);
                            text.Span(",").FontSize(12);
                        });

                        col.Item().PaddingBottom(30).AlignCenter().Text(text =>
                        {
                            text.Span("certifie avoir examiné ").FontSize(12);
                            text.Span(patient.Name).SemiBold().FontSize(12);
                            text.Span(", né(e) le N/A, ").FontSize(12);
                            text.Span($"âgé(e) de {patient.Age} ans.").FontSize(12);
                        });

                        col.Item().PaddingBottom(30).AlignCenter().Text("DIAGNOSTIC")
                            .SemiBold().FontSize(14).FontColor(Colors.Blue.Medium);

                        col.Item().PaddingBottom(30).AlignCenter().Text(diagnosis)
                            .FontSize(12);

                        col.Item().PaddingBottom(30).AlignCenter().Text("RECOMMANDATIONS")
                            .SemiBold().FontSize(14).FontColor(Colors.Blue.Medium);

                        col.Item().PaddingBottom(30).AlignCenter().Text(text =>
                        {
                            text.Span("Repos médical recommandé pour une durée adaptée à l'état du patient.").FontSize(12);
                            text.EmptyLine();
                            text.Span("Le patient est invité à suivre les traitements prescrits et à revenir en consultation si nécessaire.").FontSize(12);
                        });

                        col.Item().PaddingTop(50).Row(row =>
                        {
                            row.RelativeItem();
                            row.RelativeItem().Column(col2 =>
                            {
                                col2.Item().PaddingBottom(30).AlignCenter()
                                    .Text("Fait à Paris, le " + DateTime.Now.ToString("dd/MM/yyyy"))
                                    .FontSize(12);

                                col2.Item().PaddingTop(50).AlignCenter()
                                    .Text("Signature et cachet")
                                    .FontSize(12).FontColor(Colors.Grey.Medium);

                                col2.Item().PaddingTop(30).AlignCenter()
                                    .Text("Dr. " + doctor.Name)
                                    .SemiBold().FontSize(12);

                                col2.Item().AlignCenter()
                                    .Text(doctor.Specialty)
                                    .FontSize(10).FontColor(Colors.Grey.Medium);

                                col2.Item().AlignCenter()
                                    .Text("N° RPPS: " + (doctor.LicenseNumber ?? "En attente"))
                                    .FontSize(10).FontColor(Colors.Grey.Medium);
                            });
                        });
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Document officiel - Ne pas photocopier - ").FontSize(8);
                        text.Span("MedicalTriageSystem - ").SemiBold().FontSize(8);
                        text.Span(DateTime.Now.ToString("yyyy")).FontSize(8);
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}