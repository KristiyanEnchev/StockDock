import { useNavigate } from 'react-router-dom';
import { FileQuestion } from 'lucide-react';

export default function NotFound() {
    const navigate = useNavigate();

    return (
        <div className="min-h-screen flex items-center justify-center px-4 sm:px-6 lg:px-8 bg-gray-900 text-white relative">
            <div className="absolute inset-0 border border-gray-700 opacity-10" />
            <div className="max-w-md w-full space-y-6 bg-gray-800 p-8 rounded-lg shadow-xl relative text-center border border-gray-700">
                <div className="mx-auto w-16 h-16 bg-green-500/10 rounded-full flex items-center justify-center">
                    <FileQuestion className="w-8 h-8 text-green-400" />
                </div>
                <h1 className="mt-4 text-3xl font-bold text-white">
                    Page Not Found
                </h1>
                <p className="text-sm text-gray-400">
                    The page you're looking for doesn't exist or has been moved.
                </p>
                <button
                    onClick={() => navigate('/')}
                    className="w-full flex justify-center items-center gap-2 px-4 py-3 border border-transparent rounded-lg bg-green-500 text-white hover:bg-green-600 transition-colors"
                >
                    Go Home
                </button>
            </div>
        </div>
    );
}
