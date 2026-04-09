// utils/auth.js

/**
 * Módulo para manejar la autenticación
 */
const auth = (() => {
    // Almacena los datos del usuario actual
    let currentUser = null;

    /**
     * Inicializa la autenticación
     * @returns {Promise} - Promesa que se resuelve cuando la autenticación está inicializada
     */
    const init = async () => {
        // Verifica si hay un token en localStorage
        const token = localStorage.getItem('token');
        if (!token) {
            return;
        }

        try {
            // Obtiene el perfil del usuario
            currentUser = await api.get('/users/profile');
            updateUIForAuthState(true);
        } catch (error) {
            // Si hay un error (ej. token expirado), limpia la sesión
            logout();
        }
    };

    /**
     * Inicia sesión con email y contraseña
     * @param {string} email - Email del usuario
     * @param {string} password - Contraseña del usuario
     * @returns {Promise} - Promesa con los datos del usuario
     */
    const login = async (email, password, deviceId = '') => {
        try {
            const response = await api.post('/auth/login', { email, password, deviceId });
            api.setToken(response.token);
            currentUser = response.user;
            
            updateUIForAuthState(true);
            
            return currentUser;
        } catch (error) {
            throw error;
        }
    };

    /**
     * Registra un nuevo usuario
     * @param {Object} userData - Datos del usuario
     * @returns {Promise} - Promesa con los datos del usuario registrado
     */
    const register = async (userData) => {
        try {
            const response = await api.post('/auth/register', userData);
            return response;
        } catch (error) {
            throw error;
        }
    };

    /**
     * Cierra la sesión actual
     */
    const logout = () => {
        api.setToken(null);
        currentUser = null;
        updateUIForAuthState(false);
    };

    /**
     * Verifica si el usuario está autenticado
     * @returns {boolean} - true si el usuario está autenticado
     */
    const isAuthenticated = () => {
        return !!currentUser;
    };

    /**
     * Obtiene el usuario actual
     * @returns {Object|null} - Datos del usuario actual o null si no hay usuario
     */
    const getUser = () => {
        return currentUser;
    };

    /**
     * Actualiza la UI según el estado de autenticación
     * @param {boolean} isLoggedIn - true si el usuario está autenticado
     */
    const updateUIForAuthState = (isLoggedIn) => {
        const loggedInLinks = document.getElementById('logged-in-links');
        const loggedOutLinks = document.getElementById('logged-out-links');
        const userNameElement = document.getElementById('user-name');
        const userAvatarElement = document.getElementById('user-avatar');

        if (isLoggedIn && currentUser) {
            // Muestra los enlaces para usuarios autenticados
            loggedInLinks.classList.remove('hidden');
            loggedOutLinks.classList.add('hidden');
            
            // Actualiza el nombre y avatar del usuario
            userNameElement.textContent = currentUser.nickName || `${currentUser.firstName} ${currentUser.lastName}`;
            if (currentUser.picture) {
                userAvatarElement.src = currentUser.picture;
            }

            // Obtiene las notificaciones no leídas
            getUnreadNotifications();
        } else {
            // Muestra los enlaces para usuarios no autenticados
            loggedInLinks.classList.add('hidden');
            loggedOutLinks.classList.remove('hidden');
        }
    };

    /**
     * Obtiene el conteo de notificaciones no leídas
     */
    const getUnreadNotifications = async () => {
        try {
            if (!isAuthenticated()) return;
            
            const count = await api.get('/notifications/unread-count');
            const badge = document.getElementById('notification-badge');
            
            if (count > 0) {
                badge.textContent = count;
                badge.classList.remove('hidden');
            } else {
                badge.classList.add('hidden');
            }
        } catch (error) {
            console.error('Error al obtener notificaciones:', error);
        }
    };

    /**
     * Verifica si el usuario tiene un rol específico
     * @param {string} role - Rol a verificar
     * @returns {boolean} - true si el usuario tiene el rol
     */
    const hasRole = (role) => {
        if (!currentUser || !currentUser.roles) {
            return false;
        }
        return currentUser.roles.includes(role);
    };

    return {
        init,
        login,
        register,
        logout,
        isAuthenticated,
        getUser,
        hasRole,
        getUnreadNotifications
    };
})();