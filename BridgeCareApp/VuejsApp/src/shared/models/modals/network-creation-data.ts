export interface NetworkCreationData {
    name: string;
    owner?: string;
    creator: string;
}

export const emptyCreateNetworkData: NetworkCreationData = {
    name: 'temp name',
    creator: ''
};
