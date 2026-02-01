
(function () {
    const links = document.querySelectorAll('.app-navlink');
    const path = window.location.pathname.toLowerCase();

    links.forEach(a => {
        const href = (a.getAttribute('href') || '').toLowerCase();
        if (href && (path === href || (href !== '/' && path.startsWith(href)))) {
            a.classList.add('active');
        }
    });
})();
