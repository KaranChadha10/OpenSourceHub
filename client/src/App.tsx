import { BrowserRouter, Route, Routes } from 'react-router-dom';
import Layout from './components/Layout';
import Home from './pages/Home';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<Home />} />
          {/* Placeholder routes for now */}
          <Route path="issues" element={<div className="p-10 text-center">Issues Page (Coming Soon)</div>} />
          <Route path="repos" element={<div className="p-10 text-center">Repositories Page (Coming Soon)</div>} />
          <Route path="login" element={<div className="p-10 text-center">Login Page (Coming Soon)</div>} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
