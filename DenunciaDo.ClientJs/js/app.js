// app.js

/**
 * Punto de entrada principal de la aplicación
 */
document.addEventListener('DOMContentLoaded', async () => {
    // Inicializar módulos
    await auth.init();
    ui.init();
    router.init();

    // Manejar clics fuera del menú móvil para cerrarlo
    document.addEventListener('click', (e) => {
        const mainNav = document.querySelector('.main-nav');
        const mobileMenuToggle = document.querySelector('.mobile-menu-toggle');
        
        if (mainNav && mainNav.classList.contains('show') && 
            !e.target.closest('.main-nav') && 
            !e.target.closest('.mobile-menu-toggle')) {
            mainNav.classList.remove('show');
        }
    });

    // Configurar modal de login
    document.addEventListener('click', (e) => {
        if (e.target.matches('[data-page="login"]') || e.target.closest('[data-page="login"]')) {
            e.preventDefault();
            
            if (!auth.isAuthenticated()) {
                ui.showModal('login-modal');
            } else {
                ui.showToast('Ya has iniciado sesión', 'info');
            }
        }
    });

    // Configurar formulario de login
    const loginForm = document.getElementById('login-form');
    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault();

        if (!ui.validateForm(loginForm)) {
            return;
        }

        const email = document.getElementById('login-email').value;
        const password = document.getElementById('login-password').value;

        try {
            await auth.login(email, password);
            ui.hideModal('login-modal');
            ui.showToast('Has iniciado sesión correctamente', 'success');

            // Redirigir a la página de inicio
            router.navigateTo('/');
        } catch (error) {
            console.error('Error al iniciar sesión:', error);
            ui.showToast('Credenciales incorrectas', 'error');
        }
    });

    // Actualizar interfaz según el estado de autenticación
    if (auth.isAuthenticated()) {
        // Actualizar UI para usuario autenticado
        const user = auth.getUser();
        console.log('Usuario autenticado:', user);
    }
});