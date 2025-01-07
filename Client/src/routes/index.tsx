import { lazy } from "react";
import { Route, Routes } from "react-router-dom";
import { MainLayout } from "@/components/layout/MainLayout";
import PageWrapper from "@/components/common/PageWrapper";

/// TODO: Remove delay before production
const delayImport = <T extends { default: React.ComponentType<any> }>(
    importFunction: () => Promise<T>,
    delay: number = 5000
): Promise<T> => {
    return new Promise(resolve => {
        setTimeout(() => resolve(importFunction()), delay);
    });
};

const Home = lazy(() => delayImport(() => import("@/pages/Home")));
const NotFound = lazy(() => import("@/pages/NotFound"));
const Login = lazy(() => import("@/pages/Login"));

export default function AppRoutes() {
    return (
        <Routes>
            <Route path="/" element={<MainLayout />}>
                <Route path="/" element={<PageWrapper component={Home} />} />
                <Route path="login" element={<PageWrapper component={Login} />} />
            </Route>
            <Route path="*" element={<PageWrapper component={NotFound} />} />
        </Routes>
    );
}
