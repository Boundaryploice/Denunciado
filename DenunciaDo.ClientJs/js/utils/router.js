// utils/router.js

/**
 * Módulo para manejar el enrutamiento de la aplicación
 */
const router = (() => {
    // Rutas disponibles
    const routes = {
        '/': {
            view: homeView,
            title: 'Inicio',
            public: true
        },
        '/denuncias': {
            view: complaintsView,
            title: 'Denuncias',
            public: true
        },
        '/denuncia': {
            view: complaintDetailView,
            title: 'Detalle de Denuncia',
            public: true,
            params: ['id']
        },
        '/nueva-denuncia': {
            view: createComplaintView,
            title: 'Nueva Denuncia',
            public: false
        },
        '/mis-denuncias': {
            view: myComplaintsView,
            title: 'Mis Denuncias',
            public: false
        },
        '/profile': {
            view: profileView,
            title: 'Mi Perfil',
            public: false
        },
        '/notifications': {
            view: notificationsView,
            title: 'Notificaciones',
            public: false
        },
        '/login': {
            view: loginView,
            title: 'Iniciar Sesión',
            public: true
        },
        '/register': {
            view: registerView,
            title: 'Registrarse',
            public: true
        },
        '/about': {
            view: aboutView,
            title: 'Sobre Nosotros',
            public: true
        }
    };

    /**
     * Parsea la URL actual para obtener la ruta y los parámetros
     * @returns {Object} - Objeto con la ruta y los parámetros
     */
    const parseUrl = () => {
        const url = window.location.pathname;
        const queryString = window.location.search;
        const pathSegments = url.split('/').filter(segment => segment !== '');
        
        let path = '/';
        let params = {};
        
        if (pathSegments.length > 0) {
            path = `/${pathSegments[0]}`;
            
            // Si la ruta tiene parámetros definidos
            const route = routes[path];
            if (route && route.params && pathSegments.length > 1) {
                route.params.forEach((param, index) => {
                    if (pathSegments[index + 1]) {
                        params[param] = pathSegments[index + 1];
                    }
                });
            }
        }
        
        // Parsea los parámetros de query string
        if (queryString) {
            const searchParams = new URLSearchParams(queryString);
            for (const [key, value] of searchParams.entries()) {
                params[key] = value;
            }
        }
        
        return { path, params };
    };

    /**
     * Navega a una ruta
     * @param {string} path - Ruta a la que navegar
     * @param {Object} [params] - Parámetros para la ruta
     * @param {boolean} [useQueryString] - Si es true, usa query string en lugar de parámetros en la URL
     */
    const navigateTo = (path, params = {}, useQueryString = false) => {
        let url = path;
        
        // Si la ruta tiene parámetros definidos
        const route = routes[path];
        if (route && route.params && Object.keys(params).length > 0 && !useQueryString) {
            route.params.forEach(param => {
                if (params[param]) {
                    url += `/${params[param]}`;
                }
            });
        }
        
        // Si se deben usar query strings
        if (useQueryString && Object.keys(params).length > 0) {
            const queryString = new URLSearchParams();
            
            for (const [key, value] of Object.entries(params)) {
                queryString.append(key, value);
            }
            
            url += `?${queryString.toString()}`;
        }
        
        history.pushState(null, null, url);
        router.route();
    };

    /**
     * Renderiza la vista correspondiente a la ruta actual
     */
    const route = async () => {
        const { path, params } = parseUrl();
        const route = routes[path] || routes['/'];
        
        // Verifica si la ruta es privada y el usuario no está autenticado
        if (!route.public && !auth.isAuthenticated()) {
            navigateTo('/login');
            return;
        }
        
        // Actualiza el título de la página
        document.title = `${route.title} | Denuncia.Do`;
        
        // Muestra un loader mientras se carga la vista
        const mainContent = document.getElementById('main-content');
        mainContent.innerHTML = '<div class="loading-container"><div class="loader"></div><p class="loading-text">Cargando...</p></div>';
        
        try {
            // Renderiza la vista
            await route.view(mainContent, params);
            
            // Actualiza el enlace activo en la navegación
            updateActiveLink(path);
            
            // Desplaza al inicio de la página
            window.scrollTo(0, 0);
        } catch (error) {
            console.error('Error al cargar la vista:', error);
            mainContent.innerHTML = '<div class="empty-state"><div class="empty-state-icon"><i class="fas fa-exclamation-triangle"></i></div><h3>Ha ocurrido un error</h3><p class="empty-state-text">No se pudo cargar la página solicitada.</p><button class="btn btn-primary" onclick="router.navigateTo(\'/\')">Volver al inicio</button></div>';
        }
    };

    /**
     * Actualiza el enlace activo en la navegación
     * @param {string} path - Ruta actual
     */
    const updateActiveLink = (path) => {
        // Elimina la clase active de todos los enlaces
        document.querySelectorAll('.nav-link').forEach(link => {
            link.classList.remove('active');
        });
        
        // Agrega la clase active al enlace correspondiente a la ruta actual
        document.querySelectorAll(`.nav-link[data-page="${path.substring(1) || 'home'}"]`).forEach(link => {
            link.classList.add('active');
        });
    };

    /**
     * Inicializa el router
     */
    const init = () => {
        // Configura los event listeners para la navegación
        document.body.addEventListener('click', (e) => {
            if (e.target.matches('.nav-link') || e.target.closest('.nav-link')) {
                const link = e.target.matches('.nav-link') ? e.target : e.target.closest('.nav-link');
                
                // Evita la navegación predeterminada
                e.preventDefault();
                
                // Obtiene la ruta del enlace
                const href = link.getAttribute('href');
                
                // Navega a la ruta
                navigateTo(href);
            }
        });
        
        // Configura el evento popstate para manejar la navegación del navegador
        window.addEventListener('popstate', () => {
            route();
        });
        
        // Navega a la ruta inicial
        route();
    };

    return {
        init,
        navigateTo,
        route
    };
})();