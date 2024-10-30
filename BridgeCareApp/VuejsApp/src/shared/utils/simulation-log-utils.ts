import ReportsService from '@/services/reports.service';
import FileDownload from 'js-file-download';
import { AxiosResponse } from 'axios';
import { hasValue } from '@/shared/utils/has-value-util';

export async function downloadSimulationLog(
    networkId: string,
    scenarioId: string,
    simulationName: string,
    addSuccessNotificationAction: Function,
    addErrorNotificationAction: Function,
) {
    try {
        const response: AxiosResponse<any> = await ReportsService.downloadSimulationLog(
            networkId,
            scenarioId,
        );

        if (hasValue(response, 'data')) {
            addSuccessNotificationAction({ message: 'Simulation Log downloaded' });
            FileDownload(response.data, `Simulation Log ${simulationName}.txt`);
        } else {
            throw new Error('No data received');
        }
    } catch (error) {
        console.error('Error downloading simulation log:', error);
        addErrorNotificationAction({
            message: 'Failed to download simulation log.',
            longMessage:
                'Failed to download simulation log. Please try generating and downloading the log again.',
        });
    }
}
