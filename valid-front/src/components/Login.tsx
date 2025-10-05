import { useAuth } from '../contexts/AuthContext';
import './Login.css';

const Login = () => {
  const { login } = useAuth();

  return (
    <div className="login-container">
      <div className="login-card">
        <h1>Bem-vindo</h1>
        <p>Faça login para acessar o sistema</p>
        <button className="login-button" onClick={login}>
          Entrar com Keycloak
        </button>
      </div>
    </div>
  );
};

export default Login;
