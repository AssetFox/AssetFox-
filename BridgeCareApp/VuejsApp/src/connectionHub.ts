import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr'
import { bridgecareCoreAxiosInstance } from './shared/utils/axios-instance';

export default {
    install(Vue: any) {
        const connection = new HubConnectionBuilder()
            .withUrl(`${bridgecareCoreAxiosInstance}/bridgecarehub`)
            .configureLogging(LogLevel.Information)
            .build();

        let startedPromise = null
        function start() {
            startedPromise = connection.start().catch(err => {
                console.error('Failed to connect with hub', err)
                return new Promise((resolve, reject) =>
                    setTimeout(() => start().then(resolve).catch(reject), 5000))
            })
            return startedPromise;
        };
        connection.onclose(() => start());

        start();
    }
}