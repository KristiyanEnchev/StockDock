export function initializeTheme() {
    if (typeof window === "undefined") return true;

    try {
        const saved = localStorage.getItem("theme");
        const isDark = saved ? saved === "dark" : true;

        document.documentElement.classList.toggle("dark", isDark);
        localStorage.setItem("theme", isDark ? "dark" : "light");

        return isDark;
    } catch (error) {
        console.error("Failed to initialize theme:", error);
        return true;
    }
}
