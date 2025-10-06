import { createContext, useContext, useEffect, useState, type ReactNode } from 'react';
import keycloak from '../keycloak';

interface AuthContextType {
  isAuthenticated: boolean;
  isLoading: boolean;
  login: () => void;
  logout: () => void;
  token: string | undefined;
  userInfo: any;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

let keycloakInitialized = false;

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [userInfo, setUserInfo] = useState<any>(null);

  useEffect(() => {
    if (keycloakInitialized) {
      setIsAuthenticated(keycloak.authenticated || false);
      setIsLoading(false);
      if (keycloak.authenticated) {
        keycloak.loadUserInfo().then((info) => {
          setUserInfo(info);
        });
      }
      return;
    }

    keycloakInitialized = true;

    keycloak
      .init({
        onLoad: 'check-sso',
        checkLoginIframe: false,       // desabilita o iframe / silent-check-sso.html
        redirectUri: window.location.origin,
      })
      .then((authenticated) => {
        setIsAuthenticated(authenticated);
        if (authenticated && keycloak.tokenParsed) {
          keycloak.loadUserInfo().then((info) => {
            setUserInfo(info);
          });
        }
        setIsLoading(false);
      })
      .catch((error) => {
        console.error('Keycloak initialization failed:', error);
        setIsLoading(false);
      });
  }, []);

  const login = () => {
    keycloak.login();
  };

  const logout = () => {
    keycloak.logout();
  };

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated,
        isLoading,
        login,
        logout,
        token: keycloak.token,
        userInfo,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
};
