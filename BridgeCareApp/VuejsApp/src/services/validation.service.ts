import {EquationValidationParameters, ValidationParameter} from '@/shared/models/iAM/expression-validation';
import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';

export default class ValidationService {
    static getEquationValidationResult(equationValidationParameters: EquationValidationParameters): AxiosPromise {
        return coreAxiosInstance.post(`${API.ExpressionValidation}/GetEquationValidationResult`, equationValidationParameters);
    }

    static getCriterionValidationResult(validationParameter: ValidationParameter): AxiosPromise {
        return coreAxiosInstance.post(`${API.ExpressionValidation}/GetCriterionValidationResult`, validationParameter);
    }

    static getCriterionValidationResultNoCount(validationParameter: ValidationParameter): AxiosPromise {
        return coreAxiosInstance.post(`${API.ExpressionValidation}/GetCriterionValidationResultNoCount`, validationParameter);
    }
}