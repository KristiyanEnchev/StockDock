import { Link } from "react-router-dom";
import { LineChart } from "lucide-react";
import ThemeToggle from "../common/ThemeToggle";

export function Navigation() {
    return (
        <nav className="fixed top-0 left-0 right-0 z-50 bg-light-bg dark:bg-dark-bg text-light-text dark:text-dark-text border-b border-light-card dark:border-dark-card">
            <div className="max-w-7xl mx-auto">
                <div className="flex justify-between h-16 px-6">
                    <div className="flex items-center gap-8">
                        <Link
                            to="/"
                            className="flex items-center gap-3 text-green-600 dark:text-green-400 hover:text-green-500 dark:hover:text-green-300 transition"
                        >
                            <div className="w-10 h-10 rounded-full bg-green-500/10 flex items-center justify-center">
                                <LineChart className="w-6 h-6 text-green-600 dark:text-green-400" />
                            </div>
                            <span className="text-xl font-bold">StockDock</span>
                        </Link>
                    </div>

                    <div className="flex items-center gap-6">
                        <ThemeToggle />
                        <Link
                            to="/login"
                            className="px-4 py-2 rounded-lg bg-green-600 dark:bg-green-500 text-white hover:bg-green-700 dark:hover:bg-green-400 transition"
                        >
                            Sign In
                        </Link>
                    </div>
                </div>
            </div>
        </nav>
    );
}
