// utils/ui.js

/**
 * Módulo para manejar la interfaz de usuario
 */
const ui = (() => {
    /**
     * Muestra una notificación toast
     * @param {string} message - Mensaje a mostrar
     * @param {string} type - Tipo de notificación (success, error, warning, info)
     * @param {string} [title] - Título de la notificación
     */
    const showToast = (message, type = 'info', title = '') => {
        const toastContainer = document.getElementById('toast-container');
        
        // Crea el elemento toast
        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;
        
        // Crea el icono según el tipo
        let iconClass = 'fa-info-circle';
        if (type === 'success') iconClass = 'fa-check-circle';
        if (type === 'error') iconClass = 'fa-exclamation-circle';
        if (type === 'warning') iconClass = 'fa-exclamation-triangle';
        
        // Estructura del toast
        toast.innerHTML = `
            <div class="toast-icon">
                <i class="fas ${iconClass}"></i>
            </div>
            <div class="toast-content">
                ${title ? `<div class="toast-title">${title}</div>` : ''}
                <div class="toast-message">${message}</div>
            </div>
        `;
        
        // Agrega el toast al contenedor
        toastContainer.appendChild(toast);
        
        // Elimina el toast después de un tiempo
        setTimeout(() => {
            toast.style.animation = 'slideOut 0.3s ease forwards';
            setTimeout(() => {
                toastContainer.removeChild(toast);
            }, 300);
        }, config.toastDuration);
    };

    /**
     * Crea un elemento de paginación
     * @param {number} currentPage - Página actual
     * @param {number} totalPages - Total de páginas
     * @param {Function} onPageChange - Función a llamar cuando cambia la página
     * @returns {HTMLElement} - Elemento de paginación
     */
    const createPagination = (currentPage, totalPages, onPageChange) => {
        const pagination = document.createElement('div');
        pagination.className = 'pagination';
        
        // Botón de página anterior
        const prevButton = document.createElement('div');
        prevButton.className = 'pagination-item';
        prevButton.innerHTML = `<a href="#" class="pagination-link ${currentPage <= 1 ? 'disabled' : ''}"><i class="fas fa-chevron-left"></i></a>`;
        
        if (currentPage > 1) {
            prevButton.querySelector('a').addEventListener('click', (e) => {
                e.preventDefault();
                onPageChange(currentPage - 1);
            });
        }
        
        pagination.appendChild(prevButton);
        
        // Determina las páginas a mostrar
        let startPage = Math.max(1, currentPage - 2);
        let endPage = Math.min(totalPages, startPage + 4);
        
        if (endPage - startPage < 4 && totalPages > 5) {
            startPage = Math.max(1, endPage - 4);
        }
        
        // Siempre mostrar la primera página
        if (startPage > 1) {
            const firstPage = document.createElement('div');
            firstPage.className = 'pagination-item';
            firstPage.innerHTML = `<a href="#" class="pagination-link">1</a>`;
            
            firstPage.querySelector('a').addEventListener('click', (e) => {
                e.preventDefault();
                onPageChange(1);
            });
            
            pagination.appendChild(firstPage);
            
            // Agregar elipsis si hay un salto
            if (startPage > 2) {
                const ellipsis = document.createElement('div');
                ellipsis.className = 'pagination-item';
                ellipsis.innerHTML = `<span class="pagination-link disabled">...</span>`;
                pagination.appendChild(ellipsis);
            }
        }
        
        // Agregar las páginas
        for (let i = startPage; i <= endPage; i++) {
            const pageButton = document.createElement('div');
            pageButton.className = 'pagination-item';
            pageButton.innerHTML = `<a href="#" class="pagination-link ${i === currentPage ? 'active' : ''}">${i}</a>`;
            
            if (i !== currentPage) {
                pageButton.querySelector('a').addEventListener('click', (e) => {
                    e.preventDefault();
                    onPageChange(i);
                });
            }
            
            pagination.appendChild(pageButton);
        }
        
        // Siempre mostrar la última página
        if (endPage < totalPages) {
            // Agregar elipsis si hay un salto
            if (endPage < totalPages - 1) {
                const ellipsis = document.createElement('div');
                ellipsis.className = 'pagination-item';
                ellipsis.innerHTML = `<span class="pagination-link disabled">...</span>`;
                pagination.appendChild(ellipsis);
            }
            
            const lastPage = document.createElement('div');
            lastPage.className = 'pagination-item';
            lastPage.innerHTML = `<a href="#" class="pagination-link">${totalPages}</a>`;
            
            lastPage.querySelector('a').addEventListener('click', (e) => {
                e.preventDefault();
                onPageChange(totalPages);
            });
            
            pagination.appendChild(lastPage);
        }
        
        // Botón de página siguiente
        const nextButton = document.createElement('div');
        nextButton.className = 'pagination-item';
        nextButton.innerHTML = `<a href="#" class="pagination-link ${currentPage >= totalPages ? 'disabled' : ''}"><i class="fas fa-chevron-right"></i></a>`;
        
        if (currentPage < totalPages) {
            nextButton.querySelector('a').addEventListener('click', (e) => {
                e.preventDefault();
                onPageChange(currentPage + 1);
            });
        }
        
        pagination.appendChild(nextButton);
        
        return pagination;
    };

    /**
     * Formatea una fecha en formato local
     * @param {string} dateString - Fecha en formato ISO
     * @param {boolean} [showTime] - Si debe mostrar la hora
     * @returns {string} - Fecha formateada
     */
    const formatDate = (dateString, showTime = false) => {
        const date = new Date(dateString);
        
        const options = {
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        };
        
        if (showTime) {
            options.hour = '2-digit';
            options.minute = '2-digit';
        }
        
        return date.toLocaleDateString('es-DO', options);
    };

    /**
     * Formatea un número con separadores de miles
     * @param {number} number - Número a formatear
     * @returns {string} - Número formateado
     */
    const formatNumber = (number) => {
        return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    };

    /**
     * Trunca un texto a una longitud máxima
     * @param {string} text - Texto a truncar
     * @param {number} maxLength - Longitud máxima
     * @returns {string} - Texto truncado
     */
    const truncateText = (text, maxLength) => {
        if (text.length <= maxLength) {
            return text;
        }
        
        return text.substring(0, maxLength) + '...';
    };

    /**
     * Valida un formulario
     * @param {HTMLFormElement} form - Formulario a validar
     * @returns {boolean} - true si el formulario es válido
     */
    const validateForm = (form) => {
        const inputs = form.querySelectorAll('input, select, textarea');
        let isValid = true;
        
        inputs.forEach(input => {
            // Elimina mensajes de error previos
            const errorElement = input.parentElement.querySelector('.form-error');
            if (errorElement) {
                errorElement.remove();
            }
            
            // Valida según el tipo de campo
            if (input.hasAttribute('required') && !input.value.trim()) {
                showInputError(input, 'Este campo es obligatorio');
                isValid = false;
            } else if (input.type === 'email' && input.value.trim() && !validateEmail(input.value)) {
                showInputError(input, 'Email inválido');
                isValid = false;
            } else if (input.type === 'password' && input.value.trim() && input.value.length < config.validation.minPasswordLength) {
                showInputError(input, `La contraseña debe tener al menos ${config.validation.minPasswordLength} caracteres`);
                isValid = false;
            } else if (input.type === 'password' && input.id === 'confirm-password') {
                const password = form.querySelector('#password') || form.querySelector('#new-password');
                if (password && input.value !== password.value) {
                    showInputError(input, 'Las contraseñas no coinciden');
                    isValid = false;
                }
            }
        });
        
        return isValid;
    };

    /**
     * Muestra un mensaje de error en un campo de formulario
     * @param {HTMLElement} input - Campo de formulario
     * @param {string} message - Mensaje de error
     */
    const showInputError = (input, message) => {
        const errorElement = document.createElement('div');
        errorElement.className = 'form-error';
        errorElement.textContent = message;
        
        input.parentElement.appendChild(errorElement);
        input.classList.add('is-invalid');
    };

    /**
     * Valida un email
     * @param {string} email - Email a validar
     * @returns {boolean} - true si el email es válido
     */
    const validateEmail = (email) => {
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(email);
    };

    /**
     * Muestra un modal
     * @param {string} modalId - ID del modal a mostrar
     */
    const showModal = (modalId) => {
        const modal = document.getElementById(modalId);
        modal.style.display = 'flex';
        
        // Configurar el cierre del modal
        const closeButton = modal.querySelector('.close-modal');
        if (closeButton) {
            closeButton.addEventListener('click', () => {
                hideModal(modalId);
            });
        }
        
        // Cerrar el modal al hacer clic fuera del contenido
        modal.addEventListener('click', (event) => {
            if (event.target === modal) {
                hideModal(modalId);
            }
        });
    };

    /**
     * Oculta un modal
     * @param {string} modalId - ID del modal a ocultar
     */
    const hideModal = (modalId) => {
        const modal = document.getElementById(modalId);
        modal.style.display = 'none';
    };

    /**
     * Crea un estado vacío
     * @param {string} message - Mensaje a mostrar
     * @param {string} icon - Clase del icono
     * @param {Function} [action] - Función a ejecutar al hacer clic en el botón
     * @param {string} [buttonText] - Texto del botón
     * @returns {HTMLElement} - Elemento de estado vacío
     */
    const createEmptyState = (message, icon, action, buttonText) => {
        const emptyState = document.createElement('div');
        emptyState.className = 'empty-state';
        
        emptyState.innerHTML = `
            <div class="empty-state-icon">
                <i class="fas ${icon}"></i>
            </div>
            <p class="empty-state-text">${message}</p>
            ${action ? `<button class="btn btn-primary">${buttonText || 'Aceptar'}</button>` : ''}
        `;
        
        if (action && buttonText) {
            const button = emptyState.querySelector('button');
            button.addEventListener('click', action);
        }
        
        return emptyState;
    };

    /**
     * Inicializa los componentes de UI
     */
    const init = () => {
        // Configurar el toggle del menú móvil
        const mobileMenuToggle = document.querySelector('.mobile-menu-toggle');
        const mainNav = document.querySelector('.main-nav');
        
        if (mobileMenuToggle && mainNav) {
            mobileMenuToggle.addEventListener('click', () => {
                mainNav.classList.toggle('show');
            });
        }
        
        // Configurar el cierre de sesión
        const logoutLink = document.getElementById('logout-link');
        if (logoutLink) {
            logoutLink.addEventListener('click', (e) => {
                e.preventDefault();
                auth.logout();
                router.navigateTo('/');
                showToast('Has cerrado sesión correctamente', 'success');
            });
        }
    };

    return {
        showToast,
        createPagination,
        formatDate,
        formatNumber,
        truncateText,
        validateForm,
        showModal,
        hideModal,
        createEmptyState,
        init
    };
})();