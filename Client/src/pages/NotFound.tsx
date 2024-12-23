import { useNavigate } from "react-router-dom";
import { FileQuestion } from "lucide-react";

export default function NotFound() {
    const navigate = useNavigate();

    return (
        <div className="min-h-screen flex items-center justify-center px-4 sm:px-6 lg:px-8 bg-light-bg dark:bg-dark-bg text-light-text dark:text-dark-text relative">
            <div className="absolute inset-0 border border-light-card dark:border-dark-card opacity-10" />
            <div className="max-w-md w-full space-y-6 bg-light-card dark:bg-dark-card p-8 rounded-lg shadow-xl relative text-center border border-light-card dark:border-dark-card">
                <div className="mx-auto w-16 h-16 bg-green-500/10 rounded-full flex items-center justify-center">
                    <FileQuestion className="w-8 h-8 text-green-600 dark:text-green-400" />
                </div>
                <h1 className="mt-4 text-3xl font-bold text-light-text dark:text-dark-text">
                    Page Not Found
                </h1>
                <p className="text-sm text-light-text dark:text-dark-text opacity-70">
                    The page you're looking for doesn't exist or has been moved.
                </p>
                <button
                    onClick={() => navigate("/")}
                    className="w-full flex justify-center items-center gap-2 px-4 py-3 border border-transparent rounded-lg bg-green-600 dark:bg-green-500 text-white hover:bg-green-700 dark:hover:bg-green-400 transition-colors"
                >
                    Go Home
                </button>
            </div>
        </div>
    );
}
