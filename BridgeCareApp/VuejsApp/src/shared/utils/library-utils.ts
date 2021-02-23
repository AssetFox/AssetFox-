import {find, isNil, reject} from 'ramda';
import {getBlankGuid} from '@/shared/utils/uuid-utils';

export const hasAppliedLibrary = (libraries: any[], scenarioId: string): boolean => {
    return !libraries.every((library: any) => (library.appliedScenarioIds as string[]).indexOf(scenarioId) === -1);
};

export const getAppliedLibrary = (libraries: any[], scenarioId: string): any => {
    return find((library: any) => library.appliedScenarioIds.indexOf(scenarioId) !== -1, libraries);
};

export const getAppliedLibraryId = (libraries: any[], scenarioId: string): string => {
    const library: any = getAppliedLibrary(libraries, scenarioId);
    return !isNil(library) ? library.id : getBlankGuid();
};

export const unapplyLibrary = (library: any, scenarioId: string): any => {
    return {
        ...library,
        appliedScenarioIds: reject((id: string) => id === scenarioId, library.appliedScenarioIds)
    };
};