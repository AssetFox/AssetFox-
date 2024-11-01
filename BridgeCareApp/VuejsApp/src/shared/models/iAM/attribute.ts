import { getBlankGuid } from "@/shared/utils/uuid-utils";
import { clone } from "ramda";
import { Datasource, emptyDatasource } from "./data-source";

export interface Attribute {
    id: string
    name: string;
    type: string;
    aggregationRuleType: string;
    command: string;
    defaultValue: string;
    minimum: number | null;
    maximum: number | null;
    isCalculated: boolean;
    isAscending: boolean;
    setForAllAttributes: boolean;
    dataSource: Datasource;
}

export interface RuleDefinition {
    ruleName: string
    isText: boolean;
    isNumeric: boolean;
}

export interface NetworkAttributes {
    networkId: number;
    attributes: string[];
}

export interface AttributeSelectValues {
    attribute: string;
    values: string[];
}

export interface AttributeSelectValuesResult {
    attribute: string;
    values: string[];
    resultMessage: string;
    resultType: string;
}

export const emptyAttribute: Attribute = {
    id: getBlankGuid(),
    isAscending: false,
    isCalculated: false,
    aggregationRuleType: '',
    command: '',
    defaultValue: '',
    minimum: 0,
    maximum: 0,
    name: '',
    type: 'STRING',
    setForAllAttributes: false,
    dataSource: clone(emptyDatasource)
}
