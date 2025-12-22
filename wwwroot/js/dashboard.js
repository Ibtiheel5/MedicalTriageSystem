// Dashboard Main JavaScript
$(document).ready(function () {
    // Initialize components
    initializeDashboard();
    initializeNotifications();
    initializeSearch();
    updateServerTime();

    // Sidebar toggle
    $('#sidebarToggle').click(function () {
        $('.sidebar').toggleClass('collapsed');
        $('.main-content').toggleClass('expanded');
    });

    // Mobile sidebar toggle
    $('.mobile-menu-toggle').click(function () {
        $('.sidebar').toggleClass('mobile-open');
    });

    // Load dashboard statistics
    loadDashboardStats();

    // Initialize charts
    initializeCharts();

    // Initialize calendar
    initializeCalendar();

    // Initialize DataTables
    initializeDataTables();

    // Initialize SignalR for real-time updates
    initializeSignalR();
});

// Dashboard Initialization
function initializeDashboard() {
    // Update urgent patients badge
    $.ajax({
        url: '/Dashboard/GetUrgentPatientsCount',
        method: 'GET',
        success: function (data) {
            if (data.count > 0) {
                $('#urgentPatientsBadge').text(data.count).show();
            } else {
                $('#urgentPatientsBadge').hide();
            }
        }
    });

    // Update notification count
    updateNotificationCount();

    // Auto-refresh stats every 30 seconds
    setInterval(loadDashboardStats, 30000);
}

// Load Dashboard Statistics
function loadDashboardStats() {
    $.ajax({
        url: '/Dashboard/GetStatistics',
        method: 'GET',
        success: function (data) {
            // Update quick stats
            $('#totalPatients').text(data.totalPatients);
            $('#availableDoctors').text(data.availableDoctors);
            $('#todayTriages').text(data.todayTriages);
            $('#urgentCases').text(data.urgentCases);

            // Update patient distribution chart
            updatePatientDistributionChart(data.patientDistribution);

            // Update triage trends chart
            updateTriageTrendsChart(data.triageTrends);
        }
    });
}

// Initialize Charts
function initializeCharts() {
    // Patient Distribution Chart
    const patientCtx = document.getElementById('patientDistributionChart');
    if (patientCtx) {
        window.patientDistributionChart = new Chart(patientCtx, {
            type: 'doughnut',
            data: {
                labels: ['Urgent', 'Élevé', 'Normal', 'Faible'],
                datasets: [{
                    data: [12, 19, 35, 42],
                    backgroundColor: [
                        '#e53e3e',
                        '#d69e2e',
                        '#3182ce',
                        '#38a169'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom'
                    }
                }
            }
        });
    }

    // Triage Trends Chart
    const trendsCtx = document.getElementById('triageTrendsChart');
    if (trendsCtx) {
        window.triageTrendsChart = new Chart(trendsCtx, {
            type: 'line',
            data: {
                labels: ['Lun', 'Mar', 'Mer', 'Jeu', 'Ven', 'Sam', 'Dim'],
                datasets: [{
                    label: 'Urgent',
                    data: [12, 19, 3, 5, 2, 3, 15],
                    borderColor: '#e53e3e',
                    backgroundColor: 'rgba(229, 62, 62, 0.1)',
                    tension: 0.4
                }, {
                    label: 'Normal',
                    data: [30, 25, 40, 35, 28, 20, 25],
                    borderColor: '#3182ce',
                    backgroundColor: 'rgba(49, 130, 206, 0.1)',
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }

    // Doctor Availability Chart
    const doctorCtx = document.getElementById('doctorAvailabilityChart');
    if (doctorCtx) {
        window.doctorAvailabilityChart = new Chart(doctorCtx, {
            type: 'bar',
            data: {
                labels: ['Dr. Martin', 'Dr. Sophie', 'Dr. Jean', 'Dr. Marie'],
                datasets: [{
                    label: 'Patients traités',
                    data: [25, 42, 18, 35],
                    backgroundColor: '#4299e1'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }
}

// Update Charts with Data
function updatePatientDistributionChart(data) {
    if (window.patientDistributionChart) {
        window.patientDistributionChart.data.datasets[0].data = [
            data.urgent || 0,
            data.high || 0,
            data.normal || 0,
            data.low || 0
        ];
        window.patientDistributionChart.update();
    }
}

function updateTriageTrendsChart(data) {
    if (window.triageTrendsChart) {
        window.triageTrendsChart.data.labels = data.labels || [];
        if (window.triageTrendsChart.data.datasets[0]) {
            window.triageTrendsChart.data.datasets[0].data = data.urgent || [];
        }
        if (window.triageTrendsChart.data.datasets[1]) {
            window.triageTrendsChart.data.datasets[1].data = data.normal || [];
        }
        window.triageTrendsChart.update();
    }
}

// Initialize Calendar
function initializeCalendar() {
    const calendarEl = document.getElementById('appointmentsCalendar');
    if (calendarEl) {
        const calendar = new FullCalendar.Calendar(calendarEl, {
            initialView: 'dayGridMonth',
            locale: 'fr',
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay'
            },
            events: '/Appointments/GetCalendarEvents',
            eventClick: function (info) {
                showAppointmentDetails(info.event.id);
            },
            eventColor: '#4299e1',
            eventTextColor: '#ffffff',
            height: 600
        });
        calendar.render();
        window.calendar = calendar;
    }
}

// Initialize DataTables
function initializeDataTables() {
    $('.data-table').each(function () {
        const table = $(this);
        table.DataTable({
            language: {
                url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/fr-FR.json'
            },
            responsive: true,
            order: [[0, 'desc']],
            pageLength: 10,
            dom: '<"row"<"col-md-6"l><"col-md-6"f>>rt<"row"<"col-md-6"i><"col-md-6"p>>'
        });
    });
}

// Notifications System
function initializeNotifications() {
    loadNotifications();

    // Mark all as read
    $('#markAllRead').click(function (e) {
        e.preventDefault();
        markAllNotificationsAsRead();
    });

    // Refresh notifications every 60 seconds
    setInterval(loadNotifications, 60000);
}

function loadNotifications() {
    $.ajax({
        url: '/Notifications/GetNotifications',
        method: 'GET',
        success: function (data) {
            $('#notificationList').empty();
            let unreadCount = 0;

            data.forEach(function (notification) {
                const notificationItem = `
                    <div class="notification-item ${notification.isRead ? '' : 'unread'}" 
                         data-id="${notification.id}"
                         onclick="markNotificationAsRead(${notification.id}, this)">
                        <div class="notification-icon">
                            <i class="fas ${getNotificationIcon(notification.type)} ${getNotificationColor(notification.type)}"></i>
                        </div>
                        <div class="notification-content">
                            <p class="mb-0">${notification.message}</p>
                            <small class="text-muted">${formatTimeAgo(notification.createdAt)}</small>
                        </div>
                    </div>
                `;
                $('#notificationList').append(notificationItem);

                if (!notification.isRead) {
                    unreadCount++;
                }
            });

            updateNotificationCount(unreadCount);
        }
    });
}

function updateNotificationCount(count) {
    const badge = $('#notificationCount');
    if (count > 0) {
        badge.text(count).show();
    } else {
        badge.hide();
    }
}

function markNotificationAsRead(id, element) {
    $.ajax({
        url: '/Notifications/MarkAsRead/' + id,
        method: 'POST',
        success: function () {
            $(element).removeClass('unread');
            updateNotificationCount($('.notification-item.unread').length);
        }
    });
}

function markAllNotificationsAsRead() {
    $.ajax({
        url: '/Notifications/MarkAllAsRead',
        method: 'POST',
        success: function () {
            $('.notification-item').removeClass('unread');
            updateNotificationCount(0);
        }
    });
}

// Search Functionality
function initializeSearch() {
    const searchInput = $('#globalSearch');
    let searchTimeout;

    searchInput.on('input', function () {
        clearTimeout(searchTimeout);
        const query = $(this).val();

        if (query.length >= 2) {
            searchTimeout = setTimeout(function () {
                performGlobalSearch(query);
            }, 500);
        } else {
            hideSearchResults();
        }
    });

    // Close search results when clicking outside
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.search-box, .search-results').length) {
            hideSearchResults();
        }
    });
}

function performGlobalSearch(query) {
    $.ajax({
        url: '/Dashboard/Search',
        method: 'GET',
        data: { query: query },
        success: function (data) {
            showSearchResults(data);
        }
    });
}

function showSearchResults(results) {
    let html = '<div class="search-results">';

    if (results.patients && results.patients.length > 0) {
        html += '<div class="search-category"><h6>Patients</h6></div>';
        results.patients.forEach(function (patient) {
            html += `
                <a href="/Patients/Details/${patient.id}" class="search-result-item">
                    <i class="fas fa-user-injured"></i>
                    <div>
                        <strong>${patient.name}</strong>
                        <small>${patient.age} ans • ${patient.triageLevel || 'Non évalué'}</small>
                    </div>
                </a>
            `;
        });
    }

    if (results.doctors && results.doctors.length > 0) {
        html += '<div class="search-category"><h6>Médecins</h6></div>';
        results.doctors.forEach(function (doctor) {
            html += `
                <a href="/Doctors/Details/${doctor.id}" class="search-result-item">
                    <i class="fas fa-user-md"></i>
                    <div>
                        <strong>${doctor.name}</strong>
                        <small>${doctor.specialty} • ${doctor.isAvailable ? 'Disponible' : 'Occupé'}</small>
                    </div>
                </a>
            `;
        });
    }

    if (results.results && results.results.length > 0) {
        html += '<div class="search-category"><h6>Résultats</h6></div>';
        results.results.forEach(function (result) {
            html += `
                <a href="/TriageResults/Details/${result.id}" class="search-result-item">
                    <i class="fas fa-clipboard-list"></i>
                    <div>
                        <strong>Triage #${result.id}</strong>
                        <small>${result.level} • ${result.patientName}</small>
                    </div>
                </a>
            `;
        });
    }

    if (html === '<div class="search-results">') {
        html += '<div class="search-no-results">Aucun résultat trouvé</div>';
    }

    html += '</div>';

    $('.search-box').append(html);
}

function hideSearchResults() {
    $('.search-results').remove();
}

// SignalR for Real-time Updates
function initializeSignalR() {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .withAutomaticReconnect()
        .build();

    connection.on("ReceiveNotification", function (notification) {
        showToastNotification(notification);
        loadNotifications(); // Refresh notification list
    });

    connection.on("NewUrgentPatient", function (patient) {
        showUrgentAlert(patient);
    });

    connection.on("DoctorStatusChanged", function (doctor) {
        updateDoctorStatus(doctor);
    });

    connection.start().catch(function (err) {
        console.error('SignalR connection error: ', err.toString());
    });
}

// Toast Notifications
function showToastNotification(notification) {
    const toast = $(`
        <div class="toast fade show" role="alert">
            <div class="toast-header">
                <i class="fas ${getNotificationIcon(notification.type)} me-2 ${getNotificationColor(notification.type)}"></i>
                <strong class="me-auto">${notification.title}</strong>
                <small>${formatTimeAgo(notification.createdAt)}</small>
                <button type="button" class="btn-close" data-bs-dismiss="toast"></button>
            </div>
            <div class="toast-body">
                ${notification.message}
            </div>
        </div>
    `);

    $('#toastContainer').append(toast);

    // Auto-remove after 5 seconds
    setTimeout(function () {
        toast.remove();
    }, 5000);
}

// Urgent Patient Alert
function showUrgentAlert(patient) {
    const alert = $(`
        <div class="alert alert-danger alert-dismissible fade show urgent-alert" role="alert">
            <div class="d-flex align-items-center">
                <i class="fas fa-exclamation-triangle fs-4 me-3"></i>
                <div>
                    <h5 class="mb-1">Nouveau patient urgent !</h5>
                    <p class="mb-0">${patient.name} - ${patient.age} ans • Score: ${patient.score}</p>
                    <small>${patient.symptoms}</small>
                </div>
            </div>
            <div class="mt-2">
                <a href="/Patients/Details/${patient.id}" class="btn btn-sm btn-danger me-2">
                    <i class="fas fa-user-md me-1"></i>Voir le patient
                </a>
                <a href="/Triage/AssignDoctor/${patient.id}" class="btn btn-sm btn-outline-danger">
                    <i class="fas fa-user-plus me-1"></i>Assigner un médecin
                </a>
            </div>
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `);

    $('#alertsContainer').prepend(alert);
}

// Utility Functions
function updateServerTime() {
    const timeElement = $('#currentTime');
    if (timeElement.length) {
        setInterval(function () {
            const now = new Date();
            timeElement.text(now.toLocaleTimeString('fr-FR'));
        }, 1000);
    }
}

function formatTimeAgo(dateString) {
    const date = new Date(dateString);
    const now = new Date();
    const seconds = Math.floor((now - date) / 1000);

    if (seconds < 60) return 'à l\'instant';
    if (seconds < 3600) return `il y a ${Math.floor(seconds / 60)} min`;
    if (seconds < 86400) return `il y a ${Math.floor(seconds / 3600)} h`;
    if (seconds < 604800) return `il y a ${Math.floor(seconds / 86400)} j`;
    return date.toLocaleDateString('fr-FR');
}

function getNotificationIcon(type) {
    const icons = {
        'urgent': 'fa-exclamation-triangle',
        'warning': 'fa-exclamation-circle',
        'info': 'fa-info-circle',
        'success': 'fa-check-circle',
        'patient': 'fa-user-injured',
        'doctor': 'fa-user-md',
        'appointment': 'fa-calendar-check'
    };
    return icons[type] || 'fa-bell';
}

function getNotificationColor(type) {
    const colors = {
        'urgent': 'text-danger',
        'warning': 'text-warning',
        'info': 'text-info',
        'success': 'text-success',
        'patient': 'text-primary',
        'doctor': 'text-success',
        'appointment': 'text-info'
    };
    return colors[type] || 'text-secondary';
}

// Export Data Functionality
function exportData(format) {
    const table = $('.data-table').DataTable();
    const data = table.rows({ search: 'applied' }).data();
    const exportData = [];

    // Get headers
    const headers = table.columns().header().toArray().map(th => $(th).text());
    exportData.push(headers);

    // Get data
    data.each(function (value, index) {
        exportData.push(value.toArray());
    });

    if (format === 'csv') {
        exportToCSV(exportData);
    } else if (format === 'excel') {
        exportToExcel(exportData);
    } else if (format === 'pdf') {
        exportToPDF(exportData);
    }
}

function exportToCSV(data) {
    let csvContent = "data:text/csv;charset=utf-8,";
    data.forEach(row => {
        csvContent += row.map(cell => `"${cell}"`).join(",") + "\r\n";
    });

    const encodedUri = encodeURI(csvContent);
    const link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", "export.csv");
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

// Quick Actions
function quickAction(action) {
    switch (action) {
        case 'newTriage':
            window.location.href = '/Triage/Create';
            break;
        case 'newPatient':
            window.location.href = '/Patients/Create';
            break;
        case 'newAppointment':
            window.location.href = '/Appointments/Create';
            break;
        case 'generateReport':
            generateQuickReport();
            break;
    }
}

function generateQuickReport() {
    const date = new Date().toISOString().split('T')[0];
    $.ajax({
        url: '/Reports/GenerateDailyReport',
        method: 'POST',
        data: { date: date },
        success: function (reportUrl) {
            window.open(reportUrl, '_blank');
        }
    });
}

// Theme Toggle
function toggleTheme() {
    const body = $('body');
    const currentTheme = body.data('theme') || 'light';
    const newTheme = currentTheme === 'light' ? 'dark' : 'light';

    body.data('theme', newTheme);
    body.attr('data-theme', newTheme);
    localStorage.setItem('theme', newTheme);

    // Update icon
    const icon = $('#themeToggleIcon');
    icon.toggleClass('fa-sun fa-moon');
}

// Initialize theme from localStorage
const savedTheme = localStorage.getItem('theme') || 'light';
$('body').attr('data-theme', savedTheme);
$('#themeToggleIcon').addClass(savedTheme === 'light' ? 'fa-sun' : 'fa-moon');

// Responsive Helper
function checkMobile() {
    return window.innerWidth <= 992;
}

// Auto-refresh components on window resize
$(window).resize(function () {
    if (window.calendar) {
        window.calendar.render();
    }
});

// Error Handling
$(document).ajaxError(function (event, jqxhr, settings, thrownError) {
    console.error('AJAX Error:', thrownError);
    showErrorToast('Une erreur est survenue. Veuillez réessayer.');
});

function showErrorToast(message) {
    const toast = $(`
        <div class="toast fade show" role="alert">
            <div class="toast-header bg-danger text-white">
                <i class="fas fa-exclamation-circle me-2"></i>
                <strong class="me-auto">Erreur</strong>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>
            </div>
            <div class="toast-body">
                ${message}
            </div>
        </div>
    `);

    $('#toastContainer').append(toast);

    setTimeout(function () {
        toast.remove();
    }, 5000);
}