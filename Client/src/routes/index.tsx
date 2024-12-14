import { lazy } from "react";
import { Route, Routes } from "react-router-dom";

const Home = lazy(() => import('@/pages/Home'));
const NotFound = lazy(() => import("@/pages/NotFound"));

export default function AppRoutes() {
    return (
        <Routes>
            <Route path="/" element={<Home />} />
            <Route path="*" element={<NotFound />} />
        </Routes>
    );
}
