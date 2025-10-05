import { Link, Outlet } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import './Layout.css';

const Layout = () => {
  const { logout } = useAuth();

  return (
    <div className="layout">
      <nav className="navbar">
        <div className="nav-links">
          <Link to="/">Home</Link>
          <Link to="/cliente">Cliente</Link>
          <Link to="/pedido">Pedido</Link>
        </div>
        <button className="logout-button" onClick={logout}>
          Sair
        </button>
      </nav>
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
};

export default Layout;
