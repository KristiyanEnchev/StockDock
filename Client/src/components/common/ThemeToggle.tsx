import { useAppDispatch, useAppSelector } from "@/store/hooks";
import { toggleTheme, selectIsDark } from "@/features/theme/themeSlice";
import { Moon, Sun } from "lucide-react";

export default function ThemeToggle() {
    const dispatch = useAppDispatch();
    const isDark = useAppSelector(selectIsDark);

    return (
        <button
            onClick={() => dispatch(toggleTheme())}
            className="p-2 rounded-md border border-gray-500 transition-colors hover:bg-gray-700 dark:hover:bg-gray-300"
        >
            {isDark ? <Moon className="text-yellow-300" /> : <Sun className="text-yellow-500" />}
        </button>
    );
}
