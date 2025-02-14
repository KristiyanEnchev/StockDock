import React from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "react-hot-toast";
import { useAppDispatch } from "../../store/hooks";
import { logoutUser } from "../../features/auth/authSlice";

interface LogoutButtonProps {
    showText?: boolean;
    className?: string;
}

export const LogoutButton: React.FC<LogoutButtonProps> = ({
    showText = true,
    className = "px-4 py-2 rounded-lg border border-light-card dark:border-dark-card text-light-text dark:text-dark-text hover:bg-light-card/20 dark:hover:bg-dark-card/20 transition-colors",
}) => {
    const dispatch = useAppDispatch();
    const navigate = useNavigate();

    const handleLogout = async () => {
        try {
            await dispatch(logoutUser());
            toast.success("Successfully logged out!");
            navigate("/login");
        } catch (error) {
            console.error("Logout failed:", error);
            toast.error(`Logout failed: ${error}`);
        }
    };

    return (
        <button onClick={handleLogout} className={className} title="Logout">
            {showText && <span>Logout</span>}
        </button>
    );
};
