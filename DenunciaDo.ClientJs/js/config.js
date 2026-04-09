// config.js

// Configuración de la aplicación
const config = {
    // URL base de la API
    apiUrl: 'https://localhost:7001/api',
    
    // Configuración de tipos de denuncias
    complaintTypes: {
        1: { name: 'Problemas Viales', icon: 'fa-road', color: '#3498db' },
        2: { name: 'Gestión de Residuos', icon: 'fa-trash', color: '#e74c3c' },
        3: { name: 'Suministro de Agua', icon: 'fa-tint', color: '#2980b9' },
        4: { name: 'Electricidad', icon: 'fa-bolt', color: '#f39c12' },
        5: { name: 'Seguridad Pública', icon: 'fa-shield-alt', color: '#27ae60' },
        6: { name: 'Otros', icon: 'fa-question-circle', color: '#95a5a6' }
    },
    
    // Configuración de estados de denuncias
    complaintStatuses: {
        1: { name: 'Pendiente', color: '#f39c12' },
        2: { name: 'En Progreso', color: '#3498db' },
        3: { name: 'Resuelto', color: '#2ecc71' },
        4: { name: 'Rechazado', color: '#e74c3c' }
    },

    // Configuración de items por página
    paginationLimit: 10,
    
    // Configuración de mapas
    mapDefaultLocation: {
        lat: 18.4861,
        lng: -69.9312,
        zoom: 13
    },
    
    // Configuración de tiempos
    toastDuration: 5000, // 5 segundos
    
    // Configuración de validación
    validation: {
        minPasswordLength: 8,
        maxCommentLength: 500
    }
};