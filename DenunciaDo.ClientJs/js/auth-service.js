/**
 * Authentication Service for the Denuncia.Do application
 * Handles user authentication and state
 */
class AuthService {
    constructor() {
        this.currentUser = null;
        this.isAuthenticated = false;
        this.authStateListeners = [];
        
        // Check if user is already logged in
        this.initializeAuth();
    }

    /**
     * Initialize the authentication state
     */
    async initializeAuth() {
        const token = localStorage.getItem('token');
        
        if (token) {
            try {
                this.currentUser = await apiService.getCurrentUser();
                this.isAuthenticated = !!this.currentUser;
                this.notifyAuthStateChanged();
            } catch (error) {
                console.error('Error initializing authentication:', error);
                this.logout();
            }
        }
    }

    /**
     * Login a user with email and password
     * @param {string} email - User email
     * @param {string} password - User password
     * @returns {Promise} - Promise with the login result
     */
    async login(email, password) {
        try {
            const response = await apiService.login(email, password);
            
            if (response.success) {
                this.currentUser = response.user;
                this.isAuthenticated = true;
                this.notifyAuthStateChanged();
            }
            
            return response;
        } catch (error) {
            console.error('Login error:', error);
            throw error;
        }
    }

    /**
     * Register a new user
     * @param {string} firstName - User first name
     * @param {string} lastName - User last name
     * @param {string} email - User email
     * @param {string} password - User password
     * @returns {Promise} - Promise with the registration result
     */
    async register(firstName, lastName, email, password) {
        try {
            const response = await apiService.register(firstName, lastName, email, password);
            return response;
        } catch (error) {
            console.error('Registration error:', error);
            throw error;
        }
    }

    /**
     * Logout the current user
     */
    logout() {
        apiService.logout();
        this.currentUser = null;
        this.isAuthenticated = false;
        this.notifyAuthStateChanged();
    }

    /**
     * Check if the user is authenticated
     * @returns {boolean} - True if authenticated, false otherwise
     */
    isUserAuthenticated() {
        return this.isAuthenticated;
    }

    /**
     * Get the current user
     * @returns {Object|null} - Current user object or null if not authenticated
     */
    getCurrentUser() {
        return this.currentUser;
    }

    /**
     * Add a listener for authentication state changes
     * @param {function} listener - Function to call when auth state changes
     */
    addAuthStateListener(listener) {
        this.authStateListeners.push(listener);
    }

    /**
     * Remove a listener for authentication state changes
     * @param {function} listener - Function to remove from listeners
     */
    removeAuthStateListener(listener) {
        this.authStateListeners = this.authStateListeners.filter(l => l !== listener);
    }

    /**
     * Notify all listeners of authentication state change
     */
    notifyAuthStateChanged() {
        for (const listener of this.authStateListeners) {
            listener(this.isAuthenticated, this.currentUser);
        }
    }

    /**
     * Check if the current user has the given role
     * @param {string} role - Role to check
     * @returns {boolean} - True if user has the role, false otherwise
     */
    hasRole(role) {
        if (!this.isAuthenticated || !this.currentUser) {
            return false;
        }

        const userType = this.currentUser.userTypeName.toLowerCase();
        return userType === role.toLowerCase();
    }

    /**
     * Check if the user is the owner of the given entity
     * @param {Object} entity - Entity to check ownership
     * @returns {boolean} - True if user is the owner, false otherwise
     */
    isOwner(entity) {
        if (!this.isAuthenticated || !this.currentUser) {
            return false;
        }

        return entity.userId === this.currentUser.userId;
    }

    /**
     * Get the device identifier
     * @returns {string} - Device identifier
     */
    getDeviceIdentifier() {
        return apiService.getDeviceIdentifier();
    }
}

// Create a singleton instance
const authService = new AuthService();