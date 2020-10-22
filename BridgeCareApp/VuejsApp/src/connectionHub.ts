import { HubConnectionBuilder, LogLevel, HttpTransportType } from '@microsoft/signalr';
import { bridgecareCoreAxiosInstance } from './shared/utils/axios-instance';

export default {
    install(Vue: any) {
        const connection = new HubConnectionBuilder()
            .withUrl(`http://localhost:64469/bridgecarehub/`, {
                //skipNegotiation: false,
                transport: HttpTransportType.LongPolling
            })
            .configureLogging(LogLevel.Information)
            .build();

            const statusHub = new Vue();
            Vue.prototype.$statusHub = statusHub;

            connection.on('BroadcastAssignDataStatus', (status) => {
                statusHub.$emit('assignedData-status-event', {status});
            });

        let startedPromise = null;
        function start() {
            startedPromise = connection.start().catch(err => {
                console.error('Failed to connect with hub', err);
                return new Promise((resolve, reject) =>
                    setTimeout(() => start().then(resolve).catch(reject), 5000));
            });
            return startedPromise;
        }
        connection.onclose(() => start());

        start();
    }
};