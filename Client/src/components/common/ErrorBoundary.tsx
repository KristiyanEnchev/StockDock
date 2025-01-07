import React from "react";
import { AlertTriangle } from "lucide-react";
import { Link } from "react-router-dom";

interface Props {
    children: React.ReactNode;
}

interface State {
    hasError: boolean;
    error?: Error;
}

export class ErrorBoundary extends React.Component<Props, State> {
    constructor(props: Props) {
        super(props);
        this.state = { hasError: false };
    }

    static getDerivedStateFromError(error: Error): State {
        return { hasError: true, error };
    }

    componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
        console.error("Error caught by boundary:", error, errorInfo);
    }

    render() {
        if (this.state.hasError) {
            return <ErrorPage error={this.state.error} />;
        }

        return this.props.children;
    }
}

function ErrorPage({ error }: { error?: Error }) {
    return (
        <div className="min-h-screen flex items-center justify-center px-6 bg-light-bg dark:bg-dark-bg text-light-text dark:text-dark-text">
            <div className="max-w-md w-full text-center bg-light-card dark:bg-dark-card p-6 rounded-lg shadow-lg border border-light-card dark:border-dark-card">
                <div className="flex items-center justify-center gap-2">
                    <AlertTriangle className="w-7 h-7 text-red-500 dark:text-red-400" />
                    <h1 className="text-2xl font-semibold">Something went wrong</h1>
                </div>
                <p className="mt-3 text-sm text-light-text dark:text-dark-text opacity-80">
                    {error?.message || "An unexpected error occurred. Please try again later."}
                </p>
                <div className="mt-5 flex justify-center gap-3">
                    <Link
                        to="/"
                        className="px-4 py-2 rounded-md text-white bg-green-600 dark:bg-green-500 hover:bg-green-700 dark:hover:bg-green-400 transition"
                    >
                        Go Home
                    </Link>
                    <button
                        onClick={() => window.location.reload()}
                        className="px-4 py-2 rounded-md border border-light-card dark:border-dark-card text-light-text dark:text-dark-text hover:bg-light-card/20 dark:hover:bg-dark-card/20 transition"
                    >
                        Try Again
                    </button>
                </div>
            </div>
        </div>
    );
}
