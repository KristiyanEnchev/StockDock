/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
    "./components/**/*.{js,ts,jsx,tsx}",
    "./node_modules/@shadcn/ui/dist/**/*.js",
  ],
  darkMode: "class",
  theme: {
    extend: {
      colors: {
        "dark-bg": "#0f172a",
        "dark-card": "#1e293b",
        "dark-text": "#f8fafc",
        "light-bg": "#f8fafc",
        "light-card": "#e2e8f0",
        "light-text": "#1e293b",
      },
    },
  },
  plugins: [],
};
