import { Link, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function Navbar() {
  const { user, isAuthenticated, logout } = useAuth();
  const location = useLocation();

  if (!isAuthenticated) return null;

  const linkClass = (path: string) =>
    `px-3 py-1.5 rounded-lg text-sm font-medium transition-colors ${
      location.pathname === path
        ? 'bg-white text-black'
        : 'text-zinc-400 hover:text-white hover:bg-zinc-800'
    }`;

  return (
    <nav className="border-b border-zinc-800 bg-zinc-950">
      <div className="max-w-5xl mx-auto px-6 h-14 flex items-center justify-between">
        <div className="flex items-center gap-6">
          <Link to="/dashboard" className="text-white font-bold text-lg tracking-tight">
            Speculo
          </Link>
          <div className="flex gap-1">
            <Link to="/dashboard" className={linkClass('/dashboard')}>Dashboard</Link>
            <Link to="/log" className={linkClass('/log')}>Log Event</Link>
          </div>
        </div>
        <div className="flex items-center gap-4">
          <span className="text-sm text-zinc-400">{user?.name || user?.email}</span>
          <button
            onClick={logout}
            className="text-sm text-zinc-500 hover:text-white transition-colors"
          >
            Logout
          </button>
        </div>
      </div>
    </nav>
  );
}
