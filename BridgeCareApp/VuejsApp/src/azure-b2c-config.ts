import { AuthenticationParameters, Configuration } from 'msal';

export const azureB2CConfig: any = {
  clientId: '8c7f8f66-30ce-47db-ab02-13a10f98c865',
  tenantId: 'aratranstest.onmicrosoft.com/B2C_1_SU-SI-POL',
  tenantName: 'aratranstest.b2clogin.com/tfp',
  redirectUri: import.meta.env.VITE_APP_B2C_REDIRECT_URI as string,
  postLogoutRedirectUri: import.meta.env.VITE_APP_B2C_POST_LOGOUT_REDIRECT_URI as string,
  authority: 'https://aratranstest.b2clogin.com/tfp/aratranstest.onmicrosoft.com/B2C_1_SU-SI-POL',
  forgotPasswordAuthority: 'https://aratranstest.b2clogin.com/tfp/aratranstest.onmicrosoft.com/B2C_1_PR-POL'
};

export const msalConfig: Configuration = {
  auth: {
    clientId: azureB2CConfig.clientId,
    authority: azureB2CConfig.authority,
    redirectUri: azureB2CConfig.redirectUri,
    postLogoutRedirectUri: azureB2CConfig.postLogoutRedirectUri,
    validateAuthority: false
  },
  cache: {
    cacheLocation: 'localStorage'
  }
};

export const msalPasswordResetConfig: Configuration = {
  auth: {
    clientId: azureB2CConfig.clientId,
    authority: azureB2CConfig.forgotPasswordAuthority,
    redirectUri: azureB2CConfig.redirectUri,
    postLogoutRedirectUri: azureB2CConfig.postLogoutRedirectUri,
    validateAuthority: false
  },
  cache: {
    cacheLocation: 'localStorage'
  }
};

export const acquireTokenConfig: AuthenticationParameters = {
  scopes: ['https://aratranstest.onmicrosoft.com/api/read_data', 'https://aratranstest.onmicrosoft.com/api/write_data'],
  authority: azureB2CConfig.authority,
  redirectUri: azureB2CConfig.redirectUri,
};
