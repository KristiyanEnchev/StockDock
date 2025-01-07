import { Suspense } from "react";
import LoadingSpinner from "@/components/common/LoadingSpinner";
import { ErrorBoundary } from "@/components/common/ErrorBoundary";

interface PageWrapperProps {
    component: React.LazyExoticComponent<() => JSX.Element>;
}

export default function PageWrapper({ component: Component }: PageWrapperProps) {
    return (
        <ErrorBoundary>
            <Suspense fallback={<LoadingSpinner size="large" fullScreen={true} variant="alternative" />}>
                <Component />
            </Suspense>
        </ErrorBoundary>
    );
}
