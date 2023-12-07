export interface RuleOperator {
    name: string;
    identifier: string;
}

export interface Rule {
    name: string;
    identifier: string;
    type: string;
    operators: string[];
}

export interface QueryBuilderConfig {
    levelOperators: RuleOperator[];
    ruleOperators: RuleOperator[];
    rules: Rule[];
}