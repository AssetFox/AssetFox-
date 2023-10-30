parser grammar CalculateEvaluateParser
   ;

options
{
   tokenVocab = CalculateEvaluateLexer;
}

root
   : calculation EOF # calculationRoot
   | evaluation EOF  # evaluationRoot
   ;

calculation
   : IDENTIFIER LEFT_PAREN arguments RIGHT_PAREN                              # invocation
   | MINUS calculation                                                        # negation
   | left = calculation operation = (TIMES | DIVIDED_BY) right = calculation  # multiplicationOrDivision
   | left = calculation operation = (PLUS | MINUS) right = calculation        # additionOrSubtraction
   | NUMBER                                                                   # numberLiteral
   | IDENTIFIER                                                               # numberReference
   | LEFT_BRACKET IDENTIFIER RIGHT_BRACKET                                    # numberParameterReference
   | LEFT_PAREN calculation RIGHT_PAREN                                       # calculationGrouping
   ;

arguments
   : calculation (COMMA calculation)*
   ;

evaluation
   : left = comparisonOperand operation = EQUAL right = comparisonOperand                  # equal
   | left = comparisonOperand operation = NOT_EQUAL right = comparisonOperand              # notEqual
   | left = comparisonOperand operation = LESS_THAN right = comparisonOperand              # lessThan
   | left = comparisonOperand operation = LESS_THAN_OR_EQUAL right = comparisonOperand     # lessThanOrEqual
   | left = comparisonOperand operation = GREATER_THAN_OR_EQUAL right = comparisonOperand  # greaterThanOrEqual
   | left = comparisonOperand operation = GREATER_THAN right = comparisonOperand           # greaterThan
   | left = evaluation AND right = evaluation                                              # logicalConjunction
   | left = evaluation OR right = evaluation                                               # logicalDisjunction
   | LEFT_PAREN evaluation RIGHT_PAREN                                                     # evaluationGrouping
   ;

comparisonOperand
   : parameterReference
   | calculation
   | literal
   ;

parameterReference
   : IDENTIFIER
   | LEFT_BRACKET IDENTIFIER RIGHT_BRACKET
   | LEFT_PAREN parameterReference RIGHT_PAREN
   ;

literal
   : LITERAL_OPENING_DELIMITER_1 content = LITERAL_CONTENT_1? LITERAL_CLOSING_DELIMITER_1
   | LITERAL_OPENING_DELIMITER_2 content = LITERAL_CONTENT_2? LITERAL_CLOSING_DELIMITER_2
   ;
