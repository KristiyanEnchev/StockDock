import { Outlet } from 'react-router-dom';
import { Navigation } from '../common/Navigation';

export function MainLayout() {
    return (
        <div className="min-h-screen flex flex-col bg-library-background dark:bg-library-dark-background">
            <Navigation />
            <main className="flex-grow pt-20">
                <Outlet />
            </main>
        </div>
    );
}
