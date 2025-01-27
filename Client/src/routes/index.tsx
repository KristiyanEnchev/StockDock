import { lazy } from "react";
import { Route, Routes } from "react-router-dom";
import { MainLayout } from "@/components/layout/MainLayout";
import PageWrapper from "@/components/common/PageWrapper";
import GuestRoute from "./GuestRoute";
import ProtectedRoute from "./ProtectedRoute";

const Home = lazy(() => import("@/pages/Home"));
const NotFound = lazy(() => import("@/pages/NotFound"));
const Login = lazy(() => import("@/pages/Login"));
const Register = lazy(() => import("@/pages/Register"));
const Profile = lazy(() => import("@/pages/Profile"));

export default function AppRoutes() {
    return (
        <Routes>
            <Route path="/" element={<MainLayout />}>
                <Route path="/" element={<PageWrapper component={Home} />} />
                <Route element={<GuestRoute />}>
                    <Route path="/login" element={<PageWrapper component={Login} />} />
                    <Route path="/register" element={<PageWrapper component={Register} />} />
                </Route>
                <Route element={<ProtectedRoute />}>
                    <Route path="/profile" element={<PageWrapper component={Profile} />} />
                </Route>
            </Route>
            <Route path="*" element={<PageWrapper component={NotFound} />} />
        </Routes>
    );
}
