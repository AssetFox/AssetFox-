import {AxiosPromise} from 'axios';
import {User} from '@/shared/models/iAM/user';
import {API, axiosInstance, coreAxiosInstance} from '@/shared/utils/axios-instance';

export default class UserService {
    static getOwnUserData(): AxiosPromise {
        return axiosInstance.get('/api/GetUserCriteria');
    }

    static getAllUsers(): AxiosPromise {
        return coreAxiosInstance.get(`${API.User}/GetAllUsers`);
    }

    static updateUser(userCriteria: User): AxiosPromise {
        return axiosInstance.post('/api/SetUserCriteria', userCriteria);
    }

    static deleteUser(username: string): AxiosPromise {
        return axiosInstance.delete(`/api/DeleteUser/${username}`);
    }
}
