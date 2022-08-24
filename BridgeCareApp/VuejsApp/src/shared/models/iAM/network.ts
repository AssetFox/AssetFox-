import {getBlankGuid} from '@/shared/utils/uuid-utils';
import { emptyEquation, Equation } from '@/shared/models/iAM/equation';
import {clone} from 'ramda';

export interface BenefitQuantifier {
    networkId: string;
    equation: Equation;
}

export const emptyBenefitQuantifier: BenefitQuantifier = {
    networkId: getBlankGuid(),
    equation: clone(emptyEquation)
};

export interface Network {
    id: string;
    name: string;
    createdDate?: Date;
    lastModifiedDate?: Date;
    status?: string;
    benefitQuantifier: BenefitQuantifier;
    networkDataAssignmentPercentage: number;
    KeyAttribute: string;
}

export const emptyNetwork: Network = {
    id: getBlankGuid(),
    name: '',
    createdDate: new Date(),
    lastModifiedDate: new Date(),
    status: '',
    benefitQuantifier: clone(emptyBenefitQuantifier),
    networkDataAssignmentPercentage: 0,
    KeyAttribute: getBlankGuid()
};

export interface NetworkCreationData {
    name: string;
    owner?: string;
    creator: string;
}

export const emptyCreateNetworkData: NetworkCreationData = {
    name: '',
    creator: ''
};
