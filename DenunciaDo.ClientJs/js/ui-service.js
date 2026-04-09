/**
 * UI Service for the Denuncia.Do application
 * Handles UI-related functionality and rendering
 */
class UIService {
    constructor() {
        this.complaintTypes = [];
        this.statuses = [];
        this.initialize();
    }

    /**
     * Initialize the UI service
     */
    async initialize() {
        try {
            // Load complaint types and statuses
            this.complaintTypes = await apiService.getComplaintTypes();
            this.statuses = await apiService.getStatuses();
            
            // Populate filter dropdowns
            this.populateFilterTypes();
            this.populateFilterStatuses();
            this.populateComplaintTypeSelect();
            
            // Initialize auth state UI
            this.updateAuthUI(authService.isUserAuthenticated(), authService.getCurrentUser());
            
            // Add auth state listener
            authService.addAuthStateListener((isAuthenticated, user) => {
                this.updateAuthUI(isAuthenticated, user);
            });
        } catch (error) {
            console.error('Error initializing UI service:', error);
        }
    }

    /**
     * Update the UI based on authentication state
     * @param {boolean} isAuthenticated - Whether user is authenticated
     * @param {Object|null} user - Current user object or null
     */
    updateAuthUI(isAuthenticated, user) {
        const navbarAuth = document.getElementById('navbar-auth');
        const navbarUser = document.getElementById('navbar-user');
        const userName = document.getElementById('user-name');
        const anonymousWarning = document.getElementById('anonymous-warning');
        
        if (isAuthenticated && user) {
            navbarAuth.classList.add('hidden');
            navbarUser.classList.remove('hidden');
            userName.textContent = `${user.firstName} ${user.lastName}`;
            
            if (anonymousWarning) {
                anonymousWarning.style.display = 'none';
            }
        } else {
            navbarAuth.classList.remove('hidden');
            navbarUser.classList.add('hidden');
            
            if (anonymousWarning) {
                anonymousWarning.style.display = 'block';
            }
        }
    }

    /**
     * Populate the complaint type filter dropdown
     */
    populateFilterTypes() {
        const filterType = document.getElementById('filter-type');
        if (!filterType) return;
        
        filterType.innerHTML = '<option value="">Todos</option>';
        
        this.complaintTypes.forEach(type => {
            const option = document.createElement('option');
            option.value = type.complaintTypeId;
            option.textContent = type.name;
            filterType.appendChild(option);
        });
    }

    /**
     * Populate the status filter dropdown
     */
    populateFilterStatuses() {
        const filterStatus = document.getElementById('filter-status');
        if (!filterStatus) return;
        
        filterStatus.innerHTML = '<option value="">Todos</option>';
        
        this.statuses.forEach(status => {
            const option = document.createElement('option');
            option.value = status.statusId;
            option.textContent = status.name;
            filterStatus.appendChild(option);
        });
    }

    /**
     * Populate the complaint type select in the creation form
     */
    populateComplaintTypeSelect() {
        const complaintType = document.getElementById('complaint-type');
        if (!complaintType) return;
        
        complaintType.innerHTML = '';
        
        this.complaintTypes.forEach(type => {
            const option = document.createElement('option');
            option.value = type.complaintTypeId;
            option.textContent = type.name;
            complaintType.appendChild(option);
        });
    }

    /**
     * Render complaints in the container
     * @param {Array} complaints - Array of complaint objects
     */
    renderComplaints(complaints) {
        const container = document.getElementById('complaints-container');
        if (!container) return;
        
        container.innerHTML = '';
        
        if (complaints.length === 0) {
            container.innerHTML = '<p class="text-center">No se encontraron denuncias.</p>';
            return;
        }
        
        complaints.forEach(complaint => {
            const statusClass = this.getStatusClass(complaint.statusName);
            const defaultImage = 'img/placeholder.jpg';
            const evidenceImage = complaint.evidences.length > 0 ? complaint.evidences[0].url : defaultImage;
            
            const card = document.createElement('div');
            card.className = 'complaint-card';
            card.dataset.id = complaint.complaintId;
            
            card.innerHTML = `
                <div class="complaint-image" style="background-image: url('${evidenceImage}')"></div>
                <div class="complaint-body">
                    <h3 class="complaint-title">${complaint.title}</h3>
                    <div class="complaint-meta">
                        <span>${complaint.userName || 'Anónimo'}</span>
                        <span>${this.formatDate(complaint.createdDate)}</span>
                    </div>
                    <span class="complaint-type">${complaint.complaintTypeName}</span>
                    <span class="complaint-status ${statusClass}">${complaint.statusName}</span>
                    <p class="complaint-description">${this.truncateText(complaint.description, 100)}</p>
                    <div class="complaint-footer">
                        <div class="complaint-stats">
                            <span class="stat"><i class="fas fa-thumbs-up"></i> ${complaint.upvoteCount}</span>
                            <span class="stat"><i class="fas fa-thumbs-down"></i> ${complaint.downvoteCount}</span>
                            <span class="stat"><i class="fas fa-comment"></i> ${complaint.commentCount}</span>
                        </div>
                        <div class="complaint-actions">
                            <button class="btn btn-sm btn-outline view-details" data-id="${complaint.complaintId}">
                                <i class="fas fa-eye"></i> Ver
                            </button>
                        </div>
                    </div>
                </div>
            `;
            
            container.appendChild(card);
            
            // Add event listener to view details button
            card.querySelector('.view-details').addEventListener('click', () => {
                this.openComplaintDetails(complaint.complaintId);
            });
        });
    }
    
    /**
     * Get the CSS class for a status
     * @param {string} statusName - Status name
     * @returns {string} - CSS class
     */
    getStatusClass(statusName) {
        const statusMap = {
            'Pending': 'pending',
            'In Review': 'in-review',
            'In Progress': 'in-progress',
            'Resolved': 'resolved',
            'Rejected': 'rejected',
            'Pendiente': 'pending',
            'En Revisión': 'in-review',
            'En Progreso': 'in-progress',
            'Resuelto': 'resolved',
            'Rechazado': 'rejected'
        };
        
        return statusMap[statusName] || '';
    }
    
    /**
     * Format a date string
     * @param {string} dateString - Date string
     * @returns {string} - Formatted date
     */
    formatDate(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString('es-DO', {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        });
    }
    
    /**
     * Truncate text to a specific length
     * @param {string} text - Text to truncate
     * @param {number} length - Maximum length
     * @returns {string} - Truncated text
     */
    truncateText(text, length) {
        if (text.length <= length) return text;
        return text.substring(0, length) + '...';
    }
    
    /**
     * Open complaint details modal
     * @param {number} complaintId - Complaint ID
     */
    async openComplaintDetails(complaintId) {
        try {
            const complaint = await apiService.getComplaintById(complaintId);
            const modal = document.getElementById('complaint-detail-modal');
            const content = document.getElementById('complaint-detail-content');
            
            const statusClass = this.getStatusClass(complaint.statusName);
            const defaultImage = 'img/placeholder.jpg';
            const evidenceImage = complaint.evidences.length > 0 ? complaint.evidences[0].url : defaultImage;
            
            content.innerHTML = `
                <div class="complaint-detail">
                    <div class="complaint-detail-header" style="background-image: url('${evidenceImage}')">
                        <div class="complaint-detail-header-overlay">
                            <h1>${complaint.title}</h1>
                            <div class="complaint-detail-meta">
                                <span><i class="fas fa-user"></i> ${complaint.userName || 'Anónimo'}</span>
                                <span><i class="fas fa-calendar"></i> ${this.formatDate(complaint.createdDate)}</span>
                                <span class="complaint-type">${complaint.complaintTypeName}</span>
                                <span class="complaint-status ${statusClass}">${complaint.statusName}</span>
                            </div>
                        </div>
                    </div>
                    <div class="complaint-detail-content">
                        <div class="complaint-detail-description">
                            <h3>Descripción</h3>
                            <p>${complaint.description}</p>
                        </div>
                        
                        <div class="complaint-detail-detail">
                            <h3>Detalle</h3>
                            <p>${complaint.detail || 'No hay detalles adicionales.'}</p>
                        </div>
                        
                        ${this.renderEvidencesSection(complaint.evidences)}
                        
                        <div class="complaint-detail-actions">
                            <button class="btn btn-primary vote-up" data-id="${complaint.complaintId}">
                                <i class="fas fa-thumbs-up"></i> Apoyar (${complaint.upvoteCount})
                            </button>
                            <button class="btn btn-outline vote-down" data-id="${complaint.complaintId}">
                                <i class="fas fa-thumbs-down"></i> No Apoyar (${complaint.downvoteCount})
                            </button>
                            <button class="btn btn-danger report" data-id="${complaint.complaintId}">
                                <i class="fas fa-flag"></i> Reportar
                            </button>
                            ${this.renderEditDeleteButtons(complaint)}
                        </div>
                        
                        <div class="tabs">
                            <div class="tabs-nav">
                                <button class="tab-btn active" data-tab="comments">Comentarios (${complaint.commentCount})</button>
                            </div>
                            
                            <div class="tab-content active" id="comments">
                                <div class="comments-list" id="comments-list">
                                    <p>Cargando comentarios...</p>
                                </div>
                                
                                <div class="comment-form">
                                    <h3>Añadir comentario</h3>
                                    <form id="comment-form">
                                        <div class="form-group">
                                            <textarea id="comment-content" class="form-control" rows="3" required placeholder="Escribe tu comentario..."></textarea>
                                        </div>
                                        <button type="submit" class="btn btn-primary">Enviar comentario</button>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            `;
            
            // Load comments
            this.loadComments(complaintId);
            
            // Add event listeners
            const voteUpBtn = content.querySelector('.vote-up');
            const voteDownBtn = content.querySelector('.vote-down');
            const reportBtn = content.querySelector('.report');
            const commentForm = content.querySelector('#comment-form');
            const tabButtons = content.querySelectorAll('.tab-btn');
            
            voteUpBtn.addEventListener('click', () => this.handleVote(complaintId, true));
            voteDownBtn.addEventListener('click', () => this.handleVote(complaintId, false));
            reportBtn.addEventListener('click', () => this.showReportForm(complaintId));
            
            commentForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.handleAddComment(complaintId);
            });
            
            tabButtons.forEach(btn => {
                btn.addEventListener('click', () => {
                    const tabId = btn.dataset.tab;
                    this.switchTab(tabId);
                });
            });
            
            const editBtn = content.querySelector('.edit-complaint');
            const deleteBtn = content.querySelector('.delete-complaint');
            
            if (editBtn) {
                editBtn.addEventListener('click', () => this.openEditComplaintForm(complaint));
            }
            
            if (deleteBtn) {
                deleteBtn.addEventListener('click', () => this.confirmDeleteComplaint(complaintId));
            }
            
            // Show modal
            modal.classList.add('show');
            
        } catch (error) {
            console.error(`Error loading complaint details for ID ${complaintId}:`, error);
            alert('Error al cargar los detalles de la denuncia.');
        }
    }
    
    /**
     * Render the evidences section
     * @param {Array} evidences - Array of evidence objects
     * @returns {string} - HTML for evidences section
     */
    renderEvidencesSection(evidences) {
        if (!evidences || evidences.length === 0) {
            return '';
        }
        
        let html = `
            <div class="complaint-detail-evidences">
                <h3>Evidencias</h3>
                <div class="evidences-grid">
        `;
        
        evidences.forEach(evidence => {
            html += `<img src="${evidence.url}" class="evidence-img" alt="Evidencia">`;
        });
        
        html += `
                </div>
            </div>
        `;
        
        return html;
    }
    
    /**
     * Render edit and delete buttons if user is owner
     * @param {Object} complaint - Complaint object
     * @returns {string} - HTML for buttons
     */
    renderEditDeleteButtons(complaint) {
        const currentUser = authService.getCurrentUser();
        
        if (!currentUser || complaint.userId !== currentUser.userId) {
            return '';
        }
        
        // Check if complaint is in a status that allows editing
        const editableStatus = complaint.statusId !== 3; // Not "In Revision by Legislators"
        
        if (!editableStatus) {
            return '';
        }
        
        return `
            <button class="btn btn-outline edit-complaint" data-id="${complaint.complaintId}">
                <i class="fas fa-edit"></i> Editar
            </button>
            <button class="btn btn-danger delete-complaint" data-id="${complaint.complaintId}">
                <i class="fas fa-trash"></i> Eliminar
            </button>
        `;
    }
    
    /**
     * Load comments for a complaint
     * @param {number} complaintId - Complaint ID
     */
    async loadComments(complaintId) {
        try {
            const comments = await apiService.getCommentsByComplaint(complaintId);
            const commentsList = document.getElementById('comments-list');
            
            if (!commentsList) return;
            
            if (comments.length === 0) {
                commentsList.innerHTML = '<p>No hay comentarios todavía. Sé el primero en comentar.</p>';
                return;
            }
            
            commentsList.innerHTML = '';
            
            comments.forEach(comment => {
                const isOwner = authService.isOwner(comment);
                
                const commentEl = document.createElement('div');
                commentEl.className = 'comment';
                commentEl.innerHTML = `
                    <div class="comment-header">
                        <span class="comment-author">${comment.userName || 'Anónimo'}</span>
                        <span class="comment-date">${this.formatDate(comment.createdAt)}</span>
                    </div>
                    <p class="comment-content">${comment.content}</p>
                    ${isOwner ? `
                        <button class="btn btn-sm btn-danger delete-comment" data-id="${comment.commentId}">
                            <i class="fas fa-trash"></i> Eliminar
                        </button>
                    ` : ''}
                `;
                
                commentsList.appendChild(commentEl);
                
                // Add event listener for delete button
                const deleteBtn = commentEl.querySelector('.delete-comment');
                if (deleteBtn) {
                    deleteBtn.addEventListener('click', () => this.handleDeleteComment(comment.commentId, complaintId));
                }
            });
            
        } catch (error) {
            console.error(`Error loading comments for complaint ID ${complaintId}:`, error);
            document.getElementById('comments-list').innerHTML = '<p>Error al cargar los comentarios.</p>';
        }
    }
    
    /**
     * Handle adding a comment
     * @param {number} complaintId - Complaint ID
     */
    async handleAddComment(complaintId) {
        const commentContent = document.getElementById('comment-content').value.trim();
        
        if (!commentContent) {
            alert('Por favor escribe un comentario.');
            return;
        }
        
        try {
            const isAuthenticated = authService.isUserAuthenticated();
            const commentData = {
                content: commentContent,
                complaintId: complaintId
            };
            
            if (isAuthenticated) {
                const currentUser = authService.getCurrentUser();
                commentData.userId = currentUser.userId;
            } else {
                commentData.deviceIdentifier = authService.getDeviceIdentifier();
            }
            
            await apiService.createComment(commentData);
            
            // Refresh comments
            this.loadComments(complaintId);
            
            // Clear form
            document.getElementById('comment-content').value = '';
            
        } catch (error) {
            console.error('Error adding comment:', error);
            alert('Error al añadir el comentario.');
        }
    }
    
    /**
     * Handle deleting a comment
     * @param {number} commentId - Comment ID
     * @param {number} complaintId - Complaint ID
     */
    async handleDeleteComment(commentId, complaintId) {
        if (!confirm('¿Estás seguro de que deseas eliminar este comentario?')) {
            return;
        }
        
        try {
            await apiService.deleteComment(commentId);
            
            // Refresh comments
            this.loadComments(complaintId);
            
        } catch (error) {
            console.error(`Error deleting comment ID ${commentId}:`, error);
            alert('Error al eliminar el comentario.');
        }
    }
    
    /**
     * Handle voting on a complaint
     * @param {number} complaintId - Complaint ID
     * @param {boolean} isUpvote - Whether it's an upvote
     */
    async handleVote(complaintId, isUpvote) {
        try {
            const isAuthenticated = authService.isUserAuthenticated();
            const voteData = {
                complaintId: complaintId,
                isUpvote: isUpvote
            };
            
            if (isAuthenticated) {
                const currentUser = authService.getCurrentUser();
                voteData.userId = currentUser.userId;
            } else {
                voteData.deviceIdentifier = authService.getDeviceIdentifier();
            }
            
            await apiService.createVote(voteData);
            
            // Refresh complaint details
            this.openComplaintDetails(complaintId);
            
        } catch (error) {
            console.error('Error voting:', error);
            alert('Error al votar. Es posible que ya hayas votado en esta denuncia.');
        }
    }
    
    /**
     * Show report form modal
     * @param {number} complaintId - Complaint ID
     */
    showReportForm(complaintId) {
        const modal = document.createElement('div');
        modal.className = 'modal show';
        
        modal.innerHTML = `
            <div class="modal-content">
                <span class="close-modal">&times;</span>
                <h2>Reportar Denuncia</h2>
                <form id="report-form">
                    <div class="form-group">
                        <label for="report-reason">Razón del reporte</label>
                        <textarea id="report-reason" class="form-control" rows="3" required placeholder="Explica por qué estás reportando esta denuncia..."></textarea>
                    </div>
                    <div class="form-group">
                        <button type="submit" class="btn btn-danger btn-block">Enviar Reporte</button>
                    </div>
                </form>
            </div>
        `;
        
        document.body.appendChild(modal);
        
        const closeBtn = modal.querySelector('.close-modal');
        const reportForm = modal.querySelector('#report-form');
        
        closeBtn.addEventListener('click', () => {
            document.body.removeChild(modal);
        });
        
        reportForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const reason = document.getElementById('report-reason').value.trim();
            
            if (!reason) {
                alert('Por favor explica la razón del reporte.');
                return;
            }
            
            try {
                const isAuthenticated = authService.isUserAuthenticated();
                const reportData = {
                    reason: reason,
                    complaintId: complaintId
                };
                
                if (isAuthenticated) {
                    const currentUser = authService.getCurrentUser();
                    reportData.userId = currentUser.userId;
                } else {
                    reportData.deviceIdentifier = authService.getDeviceIdentifier();
                }
                
                await apiService.createReport(reportData);
                
                alert('Gracias por tu reporte. Será revisado por nuestro equipo.');
                document.body.removeChild(modal);
                
            } catch (error) {
                console.error('Error reporting complaint:', error);
                alert('Error al enviar el reporte.');
            }
        });
    }
    
    /**
     * Switch between tabs
     * @param {string} tabId - Tab ID to switch to
     */
    switchTab(tabId) {
        const tabButtons = document.querySelectorAll('.tab-btn');
        const tabContents = document.querySelectorAll('.tab-content');
        
        tabButtons.forEach(btn => {
            if (btn.dataset.tab === tabId) {
                btn.classList.add('active');
            } else {
                btn.classList.remove('active');
            }
        });
        
        tabContents.forEach(content => {
            if (content.id === tabId) {
                content.classList.add('active');
            } else {
                content.classList.remove('active');
            }
        });
    }
    
    /**
     * Open complaint creation form
     */
    openCreateComplaintForm() {
        const createComplaintModal = document.getElementById('create-complaint-modal');
        const evidencePreview = document.getElementById('evidence-preview');
        
        // Reset form
        document.getElementById('create-complaint-form').reset();
        evidencePreview.innerHTML = '';
        
        // Check auth status for anonymous warning
        const isAuthenticated = authService.isUserAuthenticated();
        const anonymousWarning = document.getElementById('anonymous-warning');
        
        if (isAuthenticated) {
            anonymousWarning.style.display = 'none';
        } else {
            anonymousWarning.style.display = 'block';
        }
        
        // Show modal
        createComplaintModal.classList.add('show');
        
        // Handle file input preview
        const evidenceInput = document.getElementById('complaint-evidence');
        evidenceInput.addEventListener('change', () => {
            this.handleEvidencePreview(evidenceInput, evidencePreview);
        });
    }
    
    /**
     * Handle preview of uploaded evidence
     * @param {HTMLInputElement} input - File input element
     * @param {HTMLElement} previewContainer - Preview container element
     */
    handleEvidencePreview(input, previewContainer) {
        if (input.files && input.files[0]) {
            const reader = new FileReader();
            
            reader.onload = function(e) {
                previewContainer.innerHTML = '';
                
                const previewItem = document.createElement('div');
                previewItem.className = 'evidence-item';
                previewItem.style.backgroundImage = `url('${e.target.result}')`;
                
                const removeBtn = document.createElement('button');
                removeBtn.className = 'remove-evidence';
                removeBtn.innerHTML = '<i class="fas fa-times"></i>';
                
                removeBtn.addEventListener('click', () => {
                    input.value = '';
                    previewContainer.innerHTML = '';
                });
                
                previewItem.appendChild(removeBtn);
                previewContainer.appendChild(previewItem);
            };
            
            reader.readAsDataURL(input.files[0]);
        }
    }
    
    /**
     * Handle complaint form submission
     */
    async handleCreateComplaint() {
        const form = document.getElementById('create-complaint-form');
        const title = document.getElementById('complaint-title').value.trim();
        const complaintTypeId = document.getElementById('complaint-type').value;
        const description = document.getElementById('complaint-description').value.trim();
        const detail = document.getElementById('complaint-detail').value.trim();
        const evidenceInput = document.getElementById('complaint-evidence');
        
        if (!title || !complaintTypeId || !description) {
            alert('Por favor completa todos los campos requeridos.');
            return;
        }
        
        try {
            // Create complaint data
            const isAuthenticated = authService.isUserAuthenticated();
            const complaintData = {
                title: title,
                description: description,
                detail: detail,
                complaintTypeId: parseInt(complaintTypeId)
            };
            
            if (isAuthenticated) {
                const currentUser = authService.getCurrentUser();
                complaintData.userId = currentUser.userId;
            } else {
                complaintData.deviceIdentifier = authService.getDeviceIdentifier();
            }
            
            // Create complaint
            const createdComplaint = await apiService.createComplaint(complaintData);
            
            // Upload evidence if provided
            if (evidenceInput.files && evidenceInput.files[0]) {
                await apiService.uploadEvidence(evidenceInput.files[0], createdComplaint.complaintId);
            }
            
            // Close modal and refresh complaints
            document.getElementById('create-complaint-modal').classList.remove('show');
            form.reset();
            
            // Load all complaints
            this.loadComplaints();
            
            alert('¡Denuncia creada exitosamente!');
            
        } catch (error) {
            console.error('Error creating complaint:', error);
            alert('Error al crear la denuncia.');
        }
    }
    
    /**
     * Open edit complaint form
     * @param {Object} complaint - Complaint object to edit
     */
    openEditComplaintForm(complaint) {
        const modal = document.createElement('div');
        modal.className = 'modal show';
        
        modal.innerHTML = `
            <div class="modal-content">
                <span class="close-modal">&times;</span>
                <h2>Editar Denuncia</h2>
                <form id="edit-complaint-form">
                    <div class="form-group">
                        <label for="edit-complaint-title">Título</label>
                        <input type="text" id="edit-complaint-title" class="form-control" required value="${complaint.title}">
                    </div>
                    <div class="form-group">
                        <label for="edit-complaint-type">Tipo de Denuncia</label>
                        <select id="edit-complaint-type" class="form-control" required>
                            ${this.renderComplaintTypeOptions(complaint.complaintTypeId)}
                        </select>
                    </div>
                    <div class="form-group">
                        <label for="edit-complaint-status">Estado</label>
                        <select id="edit-complaint-status" class="form-control" required>
                            ${this.renderStatusOptions(complaint.statusId)}
                        </select>
                    </div>
                    <div class="form-group">
                        <label for="edit-complaint-description">Descripción Breve</label>
                        <textarea id="edit-complaint-description" class="form-control" rows="3" required>${complaint.description}</textarea>
                    </div>
                    <div class="form-group">
                        <label for="edit-complaint-detail">Detalle</label>
                        <textarea id="edit-complaint-detail" class="form-control" rows="5" required>${complaint.detail || ''}</textarea>
                    </div>
                    <div class="form-group">
                        <button type="submit" class="btn btn-primary btn-block">Guardar Cambios</button>
                    </div>
                </form>
            </div>
        `;
        
        document.body.appendChild(modal);
        
        const closeBtn = modal.querySelector('.close-modal');
        const editForm = modal.querySelector('#edit-complaint-form');
        
        closeBtn.addEventListener('click', () => {
            document.body.removeChild(modal);
        });
        
        editForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const complaintData = {
                complaintId: complaint.complaintId,
                title: document.getElementById('edit-complaint-title').value.trim(),
                description: document.getElementById('edit-complaint-description').value.trim(),
                detail: document.getElementById('edit-complaint-detail').value.trim(),
                statusId: parseInt(document.getElementById('edit-complaint-status').value),
                complaintTypeId: parseInt(document.getElementById('edit-complaint-type').value)
            };
            
            try {
                await apiService.updateComplaint(complaint.complaintId, complaintData);
                
                alert('Denuncia actualizada exitosamente.');
                document.body.removeChild(modal);
                
                // Refresh complaint details
                this.openComplaintDetails(complaint.complaintId);
                // Also refresh complaints list if on main page
                this.loadComplaints();
                
            } catch (error) {
                console.error('Error updating complaint:', error);
                alert('Error al actualizar la denuncia.');
            }
        });
    }
    
    /**
     * Render complaint type options for select
     * @param {number} selectedId - Selected complaint type ID
     * @returns {string} - HTML for options
     */
    renderComplaintTypeOptions(selectedId) {
        let html = '';
        
        this.complaintTypes.forEach(type => {
            const selected = type.complaintTypeId === selectedId ? 'selected' : '';
            html += `<option value="${type.complaintTypeId}" ${selected}>${type.name}</option>`;
        });
        
        return html;
    }
    
    /**
     * Render status options for select
     * @param {number} selectedId - Selected status ID
     * @returns {string} - HTML for options
     */
    renderStatusOptions(selectedId) {
        let html = '';
        
        this.statuses.forEach(status => {
            const selected = status.statusId === selectedId ? 'selected' : '';
            html += `<option value="${status.statusId}" ${selected}>${status.name}</option>`;
        });
        
        return html;
    }
    
    /**
     * Confirm and handle complaint deletion
     * @param {number} complaintId - Complaint ID to delete
     */
    async confirmDeleteComplaint(complaintId) {
        if (!confirm('¿Estás seguro de que deseas eliminar esta denuncia? Esta acción no se puede deshacer.')) {
            return;
        }
        
        try {
            await apiService.deleteComplaint(complaintId);
            
            alert('Denuncia eliminada exitosamente.');
            
            // Close detail modal
            document.getElementById('complaint-detail-modal').classList.remove('show');
            
            // Refresh complaints list
            this.loadComplaints();
            
        } catch (error) {
            console.error(`Error deleting complaint ID ${complaintId}:`, error);
            alert('Error al eliminar la denuncia.');
        }
    }
    
    /**
     * Load all complaints
     */
    async loadComplaints() {
        try {
            const complaints = await apiService.getComplaints();
            this.renderComplaints(complaints);
        } catch (error) {
            console.error('Error loading complaints:', error);
            document.getElementById('complaints-container').innerHTML = '<p class="text-center">Error al cargar las denuncias.</p>';
        }
    }
    
    /**
     * Apply filters to complaints
     */
    async applyFilters() {
        const typeId = document.getElementById('filter-type').value;
        const statusId = document.getElementById('filter-status').value;
        const orderBy = document.getElementById('filter-order').value;
        
        try {
            let complaints;
            
            if (typeId) {
                complaints = await apiService.getComplaintsByType(typeId);
            } else if (statusId) {
                complaints = await apiService.getComplaintsByStatus(statusId);
            } else {
                complaints = await apiService.getComplaints();
            }
            
            // Apply ordering
            switch (orderBy) {
                case 'date-asc':
                    complaints.sort((a, b) => new Date(a.createdDate) - new Date(b.createdDate));
                    break;
                case 'date-desc':
                    complaints.sort((a, b) => new Date(b.createdDate) - new Date(a.createdDate));
                    break;
                case 'upvotes':
                    complaints.sort((a, b) => b.upvoteCount - a.upvoteCount);
                    break;
                case 'comments':
                    complaints.sort((a, b) => b.commentCount - a.commentCount);
                    break;
            }
            
            this.renderComplaints(complaints);
            
        } catch (error) {
            console.error('Error applying filters:', error);
            document.getElementById('complaints-container').innerHTML = '<p class="text-center">Error al filtrar las denuncias.</p>';
        }
    }
    
    /**
     * Search complaints
     * @param {string} searchTerm - Search term
     */
    async searchComplaints(searchTerm) {
        try {
            const complaints = await apiService.searchComplaints(searchTerm);
            this.renderComplaints(complaints);
        } catch (error) {
            console.error('Error searching complaints:', error);
            document.getElementById('complaints-container').innerHTML = '<p class="text-center">Error en la búsqueda.</p>';
        }
    }
}

// Create a singleton instance
const uiService = new UIService();