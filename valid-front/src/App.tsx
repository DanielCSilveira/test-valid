import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { useAuth } from './contexts/AuthContext';
import Login from './components/Login';
import Layout from './components/Layout';
import Home from './pages/Home';
import Cliente from './pages/Cliente';
import ClienteForm from './pages/ClienteForm';
import Pedido from './pages/Pedido';
import PedidoDetail from './pages/PedidoDetail';
import PedidoNew from './pages/PedidoNew';
import './App.css';

function App() {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div style={{ 
        display: 'flex', 
        justifyContent: 'center', 
        alignItems: 'center', 
        height: '100vh' 
      }}>
        <p>Carregando...</p>
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Login />;
  }

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<Home />} />
          <Route path="cliente" element={<Cliente />} />
          <Route path="cliente/new" element={<ClienteForm />} />
          <Route path="cliente/:id" element={<ClienteForm />} />
          <Route path="pedido" element={<Pedido />} />
          <Route path="pedido/new" element={<PedidoNew />} />
          <Route path="pedido/detail/:id" element={<PedidoDetail />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
