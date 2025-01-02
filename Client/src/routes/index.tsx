import { lazy, Suspense } from "react";
import { Route, Routes } from "react-router-dom";
import { MainLayout } from "@/components/layout/MainLayout";
import LoadingSpinner from '@/components/common/LoadingSpinner';

/// TODO : remove before finishing the project
const delayImport = <T extends { default: React.ComponentType<any> }>(
    importFunction: () => Promise<T>,
    delay: number = 5000
): Promise<T> => {
    return new Promise(resolve => {
        setTimeout(() => resolve(importFunction()), delay);
    });
};

const Home = lazy(() => delayImport(() => import('@/pages/Home')));
const NotFound = lazy(() => import("@/pages/NotFound"));
const Login = lazy(() => import("@/pages/Login"));

export default function AppRoutes() {
    return (
        <Routes>
            <Route path="/" element={<MainLayout />}>
                <Route
                    path="/"
                    element={
                        <Suspense fallback={<LoadingSpinner size="large" fullScreen={true} variant="alternative" />}>
                            <Home />
                        </Suspense>
                    }
                />
                <Route
                    path="login"
                    element={
                        <Suspense fallback={<LoadingSpinner size="large" fullScreen={true} variant="alternative" />}>
                            <Login />
                        </Suspense>
                    }
                />
            </Route>
            <Route
                path="*"
                element={
                    <Suspense fallback={<LoadingSpinner size="large" fullScreen={true} variant="alternative" />}>
                        <NotFound />
                    </Suspense>
                }
            />
        </Routes>
    );
}
