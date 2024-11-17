document.addEventListener("DOMContentLoaded", () => {
    const links = document.querySelectorAll("#sidebar .nav-link");
    links.forEach(link => {
        link.addEventListener("click", () => {
            links.forEach(link => link.classList.remove("active"));
            link.classList.add("active");
        });
    });
});
