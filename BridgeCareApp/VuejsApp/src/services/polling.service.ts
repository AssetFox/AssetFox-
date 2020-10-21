import {AxiosPromise} from 'axios';
import {nodejsBackgroundAxiosInstance, bridgecareCoreAxiosInstance} from '@/shared/utils/axios-instance';

export default class PollingService {
    /**
     * Gets the list of emitted events
     */
    static pollEvents(sessionId: string): AxiosPromise {
        return nodejsBackgroundAxiosInstance.get(`/api/Polling/${sessionId}`);
    }

    static getRealTimeData(): AxiosPromise {
        return bridgecareCoreAxiosInstance.get(`api/StatusHub/GetStatus/`);
    }
}