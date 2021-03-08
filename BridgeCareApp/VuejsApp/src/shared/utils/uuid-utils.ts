import {v4, NIL} from 'uuid';

export const getBlankGuid = (): string => {
    return NIL;
};

export const getNewGuid = (): string => {
    return v4().toLowerCase();
};