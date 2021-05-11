import {UserInfo} from '@/shared/models/iAM/authentication';
import {parseLDAP} from './parse-ldap';
import AuthenticationModule from '@/store-modules/authentication.module';

export const getUserInfo = () => {
    return JSON.parse(localStorage.getItem('UserInfo') as string) as UserInfo;
};

export const getUserName = () => {
    return AuthenticationModule.state.securityType === 'ESEC'
      ? parseLDAP(getUserInfo().sub)[0]
      : localStorage.getItem('LoggedInUser') as string;
};

export const getUserRoles = () => {
    return parseLDAP(getUserInfo().roles);
};