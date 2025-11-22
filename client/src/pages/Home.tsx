import { Code2, GitPullRequest, Search } from 'lucide-react';
import { Link } from 'react-router-dom';

const Home = () => {
    return (
        <div className="space-y-20 pb-20">
            {/* Hero Section */}
            <section className="relative overflow-hidden pt-20 pb-32">
                <div className="absolute top-0 left-1/2 -translate-x-1/2 w-[1000px] h-[500px] bg-blue-600/20 rounded-full blur-3xl -z-10" />
                <div className="max-w-screen-xl mx-auto px-4 text-center">
                    <h1 className="text-5xl md:text-7xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-blue-400 to-purple-600 mb-6">
                        Discover Your Next <br /> Open Source Contribution
                    </h1>
                    <p className="text-xl text-gray-400 mb-10 max-w-2xl mx-auto">
                        Find the perfect issues to work on based on your skills and interests.
                        Filter by language, difficulty, and more.
                    </p>
                    <div className="flex flex-col sm:flex-row justify-center gap-4">
                        <Link
                            to="/issues"
                            className="inline-flex items-center justify-center px-8 py-4 text-lg font-medium text-white bg-blue-600 rounded-full hover:bg-blue-700 transition-colors"
                        >
                            <Search className="w-5 h-5 mr-2" />
                            Find Issues
                        </Link>
                        <Link
                            to="/repos"
                            className="inline-flex items-center justify-center px-8 py-4 text-lg font-medium text-gray-300 bg-gray-800 border border-gray-700 rounded-full hover:bg-gray-700 transition-colors"
                        >
                            Browse Repos
                        </Link>
                    </div>
                </div>
            </section>

            {/* Features Section */}
            <section className="max-w-screen-xl mx-auto px-4">
                <div className="grid md:grid-cols-3 gap-8">
                    <div className="p-8 rounded-2xl bg-gray-800/50 border border-gray-700 backdrop-blur-sm">
                        <div className="w-12 h-12 bg-blue-900/50 rounded-lg flex items-center justify-center mb-6">
                            <Search className="w-6 h-6 text-blue-400" />
                        </div>
                        <h3 className="text-xl font-bold mb-3">Smart Discovery</h3>
                        <p className="text-gray-400">
                            Advanced filtering to find issues that match your tech stack and experience level.
                        </p>
                    </div>
                    <div className="p-8 rounded-2xl bg-gray-800/50 border border-gray-700 backdrop-blur-sm">
                        <div className="w-12 h-12 bg-purple-900/50 rounded-lg flex items-center justify-center mb-6">
                            <Code2 className="w-6 h-6 text-purple-400" />
                        </div>
                        <h3 className="text-xl font-bold mb-3">Language Focused</h3>
                        <p className="text-gray-400">
                            Filter by your favorite programming languages and find relevant repositories.
                        </p>
                    </div>
                    <div className="p-8 rounded-2xl bg-gray-800/50 border border-gray-700 backdrop-blur-sm">
                        <div className="w-12 h-12 bg-green-900/50 rounded-lg flex items-center justify-center mb-6">
                            <GitPullRequest className="w-6 h-6 text-green-400" />
                        </div>
                        <h3 className="text-xl font-bold mb-3">Track Contributions</h3>
                        <p className="text-gray-400">
                            Keep track of your pull requests and contributions across different projects.
                        </p>
                    </div>
                </div>
            </section>
        </div>
    );
};

export default Home;
