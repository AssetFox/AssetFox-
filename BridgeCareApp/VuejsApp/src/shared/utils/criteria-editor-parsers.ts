import { clone, isEmpty, isNil } from 'ramda';
import {
    Criteria,
    CriteriaRule,
    CriteriaType,
    emptyCriteria,
} from '@/shared/models/iAM/criteria';
import { hasValue } from '@/shared/utils/has-value-util';

const operators: string[] = ['<=', '>=', '<>', '=', '<', '>'];

const invalidCharRegex: RegExp = /\s|\(|\)/g;

const queryBuilderTypes: any = {
    QueryBuilderRule: 'query-builder-rule',
    QueryBuilderGroup: 'query-builder-group',
};

/**
 * Creates a criteria expression from a given Criteria object
 */
export const convertCriteriaObjectToCriteriaExpression = (
    criteria: Criteria,
    recursiveCallNumber: number = 0,
): string[] | null => {
    // create an empty string list to build the where clause
    const clause: string[] = [];
    try {
        if (!isNil(criteria) && !isEmpty(criteria.children)) {
            // set the logical operator
            const logicalOperator = ` ${criteria.logicalOperator} `;
            // @ts-ignore
            // loop over the criteria children and append all rules
            criteria.children.forEach((child: CriteriaType) => {
                if (child.type === queryBuilderTypes.QueryBuilderRule) {
                    // create a clause rule from the query builder child.query
                    const rule = createCriteriaExpression(
                        child.query as CriteriaRule,
                    );
                    if (hasValue(rule)) {
                        // append the logical operator if the string list is not empty
                        if (hasValue(clause)) {
                            clause.push(logicalOperator);
                        }
                        // append rule to string list
                        clause.push(rule);
                    }
                } else {
                    const clauseGroup = convertCriteriaObjectToCriteriaExpression(
                        child.query as Criteria,
                        recursiveCallNumber++,
                    );
                    if (clauseGroup && hasValue(clauseGroup.join(''))) {
                        // append the logical operator if the string list is not empty
                        if (hasValue(clause)) {
                            clause.push(logicalOperator);
                        }

                        // create a rule group
                        if (recursiveCallNumber === 0) {
                            clause.push(...clauseGroup);
                        } else {
                            clause.push('(');
                            clause.push(...clauseGroup);
                            clause.push(')');
                        }
                    }
                }
            });
        }
    } catch (e) {
        return null;
    }
    return clause;
};

/**
 * Creates a criteria expression string from a given CriteriaType object
 */
export const convertCriteriaTypeObjectToCriteriaExpression = (
    criteriaType: CriteriaType,
): string => {
    let clause: string = '';
    if (!isNil(criteriaType)) {
        if (criteriaType.type === queryBuilderTypes.QueryBuilderRule) {
            const rule = createCriteriaExpression(
                criteriaType.query as CriteriaRule,
            );
            if (hasValue(rule)) {
                clause = rule;
            }
        } else {
            const ruleGroup = convertCriteriaObjectToCriteriaExpression(
                criteriaType.query as Criteria,
            );
            if (ruleGroup && hasValue(ruleGroup.join(''))) {
                clause = ruleGroup.join('');
            }
        }
    }
    return clause;
};

/**
 * Creates a criteria expression from a given CriteriaRule object
 */
function createCriteriaExpression(criteriaRule: CriteriaRule): string {
    // return the concatenated rule string
    if (
        typeof criteriaRule.value != 'undefined' &&
        hasValue(criteriaRule.value)
    ) {
        if (criteriaRule.value[0] != '[') {
            return `[${criteriaRule.selectedOperand}]${criteriaRule.selectedOperator}'${criteriaRule.value}'`;
        } else {
            return `[${criteriaRule.selectedOperand}]${criteriaRule.selectedOperator}${criteriaRule.value}`;
        }
    } else {
        return '';
    }
}

/**
 * Creates a CriteriaRule object from a given criteria expression
 */
function createCriteriaRuleObject(clause: string): CriteriaRule {
    let operator: string = '';

    for (
        let operatorIndex = 0;
        operatorIndex < operators.length;
        operatorIndex++
    ) {
        if (clause.indexOf(operators[operatorIndex]) !== -1) {
            operator = operators[operatorIndex];
            break;
        }
    }

    if (!hasValue(operator)) {
        throw new Error('The criteria expression is invalid.');
    }

    const operandAndValue: string[] = clause.split(operator);
    operandAndValue[0] = operandAndValue[0]
        .replace(/\[/g, '')
        .replace(/]/g, '');
    operandAndValue[1] = operandAndValue[1]
        .replace(/'/g, '')
        .replace(/\|/g, '');

    return {
        rule: operandAndValue[0],
        selectedOperator: operator,
        selectedOperand: operandAndValue[0],
        value: operandAndValue[1],
    } as CriteriaRule;
}

/**
 * Converts a string into a Criteria object
 * @param expression The string to convert
 * @param criteria The Criteria object to use for property setting
 */
function createCriteriaObject(
    expression: string,
    criteria: Criteria,
): Criteria {
    if (
        (expression.match(/\(/g) || []).length !==
        (expression.match(/\)/g) || []).length
    ) {
        throw new Error(
            'The criteria expression has mismatching numbers of open/close parentheses.',
        );
    }

    let currentClause: string = '';
    let currentCharIndex: number = 0;
    let currentOpenParentheses: string[] = [];
    let currentCloseParentheses: string[] = [];

    while (currentCharIndex < expression.length) {
        if (expression[currentCharIndex] === '(') {
            if (hasValue(currentClause)) {
                criteria.children!.push(
                    createCriteriaTypeObject(
                        queryBuilderTypes.QueryBuilderRule,
                        createCriteriaRuleObject(currentClause),
                    ),
                );
                currentClause = '';
            }

            let subCharIndex: number = currentCharIndex;
            do {
                if (expression[subCharIndex] === '(') {
                    currentOpenParentheses.push('(');
                }

                if (expression[subCharIndex] === ')') {
                    currentCloseParentheses.push(')');
                }

                subCharIndex++;
            } while (
                currentOpenParentheses.length !== currentCloseParentheses.length
            );

            criteria.children!.push(
                createCriteriaTypeObject(
                    queryBuilderTypes.QueryBuilderGroup,
                    createCriteriaObject(
                        expression.substring(
                            currentCharIndex + 1,
                            subCharIndex - 1,
                        ),
                        clone(emptyCriteria),
                    ),
                ),
            );
            //}

            currentOpenParentheses = [];
            currentCloseParentheses = [];

            currentCharIndex = subCharIndex;
        } else if (expression[currentCharIndex] === ' ') {
            if (
                expression.substring(currentCharIndex, currentCharIndex + 5) ===
                    ' AND ' ||
                expression.substring(currentCharIndex, currentCharIndex + 4) ===
                    ' OR '
            ) {
                if (hasValue(currentClause)) {
                    criteria.children!.push(
                        createCriteriaTypeObject(
                            queryBuilderTypes.QueryBuilderRule,
                            createCriteriaRuleObject(currentClause),
                        ),
                    );
                    currentClause = '';
                }

                if (
                    expression.substring(
                        currentCharIndex,
                        currentCharIndex + 5,
                    ) === ' AND '
                ) {
                    if (!hasValue(criteria.logicalOperator)) {
                        criteria.logicalOperator = 'AND';
                    }
                    currentCharIndex = currentCharIndex + 5;
                } else {
                    if (!hasValue(criteria.logicalOperator)) {
                        criteria.logicalOperator = 'OR';
                    }
                    currentCharIndex = currentCharIndex + 4;
                }
            }
        } else {
            if (!invalidCharRegex.test(expression[currentCharIndex])) {
                currentClause = `${currentClause}${expression[currentCharIndex]}`;
            }

            currentCharIndex++;

            if (
                hasValue(currentClause) &&
                currentCharIndex === expression.length
            ) {
                criteria.children!.push(
                    createCriteriaTypeObject(
                        queryBuilderTypes.QueryBuilderRule,
                        createCriteriaRuleObject(currentClause),
                    ),
                );
                currentClause = '';
            }
        }
    }

    if (
        !hasValue(criteria.logicalOperator) &&
        criteria.children!.length === 1 &&
        criteria.children![0].type === queryBuilderTypes.QueryBuilderGroup
    ) {
        do {
            criteria = criteria.children![0].query as Criteria;
        } while (
            !hasValue(criteria.logicalOperator) &&
            criteria.children!.length === 1 &&
            criteria.children![0].type === queryBuilderTypes.QueryBuilderGroup
        );
    } else if (
        !hasValue(criteria.logicalOperator) &&
        criteria.children!.length > 1
    ) {
        throw new Error('The criteria expression is malformed.');
    }

    return criteria;
}

/**
 * Creates a CriteriaType object of the specified type for the given CriteriaRule object or Criteria object
 */
function createCriteriaTypeObject(
    type: string,
    query: CriteriaRule | Criteria,
): CriteriaType {
    return {
        type: type,
        query: query,
    } as CriteriaType;
}

/**
 * Converts a criteria expression into its Criteria object equivalent
 */
export const convertCriteriaExpressionToCriteriaObject = (
    expression: string,
) => {
    try {
        if (hasValue(expression)) {
            const trimmedExpression: string = expression.trim();
            return createCriteriaObject(
                trimmedExpression,
                clone(emptyCriteria),
            );
        }
    } catch (e) {
        return null;
    }

    return { ...emptyCriteria };
};
