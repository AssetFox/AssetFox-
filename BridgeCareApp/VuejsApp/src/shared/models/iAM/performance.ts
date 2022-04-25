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
    criterionLibrary: CriterionLibrary;
    equation: Equation;
}

export interface PerformanceCurveLibrary {
    id: string;
    name: string;
    description: string;
    performanceCurves: PerformanceCurve[];
    appliedScenarioIds: string[];
    owner?: string;
    isShared: boolean;
}

export const emptyPerformanceCurve: PerformanceCurve = {
    id: getBlankGuid(),
    attribute: '',
    name: '',
    shift: false,
    equation: clone(emptyEquation),
    criterionLibrary: clone(emptyCriterionLibrary),
};

export const emptyPerformanceCurveLibrary: PerformanceCurveLibrary = {
    id: getBlankGuid(),
    name: '',
    description: '',
    performanceCurves: [],
    appliedScenarioIds: [],
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
