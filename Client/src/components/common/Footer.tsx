import { LineChart } from "lucide-react";

export function Footer() {
    return (
        <footer className="w-full py-6 border-t border-light-card dark:border-dark-card bg-light-bg dark:bg-dark-bg">
            <div className="container mx-auto px-4 text-center">
                <div className="flex flex-col sm:flex-row items-center justify-center gap-2 text-light-text dark:text-dark-text opacity-80">
                    <span className="flex items-center gap-2">
                        {new Date().getFullYear()} StockDock
                        <LineChart className="w-4 h-4 text-green-600 dark:text-green-400" />
                        by{" "}
                        <a
                            href="https://kristiyan-enchev-website.web.app/"
                            target="_blank"
                            rel="noreferrer"
                            className="hover:text-green-600 dark:hover:text-green-400 transition"
                        >
                            Kristiyan Enchev
                        </a>
                    </span>
                </div>
            </div>
        </footer>
    );
}
