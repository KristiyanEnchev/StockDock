import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { toast } from "react-hot-toast";
import { User, Key, Mail, Shield, Save, Loader2 } from "lucide-react";
import { useAppSelector } from "@/store/hooks";
import { selectCurrentUser } from "@/features/auth/authSlice";
import { useChangePasswordMutation, useUpdateEmailMutation } from "@/features/auth/usersApi";

const passwordSchema = z.object({
    currentPassword: z.string().min(6, "Current password is required"),
    newPassword: z.string().min(6, "Password must be at least 6 characters"),
    confirmNewPassword: z.string().min(6, "Please confirm your new password")
}).refine(data => data.newPassword === data.confirmNewPassword, {
    message: "Passwords don't match",
    path: ["confirmNewPassword"]
});

const emailSchema = z.object({
    newEmail: z.string().email("Please enter a valid email address"),
    currentPassword: z.string().min(6, "Current password is required")
});

type PasswordFormValues = z.infer<typeof passwordSchema>;
type EmailFormValues = z.infer<typeof emailSchema>;

export default function Profile() {
    const user = useAppSelector(selectCurrentUser);
    const [activeTab, setActiveTab] = useState("account");
    const [changePassword] = useChangePasswordMutation();
    const [updateEmail] = useUpdateEmailMutation();

    const {
        register: registerPassword,
        handleSubmit: handlePasswordSubmit,
        formState: { errors: passwordErrors, isSubmitting: isPasswordSubmitting },
        reset: resetPasswordForm
    } = useForm<PasswordFormValues>({
        resolver: zodResolver(passwordSchema),
    });

    const {
        register: registerEmail,
        handleSubmit: handleEmailSubmit,
        formState: { errors: emailErrors, isSubmitting: isEmailSubmitting },
        reset: resetEmailForm
    } = useForm<EmailFormValues>({
        resolver: zodResolver(emailSchema),
    });

    const onPasswordSubmit = async (data: PasswordFormValues) => {
        try {
            await changePassword({
                id: user?.id!,
                request: {
                    currentPassword: data.currentPassword,
                    newPassword: data.newPassword,
                    confirmNewPassword: data.confirmNewPassword
                }
            }).unwrap();

            toast.success("Password updated successfully");
            resetPasswordForm();
        } catch (error) {
            toast.error("Failed to update password");
            console.error("Password change error:", error);
        }
    };

    const onEmailSubmit = async (data: EmailFormValues) => {
        try {
            await updateEmail({
                id: user?.id!,
                request: {
                    newEmail: data.newEmail,
                    currentPassword: data.currentPassword
                }
            }).unwrap();

            toast.success("Email updated successfully. Please check your inbox to confirm.");
            resetEmailForm();
        } catch (error) {
            toast.error("Failed to update email");
            console.error("Email update error:", error);
        }
    };

    if (!user) {
        return (
            <div className="flex justify-center items-center h-screen">
                <Loader2 className="h-8 w-8 text-green-600 dark:text-green-400 animate-spin" />
            </div>
        );
    }

    return (
        <div className="max-w-4xl mx-auto pt-8 px-4 sm:px-6 lg:px-8">
            <div className="bg-light-card dark:bg-dark-card rounded-lg shadow-md overflow-hidden">
                <div className="p-6 bg-gradient-to-r from-green-600 to-green-500 dark:from-green-500 dark:to-green-400">
                    <div className="flex items-center space-x-4">
                        <div className="bg-light-bg dark:bg-dark-bg p-3 rounded-full">
                            <User className="h-10 w-10 text-green-600 dark:text-green-400" />
                        </div>
                        <div>
                            <h1 className="text-xl font-bold text-white">
                                {user.firstName} {user.lastName}
                            </h1>
                            <p className="text-green-100">{user.email}</p>
                        </div>
                    </div>
                </div>

                <div className="border-b border-light-card dark:border-dark-card">
                    <div className="flex">
                        <button
                            onClick={() => setActiveTab("account")}
                            className={`px-4 py-3 text-sm font-medium ${activeTab === "account"
                                ? "border-b-2 border-green-600 dark:border-green-400 text-green-600 dark:text-green-400"
                                : "text-light-text dark:text-dark-text hover:text-green-600 dark:hover:text-green-400"
                                }`}
                        >
                            <div className="flex items-center space-x-2">
                                <User className="h-4 w-4" />
                                <span>Account</span>
                            </div>
                        </button>
                        <button
                            onClick={() => setActiveTab("security")}
                            className={`px-4 py-3 text-sm font-medium ${activeTab === "security"
                                ? "border-b-2 border-green-600 dark:border-green-400 text-green-600 dark:text-green-400"
                                : "text-light-text dark:text-dark-text hover:text-green-600 dark:hover:text-green-400"
                                }`}
                        >
                            <div className="flex items-center space-x-2">
                                <Shield className="h-4 w-4" />
                                <span>Security</span>
                            </div>
                        </button>
                    </div>
                </div>

                <div className="p-6">
                    {activeTab === "account" && (
                        <div className="space-y-6">
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                                <div>
                                    <h3 className="text-lg font-medium text-light-text dark:text-dark-text mb-2">
                                        Account Information
                                    </h3>
                                    <div className="space-y-4">
                                        <div className="bg-light-bg dark:bg-dark-bg p-4 rounded-md">
                                            <div className="flex justify-between">
                                                <div className="text-xs text-light-text dark:text-dark-text opacity-70">Name</div>
                                                <div className="text-sm font-medium text-light-text dark:text-dark-text">
                                                    {user.firstName} {user.lastName}
                                                </div>
                                            </div>
                                        </div>
                                        <div className="bg-light-bg dark:bg-dark-bg p-4 rounded-md">
                                            <div className="flex justify-between">
                                                <div className="text-xs text-light-text dark:text-dark-text opacity-70">Email</div>
                                                <div className="text-sm font-medium text-light-text dark:text-dark-text">
                                                    {user.email}
                                                </div>
                                            </div>
                                        </div>
                                        <div className="bg-light-bg dark:bg-dark-bg p-4 rounded-md">
                                            <div className="flex justify-between">
                                                <div className="text-xs text-light-text dark:text-dark-text opacity-70">Roles</div>
                                                <div className="text-sm font-medium text-light-text dark:text-dark-text">
                                                    {user.roles?.join(", ") || "User"}
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div>
                                    <h3 className="text-lg font-medium text-light-text dark:text-dark-text mb-2">
                                        Update Email
                                    </h3>
                                    <form onSubmit={handleEmailSubmit(onEmailSubmit)} className="space-y-4">
                                        <div>
                                            <label className="block text-sm font-medium text-light-text dark:text-dark-text mb-1">
                                                New Email Address
                                            </label>
                                            <div className="relative">
                                                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                                                    <Mail className="h-5 w-5 text-light-text dark:text-dark-text opacity-40" />
                                                </div>
                                                <input
                                                    {...registerEmail("newEmail")}
                                                    type="email"
                                                    className="pl-10 w-full px-4 py-2 bg-light-bg dark:bg-dark-bg border border-light-card dark:border-dark-card rounded-md focus:ring-2 focus:ring-green-500 dark:focus:ring-green-400"
                                                    placeholder="Enter new email address"
                                                />
                                            </div>
                                            {emailErrors.newEmail && (
                                                <p className="mt-1 text-xs text-red-600">{emailErrors.newEmail.message}</p>
                                            )}
                                        </div>
                                        <div>
                                            <label className="block text-sm font-medium text-light-text dark:text-dark-text mb-1">
                                                Current Password
                                            </label>
                                            <div className="relative">
                                                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                                                    <Key className="h-5 w-5 text-light-text dark:text-dark-text opacity-40" />
                                                </div>
                                                <input
                                                    {...registerEmail("currentPassword")}
                                                    type="password"
                                                    className="pl-10 w-full px-4 py-2 bg-light-bg dark:bg-dark-bg border border-light-card dark:border-dark-card rounded-md focus:ring-2 focus:ring-green-500 dark:focus:ring-green-400"
                                                    placeholder="Enter your current password"
                                                />
                                            </div>
                                            {emailErrors.currentPassword && (
                                                <p className="mt-1 text-xs text-red-600">{emailErrors.currentPassword.message}</p>
                                            )}
                                        </div>
                                        <button
                                            type="submit"
                                            disabled={isEmailSubmitting}
                                            className="w-full flex justify-center items-center space-x-2 px-4 py-2 bg-green-600 dark:bg-green-500 text-white rounded-md hover:bg-green-700 dark:hover:bg-green-400 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 disabled:opacity-50 disabled:cursor-not-allowed"
                                        >
                                            {isEmailSubmitting ? (
                                                <Loader2 className="h-5 w-5 animate-spin" />
                                            ) : (
                                                <>
                                                    <Save className="h-5 w-5" />
                                                    <span>Update Email</span>
                                                </>
                                            )}
                                        </button>
                                    </form>
                                </div>
                            </div>
                        </div>
                    )}


                </div>
            </div>
        </div>
    );
}
