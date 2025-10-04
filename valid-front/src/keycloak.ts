import Keycloak from 'keycloak-js';

const keycloak = new Keycloak({
  url: import.meta.env.VITE_KEYCLOAK_URL,
  realm: import.meta.env.VITE_KEYCLOAK_REALM,
  clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID,
});

keycloak.onTokenExpired = () => {
  keycloak.updateToken(70)
    .then((refreshed) => {
      if (refreshed) {
        console.log('Token atualizado');
      }
    })
    .catch(() => {
      console.error('Falha ao atualizar token');
      keycloak.login();
    });
};

export default keycloak;
