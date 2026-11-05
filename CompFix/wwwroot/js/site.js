document.addEventListener('DOMContentLoaded', function () {
    const btnOpen = document.getElementById('accountDrawerBtn');
    const btnClose = document.getElementById('closeDrawerBtn');
    const drawer = document.getElementById('accountDrawer');
    const overlay = document.getElementById('drawerOverlay');

    function openDrawer() {
        if (drawer && overlay) {
            drawer.classList.add('open');
            overlay.classList.add('active');
            drawer.setAttribute('aria-hidden', 'false');
            document.body.style.overflow = 'hidden'; // Заборонити прокрутку сторінки
        }
    }

    function closeDrawer() {
        if (drawer && overlay) {
            drawer.classList.remove('open');
            overlay.classList.remove('active');
            drawer.setAttribute('aria-hidden', 'true');
            document.body.style.overflow = ''; // Відновити прокрутку
        }
    }

    if (btnOpen) {
        btnOpen.addEventListener('click', function (e) {
            e.preventDefault();
            openDrawer();
        });
    }

    if (btnClose) {
        btnClose.addEventListener('click', closeDrawer);
    }

    if (overlay) {
        overlay.addEventListener('click', closeDrawer);
    }

    // Закриття по клавіші Escape
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && drawer && drawer.classList.contains('open')) {
            closeDrawer();
        }
    });
});