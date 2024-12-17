import { lazy, Suspense } from "react";
import { Route, Routes } from "react-router-dom";

const Home = lazy(() => import('@/pages/Home'));
const NotFound = lazy(() => import("@/pages/NotFound"));

export default function AppRoutes() {
    return (
        <Routes>
            <Route path="/" element={
                <Suspense fallback={"Loading...."}>
                    <Home />
                </Suspense>}>
            </Route>

            <Route
                path="*"
                element={
                    <Suspense fallback={"Loading...."}>
                        <NotFound />
                    </Suspense>
                }
            />
        </Routes>
    );
}
