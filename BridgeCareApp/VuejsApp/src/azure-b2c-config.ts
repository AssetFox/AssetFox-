import { AuthenticationParameters, Configuration } from 'msal';

export const azureB2CConfig: any = {
  clientId: '1608915b-a49f-4a59-bcd1-9dc1cd48bdb6',
  tenantId: 'penndotbridgecare.onmicrosoft.com/B2C_1_SignupSignin',
  tenantName: 'penndotbridgecare.b2clogin.com',
  redirectUri: import.meta.env.VITE_APP_B2C_REDIRECT_URI as string,
  postLogoutRedirectUri: import.meta.env.VITE_APP_B2C_POST_LOGOUT_REDIRECT_URI as string,
  authority: 'https://penndotbridgecare.b2clogin.com/penndotbridgecare.onmicrosoft.com/B2C_1_SignupSignin',
  forgotPasswordAuthority: 'https://penndotbridgecare.b2clogin.com/penndotbridgecare.onmicrosoft.com/B2C_1_PasswordReset'
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
  scopes: ['https://penndotbridgecare.onmicrosoft.com/api2/read_data', 'https://penndotbridgecare.onmicrosoft.com/api2/write_data'],
  authority: azureB2CConfig.authority,
  redirectUri: azureB2CConfig.redirectUri,
};
