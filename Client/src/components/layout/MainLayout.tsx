import { Outlet } from 'react-router-dom';
import { Navigation } from '../common/Navigation';
import { Footer } from '../common/Footer';

export function MainLayout() {
    return (
        <div className="min-h-screen flex flex-col bg-library-background dark:bg-library-dark-background">
            <Navigation />
            <main className="flex-grow pt-16">
                <Outlet />
            </main>
            <Footer />
        </div>
    );
}
