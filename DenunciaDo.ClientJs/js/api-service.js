/**
 * API Service for the Denuncia.Do application
 * Handles communication with the backend API
 */
class ApiService {
    constructor() {
        this.baseUrl = 'https://localhost:5001/api';
        this.token = localStorage.getItem('token');
    }

    /**
     * Set the authentication token
     * @param {string} token - JWT token
     */
    setToken(token) {
        this.token = token;
        localStorage.setItem('token', token);
    }

    /**
     * Clear the authentication token
     */
    clearToken() {
        this.token = null;
        localStorage.removeItem('token');
    }

    /**
     * Get the headers to use for API requests
     * @returns {Object} - Headers object
     */
    getHeaders() {
        const headers = {
            'Content-Type': 'application/json'
        };

        if (this.token) {
            headers['Authorization'] = `Bearer ${this.token}`;
        }

        return headers;
    }

    /**
     * Make a GET request to the API
     * @param {string} endpoint - API endpoint
     * @returns {Promise} - Promise with the response data
     */
    async get(endpoint) {
        try {
            const response = await fetch(`${this.baseUrl}/${endpoint}`, {
                method: 'GET',
                headers: this.getHeaders()
            });

            if (!response.ok) {
                throw new Error(`Error: ${response.status} - ${response.statusText}`);
            }

            return await response.json();
        } catch (error) {
            console.error(`Error in GET request to ${endpoint}:`, error);
            throw error;
        }
    }

    /**
     * Make a POST request to the API
     * @param {string} endpoint - API endpoint
     * @param {Object} data - Data to send
     * @returns {Promise} - Promise with the response data
     */
    async post(endpoint, data) {
        try {
            const response = await fetch(`${this.baseUrl}/${endpoint}`, {
                method: 'POST',
                headers: this.getHeaders(),
                body: JSON.stringify(data)
            });

            if (!response.ok) {
                throw new Error(`Error: ${response.status} - ${response.statusText}`);
            }

            return await response.json();
        } catch (error) {
            console.error(`Error in POST request to ${endpoint}:`, error);
            throw error;
        }
    }

    /**
     * Make a PUT request to the API
     * @param {string} endpoint - API endpoint
     * @param {Object} data - Data to send
     * @returns {Promise} - Promise with the response data
     */
    async put(endpoint, data) {
        try {
            const response = await fetch(`${this.baseUrl}/${endpoint}`, {
                method: 'PUT',
                headers: this.getHeaders(),
                body: JSON.stringify(data)
            });

            if (!response.ok) {
                throw new Error(`Error: ${response.status} - ${response.statusText}`);
            }

            return await response.json();
        } catch (error) {
            console.error(`Error in PUT request to ${endpoint}:`, error);
            throw error;
        }
    }

    /**
     * Make a DELETE request to the API
     * @param {string} endpoint - API endpoint
     * @returns {Promise} - Promise with the response data
     */
    async delete(endpoint) {
        try {
            const response = await fetch(`${this.baseUrl}/${endpoint}`, {
                method: 'DELETE',
                headers: this.getHeaders()
            });

            if (!response.ok) {
                throw new Error(`Error: ${response.status} - ${response.statusText}`);
            }

            return await response.json();
        } catch (error) {
            console.error(`Error in DELETE request to ${endpoint}:`, error);
            throw error;
        }
    }

    /**
     * Upload a file to the API
     * @param {string} endpoint - API endpoint
     * @param {FormData} formData - Form data with the file
     * @returns {Promise} - Promise with the response data
     */
    async uploadFile(endpoint, formData) {
        try {
            const headers = {};
            if (this.token) {
                headers['Authorization'] = `Bearer ${this.token}`;
            }

            const response = await fetch(`${this.baseUrl}/${endpoint}`, {
                method: 'POST',
                headers: headers,
                body: formData
            });

            if (!response.ok) {
                throw new Error(`Error: ${response.status} - ${response.statusText}`);
            }

            return await response.json();
        } catch (error) {
            console.error(`Error in file upload to ${endpoint}:`, error);
            throw error;
        }
    }

    // Authentication methods
    async login(email, password) {
        try {
            const response = await this.post('Users/authenticate', {
                email,
                password
            });

            if (response.success) {
                this.setToken(response.token);
            }

            return response;
        } catch (error) {
            console.error('Error during login:', error);
            throw error;
        }
    }

    async register(firstName, lastName, email, password) {
        try {
            return await this.post('Users', {
                firstName,
                lastName,
                email,
                password,
                userTypeId: 2 // Citizen user type
            });
        } catch (error) {
            console.error('Error during registration:', error);
            throw error;
        }
    }

    async logout() {
        this.clearToken();
    }

    // User methods
    async getCurrentUser() {
        try {
            const decodedToken = this.getDecodedToken();
            if (!decodedToken || !decodedToken.nameid) {
                return null;
            }

            return await this.get(`Users/${decodedToken.nameid}`);
        } catch (error) {
            console.error('Error getting current user:', error);
            return null;
        }
    }

    getDecodedToken() {
        if (!this.token) return null;
        
        try {
            const base64Url = this.token.split('.')[1];
            const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            }).join(''));

            return JSON.parse(jsonPayload);
        } catch (error) {
            console.error('Error decoding token:', error);
            return null;
        }
    }

    // Complaint methods
    async getComplaints() {
        return await this.get('Complaints');
    }

    async getComplaintById(id) {
        return await this.get(`Complaints/${id}`);
    }

    async createComplaint(complaintData) {
        return await this.post('Complaints', complaintData);
    }

    async updateComplaint(id, complaintData) {
        return await this.put(`Complaints/${id}`, complaintData);
    }

    async deleteComplaint(id) {
        return await this.delete(`Complaints/${id}`);
    }

    async uploadEvidence(file, complaintId) {
        const formData = new FormData();
        formData.append('file', file);
        formData.append('complaintId', complaintId);

        return await this.uploadFile('Evidences', formData);
    }

    async getComplaintsByUser(userId) {
        return await this.get(`Complaints/user/${userId}`);
    }

    async getComplaintsByDevice(deviceId) {
        return await this.get(`Complaints/device/${deviceId}`);
    }

    async getComplaintsByStatus(statusId) {
        return await this.get(`Complaints/status/${statusId}`);
    }

    async getComplaintsByType(typeId) {
        return await this.get(`Complaints/type/${typeId}`);
    }

    async searchComplaints(searchTerm) {
        return await this.get(`Complaints/search?searchTerm=${encodeURIComponent(searchTerm)}`);
    }

    // Comment methods
    async getCommentsByComplaint(complaintId) {
        return await this.get(`Comments/complaint/${complaintId}`);
    }

    async createComment(commentData) {
        return await this.post('Comments', commentData);
    }

    async deleteComment(id) {
        return await this.delete(`Comments/${id}`);
    }

    // Vote methods
    async getVotesByComplaint(complaintId) {
        return await this.get(`Votes/complaint/${complaintId}`);
    }

    async createVote(voteData) {
        return await this.post('Votes', voteData);
    }

    async deleteVote(id) {
        return await this.delete(`Votes/${id}`);
    }

    // Report methods
    async getReportsByComplaint(complaintId) {
        return await this.get(`Reports/complaint/${complaintId}`);
    }

    async createReport(reportData) {
        return await this.post('Reports', reportData);
    }

    // ComplaintType, Status, and UserType methods
    async getComplaintTypes() {
        return await this.get('ComplaintTypes');
    }

    async getStatuses() {
        return await this.get('Statuses');
    }

    async getUserTypes() {
        return await this.get('UserTypes');
    }

    // Device identifier
    getDeviceIdentifier() {
        let deviceId = localStorage.getItem('deviceId');
        
        if (!deviceId) {
            deviceId = this.generateUUID();
            localStorage.setItem('deviceId', deviceId);
        }
        
        return deviceId;
    }
    
    generateUUID() {
        let dt = new Date().getTime();
        const uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
            const r = (dt + Math.random() * 16) % 16 | 0;
            dt = Math.floor(dt / 16);
            return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
        });
        return uuid;
    }
}

// Create a singleton instance
const apiService = new ApiService();