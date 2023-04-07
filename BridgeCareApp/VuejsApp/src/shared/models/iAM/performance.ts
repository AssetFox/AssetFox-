import { emptyEquation, Equation } from '@/shared/models/iAM/equation';
import {
    CriterionLibrary,
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { clone } from 'ramda';
import { getBlankGuid } from '@/shared/utils/uuid-utils';

export interface PerformanceCurve {
    id: string;
    attribute: string;
    name: string;
    shift: boolean;
    libraryId: string;
    isModified: boolean;
    criterionLibrary: CriterionLibrary;
    equation: Equation;
}

export interface PerformanceCurveLibrary {
    id: string;
    name: string;
    description: string;
    performanceCurves: PerformanceCurve[];
    users: PerformanceCurveLibraryUser[];
    appliedScenarioIds: string[];
    owner?: string;
    isShared: boolean;
}
export interface PerformanceCurveLibraryUser {
    userId: string;
    username: string;
    canModify: boolean;
    isOwner: boolean;
}
export const emptyPerformanceCurve: PerformanceCurve = {
    id: getBlankGuid(),
    attribute: '',
    name: '',
    shift: false,
    libraryId: getBlankGuid(),
    isModified: false,
    equation: clone(emptyEquation),
    criterionLibrary: clone(emptyCriterionLibrary),
};

export const emptyPerformanceCurveLibraryUsers: PerformanceCurveLibraryUser[] = [{
    userId: '',
    username: '',
    canModify: false,
    isOwner: false
}];

export const emptyPerformanceCurveLibrary: PerformanceCurveLibrary = {
    id: getBlankGuid(),
    name: '',
    description: '',
    performanceCurves: [],
    appliedScenarioIds: [],
    users: [],
    isShared: false
};

export interface PerformanceCurvesFileImport {
    file: File;
}

export interface LibraryPerformanceCurvesFileImport
    extends PerformanceCurvesFileImport {
    libraryId: string;
}

export interface ScenarioPerformanceCurvesFileImport
    extends PerformanceCurvesFileImport {
    scenarioId: string;
}
