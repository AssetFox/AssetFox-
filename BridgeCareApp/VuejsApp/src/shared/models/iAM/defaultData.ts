import {OptimizationStrategy} from '@/shared/models/iAM/analysis-method';

export interface AnalysisDefaultData {
    Weighting: string;
    OptimizationStrategy: OptimizationStrategy;
    BenefitAttribute: string;
    BenefitLimit: number;
}

export interface InvestmentDefaultData {
    MinimumProjectCostLimit: number;
    InflationRatePercentage: number;    
}

export const emptyAnalysisDefaultData: AnalysisDefaultData = {
    Weighting: '',
    OptimizationStrategy: OptimizationStrategy.BenefitToCostRatio,
    BenefitAttribute: '',
    BenefitLimit: 0
};

export const emptyInvestmentDefaultData: InvestmentDefaultData = {
    MinimumProjectCostLimit: 0,
    InflationRatePercentage: 0
};