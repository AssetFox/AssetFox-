export interface NewNetwork {
    id: string;
    name: string;
    createdDate?: Date;
    lastModifiedDate?: Date;
    assignmentStatus?: string;
}

export const emptyNewNetwork: NewNetwork = {
    id: '',
    name: '',
    createdDate: new Date(),
    lastModifiedDate: new Date(),
    assignmentStatus: 'dummy assignment',
};

export interface NetworkCreationData {
    name: string;
    owner?: string;
    creator: string;
}

export const emptyCreateNetworkData: NetworkCreationData = {
    name: 'temp name',
    creator: ''
};