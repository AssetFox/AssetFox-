import { UserInfo } from '@/shared/models/iAM/authentication';
import { parseLDAP } from './parse-ldap';
import authenticationModule from '@/store-modules/authentication.module';
import { SecurityTypes } from '@/shared/utils/security-types';

export const getUserInfo = () => {
    return JSON.parse(localStorage.getItem('UserInfo') as string) as UserInfo;
};

export const getUserName = () => {
    return authenticationModule.state.securityType === SecurityTypes.esec
        ? parseLDAP(getUserInfo().sub)[0]
        : (localStorage.getItem('LoggedInUser') as string);
};
