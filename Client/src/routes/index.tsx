import { lazy, Suspense } from "react";
import { Route, Routes } from "react-router-dom";

const Home = lazy(() => import('@/pages/Home'));
const NotFound = lazy(() => import("@/pages/NotFound"));

const Loader = () => (
    <div className="flex items-center justify-center min-h-screen bg-gray-900 text-white">
        <span className="animate-pulse">Loading...</span>
    </div>
);

export default function AppRoutes() {
    return (
        <Routes>
            <Route
                path="/"
                element={
                    <Suspense fallback={<Loader />}>
                        <Home />
                    </Suspense>
                }
            />
            <Route
                path="*"
                element={
                    <Suspense fallback={<Loader />}>
                        <NotFound />
                    </Suspense>
                }
            />
        </Routes>
    );
}
