// views/complaints.js

/**
 * Vista de la página de denuncias
 * @param {HTMLElement} container - Contenedor donde se renderizará la vista
 * @param {Object} params - Parámetros de la URL
 */
const complaintsView = async (container, params) => {
    // Parámetros de consulta
    const pageIndex = parseInt(params.page) || 1;
    const pageSize = parseInt(params.size) || config.paginationLimit;
    const typeId = parseInt(params.type) || null;
    const statusId = parseInt(params.status) || null;
    const districtId = parseInt(params.district) || null;
    const searchTerm = params.search || '';

    // Estructura de la página
    container.innerHTML = `
        <div class="container">
            <div class="section">
                <div class="section-title">
                    <h2>Denuncias Ciudadanas</h2>
                </div>

                <div class="row">
                    <div class="search-container">
                        <input type="text" id="search-input" class="search-input" placeholder="Buscar denuncias..." value="${searchTerm}">
                        <button id="search-button" class="search-icon"><i class="fas fa-search"></i></button>
                    </div>
                </div>

                <div class="filters">
                    <div class="filter-item">
                        <label for="type-filter">Tipo</label>
                        <select id="type-filter" class="form-control">
                            <option value="">Todos los tipos</option>
                            <!-- Los tipos se cargarán dinámicamente -->
                        </select>
                    </div>
                    <div class="filter-item">
                        <label for="status-filter">Estado</label>
                        <select id="status-filter" class="form-control">
                            <option value="">Todos los estados</option>
                            <!-- Los estados se cargarán dinámicamente -->
                        </select>
                    </div>
                    <div class="filter-item">
                        <label for="district-filter">Distrito</label>
                        <select id="district-filter" class="form-control">
                            <option value="">Todos los distritos</option>
                            <!-- Los distritos se cargarán dinámicamente -->
                        </select>
                    </div>
                    <div class="filter-item">
                        <label for="sort-by">Ordenar por</label>
                        <select id="sort-by" class="form-control">
                            <option value="date">Fecha (más reciente)</option>
                            <option value="votes">Más votados</option>
                            <option value="comments">Más comentados</option>
                        </select>
                    </div>
                </div>

                <div id="complaints-container">
                    <div class="loading-container">
                        <div class="loader"></div>
                        <p class="loading-text">Cargando denuncias...</p>
                    </div>
                </div>

                <div id="pagination-container" class="mt-4"></div>
            </div>
        </div>
    `;

    // Configurar el evento de búsqueda
    const searchInput = document.getElementById('search-input');
    const searchButton = document.getElementById('search-button');

    searchButton.addEventListener('click', () => {
        const searchValue = searchInput.value.trim();
        if (searchValue) {
            router.navigateTo('/denuncias', { search: searchValue, page: 1 }, true);
        } else if (params.search) {
            // Si ya había un término de búsqueda, lo eliminamos
            const newParams = { ...params };
            delete newParams.search;
            router.navigateTo('/denuncias', newParams, true);
        }
    });

    searchInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') {
            searchButton.click();
        }
    });

    // Cargar filtros y datos
    try {
        // Cargar tipos de denuncias
        loadComplaintTypesFilter(typeId);
        
        // Cargar estados
        loadStatusesFilter(statusId);
        
        // Cargar distritos
        loadDistrictsFilter(districtId);
        
        // Configurar ordenamiento
        configureSort(params.sort || 'date');
        
        // Cargar denuncias
        loadComplaints(params);
    } catch (error) {
        console.error('Error al cargar la página de denuncias:', error);
        ui.showToast('Error al cargar denuncias', 'error');
    }
};

/**
 * Carga los tipos de denuncias para el filtro
 * @param {number|null} selectedTypeId - ID del tipo seleccionado
 */
const loadComplaintTypesFilter = async (selectedTypeId) => {
    try {
        const typeFilter = document.getElementById('type-filter');
        const types = await api.get('/complainttypes');
        
        // Agregar opciones al select
        types.forEach(type => {
            const option = document.createElement('option');
            option.value = type.id;
            option.textContent = type.name;
            
            if (selectedTypeId && parseInt(type.id) === parseInt(selectedTypeId)) {
                option.selected = true;
            }
            
            typeFilter.appendChild(option);
        });
        
        // Agregar evento de cambio
        typeFilter.addEventListener('change', () => {
            const params = new URLSearchParams(window.location.search);
            
            if (typeFilter.value) {
                params.set('type', typeFilter.value);
            } else {
                params.delete('type');
            }
            
            params.set('page', '1'); // Reiniciar a la primera página
            
            router.navigateTo('/denuncias', Object.fromEntries(params), true);
        });
    } catch (error) {
        console.error('Error al cargar tipos de denuncias:', error);
    }
};

/**
 * Carga los estados para el filtro
 * @param {number|null} selectedStatusId - ID del estado seleccionado
 */
const loadStatusesFilter = async (selectedStatusId) => {
    try {
        const statusFilter = document.getElementById('status-filter');
        const statuses = await api.get('/statuses');
        
        // Agregar opciones al select
        statuses.forEach(status => {
            const option = document.createElement('option');
            option.value = status.id;
            option.textContent = status.name;
            
            if (selectedStatusId && parseInt(status.id) === parseInt(selectedStatusId)) {
                option.selected = true;
            }
            
            statusFilter.appendChild(option);
        });
        
        // Agregar evento de cambio
        statusFilter.addEventListener('change', () => {
            const params = new URLSearchParams(window.location.search);
            
            if (statusFilter.value) {
                params.set('status', statusFilter.value);
            } else {
                params.delete('status');
            }
            
            params.set('page', '1'); // Reiniciar a la primera página
            
            router.navigateTo('/denuncias', Object.fromEntries(params), true);
        });
    } catch (error) {
        console.error('Error al cargar estados:', error);
    }
};

/**
 * Carga los distritos para el filtro
 * @param {number|null} selectedDistrictId - ID del distrito seleccionado
 */
const loadDistrictsFilter = async (selectedDistrictId) => {
    try {
        const districtFilter = document.getElementById('district-filter');
        const municipalities = await api.get('/municipalities');
        
        // Para cada municipio, cargar sus distritos
        for (const municipality of municipalities) {
            const municipalityData = await api.get(`/municipalities/${municipality.id}/districts`);
            
            // Crear un optgroup para el municipio
            const optgroup = document.createElement('optgroup');
            optgroup.label = municipality.name;
            
            // Agregar los distritos al optgroup
            municipalityData.districts.forEach(district => {
                const option = document.createElement('option');
                option.value = district.id;
                option.textContent = district.name;
                
                if (selectedDistrictId && parseInt(district.id) === parseInt(selectedDistrictId)) {
                    option.selected = true;
                }
                
                optgroup.appendChild(option);
            });
            
            districtFilter.appendChild(optgroup);
        }
        
        // Agregar evento de cambio
        districtFilter.addEventListener('change', () => {
            const params = new URLSearchParams(window.location.search);
            
            if (districtFilter.value) {
                params.set('district', districtFilter.value);
            } else {
                params.delete('district');
            }
            
            params.set('page', '1'); // Reiniciar a la primera página
            
            router.navigateTo('/denuncias', Object.fromEntries(params), true);
        });
    } catch (error) {
        console.error('Error al cargar distritos:', error);
    }
};

/**
 * Configura el ordenamiento
 * @param {string} selectedSort - Criterio de ordenamiento seleccionado
 */
const configureSort = (selectedSort) => {
    const sortBy = document.getElementById('sort-by');
    
    // Seleccionar la opción actual
    Array.from(sortBy.options).forEach(option => {
        if (option.value === selectedSort) {
            option.selected = true;
        }
    });
    
    // Agregar evento de cambio
    sortBy.addEventListener('change', () => {
        const params = new URLSearchParams(window.location.search);
        
        params.set('sort', sortBy.value);
        params.set('page', '1'); // Reiniciar a la primera página
        
        router.navigateTo('/denuncias', Object.fromEntries(params), true);
    });
};

/**
 * Carga las denuncias según los filtros
 * @param {Object} params - Parámetros de la URL
 */
const loadComplaints = async (params) => {
    try {
        const complaintsContainer = document.getElementById('complaints-container');
        const paginationContainer = document.getElementById('pagination-container');
        
        // Construir la URL según los filtros
        let url = '/complaints';
        const urlParams = new URLSearchParams();
        
        // Agregar parámetros de paginación
        urlParams.append('pageIndex', params.page || '1');
        urlParams.append('pageSize', params.size || config.paginationLimit);
        
        // Filtros específicos
        if (params.type) {
            url = `/complaints/type/${params.type}`;
        } else if (params.status) {
            url = `/complaints/status/${params.status}`;
        } else if (params.district) {
            url = `/complaints/district/${params.district}`;
        } else if (params.search) {
            url = '/complaints/search';
            urlParams.append('term', params.search);
        }
        
        // Obtener las denuncias
        const result = await api.get(`${url}?${urlParams.toString()}`);
        
        // Mostrar mensaje si no hay denuncias
        if (result.items.length === 0) {
            complaintsContainer.innerHTML = `
                <div class="empty-state">
                    <div class="empty-state-icon">
                        <i class="fas fa-search"></i>
                    </div>
                    <p class="empty-state-text">No se encontraron denuncias con los filtros seleccionados</p>
                    <button class="btn btn-primary" id="clear-filters-btn">Limpiar filtros</button>
                </div>
            `;
            
            document.getElementById('clear-filters-btn').addEventListener('click', () => {
                router.navigateTo('/denuncias');
            });
            
            paginationContainer.innerHTML = '';
            return;
        }
        
        // Crear grid de denuncias
        complaintsContainer.innerHTML = '<div class="complaints-grid"></div>';
        const grid = complaintsContainer.querySelector('.complaints-grid');
        
        // Renderizar cada denuncia
        result.items.forEach(complaint => {
            const card = createComplaintCard(complaint);
            grid.appendChild(card);
        });
        
        // Crear paginación
        if (result.totalPages > 1) {
            paginationContainer.appendChild(
                ui.createPagination(result.pageIndex, result.totalPages, (page) => {
                    // Actualizar parámetro de página y navegar
                    const newParams = { ...params, page };
                    router.navigateTo('/denuncias', newParams, true);
                })
            );
        } else {
            paginationContainer.innerHTML = '';
        }
    } catch (error) {
        console.error('Error al cargar denuncias:', error);
        document.getElementById('complaints-container').innerHTML = `
            <div class="empty-state">
                <div class="empty-state-icon">
                    <i class="fas fa-exclamation-circle"></i>
                </div>
                <p class="empty-state-text">Error al cargar denuncias</p>
            </div>
        `;
        document.getElementById('pagination-container').innerHTML = '';
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
        if (!e.target.classList.contains('btn') && !e.target.closest('.btn')) {
            router.navigateTo(`/denuncia/${complaint.id}`);
        }
    });
    
    return card;
};