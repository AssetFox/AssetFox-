import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {Equation} from '@/shared/models/iAM/equation';

export default class EquationEditorService {
    /**
     * Checks an equation's validity
     * @param equationValidation Equation info to validate
     */
    // TODO: update parameter type to Equation DTO after .net core API setup
    static checkEquationValidity(equationValidation: any): AxiosPromise {
        return axiosInstance.post('/api/ValidateEquation', equationValidation);
    }
}
