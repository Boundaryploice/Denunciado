/**
 * Main JavaScript file for the Denuncia.Do application
 */
document.addEventListener('DOMContentLoaded', function() {
    // Initialize UI by loading complaints
    uiService.loadComplaints();
    
    // Set up event listeners
    setupNavbar();
    setupModals();
    setupButtons();
    setupFilters();
    setupForms();
});

/**
 * Set up navbar functionality
 */
function setupNavbar() {
    const navbarToggle = document.getElementById('navbar-toggle');
    const navbarMenu = document.getElementById('navbar-menu');
    const userDropdown = document.getElementById('user-dropdown');
    const dropdownMenu = document.querySelector('.dropdown-menu');
    
    // Toggle responsive navbar
    if (navbarToggle) {
        navbarToggle.addEventListener('click', () => {
            navbarMenu.classList.toggle('show');
        });
    }
    
    // Toggle user dropdown
    if (userDropdown) {
        userDropdown.addEventListener('click', (e) => {
            e.stopPropagation();
            dropdownMenu.classList.toggle('show');
        });
    }
    
    // Close dropdown when clicking elsewhere
    document.addEventListener('click', () => {
        if (dropdownMenu && dropdownMenu.classList.contains('show')) {
            dropdownMenu.classList.remove('show');
        }
    });
    
    // Logout button
    const logoutBtn = document.getElementById('logout-btn');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', (e) => {
            e.preventDefault();
            authService.logout();
            window.location.href = 'index.html';
        });
    }
}

/**
 * Set up modal functionality
 */
function setupModals() {
    // Setup modal close buttons
    document.querySelectorAll('.close-modal').forEach(btn => {
        btn.addEventListener('click', () => {
            const modal = btn.closest('.modal');
            modal.classList.remove('show');
        });
    });
    
    // Setup modal opening buttons
    const loginBtn = document.getElementById('login-btn');
    const registerBtn = document.getElementById('register-btn');
    const createComplaintBtn = document.getElementById('create-complaint-btn');
    
    if (loginBtn) {
        loginBtn.addEventListener('click', () => {
            document.getElementById('login-modal').classList.add('show');
        });
    }
    
    if (registerBtn) {
        registerBtn.addEventListener('click', () => {
            document.getElementById('register-modal').classList.add('show');
        });
    }
    
    if (createComplaintBtn) {
        createComplaintBtn.addEventListener('click', () => {
            uiService.openCreateComplaintForm();
        });
    }
    
    // Handle clicks outside modals to close them
    document.querySelectorAll('.modal').forEach(modal => {
        modal.addEventListener('click', (e) => {
            if (e.target === modal) {
                modal.classList.remove('show');
            }
        });
    });
    
    // Modal switching links
    const switchToRegisterLink = document.getElementById('switch-to-register');
    const switchToLoginLink = document.getElementById('switch-to-login');
    
    if (switchToRegisterLink) {
        switchToRegisterLink.addEventListener('click', (e) => {
            e.preventDefault();
            document.getElementById('login-modal').classList.remove('show');
            document.getElementById('register-modal').classList.add('show');
        });
    }
    
    if (switchToLoginLink) {
        switchToLoginLink.addEventListener('click', (e) => {
            e.preventDefault();
            document.getElementById('register-modal').classList.remove('show');
            document.getElementById('login-modal').classList.add('show');
        });
    }
    
    // Anonymous warning buttons
    const loginToContinueBtn = document.getElementById('login-to-continue');
    const continueAnonymousBtn = document.getElementById('continue-anonymous');
    
    if (loginToContinueBtn) {
        loginToContinueBtn.addEventListener('click', () => {
            document.getElementById('create-complaint-modal').classList.remove('show');
            document.getElementById('login-modal').classList.add('show');
        });
    }
    
    if (continueAnonymousBtn) {
        continueAnonymousBtn.addEventListener('click', () => {
            document.getElementById('anonymous-warning').style.display = 'none';
        });
    }
}

/**
 * Set up main button functionality
 */
function setupButtons() {
    const viewComplaintsBtn = document.getElementById('view-complaints-btn');
    
    if (viewComplaintsBtn) {
        viewComplaintsBtn.addEventListener('click', () => {
            const complaintsSection = document.querySelector('.complaints-section');
            if (complaintsSection) {
                window.scrollTo({
                    top: complaintsSection.offsetTop - 100,
                    behavior: 'smooth'
                });
            }
        });
    }
}

/**
 * Set up filters for complaints
 */
function setupFilters() {
    const filterType = document.getElementById('filter-type');
    const filterStatus = document.getElementById('filter-status');
    const filterOrder = document.getElementById('filter-order');
    const searchInput = document.getElementById('search-input');
    const searchBtn = document.getElementById('search-btn');
    
    // Apply filters on change
    if (filterType) {
        filterType.addEventListener('change', () => {
            uiService.applyFilters();
        });
    }
    
    if (filterStatus) {
        filterStatus.addEventListener('change', () => {
            uiService.applyFilters();
        });
    }
    
    if (filterOrder) {
        filterOrder.addEventListener('change', () => {
            uiService.applyFilters();
        });
    }
    
    // Handle search
    if (searchBtn) {
        searchBtn.addEventListener('click', () => {
            const searchTerm = searchInput.value.trim();
            if (searchTerm) {
                uiService.searchComplaints(searchTerm);
            } else {
                uiService.loadComplaints();
            }
        });
    }
    
    if (searchInput) {
        searchInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                e.preventDefault();
                const searchTerm = searchInput.value.trim();
                if (searchTerm) {
                    uiService.searchComplaints(searchTerm);
                } else {
                    uiService.loadComplaints();
                }
            }
        });
    }
}

/**
 * Set up form submissions
 */
function setupForms() {
    const loginForm = document.getElementById('login-form');
    const registerForm = document.getElementById('register-form');
    const createComplaintForm = document.getElementById('create-complaint-form');
    
    // Login form
    if (loginForm) {
        loginForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const email = document.getElementById('login-email').value.trim();
            const password = document.getElementById('login-password').value;
            
            try {
                const response = await authService.login(email, password);
                
                if (response.success) {
                    document.getElementById('login-modal').classList.remove('show');
                    window.location.reload();
                } else {
                    alert(response.message || 'Error al iniciar sesión. Verifica tus credenciales.');
                }
                
            } catch (error) {
                console.error('Login error:', error);
                alert('Error al iniciar sesión. Inténtalo de nuevo más tarde.');
            }
        });
    }
    
    // Register form
    if (registerForm) {
        registerForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const firstName = document.getElementById('register-firstname').value.trim();
            const lastName = document.getElementById('register-lastname').value.trim();
            const email = document.getElementById('register-email').value.trim();
            const password = document.getElementById('register-password').value;
            const confirmPassword = document.getElementById('register-confirm-password').value;
            
            if (password !== confirmPassword) {
                alert('Las contraseñas no coinciden.');
                return;
            }
            
            try {
                const response = await authService.register(firstName, lastName, email, password);
                
                alert('Registro exitoso. Ahora puedes iniciar sesión.');
                document.getElementById('register-modal').classList.remove('show');
                document.getElementById('login-modal').classList.add('show');
                
            } catch (error) {
                console.error('Registration error:', error);
                alert('Error al registrarse. Inténtalo de nuevo más tarde.');
            }
        });
    }
    
    // Create complaint form
    if (createComplaintForm) {
        createComplaintForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            uiService.handleCreateComplaint();
        });
    }
}