export interface PerformanceCurve {
    id: string;
    attribute: string;
    equationName: string;
    equation: string;
    criteria: string;
    shift: boolean;
    piecewise: boolean;
    isFunction: boolean;
}

export interface PerformanceCurveLibrary {
    id: string;
    name: string;
    owner?: string;
    shared?: boolean;
    description: string;
    performanceCurves: PerformanceCurve[];
}

export const defaultPerformanceCurve: PerformanceCurve = {
    id: '0',
    attribute: '',
    equationName: '',
    equation: '',
    criteria: '',
    shift: false,
    piecewise: false,
    isFunction: false,
};

export const emptyPerformanceLibrary: PerformanceCurveLibrary = {
    id: '0',
    name: '',
    description: '',
    performanceCurves: []
};
