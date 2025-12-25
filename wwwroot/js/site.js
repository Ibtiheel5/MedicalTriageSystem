// Fonctions utilitaires médicales
class MedicalUtils {
    static formatDateTime(date) {
        return new Date(date).toLocaleString('fr-FR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    }

    static calculateAge(birthDate) {
        const today = new Date();
        const birth = new Date(birthDate);
        let age = today.getFullYear() - birth.getFullYear();
        const monthDiff = today.getMonth() - birth.getMonth();

        if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
            age--;
        }
        return age;
    }

    static validatePhone(phone) {
        const regex = /^(\+33|0)[1-9](\d{2}){4}$/;
        return regex.test(phone.replace(/\s/g, ''));
    }

    static validateEmail(email) {
        const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return regex.test(email);
    }
}

// Gestionnaire de triage
class TriageHandler {
    constructor() {
        this.currentStep = 1;
        this.totalSteps = 5;
        this.answers = {};
    }

    nextStep() {
        if (this.currentStep < this.totalSteps) {
            this.currentStep++;
            this.updateProgress();
        }
    }

    prevStep() {
        if (this.currentStep > 1) {
            this.currentStep--;
            this.updateProgress();
        }
    }

    updateProgress() {
        const progress = (this.currentStep / this.totalSteps) * 100;
        $('.triage-progress').css('width', `${progress}%`);
        $('.triage-step').removeClass('active');
        $(`.triage-step-${this.currentStep}`).addClass('active');
    }

    calculateScore() {
        let score = 0;
        // Logique de calcul du score basée sur les réponses
        // À adapter selon votre algorithme de triage
        return Math.min(100, Math.max(0, score));
    }

    getRecommendation(score) {
        if (score >= 80) return { level: 'Urgent', action: 'Consulter immédiatement aux urgences' };
        if (score >= 60) return { level: 'Élevé', action: 'Consulter un médecin dans les 24h' };
        if (score >= 40) return { level: 'Moyen', action: 'Prendre rendez-vous avec votre médecin' };
        return { level: 'Faible', action: 'Surveillance à domicile recommandée' };
    }
}

// Gestionnaire de notifications en temps réel
class NotificationManager {
    constructor() {
        this.connection = null;
        this.initializeConnection();
    }

    async initializeConnection() {
        try {
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl("/notificationHub")
                .withAutomaticReconnect()
                .build();

            await this.connection.start();
            this.setupListeners();
        } catch (err) {
            console.error('SignalR Connection Error:', err);
            setTimeout(() => this.initializeConnection(), 5000);
        }
    }

    setupListeners() {
        this.connection.on("ReceiveAppointmentNotification", (appointment) => {
            this.showAppointmentNotification(appointment);
        });

        this.connection.on("ReceiveEmergencyAlert", (alert) => {
            this.showEmergencyAlert(alert);
        });

        this.connection.on("ReceiveTriageResult", (result) => {
            this.showTriageResult(result);
        });
    }

    showAppointmentNotification(appointment) {
        const html = `
            <div class="toast show" role="alert">
                <div class="toast-header bg-primary text-white">
                    <strong class="me-auto">
                        <i class="fas fa-calendar-check me-2"></i>
                        Nouveau Rendez-vous
                    </strong>
                    <small>${MedicalUtils.formatDateTime(new Date())}</small>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>
                </div>
                <div class="toast-body">
                    <p>${appointment.reason}</p>
                    <p class="mb-0">
                        <small>${appointment.date} à ${appointment.time}</small>
                    </p>
                </div>
            </div>
        `;
        this.addNotification(html);
    }

    showEmergencyAlert(alert) {
        const html = `
            <div class="toast show" role="alert">
                <div class="toast-header bg-danger text-white">
                    <strong class="me-auto">
                        <i class="fas fa-exclamation-triangle me-2"></i>
                        Alerte Urgente
                    </strong>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>
                </div>
                <div class="toast-body">
                    <p class="fw-bold">${alert.message}</p>
                    <a href="${alert.link}" class="btn btn-danger btn-sm">
                        <i class="fas fa-arrow-right me-1"></i> Voir les détails
                    </a>
                </div>
            </div>
        `;
        this.addNotification(html);
    }

    addNotification(html) {
        const container = $('#notification-container');
        if (!container.length) {
            $('body').append('<div id="notification-container" class="position-fixed top-0 end-0 p-3" style="z-index: 9999"></div>');
        }
        $('#notification-container').append(html);

        // Auto-remove after 10 seconds
        setTimeout(() => {
            $(html).remove();
        }, 10000);
    }
}

// Initialisation au chargement
$(document).ready(function () {
    // Initialiser les tooltips
    $('[data-bs-toggle="tooltip"]').tooltip();

    // Initialiser les popovers
    $('[data-bs-toggle="popover"]').popover();

    // Gestion des formulaires
    $('form').on('submit', function (e) {
        const submitBtn = $(this).find('button[type="submit"]');
        if (submitBtn.length) {
            submitBtn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-2"></span>En cours...');
        }
    });

    // Auto-format des numéros de téléphone
    $('.phone-input').on('input', function (e) {
        let value = $(this).val().replace(/\D/g, '');
        if (value.length > 0) {
            if (value.length <= 2) {
                value = value.replace(/(\d{2})/, '$1');
            } else if (value.length <= 4) {
                value = value.replace(/(\d{2})(\d{2})/, '$1 $2');
            } else if (value.length <= 6) {
                value = value.replace(/(\d{2})(\d{2})(\d{2})/, '$1 $2 $3');
            } else if (value.length <= 8) {
                value = value.replace(/(\d{2})(\d{2})(\d{2})(\d{2})/, '$1 $2 $3 $4');
            } else {
                value = value.replace(/(\d{2})(\d{2})(\d{2})(\d{2})(\d{2})/, '$1 $2 $3 $4 $5');
            }
        }
        $(this).val(value);
    });

    // Gestion des dates
    $('.datepicker').datepicker({
        format: 'dd/mm/yyyy',
        language: 'fr',
        autoclose: true,
        todayHighlight: true
    });

    // Initialiser le gestionnaire de notifications
    if (typeof signalR !== 'undefined') {
        window.notificationManager = new NotificationManager();
    }
});

// Extension String pour tronquer
String.prototype.truncate = function (n) {
    return this.length > n ? this.substr(0, n - 1) + '...' : this;
};

// Export pour utilisation globale
window.MedicalUtils = MedicalUtils;
window.TriageHandler = TriageHandler;