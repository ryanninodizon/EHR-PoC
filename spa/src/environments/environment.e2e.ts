export const environment = {
  production: false,
  msalConfig: {
    auth: {
      clientId: '<Client Id From EntraID>',
      authority: 'https://login.microsoftonline.com/<Tenant Id from EntraID>',
    },
  },
  apiConfig: {
    scopes: ['ENTER_SCOPE'],
    uri: 'ENTER_URI',
  },
};
