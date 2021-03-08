import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface Network {
    id: string;
    name: string;
    createdDate?: Date;
    lastModifiedDate?: Date;
    assignmentStatus?: string;
    networkId: number;
}

export const emptyNetwork: Network = {
    id: getBlankGuid(),
    name: '',
    createdDate: new Date(),
    lastModifiedDate: new Date(),
    assignmentStatus: '',
    networkId: 0
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
