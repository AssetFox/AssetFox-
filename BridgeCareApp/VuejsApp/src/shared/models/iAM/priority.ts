export interface PriorityFund {
    id: string;
    budget: string;
    funding: number;
}

export interface Priority {
    id: string;
    priorityLevel: number;
    year: number | null;
    criteria: string;
}

export interface ScenarioPriority extends Priority {
    priorityFunds: PriorityFund[];
}

export interface PriorityLibrary {
    id: string;
    name: string;
    owner?: string;
    shared?: boolean;
    description: string;
    priorities: Priority[] | ScenarioPriority[];
}

export const emptyPriority: Priority = {
    id: '0',
    priorityLevel: 1,
    year: null,
    criteria: ''
};

export const emptyPriorityLibrary: PriorityLibrary = {
    id: '0',
    name: '',
    description: '',
    priorities: []
};

export interface PrioritiesDataTableRow {
    id: string;
    priorityLevel: string;
    year: string;
    criteria: string;
    [budgetName: string]: string;
}
