function openModal(type) {
    const d = capsules.find(x => x.id === type);
    if (!d) return;

    document.getElementById('modal-img').className = 'modal-img ' + d.imgClass;

    const badge = document.getElementById('modal-badge');
    badge.textContent = d.badge || '';
    badge.className = 'modal-badge ' + (d.badgeClass || '');

    document.getElementById('modal-title').textContent = d.title;
    document.getElementById('modal-subtitle').textContent = d.subtitle;
    document.getElementById('modal-desc').textContent = d.desc;
    document.getElementById('modal-price').textContent = d.price;

    document.getElementById('modal-amenities').innerHTML =
        d.amenities.map(a => `<li>${a}</li>`).join('');

    document.getElementById('modal-overlay').classList.add('active');
    document.body.style.overflow = 'hidden';
}

function closeModal() {
    document.getElementById('modal-overlay').classList.remove('active');
    document.body.style.overflow = '';
}

function handleOverlayClick(e) {
    if (e.target === document.getElementById('modal-overlay')) {
        closeModal();
    }
}

document.addEventListener('keydown', e => {
    if (e.key === 'Escape') closeModal();
});

function scrollCarousel(dir) {
    const grid = document.getElementById('capsules-grid');
    if (!grid) return;

    grid.scrollBy({
        left: dir * 324,
        behavior: 'smooth'
    });
}