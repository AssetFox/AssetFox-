import { WorkType } from './scenario';

export interface importCompletion{
    id: string;
    workType: WorkType;
    areBudgetsOverWritten: boolean;
}
