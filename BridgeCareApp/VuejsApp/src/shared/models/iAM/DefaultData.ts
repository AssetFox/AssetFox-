import {OptimizationStrategy} from '@/shared/models/iAM/analysis-method';

export interface AnalysisDefaultData {
    weighting: string;
    optimizationStrategy: OptimizationStrategy;
    benefitAttribute: string;
    benefitLimit: number;
}

export interface InvestmentDefaultData {
    MinimumProjectCostLimit: number;
    InflationRatePercentage: number;    
}

export const emptyAnalysisDefaultData: AnalysisDefaultData = {
    weighting: '',
    optimizationStrategy: OptimizationStrategy.BenefitToCostRatio,
    benefitAttribute: '',
    benefitLimit: 0
};

export const emptyInvestmentDefaultData: InvestmentDefaultData = {
    MinimumProjectCostLimit: 0,
    InflationRatePercentage: 0
};