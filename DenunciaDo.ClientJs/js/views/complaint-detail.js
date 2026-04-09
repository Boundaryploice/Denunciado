// views/complaint-detail.js

/**
 * Vista de la página de detalle de denuncia
 * @param {HTMLElement} container - Contenedor donde se renderizará la vista
 * @param {Object} params - Parámetros de la URL
 */
const complaintDetailView = async (container, params) => {
    // Verificar si se proporcionó un ID
    if (!params.id) {
        container.innerHTML = `
            <div class="container">
                <div class="section">
                    <div class="empty-state">
                        <div class="empty-state-icon">
                            <i class="fas fa-exclamation-circle"></i>
                        </div>
                        <p class="empty-state-text">No se especificó una denuncia</p>
                        <a href="/denuncias" class="btn btn-primary nav-link">Ver todas las denuncias</a>
                    </div>
                </div>
            </div>
        `;
        return;
    }

    // Mostrar loader mientras se carga la denuncia
    container.innerHTML = `
        <div class="container">
            <div class="section">
                <div class="loading-container">
                    <div class="loader"></div>
                    <p class="loading-text">Cargando denuncia...</p>
                </div>
            </div>
        </div>
    `;

    try {
        // Obtener datos de la denuncia
        const complaint = await api.get(`/complaints/${params.id}`);
        
        // Renderizar la denuncia
        renderComplaintDetail(container, complaint);
        
        // Inicializar mapas y otros componentes interactivos
        initMap(complaint);
        
        // Inicializar comentarios
        initComments(complaint);
        
        // Inicializar votación
        initVoting(complaint);
    } catch (error) {
        console.error('Error al cargar denuncia:', error);
        container.innerHTML = `
            <div class="container">
                <div class="section">
                    <div class="empty-state">
                        <div class="empty-state-icon">
                            <i class="fas fa-exclamation-circle"></i>
                        </div>
                        <p class="empty-state-text">Error al cargar la denuncia</p>
                        <a href="/denuncias" class="btn btn-primary nav-link">Ver todas las denuncias</a>
                    </div>
                </div>
            </div>
        `;
    }
};

/**
 * Renderiza el detalle de una denuncia
 * @param {HTMLElement} container - Contenedor donde se renderizará la vista
 * @param {Object} complaint - Datos de la denuncia
 */
const renderComplaintDetail = (container, complaint) => {
    // Obtener tipo de denuncia y estado
    const complaintType = config.complaintTypes[complaint.complaintTypeId] || { name: 'Otro', icon: 'fa-question-circle' };
    const statusColor = complaint.statusColor || config.complaintStatuses[complaint.statusId]?.color || '#6c757d';
    
    // Estructura de la página
    container.innerHTML = `
        <div class="container">
            <div class="section complaint-detail">
                <div class="complaint-detail-header">
                    <div>
                        <a href="/denuncias" class="btn btn-sm btn-outline mb-3 nav-link">
                            <i class="fas fa-arrow-left"></i> Volver a denuncias
                        </a>
                        <h2>${complaint.title}</h2>
                        <div class="complaint-detail-meta">
                            <div class="complaint-detail-meta-item">
                                <i class="fas ${complaintType.icon}" style="color: ${complaintType.color || '#6c757d'}"></i> ${complaintType.name}
                            </div>
                            <div class="complaint-detail-meta-item">
                                <span class="complaint-status" style="background-color: ${statusColor}">${complaint.statusName}</span>
                            </div>
                            <div class="complaint-detail-meta-item">
                                <i class="fas fa-map-marker-alt"></i> ${complaint.districtName || 'No especificado'} ${complaint.municipalityName ? `- ${complaint.municipalityName}` : ''}
                            </div>
                            <div class="complaint-detail-meta-item">
                                <i class="fas fa-calendar-alt"></i> ${ui.formatDate(complaint.createdAt, true)}
                            </div>
                            <div class="complaint-detail-meta-item">
                                <i class="fas fa-user"></i> ${complaint.userName}
                            </div>
                        </div>
                    </div>
                    <div class="complaint-detail-actions">
                        <div class="complaint-vote-container">
                            <button id="upvote-btn" class="btn complaint-vote-button ${complaint.hasUserVoted && complaint.userVoteType ? 'btn-primary' : 'btn-outline'}" ${auth.isAuthenticated() ? '' : 'disabled'}>
                                <i class="fas fa-thumbs-up"></i> <span id="upvote-count">${complaint.upvoteCount || 0}</span>
                            </button>
                            <button id="downvote-btn" class="btn complaint-vote-button ${complaint.hasUserVoted && !complaint.userVoteType ? 'btn-secondary' : 'btn-outline'}" ${auth.isAuthenticated() ? '' : 'disabled'}>
                                <i class="fas fa-thumbs-down"></i> <span id="downvote-count">${complaint.downvoteCount || 0}</span>
                            </button>
                        </div>
                        ${auth.isAuthenticated() && (auth.getUser().id === complaint.userId || auth.hasRole('Admin') || auth.hasRole('Staff')) ? `
                            <div class="complaint-admin-actions">
                                ${complaint.statusId === 1 && auth.getUser().id === complaint.userId ? `
                                    <a href="#" id="edit-complaint-btn" class="btn btn-sm btn-outline">
                                        <i class="fas fa-edit"></i> Editar
                                    </a>
                                    <a href="#" id="delete-complaint-btn" class="btn btn-sm btn-danger">
                                        <i class="fas fa-trash"></i> Eliminar
                                    </a>
                                ` : ''}
                                ${(auth.hasRole('Admin') || auth.hasRole('Staff')) ? `
                                    <a href="#" id="update-status-btn" class="btn btn-sm btn-primary">
                                        <i class="fas fa-cog"></i> Cambiar estado
                                    </a>
                                ` : ''}
                            </div>
                        ` : ''}
                    </div>
                </div>

                ${complaint.image ? `
                    <div class="complaint-detail-image">
                        <img src="${complaint.image}" alt="${complaint.title}" class="complaint-detail-img">
                    </div>
                ` : ''}

                <div class="complaint-detail-content">
                    <div class="complaint-detail-section">
                        <h4>Descripción</h4>
                        <p>${complaint.description}</p>
                    </div>

                    <div class="complaint-detail-section">
                        <h4>Detalles</h4>
                        <p>${complaint.detail || 'No se proporcionaron detalles adicionales.'}</p>
                    </div>

                    ${complaint.address ? `
                        <div class="complaint-detail-section">
                            <h4>Dirección</h4>
                            <p>${complaint.address}</p>
                        </div>
                    ` : ''}

                    <div class="complaint-detail-section">
                        <h4>Ubicación</h4>
                        <div id="map" class="map-container"></div>
                    </div>

                    ${complaint.attachments && complaint.attachments.length > 0 ? `
                        <div class="complaint-detail-section">
                            <h4>Archivos adjuntos</h4>
                            <div class="attachments-list">
                                ${complaint.attachments.map(attachment => `
                                    <a href="${attachment.filePath}" target="_blank" class="attachment-item">
                                        <i class="fas ${getFileIcon(attachment.contentType)}"></i>
                                        <span>${attachment.fileName}</span>
                                    </a>
                                `).join('')}
                            </div>
                        </div>
                    ` : ''}

                    ${complaint.history && complaint.history.length > 0 ? `
                        <div class="complaint-detail-section">
                            <h4>Historial</h4>
                            <div class="timeline">
                                ${complaint.history.map(item => `
                                    <div class="timeline-item">
                                        <div class="timeline-badge" style="background-color: ${item.statusColor || config.complaintStatuses[item.statusId]?.color || '#6c757d'}">
                                            <i class="fas fa-check"></i>
                                        </div>
                                        <div class="timeline-content">
                                            <h5>${item.statusName}</h5>
                                            <p class="timeline-date">${ui.formatDate(item.createdAt, true)}</p>
                                            <p>${item.comments || 'Sin comentarios'}</p>
                                            <p class="timeline-user">Por: ${item.userName || 'Sistema'}</p>
                                        </div>
                                    </div>
                                `).join('')}
                            </div>
                        </div>
                    ` : ''}
                </div>

                <div class="complaint-comments">
                    <h4>Comentarios (${complaint.commentCount || 0})</h4>
                    
                    ${auth.isAuthenticated() ? `
                        <div class="comment-form">
                            <form id="comment-form">
                                <div class="form-group">
                                    <textarea id="comment-content" class="form-control" placeholder="Escribe un comentario..." required></textarea>
                                </div>
                                <div class="form-group text-right">
                                    <button type="submit" class="btn btn-primary">Comentar</button>
                                </div>
                            </form>
                        </div>
                    ` : `
                        <div class="alert alert-info">
                            <i class="fas fa-info-circle"></i> Debes <a href="/login" class="nav-link">iniciar sesión</a> para comentar.
                        </div>
                    `}
                    
                    <div id="comments-container">
                        ${complaint.comments && complaint.comments.length > 0 ? complaint.comments.map(comment => createCommentHtml(comment)).join('') : `
                            <div class="empty-state">
                                <div class="empty-state-icon">
                                    <i class="fas fa-comments"></i>
                                </div>
                                <p class="empty-state-text">No hay comentarios. Sé el primero en comentar.</p>
                            </div>
                        `}
                    </div>
                </div>
            </div>
        </div>
    `;
};

/**
 * Obtiene el icono según el tipo de archivo
 * @param {string} contentType - Tipo de contenido MIME
 * @returns {string} - Clase del icono
 */
const getFileIcon = (contentType) => {
    if (contentType.includes('image')) {
        return 'fa-file-image';
    } else if (contentType.includes('pdf')) {
        return 'fa-file-pdf';
    } else if (contentType.includes('word') || contentType.includes('document')) {
        return 'fa-file-word';
    } else if (contentType.includes('excel') || contentType.includes('sheet')) {
        return 'fa-file-excel';
    } else if (contentType.includes('powerpoint') || contentType.includes('presentation')) {
        return 'fa-file-powerpoint';
    } else if (contentType.includes('zip') || contentType.includes('rar') || contentType.includes('compressed')) {
        return 'fa-file-archive';
    } else if (contentType.includes('audio')) {
        return 'fa-file-audio';
    } else if (contentType.includes('video')) {
        return 'fa-file-video';
    } else if (contentType.includes('text')) {
        return 'fa-file-alt';
    } else {
        return 'fa-file';
    }
};

/**
 * Inicializa el mapa
 * @param {Object} complaint - Datos de la denuncia
 */
const initMap = (complaint) => {
    // En una aplicación real, aquí se inicializaría un mapa con la ubicación de la denuncia
    // Por simplicidad, solo mostraremos un mensaje
    
    const mapContainer = document.getElementById('map');
    
    if (!complaint.latitude || !complaint.longitude) {
        mapContainer.innerHTML = `
            <div class="empty-state">
                <div class="empty-state-icon">
                    <i class="fas fa-map-marker-alt"></i>
                </div>
                <p class="empty-state-text">No se especificó una ubicación para esta denuncia.</p>
            </div>
        `;
        return;
    }
    
    // Aquí se inicializaría un mapa real con las coordenadas
    mapContainer.innerHTML = `
        <div class="map-placeholder">
            <div class="map-placeholder-content">
                <i class="fas fa-map-marker-alt"></i>
                <p>Ubicación: ${complaint.latitude}, ${complaint.longitude}</p>
                <p class="map-placeholder-note">En una aplicación real, aquí se mostraría un mapa interactivo.</p>
            </div>
        </div>
    `;
};

/**
 * Inicializa la sección de comentarios
 * @param {Object} complaint - Datos de la denuncia
 */
const initComments = (complaint) => {
    // Si el usuario no está autenticado, no hay formulario
    if (!auth.isAuthenticated()) {
        return;
    }
    
    const commentForm = document.getElementById('comment-form');
    
    commentForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const commentContent = document.getElementById('comment-content').value.trim();
        
        if (!commentContent) {
            ui.showToast('El comentario no puede estar vacío', 'warning');
            return;
        }
        
        try {
            // Crear comentario
            const commentData = {
                content: commentContent,
                complaintId: complaint.id
            };
            
            const newComment = await api.post('/comments', commentData);
            
            // Actualizar la lista de comentarios
            const commentsContainer = document.getElementById('comments-container');
            
            // Si no había comentarios, limpiar el contenedor
            if (commentsContainer.querySelector('.empty-state')) {
                commentsContainer.innerHTML = '';
            }
            
            // Agregar el nuevo comentario
            const commentHtml = createCommentHtml(newComment);
            commentsContainer.insertAdjacentHTML('afterbegin', commentHtml);
            
            // Limpiar el formulario
            document.getElementById('comment-content').value = '';
            
            // Actualizar contador de comentarios
            const commentCount = parseInt(document.querySelector('.complaint-comments h4').textContent.match(/\d+/)[0]) + 1;
            document.querySelector('.complaint-comments h4').textContent = `Comentarios (${commentCount})`;
            
            ui.showToast('Comentario agregado correctamente', 'success');
        } catch (error) {
            console.error('Error al agregar comentario:', error);
            ui.showToast('Error al agregar comentario', 'error');
        }
    });
    
    // Configurar eventos para responder a comentarios
    const commentsContainer = document.getElementById('comments-container');
    
    commentsContainer.addEventListener('click', async (e) => {
        // Botón de responder
        if (e.target.matches('.reply-btn') || e.target.closest('.reply-btn')) {
            const replyBtn = e.target.matches('.reply-btn') ? e.target : e.target.closest('.reply-btn');
            const commentId = replyBtn.dataset.commentId;
            const commentElement = document.querySelector(`.comment[data-comment-id="${commentId}"]`);
            
            // Verificar si ya existe un formulario de respuesta
            if (commentElement.querySelector('.reply-form')) {
                return;
            }
            
            // Crear formulario de respuesta
            const replyForm = document.createElement('div');
            replyForm.className = 'reply-form';
            replyForm.innerHTML = `
                <form class="comment-reply-form" data-comment-id="${commentId}">
                    <div class="form-group">
                        <textarea class="form-control reply-content" placeholder="Escribe tu respuesta..." required></textarea>
                    </div>
                    <div class="form-group text-right">
                        <button type="button" class="btn btn-sm btn-outline cancel-reply-btn">Cancelar</button>
                        <button type="submit" class="btn btn-sm btn-primary">Responder</button>
                    </div>
                </form>
            `;
            
            // Agregar el formulario
            const commentReplies = commentElement.querySelector('.comment-replies') || document.createElement('div');
            
            if (!commentElement.querySelector('.comment-replies')) {
                commentReplies.className = 'comment-replies';
                commentElement.appendChild(commentReplies);
            }
            
            commentReplies.insertAdjacentElement('afterbegin', replyForm);
            
            // Enfocar el textarea
            replyForm.querySelector('textarea').focus();
            
            // Configurar evento para cancelar la respuesta
            replyForm.querySelector('.cancel-reply-btn').addEventListener('click', () => {
                replyForm.remove();
            });
            
            // Configurar evento para enviar la respuesta
            replyForm.querySelector('form').addEventListener('submit', async (e) => {
                e.preventDefault();
                
                const replyContent = replyForm.querySelector('textarea').value.trim();
                
                if (!replyContent) {
                    ui.showToast('La respuesta no puede estar vacía', 'warning');
                    return;
                }
                
                try {
                    // Crear respuesta
                    const replyData = {
                        content: replyContent,
                        complaintId: complaint.id,
                        parentCommentId: parseInt(commentId)
                    };
                    
                    const newReply = await api.post('/comments', replyData);
                    
                    // Eliminar el formulario
                    replyForm.remove();
                    
                    // Agregar la nueva respuesta
                    const replyHtml = createCommentHtml(newReply, true);
                    commentReplies.insertAdjacentHTML('beforeend', replyHtml);
                    
                    // Actualizar contador de comentarios
                    const commentCount = parseInt(document.querySelector('.complaint-comments h4').textContent.match(/\d+/)[0]) + 1;
                    document.querySelector('.complaint-comments h4').textContent = `Comentarios (${commentCount})`;
                    
                    ui.showToast('Respuesta agregada correctamente', 'success');
                } catch (error) {
                    console.error('Error al agregar respuesta:', error);
                    ui.showToast('Error al agregar respuesta', 'error');
                }
            });
        }
        
        // Botón de editar comentario
        if (e.target.matches('.edit-comment-btn') || e.target.closest('.edit-comment-btn')) {
            const editBtn = e.target.matches('.edit-comment-btn') ? e.target : e.target.closest('.edit-comment-btn');
            const commentId = editBtn.dataset.commentId;
            const commentElement = document.querySelector(`.comment[data-comment-id="${commentId}"]`);
            const commentText = commentElement.querySelector('.comment-text').textContent.trim();
            
            // Cambiar el texto por un textarea
            const commentContent = commentElement.querySelector('.comment-content');
            const originalHtml = commentContent.innerHTML;
            
            commentContent.innerHTML = `
                <form class="edit-comment-form" data-comment-id="${commentId}">
                    <div class="form-group">
                        <textarea class="form-control edit-content" required>${commentText}</textarea>
                    </div>
                    <div class="form-group text-right">
                        <button type="button" class="btn btn-sm btn-outline cancel-edit-btn">Cancelar</button>
                        <button type="submit" class="btn btn-sm btn-primary">Guardar</button>
                    </div>
                </form>
            `;
            
            // Enfocar el textarea
            commentContent.querySelector('textarea').focus();
            
            // Configurar evento para cancelar la edición
            commentContent.querySelector('.cancel-edit-btn').addEventListener('click', () => {
                commentContent.innerHTML = originalHtml;
            });
            
            // Configurar evento para guardar la edición
            commentContent.querySelector('form').addEventListener('submit', async (e) => {
                e.preventDefault();
                
                const editedContent = commentContent.querySelector('textarea').value.trim();
                
                if (!editedContent) {
                    ui.showToast('El comentario no puede estar vacío', 'warning');
                    return;
                }
                
                try {
                    // Actualizar comentario
                    const updatedComment = await api.put(`/comments/${commentId}`, editedContent);
                    
                    // Actualizar el comentario en la UI
                    commentElement.querySelector('.comment-text').textContent = editedContent;
                    
                    // Restaurar la vista original
                    commentContent.innerHTML = originalHtml;
                    
                    ui.showToast('Comentario actualizado correctamente', 'success');
                } catch (error) {
                    console.error('Error al actualizar comentario:', error);
                    ui.showToast('Error al actualizar comentario', 'error');
                    
                    // Restaurar la vista original en caso de error
                    commentContent.innerHTML = originalHtml;
                }
            });
        }
        
        // Botón de eliminar comentario
        if (e.target.matches('.delete-comment-btn') || e.target.closest('.delete-comment-btn')) {
            const deleteBtn = e.target.matches('.delete-comment-btn') ? e.target : e.target.closest('.delete-comment-btn');
            const commentId = deleteBtn.dataset.commentId;
            const commentElement = document.querySelector(`.comment[data-comment-id="${commentId}"]`);
            
            // Confirmar eliminación
            if (confirm('¿Estás seguro de que deseas eliminar este comentario?')) {
                try {
                    // Eliminar comentario
                    await api.delete(`/comments/${commentId}`);
                    
                    // Eliminar el comentario de la UI
                    commentElement.remove();
                    
                    // Actualizar contador de comentarios
                    const commentCount = parseInt(document.querySelector('.complaint-comments h4').textContent.match(/\d+/)[0]) - 1;
                    document.querySelector('.complaint-comments h4').textContent = `Comentarios (${commentCount})`;
                    
                    ui.showToast('Comentario eliminado correctamente', 'success');
                } catch (error) {
                    console.error('Error al eliminar comentario:', error);
                    ui.showToast('Error al eliminar comentario', 'error');
                }
            }
        }
    });
};

/**
 * Crea el HTML para un comentario
 * @param {Object} comment - Datos del comentario
 * @param {boolean} [isReply] - Indica si es una respuesta
 * @returns {string} - HTML del comentario
 */
const createCommentHtml = (comment, isReply = false) => {
    const user = auth.getUser();
    const isAuthor = user && user.id === comment.userId;
    const isAdmin = user && auth.hasRole('Admin');
    
    return `
        <div class="comment ${isReply ? 'comment-reply' : ''}" data-comment-id="${comment.id}">
            <img src="${comment.userPicture || 'img/default-avatar.png'}" alt="${comment.userName}" class="comment-avatar">
            <div class="comment-content">
                <div class="comment-header">
                    <div class="comment-author">${comment.userName}</div>
                    <div class="comment-date">${ui.formatDate(comment.createdAt, true)}</div>
                </div>
                <div class="comment-text">${comment.content}</div>
                <div class="comment-actions">
                    ${auth.isAuthenticated() ? `
                        <a href="#" class="reply-btn" data-comment-id="${comment.id}">
                            <i class="fas fa-reply"></i> Responder
                        </a>
                    ` : ''}
                    ${isAuthor || isAdmin ? `
                        <a href="#" class="edit-comment-btn" data-comment-id="${comment.id}">
                            <i class="fas fa-edit"></i> Editar
                        </a>
                        <a href="#" class="delete-comment-btn" data-comment-id="${comment.id}">
                            <i class="fas fa-trash"></i> Eliminar
                        </a>
                    ` : ''}
                </div>
                ${comment.replies && comment.replies.length > 0 ? `
                    <div class="comment-replies">
                        ${comment.replies.map(reply => createCommentHtml(reply, true)).join('')}
                    </div>
                ` : ''}
            </div>
        </div>
    `;
};

/**
 * Inicializa la votación
 * @param {Object} complaint - Datos de la denuncia
 */
const initVoting = (complaint) => {
    // Si el usuario no está autenticado, no hay votación
    if (!auth.isAuthenticated()) {
        return;
    }
    
    const upvoteBtn = document.getElementById('upvote-btn');
    const downvoteBtn = document.getElementById('downvote-btn');
    
    // Botón de upvote
    upvoteBtn.addEventListener('click', async () => {
        try {
            // Si ya está votado con upvote, eliminar el voto
            if (complaint.hasUserVoted && complaint.userVoteType) {
                await api.delete(`/votes/${complaint.id}`);
                
                // Actualizar UI
                upvoteBtn.classList.remove('btn-primary');
                upvoteBtn.classList.add('btn-outline');
                
                // Actualizar contador
                const upvoteCount = document.getElementById('upvote-count');
                upvoteCount.textContent = parseInt(upvoteCount.textContent) - 1;
                
                // Actualizar estado de la denuncia
                complaint.hasUserVoted = false;
                complaint.userVoteType = null;
                
                ui.showToast('Voto eliminado', 'info');
            } else {
                // Crear o actualizar voto
                const voteData = {
                    complaintId: complaint.id,
                    isUpvote: true
                };
                
                await api.post('/votes', voteData);
                
                // Actualizar UI
                upvoteBtn.classList.add('btn-primary');
                upvoteBtn.classList.remove('btn-outline');
                downvoteBtn.classList.remove('btn-secondary');
                downvoteBtn.classList.add('btn-outline');
                
                // Actualizar contadores
                const upvoteCount = document.getElementById('upvote-count');
                const downvoteCount = document.getElementById('downvote-count');
                
                if (complaint.hasUserVoted && !complaint.userVoteType) {
                    // Cambiar de downvote a upvote
                    upvoteCount.textContent = parseInt(upvoteCount.textContent) + 1;
                    downvoteCount.textContent = parseInt(downvoteCount.textContent) - 1;
                } else {
                    // Nuevo voto
                    upvoteCount.textContent = parseInt(upvoteCount.textContent) + 1;
                }
                
                // Actualizar estado de la denuncia
                complaint.hasUserVoted = true;
                complaint.userVoteType = true;
                
                ui.showToast('Voto registrado', 'success');
            }
        } catch (error) {
            console.error('Error al votar:', error);
            ui.showToast('Error al votar', 'error');
        }
    });
    
    // Botón de downvote
    downvoteBtn.addEventListener('click', async () => {
        try {
            // Si ya está votado con downvote, eliminar el voto
            if (complaint.hasUserVoted && !complaint.userVoteType) {
                await api.delete(`/votes/${complaint.id}`);
                
                // Actualizar UI
                downvoteBtn.classList.remove('btn-secondary');
                downvoteBtn.classList.add('btn-outline');
                
                // Actualizar contador
                const downvoteCount = document.getElementById('downvote-count');
                downvoteCount.textContent = parseInt(downvoteCount.textContent) - 1;
                
                // Actualizar estado de la denuncia
                complaint.hasUserVoted = false;
                complaint.userVoteType = null;
                
                ui.showToast('Voto eliminado', 'info');
            } else {
                // Crear o actualizar voto
                const voteData = {
                    complaintId: complaint.id,
                    isUpvote: false
                };
                
                await api.post('/votes', voteData);
                
                // Actualizar UI
                downvoteBtn.classList.add('btn-secondary');
                downvoteBtn.classList.remove('btn-outline');
                upvoteBtn.classList.remove('btn-primary');
                upvoteBtn.classList.add('btn-outline');
                
                // Actualizar contadores
                const upvoteCount = document.getElementById('upvote-count');
                const downvoteCount = document.getElementById('downvote-count');
                
                if (complaint.hasUserVoted && complaint.userVoteType) {
                    // Cambiar de upvote a downvote
                    downvoteCount.textContent = parseInt(downvoteCount.textContent) + 1;
                    upvoteCount.textContent = parseInt(upvoteCount.textContent) - 1;
                } else {
                    // Nuevo voto
                    downvoteCount.textContent = parseInt(downvoteCount.textContent) + 1;
                }
                
                // Actualizar estado de la denuncia
                complaint.hasUserVoted = true;
                complaint.userVoteType = false;
                
                ui.showToast('Voto registrado', 'success');
            }
        } catch (error) {
            console.error('Error al votar:', error);
            ui.showToast('Error al votar', 'error');
        }
    });
    
    // Configurar eventos para editar y eliminar denuncia
    const editBtn = document.getElementById('edit-complaint-btn');
    const deleteBtn = document.getElementById('delete-complaint-btn');
    const updateStatusBtn = document.getElementById('update-status-btn');
    
    if (editBtn) {
        editBtn.addEventListener('click', (e) => {
            e.preventDefault();
            router.navigateTo(`/editar-denuncia/${complaint.id}`);
        });
    }
    
    if (deleteBtn) {
        deleteBtn.addEventListener('click', async (e) => {
            e.preventDefault();
            
            // Confirmar eliminación
            if (confirm('¿Estás seguro de que deseas eliminar esta denuncia? Esta acción no se puede deshacer.')) {
                try {
                    await api.delete(`/complaints/${complaint.id}`);
                    ui.showToast('Denuncia eliminada correctamente', 'success');
                    router.navigateTo('/denuncias');
                } catch (error) {
                    console.error('Error al eliminar denuncia:', error);
                    ui.showToast('Error al eliminar denuncia', 'error');
                }
            }
        });
    }
    
    if (updateStatusBtn) {
        updateStatusBtn.addEventListener('click', async (e) => {
            e.preventDefault();
            
            try {
                // Obtener los estados disponibles
                const statuses = await api.get('/statuses');
                
                // Crear modal para actualizar estado
                const modal = document.createElement('div');
                modal.className = 'modal';
                modal.id = 'update-status-modal';
                
                modal.innerHTML = `
                    <div class="modal-content">
                        <span class="close-modal">&times;</span>
                        <h3>Actualizar estado de la denuncia</h3>
                        <form id="update-status-form">
                            <div class="form-group">
                                <label for="status-select">Estado</label>
                                <select id="status-select" class="form-control" required>
                                    ${statuses.map(status => `
                                        <option value="${status.id}" ${status.id === complaint.statusId ? 'selected' : ''}>
                                            ${status.name}
                                        </option>
                                    `).join('')}
                                </select>
                            </div>
                            <div class="form-group">
                                <label for="status-comments">Comentarios</label>
                                <textarea id="status-comments" class="form-control" placeholder="Explica el motivo del cambio de estado..."></textarea>
                            </div>
                            <div class="form-group text-right">
                                <button type="button" class="btn btn-outline cancel-btn">Cancelar</button>
                                <button type="submit" class="btn btn-primary">Actualizar</button>
                            </div>
                        </form>
                    </div>
                `;
                
                document.body.appendChild(modal);
                
                // Mostrar modal
                modal.style.display = 'flex';
                
                // Configurar cierre del modal
                const closeModal = () => {
                    modal.remove();
                };
                
                modal.querySelector('.close-modal').addEventListener('click', closeModal);
                modal.querySelector('.cancel-btn').addEventListener('click', closeModal);
                
                // Configurar formulario
                modal.querySelector('form').addEventListener('submit', async (e) => {
                    e.preventDefault();
                    
                    const statusId = parseInt(document.getElementById('status-select').value);
                    const comments = document.getElementById('status-comments').value.trim();
                    
                    try {
                        // Actualizar estado
                        const updateData = {
                            complaintId: complaint.id,
                            statusId,
                            comments
                        };
                        
                        const updatedComplaint = await api.put('/complaints/status', updateData);
                        
                        // Cerrar modal
                        closeModal();
                        
                        // Recargar la página
                        ui.showToast('Estado actualizado correctamente', 'success');
                        
                        // Redirigir para refrescar la página
                        setTimeout(() => {
                            router.navigateTo(`/denuncia/${complaint.id}`);
                        }, 1000);
                    } catch (error) {
                        console.error('Error al actualizar estado:', error);
                        ui.showToast('Error al actualizar estado', 'error');
                    }
                });
            } catch (error) {
                console.error('Error al cargar estados:', error);
                ui.showToast('Error al cargar estados', 'error');
            }
        });
    }
};