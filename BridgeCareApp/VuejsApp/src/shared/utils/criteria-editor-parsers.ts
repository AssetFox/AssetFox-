import { clone, forEach, isEmpty, isNil } from 'ramda';
import {
    Criteria,
    CriteriaRule,
    CriteriaType,
    emptyCriteria,
} from '@/shared/models/iAM/criteria';
import { hasValue } from '@/shared/utils/has-value-util';
import { getNewGuid } from './uuid-utils';
import { transformWithEsbuild } from 'vite';

const operators: string[] = ['<=', '>=', '<>', '=', '<', '>'];

const invalidCharRegex: RegExp = /\s|\(|\)/g;

export const queryBuilderTypes: any = {
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
        let expression = '';
        if(criteriaRule.isEquation){
            expression = `${criteriaRule.selectedOperand}${criteriaRule.selectedOperator}`
        }
        else
        {
            expression = `[${criteriaRule.selectedOperand}]${criteriaRule.selectedOperator}`
        }
        if (criteriaRule.value[0] != '[') {
            return expression + `'${criteriaRule.value}'`;
        } else {
            return expression + `${criteriaRule.value}`;
        }
    } else {
        return '';
    }
}

function getCharIndex(
    expression: string,
    currentCharIndex: number,
    numberToAddToCurrentCharIndex: number = 0,
): number {
    return currentCharIndex + numberToAddToCurrentCharIndex < expression.length
        ? currentCharIndex + numberToAddToCurrentCharIndex
        : expression.length - 1;
}

/**
 * Creates a CriteriaRule object from a given criteria expression
 */
function createCriteriaRuleObject(clause: string): CriteriaRule {
    let operator: string = '';
    let isEquation = true;
    clause = clause.replace(/\s/g, "");
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
    let foo = operandAndValue[0];
    foo = foo.trim()
    operandAndValue[0] = operandAndValue[0]
        .replace(/\[/g, '')
        .replace(/]/g, '');
    if(foo.startsWith('[') && foo.endsWith(']') && foo.length - operandAndValue[0].length == 2)
        isEquation = false
    else
        operandAndValue[0] = foo;

    operandAndValue[1] = operandAndValue[1]
        .replace(/'/g, '')
        .replace(/\|/g, '');

    return {
        rule: operandAndValue[0],
        selectedOperator: operator,
        selectedOperand: operandAndValue[0],
        value: operandAndValue[1],
        isEquation: isEquation
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
    
    let loopPasses: number = 0;
    let quotesClosed: boolean = true; // This check is to allow values with spaces in it to pass through the while loop
    let isInEquation: boolean = false;
    while (currentCharIndex < expression.length) {
        loopPasses++;
        if (loopPasses > expression.length) {
            break;
            //throw new Error('The criteria expression is invalid.');
        }
        if(expression[currentCharIndex] == "'"){
            quotesClosed = !quotesClosed;
        }
        if (expression[currentCharIndex] === '(' && !isInEquation) {
            let subCharIndex: number = currentCharIndex;
            if(!isInEquation)
            {
                let parenthesesCounter = 0;
                let operatorFound: boolean = false;
                do {
                    if (expression[subCharIndex] === '(') {
                        parenthesesCounter++;
                    }
    
                    if (expression[subCharIndex] === ')') {
                        parenthesesCounter--;
                    }

                    if(operators.includes(expression[subCharIndex]))
                        operatorFound = true;
                    subCharIndex++;
                } while (
                    parenthesesCounter != 0 &&
                    subCharIndex < expression.length
                );
                if(!operatorFound)
                    isInEquation = true;
                if(!isInEquation)
                {
                    if (hasValue(currentClause)) {
                        criteria.children!.push(
                            createCriteriaTypeObject(
                                queryBuilderTypes.QueryBuilderRule,
                                createCriteriaRuleObject(currentClause),
                            ),
                        );
                        currentClause = '';
                    }
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
                    currentCharIndex = subCharIndex;
                }
                
            }
                
        } else if (expression[currentCharIndex] === ' ' && quotesClosed) {
            if (
                expression.substring(
                    currentCharIndex,
                    getCharIndex(expression, currentCharIndex, 5),
                ) === ' AND ' ||
                expression.substring(
                    currentCharIndex,
                    getCharIndex(expression, currentCharIndex, 4),
                ) === ' OR '
            ) {
                if (hasValue(currentClause)) {
                    criteria.children!.push(
                        createCriteriaTypeObject(
                            queryBuilderTypes.QueryBuilderRule,
                            createCriteriaRuleObject(currentClause),
                        ),
                    );
                    currentClause = '';
                    isInEquation = false;
                }

                if (
                    expression.substring(
                        currentCharIndex,
                        getCharIndex(expression, currentCharIndex, 5),
                    ) === ' AND '
                ) {
                    if (!hasValue(criteria.logicalOperator)) {
                        criteria.logicalOperator = 'AND';
                    }
                    currentCharIndex = getCharIndex(
                        expression,
                        currentCharIndex,
                        5,
                    );
                } else {
                    if (!hasValue(criteria.logicalOperator)) {
                        criteria.logicalOperator = 'OR';
                    }
                    currentCharIndex = getCharIndex(
                        expression,
                        currentCharIndex,
                        4,
                    );
                }
            }
            else
                currentCharIndex++         
        } else {
            if(quotesClosed){
                if(!invalidCharRegex.test(expression[currentCharIndex])){
                    currentClause = `${currentClause}${expression[currentCharIndex]}`;
                }
            }
            else
            {
                currentClause = `${currentClause}${expression[currentCharIndex]}`;
            }

            currentCharIndex++;

            if (
                hasValue(currentClause) &&
                currentCharIndex === expression.length
            ) {
                if(hasValue(currentClause)){
                    criteria.children!.push(
                        createCriteriaTypeObject(
                            queryBuilderTypes.QueryBuilderRule,
                            createCriteriaRuleObject(currentClause),
                        ),
                    );
                }
                currentClause = '';
                isInEquation = false;
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
        id: getNewGuid(),
        type: type,
        query: query,
    } as CriteriaType;
}


function hasAttributeToTest(clause: string, currentCharIndex: number): boolean{
    
    if(clause.startsWith('[') && clause.indexOf(']') == -1){
        return false;
    }
    else {
        var hasOp = false;
        operators.forEach(op => {
            if(clause.includes(op)){
                hasOp = true;
            }
        });
        if(!hasOp && !clause.endsWith(' ')){
            return false;
        }
    }

    return true;
}
/**
 * Converts a criteria expression into its Criteria object equivalent
 */
export const convertCriteriaExpressionToCriteriaObject = (
    expression: string,
    addErrorNotificationActionCallback: any,
) => {
    try {
        if (hasValue(expression)) {
            expression = expression.replace(/  +/g, ' ');
            return createCriteriaObject(
                expression.trim(),
                clone(emptyCriteria),
            );
        }
    } catch (e) {
        addErrorNotificationActionCallback({ message: e.message });
        return null;
    }

    return { ...clone(emptyCriteria) };
};
