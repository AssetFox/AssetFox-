import axios, {AxiosInstance} from 'axios';

export const axiosInstance: AxiosInstance = axios.create({
    baseURL: process.env.VUE_APP_URL
});

export const nodejsAxiosInstance: AxiosInstance = axios.create({
    baseURL: process.env.VUE_APP_NODE_URL
});

export const coreAxiosInstance: AxiosInstance = axios.create({
    baseURL: process.env.VUE_APP_BRIDGECARE_CORE_URL
});

// The other nodejs axios instance has an interceptor
// which causes a spinner to be shown when it is being used,
// briefly blocking the page from being used. This instance does not
// have that spinner, and should be used for background tasks such
// as polling.
export const nodejsBackgroundAxiosInstance: AxiosInstance = axios.create({
    baseURL: process.env.VUE_APP_NODE_URL
});

export const API = {
    PerformanceCurve: '/api/PerformanceCurve',
    CriterionLibrary: '/api/CriterionLibrary',
    Network: '/api/Network',
    CashFlow: '/api/CashFlow',
    RemainingLifeLimit: '/api/RemainingLifeLimit',
    DeficientConditionGoal: '/api/DeficientConditionGoal',
    TargetConditionGoal: '/api/TargetConditionGoal',
    Treatment: '/api/Treatment',
    Investment: '/api/Investment',
    BudgetPriority: '/api/BudgetPriority',
    Authentication: '/api/Authentication',
    Scenario: '/api/Simulation',
    User: '/api/User',
    Attribute: '/api/Attribute',
    AnalysisMethod: '/api/AnalysisMethod',
    SimulationLog: '/api/SimulationLog',
    Report: '/api/Report',
    ExpressionValidation: '/api/ExpressionValidation',
    UserCriteria: '/api/UserCriteria',
    CommittedProject: '/api/CommittedProject',
    Announcement: '/api/Announcement',
    CalculatedAttributes: '/api/CalculatedAttributes',
    Aggregation: '/api/Aggregation',
    DataSource: '/api/DataSource',
    RawData: '/api/RawData',
};