import {AxiosPromise} from 'axios';
import {User} from '@/shared/models/iAM/user';
import {API, axiosInstance, coreAxiosInstance} from '@/shared/utils/axios-instance';
import { UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';

export default class UserService {

    static getAllUsers(): AxiosPromise {
        return coreAxiosInstance.get(`${API.User}/GetAllUsers`);
    }

    // static updateUser(userCriteria: User): AxiosPromise {
    //     return axiosInstance.post('/api/SetUserCriteria', userCriteria);
    // }

    static deleteUser(userId: string): AxiosPromise {
        return coreAxiosInstance.delete(`/api/DeleteUser/${userId}`);
    }

    ////////////////////

    static getUserCriteriaFilterData(): AxiosPromise {
        return coreAxiosInstance.get('/api/GetUserCriteria');
    }

    static getAllUsersCriteriaFilterData(): AxiosPromise {
        return coreAxiosInstance.get(`api/GetAllUserCriteria`);
    }

    static updateUserCriteriaFilterData(userCriteria: UserCriteriaFilter): AxiosPromise {
        return coreAxiosInstance.post('/api/SetUserCriteria', userCriteria);
    }

    static revokeUserCriteriaFilterData(userCriteriaId: string): AxiosPromise {
        return coreAxiosInstance.delete(`/api/RevokeUserAccess/${userCriteriaId}`);
    }
    
}
