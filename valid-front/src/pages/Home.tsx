import { useAuth } from '../contexts/AuthContext';

const Home = () => {
  const { userInfo } = useAuth();

  return (
    <div>
      <h1>Bem-vindo ao Sistema</h1>
      {userInfo && (
        <p>Olá, {userInfo.preferred_username || userInfo.name || 'Usuário'}!</p>
      )}
    </div>
  );
};

export default Home;
