import { Outlet } from 'react-router-dom';
import Navbar from './Navbar';

const Layout = () => {
    return (
        <div className="min-h-screen bg-gray-900 text-white">
            <Navbar />
            <main className="pt-20">
                <Outlet />
            </main>
            <footer className="bg-gray-800 text-gray-400 py-8 mt-auto border-t border-gray-700">
                <div className="max-w-screen-xl mx-auto px-4 text-center">
                    <p>&copy; {new Date().getFullYear()} OpenSourceHub. Built for developers.</p>
                </div>
            </footer>
        </div>
    );
};

export default Layout;
