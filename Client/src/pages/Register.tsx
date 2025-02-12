import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { toast } from "react-hot-toast";
import { Loader2, LineChart } from "lucide-react";
import { useRegisterMutation } from "../features/auth/authApi";

const registerSchema = z
    .object({
        firstName: z.string().min(2, "First Name must be at least 2 characters"),
        lastName: z.string().min(2, "Last Name must be at least 2 characters"),
        email: z.string().email("Invalid email address"),
        password: z.string().min(6, "Password must be at least 6 characters"),
        confirmPassword: z.string(),
    })
    .refine((data) => data.password === data.confirmPassword, {
        message: "Passwords don't match",
        path: ["confirmPassword"],
    });

type RegisterFormData = z.infer<typeof registerSchema>;

export default function Register() {
    const navigate = useNavigate();
    const [register] = useRegisterMutation();
    const [isLoading, setIsLoading] = useState(false);

    const {
        register: registerField,
        handleSubmit,
        formState: { errors },
    } = useForm<RegisterFormData>({
        resolver: zodResolver(registerSchema),
    });

    const onSubmit = async (data: RegisterFormData) => {
        try {
            setIsLoading(true);
            await register({
                firstName: data.firstName,
                lastName: data.lastName,
                email: data.email,
                password: data.password,
                confirmPassword: data.confirmPassword,
            }).unwrap();

            navigate("/", { replace: true });
            toast.success("Registration successful!");
        } catch {
            toast.error("Failed to register. Please try again.");
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center px-4 sm:px-6 lg:px-8 bg-light-bg dark:bg-dark-bg">
            <div className="max-w-md w-full bg-light-card dark:bg-dark-card p-6 rounded-lg shadow-lg border border-light-card dark:border-dark-card">
                <div className="flex items-center justify-center gap-2 text-light-text dark:text-dark-text">
                    <LineChart className="w-7 h-7 text-green-600 dark:text-green-400" />
                    <h2 className="text-2xl font-semibold">Create an Account</h2>
                </div>
                <p className="text-center text-sm text-light-text dark:text-dark-text opacity-80 mt-1">
                    Already have an account?{" "}
                    <Link
                        to="/login"
                        className="font-medium text-green-600 dark:text-green-400 hover:text-green-700 dark:hover:text-green-300 transition"
                    >
                        Sign in here
                    </Link>
                </p>
                <form className="mt-5 space-y-4" onSubmit={handleSubmit(onSubmit)}>
                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <input
                                type="text"
                                {...registerField("firstName")}
                                placeholder="First Name"
                                className="w-full text-light-text dark:text-dark-text px-4 py-2 bg-light-bg dark:bg-dark-bg border border-light-card dark:border-dark-card rounded-md focus:ring-2 focus:ring-green-500 dark:focus:ring-green-400"
                                disabled={isLoading}
                            />
                            {errors.firstName && <p className="text-xs text-red-500">{errors.firstName.message}</p>}
                        </div>
                        <div>
                            <input
                                type="text"
                                {...registerField("lastName")}
                                placeholder="Last Name"
                                className="w-full text-light-text dark:text-dark-text px-4 py-2 bg-light-bg dark:bg-dark-bg border border-light-card dark:border-dark-card rounded-md focus:ring-2 focus:ring-green-500 dark:focus:ring-green-400"
                                disabled={isLoading}
                            />
                            {errors.lastName && <p className="text-xs text-red-500">{errors.lastName.message}</p>}
                        </div>
                    </div>
                    <div>
                        <input
                            type="email"
                            {...registerField("email")}
                            placeholder="Email"
                            className="w-full text-light-text dark:text-dark-text px-4 py-2 bg-light-bg dark:bg-dark-bg border border-light-card dark:border-dark-card rounded-md focus:ring-2 focus:ring-green-500 dark:focus:ring-green-400"
                            disabled={isLoading}
                        />
                        {errors.email && <p className="text-xs text-red-500">{errors.email.message}</p>}
                    </div>
                    <div>
                        <input
                            type="password"
                            {...registerField("password")}
                            placeholder="Password"
                            className="w-full text-light-text dark:text-dark-text px-4 py-2 bg-light-bg dark:bg-dark-bg border border-light-card dark:border-dark-card rounded-md focus:ring-2 focus:ring-green-500 dark:focus:ring-green-400"
                            disabled={isLoading}
                        />
                        {errors.password && <p className="text-xs text-red-500">{errors.password.message}</p>}
                    </div>
                    <div>
                        <input
                            type="password"
                            {...registerField("confirmPassword")}
                            placeholder="Confirm Password"
                            className="w-full px-4 py-2 text-light-text dark:text-dark-text bg-light-bg dark:bg-dark-bg border border-light-card dark:border-dark-card rounded-md focus:ring-2 focus:ring-green-500 dark:focus:ring-green-400"
                            disabled={isLoading}
                        />
                        {errors.confirmPassword && <p className="text-xs text-red-500">{errors.confirmPassword.message}</p>}
                    </div>
                    <button
                        type="submit"
                        disabled={isLoading}
                        className="w-full flex justify-center items-center gap-2 px-4 py-2 rounded-md bg-green-600 dark:bg-green-500 text-white hover:bg-green-700 dark:hover:bg-green-400 transition disabled:opacity-50"
                    >
                        {isLoading ? <Loader2 className="h-5 w-5 animate-spin" /> : <>Create Account</>}
                    </button>
                </form>
            </div>
        </div>
    );
}
