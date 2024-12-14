import { lazy } from "react";
import { Route, Routes } from "react-router-dom";

const Home = lazy(() => import('@/pages/Home'));

export default function AppRoutes() {
    return (
        <Routes>
            <Route path="/" element={<Home />}>
            </Route>
        </Routes>
    );
}
