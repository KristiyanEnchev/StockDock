export function initializeTheme() {
    if (typeof window === "undefined") return true;

    const saved = localStorage.getItem("theme");
    const isDark = saved ? saved === "dark" : true;

    if (isDark) {
        document.documentElement.classList.add("dark");
        localStorage.setItem("theme", "dark");
    } else {
        document.documentElement.classList.remove("dark");
        localStorage.setItem("theme", "light");
    }

    return isDark;
}
