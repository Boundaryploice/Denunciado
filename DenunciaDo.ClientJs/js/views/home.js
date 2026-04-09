// views/home.js

/**
 * Vista de la página de inicio
 * @param {HTMLElement} container - Contenedor donde se renderizará la vista
 * @param {Object} params - Parámetros de la URL
 */
const homeView = async (container, params) => {
    // Estructura de la página
    container.innerHTML = `
        <!-- Hero Section -->
        <section class="hero">
            <div class="container">
                <div class="hero-content">
                    <h2>Mejora tu comunidad con Denuncia.Do</h2>
                    <p>Plataforma de denuncias ciudadanas para una mejor comunicación entre municipios y comunidades.</p>
                    <div class="hero-actions">
                        <a href="/denuncias" class="btn btn-secondary nav-link" data-page="complaints">Ver denuncias</a>
                        <a href="/nueva-denuncia" class="btn btn-primary nav-link" data-page="create-complaint">Crear denuncia</a>
                    </div>
                </div>
            </div>
        </section>

        <!-- Statistics Section -->
        <section class="section">
            <div class="container">
                <div class="stats-grid">
                    <div class="stats-item" id="total-complaints">
                        <div class="stats-icon">
                            <i class="fas fa-exclamation-circle"></i>
                        </div>
                        <div class="stats-number">0</div>
                        <div class="stats-label">Denuncias totales</div>
                    </div>
                    <div class="stats-item" id="resolved-complaints">
                        <div class="stats-icon">
                            <i class="fas fa-check-circle"></i>
                        </div>
                        <div class="stats-number">0</div>
                        <div class="stats-label">Denuncias resueltas</div>
                    </div>
                    <div class="stats-item" id="active-users">
                        <div class="stats-icon">
                            <i class="fas fa-users"></i>
                        </div>
                        <div class="stats-number">0</div>
                        <div class="stats-label">Usuarios activos</div>
                    </div>
                    <div class="stats-item" id="municipalities">
                        <div class="stats-icon">
                            <i class="fas fa-city"></i>
                        </div>
                        <div class="stats-number">0</div>
                        <div class="stats-label">Municipios</div>
                    </div>
                </div>
            </div>
        </section>

        <!-- Features Section -->
        <section class="section bg-light">
            <div class="container">
                <div class="section-title">
                    <h2>¿Cómo funciona?</h2>
                </div>
                <div class="features">
                    <div class="feature">
                        <div class="feature-icon">
                            <i class="fas fa-edit"></i>
                        </div>
                        <h3>Crea una denuncia</h3>
                        <p>Registra tu denuncia con todos los detalles necesarios para que sea atendida correctamente.</p>
                    </div>
                    <div class="feature">
                        <div class="feature-icon">
                            <i class="fas fa-share-alt"></i>
                        </div>
                        <h3>Comparte y apoya</h3>
                        <p>Comparte denuncias existentes y apoya con tus votos para dar mayor visibilidad a los problemas.</p>
                    </div>
                    <div class="feature">
                        <div class="feature-icon">
                            <i class="fas fa-clipboard-check"></i>
                        </div>
                        <h3>Seguimiento</h3>
                        <p>Realiza un seguimiento de tus denuncias y recibe notificaciones sobre su estado.</p>
                    </div>
                </div>
            </div>
        </section>

        <!-- Recent Complaints Section -->
        <section class="section">
            <div class="container">
                <div class="section-title">
                    <h2>Denuncias recientes</h2>
                </div>
                <div class="complaints-grid" id="recent-complaints">
                    <div class="loading-container">
                        <div class="loader"></div>
                        <p class="loading-text">Cargando denuncias recientes...</p>
                    </div>
                </div>
                <div class="text-center mt-4">
                    <a href="/denuncias" class="btn btn-primary nav-link" data-page="complaints">Ver todas las denuncias</a>
                </div>
            </div>
        </section>

        <!-- Types Section -->
        <section class="section bg-light">
            <div class="container">
                <div class="section-title">
                    <h2>Tipos de denuncias</h2>
                </div>
                <div class="complaint-types-grid" id="complaint-types">
                    <!-- Los tipos de denuncias se cargarán dinámicamente -->
                </div>
            </div>
        </section>

        <!-- Call to Action -->
        <section class="section cta-section">
            <div class="container">
                <div class="cta-content">
                    <h2>¿Listo para mejorar tu comunidad?</h2>
                    <p>Regístrate ahora y comienza a crear denuncias para mejorar tu entorno.</p>
                    <div class="cta-buttons">
                        <a href="/register" class="btn btn-primary btn-lg nav-link" data-page="register">Registrarse</a>
                        <a href="/about" class="btn btn-outline btn-lg nav-link" data-page="about">Conocer más</a>
                    </div>
                </div>
            </div>
        </section>
    `;

    // Cargar datos
    try {
        // Cargar denuncias recientes
        loadRecentComplaints();
        
        // Cargar tipos de denuncias
        loadComplaintTypes();
        
        // Cargar estadísticas
        loadStatistics();
    } catch (error) {
        console.error('Error al cargar datos de inicio:', error);
        ui.showToast('Error al cargar los datos', 'error');
    }
};

/**
 * Carga las denuncias recientes
 */
const loadRecentComplaints = async () => {
    try {
        const recentComplaintsContainer = document.getElementById('recent-complaints');
        const complaints = await api.get('/complaints/recent?count=6');
        
        if (complaints.length === 0) {
            recentComplaintsContainer.innerHTML = `
                <div class="empty-state">
                    <div class="empty-state-icon">
                        <i class="fas fa-search"></i>
                    </div>
                    <p class="empty-state-text">No hay denuncias recientes</p>
                </div>
            `;
            return;
        }
        
        recentComplaintsContainer.innerHTML = '';
        
        // Renderizar cada denuncia
        complaints.forEach(complaint => {
            const card = createComplaintCard(complaint);
            recentComplaintsContainer.appendChild(card);
        });
    } catch (error) {
        console.error('Error al cargar denuncias recientes:', error);
        document.getElementById('recent-complaints').innerHTML = `
            <div class="empty-state">
                <div class="empty-state-icon">
                    <i class="fas fa-exclamation-circle"></i>
                </div>
                <p class="empty-state-text">Error al cargar denuncias recientes</p>
            </div>
        `;
    }
};

/**
 * Crea una tarjeta de denuncia
 * @param {Object} complaint - Datos de la denuncia
 * @returns {HTMLElement} - Elemento de tarjeta de denuncia
 */
const createComplaintCard = (complaint) => {
    const card = document.createElement('div');
    card.className = 'card complaint-card';
    
    // Obtener tipo de denuncia y estado
    const complaintType = config.complaintTypes[complaint.complaintTypeId] || { name: 'Otro', icon: 'fa-question-circle' };
    const statusColor = complaint.statusColor || config.complaintStatuses[complaint.statusId]?.color || '#6c757d';
    
    // Crear la estructura de la tarjeta
    card.innerHTML = `
        <div class="card-header complaint-card-header">
            ${complaint.image ? `<img src="${complaint.image}" alt="${complaint.title}" class="complaint-img">` : `<div class="complaint-no-img"><i class="fas ${complaintType.icon}"></i></div>`}
            <span class="complaint-type-badge" style="background-color: ${complaintType.color || '#6c757d'}">
                <i class="fas ${complaintType.icon}"></i> ${complaintType.name}
            </span>
        </div>
        <div class="card-body complaint-card-body">
            <span class="complaint-status" style="background-color: ${statusColor}">${complaint.statusName}</span>
            <h5 class="card-title">${complaint.title}</h5>
            <p class="card-text">${ui.truncateText(complaint.description, 100)}</p>
            <div class="complaint-meta">
                <div class="complaint-location">
                    <i class="fas fa-map-marker-alt"></i> ${complaint.districtName || 'No especificado'}
                </div>
                <div class="complaint-date">
                    <i class="fas fa-calendar-alt"></i> ${ui.formatDate(complaint.createdAt)}
                </div>
            </div>
        </div>
        <div class="card-footer complaint-footer">
            <div class="complaint-meta">
                <div class="complaint-votes">
                    <i class="fas fa-thumbs-up"></i> ${complaint.upvoteCount || 0}
                </div>
                <div class="complaint-comments">
                    <i class="fas fa-comment"></i> ${complaint.commentCount || 0}
                </div>
            </div>
            <div class="complaint-action">
                <a href="/denuncia/${complaint.id}" class="btn btn-sm btn-outline nav-link">Ver detalles</a>
            </div>
        </div>
    `;
    
    // Agregar evento para navegar a la página de detalle
    card.addEventListener('click', (e) => {
        if (!e.target.classList.contains('btn')) {
            router.navigateTo(`/denuncia/${complaint.id}`);
        }
    });
    
    return card;
};

/**
 * Carga los tipos de denuncias
 */
const loadComplaintTypes = async () => {
    try {
        const typesContainer = document.getElementById('complaint-types');
        const types = await api.get('/complainttypes');
        
        typesContainer.innerHTML = '';
        
        // Renderizar cada tipo de denuncia
        types.forEach(type => {
            const typeElement = document.createElement('div');
            typeElement.className = 'complaint-type-item';
            
            typeElement.innerHTML = `
                <div class="complaint-type-icon" style="background-color: ${config.complaintTypes[type.id]?.color || '#6c757d'}">
                    <i class="fas ${type.icon || config.complaintTypes[type.id]?.icon || 'fa-question-circle'}"></i>
                </div>
                <h4>${type.name}</h4>
                <p>${type.description || 'Sin descripción'}</p>
                <a href="/denuncias?type=${type.id}" class="btn btn-sm btn-outline">Ver denuncias</a>
            `;
            
            // Agregar evento para navegar a la página de denuncias filtradas
            typeElement.querySelector('.btn').addEventListener('click', (e) => {
                e.preventDefault();
                router.navigateTo('/denuncias', { type: type.id }, true);
            });
            
            typesContainer.appendChild(typeElement);
        });
    } catch (error) {
        console.error('Error al cargar tipos de denuncias:', error);
        document.getElementById('complaint-types').innerHTML = `
            <div class="empty-state">
                <div class="empty-state-icon">
                    <i class="fas fa-exclamation-circle"></i>
                </div>
                <p class="empty-state-text">Error al cargar tipos de denuncias</p>
            </div>
        `;
    }
};

/**
 * Carga las estadísticas
 */
const loadStatistics = async () => {
    try {
        // En una aplicación real, estos datos vendrían de endpoints específicos
        // Para esta demo, usaremos datos estáticos
        document.getElementById('total-complaints').querySelector('.stats-number').textContent = '1,234';
        document.getElementById('resolved-complaints').querySelector('.stats-number').textContent = '856';
        document.getElementById('active-users').querySelector('.stats-number').textContent = '5,678';
        document.getElementById('municipalities').querySelector('.stats-number').textContent = '32';
    } catch (error) {
        console.error('Error al cargar estadísticas:', error);
    }
};