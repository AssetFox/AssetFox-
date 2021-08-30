import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';

export default class InvestmentDefaultDataService {
    static getInvestmentDefaultData(): AxiosPromise {
        return coreAxiosInstance.get(`${API.InvestmentDefaultData}/GetInvestmentDefaultData`);
    }
}
