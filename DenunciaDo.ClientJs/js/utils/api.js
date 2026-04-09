// utils/api.js

/**
 * Módulo para manejar las solicitudes a la API
 */
const api = (() => {
    // Token para autenticación
    let token = localStorage.getItem('token');

    /**
     * Configuración base para las solicitudes fetch
     * @param {string} method - Método HTTP (GET, POST, PUT, DELETE)
     * @param {Object} [body] - Cuerpo de la solicitud para POST y PUT
     * @param {boolean} [isFormData] - Indica si el body es FormData
     * @returns {Object} - Configuración para la solicitud fetch
     */
    const baseConfig = (method, body, isFormData = false) => {
        const config = {
            method,
            headers: {
                'Accept': 'application/json'
            }
        };

        if (token) {
            config.headers['Authorization'] = `Bearer ${token}`;
        }

        if (body) {
            if (isFormData) {
                // Para FormData no agregamos Content-Type
                config.body = body;
            } else {
                config.headers['Content-Type'] = 'application/json';
                config.body = JSON.stringify(body);
            }
        }

        return config;
    };

    /**
     * Maneja la respuesta de la API
     * @param {Response} response - Respuesta del fetch
     * @returns {Promise} - Promesa con los datos de la respuesta
     */
    const handleResponse = async (response) => {
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({}));
            throw {
                status: response.status,
                message: errorData.message || 'Ha ocurrido un error',
                data: errorData
            };
        }

        // Algunos endpoints devuelven un archivo o no tienen contenido JSON
        if (response.headers.get('content-type')?.includes('application/json')) {
            return response.json();
        }

        return response;
    };

    /**
     * Realiza una solicitud GET a la API
     * @param {string} endpoint - Endpoint de la API
     * @returns {Promise} - Promesa con los datos de la respuesta
     */
    const get = async (endpoint) => {
        try {
            const response = await fetch(`${config.apiUrl}${endpoint}`, baseConfig('GET'));
            return await handleResponse(response);
        } catch (error) {
            console.error('Error en solicitud GET:', error);
            throw error;
        }
    };

    /**
     * Realiza una solicitud POST a la API
     * @param {string} endpoint - Endpoint de la API
     * @param {Object} data - Datos a enviar
     * @param {boolean} [isFormData] - Indica si data es FormData
     * @returns {Promise} - Promesa con los datos de la respuesta
     */
    const post = async (endpoint, data, isFormData = false) => {
        try {
            const response = await fetch(`${config.apiUrl}${endpoint}`, baseConfig('POST', data, isFormData));
            return await handleResponse(response);
        } catch (error) {
            console.error('Error en solicitud POST:', error);
            throw error;
        }
    };

    /**
     * Realiza una solicitud PUT a la API
     * @param {string} endpoint - Endpoint de la API
     * @param {Object} data - Datos a enviar
     * @param {boolean} [isFormData] - Indica si data es FormData
     * @returns {Promise} - Promesa con los datos de la respuesta
     */
    const put = async (endpoint, data, isFormData = false) => {
        try {
            const response = await fetch(`${config.apiUrl}${endpoint}`, baseConfig('PUT', data, isFormData));
            return await handleResponse(response);
        } catch (error) {
            console.error('Error en solicitud PUT:', error);
            throw error;
        }
    };

    /**
     * Realiza una solicitud DELETE a la API
     * @param {string} endpoint - Endpoint de la API
     * @returns {Promise} - Promesa con los datos de la respuesta
     */
    const del = async (endpoint) => {
        try {
            const response = await fetch(`${config.apiUrl}${endpoint}`, baseConfig('DELETE'));
            return await handleResponse(response);
        } catch (error) {
            console.error('Error en solicitud DELETE:', error);
            throw error;
        }
    };

    /**
     * Establece el token de autenticación
     * @param {string} newToken - Nuevo token
     */
    const setToken = (newToken) => {
        token = newToken;
        if (newToken) {
            localStorage.setItem('token', newToken);
        } else {
            localStorage.removeItem('token');
        }
    };

    return {
        get,
        post,
        put,
        delete: del,
        setToken
    };
})();