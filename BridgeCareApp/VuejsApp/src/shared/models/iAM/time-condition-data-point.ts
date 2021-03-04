import {getBlankGuid} from '@/shared/utils/uuid-utils';

export interface TimeConditionDataPoint {
    id: string;
    timeValue: number;
    conditionValue: number;
}

export const emptyTimeConditionDataPoint = {
    id: getBlankGuid(),
    timeValue: 0,
    conditionValue: 0
};
