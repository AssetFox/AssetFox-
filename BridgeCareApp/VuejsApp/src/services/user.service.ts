import {AxiosPromise} from 'axios';
import {User, UserNewsAccessDate} from '@/shared/models/iAM/user';
import {API, axiosInstance, coreAxiosInstance} from '@/shared/utils/axios-instance';
import { UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';

export default class UserService {

    static getAllUsers(): AxiosPromise {
        return coreAxiosInstance.get(`${API.User}/GetAllUsers`);
    }

    static getUserByUserName(userName: string): AxiosPromise {
        return coreAxiosInstance.get(`${API.User}/GetUserByUserName/${userName}`);
    }

    static getUserById(id: string): AxiosPromise {
        return coreAxiosInstance.get(`${API.User}/GetUserById/${id}`);
    }

    static updateLastNewsAccessDate(data: UserNewsAccessDate): AxiosPromise {
        return coreAxiosInstance.post(`${API.User}/UpdateLastNewsAccessDate/`, data);
    }

    // static updateUser(userCriteria: User): AxiosPromise {
    //     return axiosInstance.post('/api/SetUserCriteria', userCriteria);
    // }

    static deleteUser(userId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.UserCriteria}/DeleteUser/${userId}`);
    }

    ////////////////////

    static getUserCriteriaFilterData(): AxiosPromise {
        return coreAxiosInstance.get(`${API.UserCriteria}/GetUserCriteria`);
    }

    static getAllUsersCriteriaFilterData(): AxiosPromise {
        return coreAxiosInstance.get(`${API.UserCriteria}/GetAllUserCriteria`);
    }

    static updateUserCriteriaFilterData(userCriteria: UserCriteriaFilter): AxiosPromise {
        return coreAxiosInstance.post(`${API.UserCriteria}/UpsertUserCriteria`, userCriteria);
    }

    static revokeUserCriteriaFilterData(userCriteriaId: string): AxiosPromise {
        return coreAxiosInstance.delete(`${API.UserCriteria}/RevokeUserAccess/${userCriteriaId}`);
    }
    
}
