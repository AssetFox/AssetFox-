export interface AnalysisDefaultData {
    weighting: string;
    optimizationStrategy: string;
    benefitAttribute: string;
    benefitLimit: number;
}

export interface InvestmentDefaultData {
    MinimumProjectCostLimit: number;
    InflationRatePercentage: number;    
}

export const emptyAnalysisDefaultData: AnalysisDefaultData = {
    weighting: '',
    optimizationStrategy: '',
    benefitAttribute: '',
    benefitLimit: 0
};

export const emptyInvestmentDefaultData: InvestmentDefaultData = {
    MinimumProjectCostLimit: 0,
    InflationRatePercentage: 0
};