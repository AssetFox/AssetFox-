import {prop, sort, sortBy} from 'ramda';
import { SelectItem } from '../models/vue/select-item';

const isNumber = (item: any): item is number => {
    return typeof item === 'number';
};

const isString = (item: any): item is string => {
    return typeof item === 'string';
};

const isDate = (item: any): item is Date => {
    return typeof item === 'object';
};

const numberSorter = (number1: number, number2: number) => {
    return number1 - number2;
};

const stringSorter = (string1: string, string2: string) => {
    return string1.localeCompare(string2);
};

const dateSorter = (date1: Date, date2: Date) => {
    return date1.getTime() - date2.getTime();
};

export const sorter = (items: any[]) => {
    if (items.every(isNumber)) {
        return sort(numberSorter, items);
    }
    if (items.every(isString)) {
        return sort(stringSorter, items);
    }
    if (items.every(isDate)) {
        return sort(dateSorter, items);
    }
};

export const sortByProperty = (property: string, items: any[]): any[] => {
    const propSorter = sortBy(prop(property));
    return propSorter(items);
};

export function sortSelectItemsAlphabetically(items: SelectItem[]): SelectItem[] {
    return items.sort((a, b) => a.text.localeCompare(b.text));
}
