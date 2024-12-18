import colors from "tailwindcss/colors";

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
        "dark-bg": colors.slate[900],
        "dark-card": colors.gray[800],
        "dark-text": colors.gray[100],
        "light-bg": colors.gray[100],
        "light-card": colors.gray[300],
        "light-text": colors.gray[900],
      },
    },
  },
  plugins: [],
};
