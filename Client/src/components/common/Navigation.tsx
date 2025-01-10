import { Link } from "react-router-dom";
import { ChevronDown, LineChart, User } from "lucide-react";
import ThemeToggle from "../common/ThemeToggle";
import { LogoutButton } from "./LogoutButton";
import { useEffect, useRef, useState } from "react";
import { selectCurrentUser } from "@/features/auth/authSlice";
import { useAppSelector } from "@/store/hooks";

export function Navigation() {
    const user = useAppSelector(selectCurrentUser);
    const [isDropdownOpen, setIsDropdownOpen] = useState(false);
    const dropdownRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
                setIsDropdownOpen(false);
            }
        };

        document.addEventListener("mousedown", handleClickOutside);
        return () => document.removeEventListener("mousedown", handleClickOutside);
    }, []);

    return (
        <nav className="fixed top-0 left-0 right-0 z-50 bg-light-bg dark:bg-dark-bg text-light-text dark:text-dark-text border-b border-light-card dark:border-dark-card">
            <div className="max-w-7xl mx-auto">
                <div className="flex justify-between h-16 px-6 items-center">
                    <Link
                        to="/"
                        className="flex items-center gap-3 text-green-600 dark:text-green-400 hover:text-green-500 dark:hover:text-green-300 transition"
                    >
                        <div className="w-10 h-10 rounded-full bg-green-500/10 flex items-center justify-center">
                            <LineChart className="w-6 h-6 text-green-600 dark:text-green-400" />
                        </div>
                        <span className="text-xl font-bold">StockDock</span>
                    </Link>

                    <div className="flex items-center gap-4">
                        <ThemeToggle />

                        {user ? (
                            <div className="relative" ref={dropdownRef}>
                                <button
                                    onClick={() => setIsDropdownOpen(!isDropdownOpen)}
                                    className="flex items-center gap-2 px-4 py-2 text-light-text dark:text-dark-text hover:text-green-600 dark:hover:text-green-400 transition"
                                >
                                    <User className="w-5 h-5" />
                                    <span>{user.firstName}</span>
                                    <ChevronDown
                                        className={`w-4 h-4 transition-transform ${isDropdownOpen ? "rotate-180" : ""}`}
                                    />
                                </button>

                                {isDropdownOpen && (
                                    <div className="absolute right-0 mt-2 w-48 rounded-md shadow-lg bg-light-card dark:bg-dark-card border border-light-card dark:border-dark-card">
                                        <div className="py-1">
                                            <Link
                                                to="/profile"
                                                className="block px-4 py-2 text-light-text dark:text-dark-text hover:bg-light-bg dark:hover:bg-dark-bg transition"
                                            >
                                                Profile
                                            </Link>
                                        </div>
                                    </div>
                                )}
                            </div>
                        ) : (
                            <div className="flex gap-3">
                                <Link
                                    to="/login"
                                    className="px-4 py-2 rounded-lg bg-green-600 dark:bg-green-500 text-white hover:bg-green-700 dark:hover:bg-green-400 transition"
                                >
                                    Sign In
                                </Link>
                                <Link
                                    to="/register"
                                    className="px-4 py-2 rounded-lg border border-green-600 dark:border-green-500 text-green-600 dark:text-green-400 hover:bg-green-600 dark:hover:bg-green-500 hover:text-white transition"
                                >
                                    Register
                                </Link>
                            </div>
                        )}

                        {user && <LogoutButton className={"px-4 py-2 rounded-lg border border-green-600 dark:border-green-500 text-green-600 dark:text-green-400 hover:bg-green-600 dark:hover:bg-green-500 hover:text-white transition"} />}
                    </div>
                </div>
            </div>
        </nav>
    );
}
