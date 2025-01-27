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

        </div>
    );
}
