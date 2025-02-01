export const environment = {
  production: false,
  msalConfig: {
    auth: {
      clientId: '<Client Id From EntraID>',
      authority: 'https://login.microsoftonline.com/<Tenant Id from EntraID>',
    },
  },
  apiConfig: {
    scopes: ['user.read'],
    uri: 'https://graph.microsoft.com/v1.0/me',
  },
};
