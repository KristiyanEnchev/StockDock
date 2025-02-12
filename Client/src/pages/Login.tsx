import { useState } from "react";
import { useNavigate, useLocation, Link } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { toast } from "react-hot-toast";
import { Loader2, LineChart } from "lucide-react";
import { useLoginMutation } from "../features/auth/authApi";
import { useAppDispatch } from "../store/hooks";
import { setCredentials } from "../features/auth/authSlice";

const loginSchema = z.object({
    email: z.string().email("Invalid email address"),
    password: z.string().min(6, "Password must be at least 6 characters"),
});

type LoginFormData = z.infer<typeof loginSchema>;

export default function Login() {
    const navigate = useNavigate();
    const location = useLocation();
    const dispatch = useAppDispatch();
    const [login] = useLoginMutation();
    const [isLoading, setIsLoading] = useState(false);

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<LoginFormData>({
        resolver: zodResolver(loginSchema),
    });

    const onSubmit = async (data: LoginFormData) => {
        try {
            setIsLoading(true);
            const result = await login(data).unwrap();
            dispatch(setCredentials(result));
            navigate(location.state?.from?.pathname || "/", { replace: true });
            toast.success("Successfully logged in!");
        } catch {
            toast.error("Failed to login. Please check your credentials.");
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center px-4 sm:px-6 lg:px-8 bg-light-bg dark:bg-dark-bg">
            <div className="max-w-md w-full bg-light-card dark:bg-dark-card p-6 rounded-lg shadow-lg border border-light-card dark:border-dark-card">
                <div className="flex items-center justify-center gap-2 text-light-text dark:text-dark-text">
                    <LineChart className="w-7 h-7 text-green-600 dark:text-green-400" />
                    <h2 className="text-2xl font-semibold">Sign in</h2>
                </div>
                <p className="text-center text-sm text-light-text dark:text-dark-text opacity-80 mt-1">
                    Use <span className="font-medium">admin@admin.com / 123456</span> to sign in
                </p>
                <form className="mt-5 space-y-4" onSubmit={handleSubmit(onSubmit)}>
                    <div>
                        <input
                            type="email"
                            {...register("email")}
                            placeholder="Email"
                            className="w-full px-4 py-2 bg-light-bg dark:bg-dark-bg border border-light-card dark:border-dark-card rounded-md focus:ring-2 focus:ring-green-500 dark:focus:ring-green-400"
                            disabled={isLoading}
                        />
                        {errors.email && <p className="text-xs text-red-500">{errors.email.message}</p>}
                    </div>
                    <div>
                        <input
                            type="password"
                            {...register("password")}
                            placeholder="Password"
                            className="w-full px-4 py-2 text-light-text dark:text-dark-text bg-light-bg dark:bg-dark-bg border border-light-card dark:border-dark-card rounded-md focus:ring-2 focus:ring-green-500 dark:focus:ring-green-400"
                            disabled={isLoading}
                        />
                        {errors.password && <p className="text-xs text-red-500">{errors.password.message}</p>}
                    </div>
                    <button
                        type="submit"
                        disabled={isLoading}
                        className="w-full flex justify-center items-center gap-2 px-4 py-2 rounded-md bg-green-600 dark:bg-green-500 text-white hover:bg-green-700 dark:hover:bg-green-400 transition disabled:opacity-50"
                    >
                        {isLoading ? <Loader2 className="h-5 w-5 animate-spin" /> : "Sign in"}
                    </button>
                </form>
                <p className="text-center text-sm text-light-text dark:text-dark-text mt-3">
                    Don't have an account?{" "}
                    <Link to="/register" className="font-medium text-green-600 dark:text-green-400 hover:text-green-700 dark:hover:text-green-300">
                        Sign up
                    </Link>
                </p>
            </div>
        </div>
    );
}
