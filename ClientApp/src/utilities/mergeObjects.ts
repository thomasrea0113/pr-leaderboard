import reduce from 'lodash/fp/reduce';
import mergeWith from 'lodash/fp/mergeWith';
import { isArray } from 'util';

export const mergeIteree = (objVal: unknown, srcVal: unknown) =>
    isArray(objVal) && isArray(srcVal) ? [...objVal, ...srcVal] : undefined;

export const mergeReducer = <T>(merged: T, src: Partial<T>) =>
    mergeWith(mergeIteree, merged, src);

export const mergeObjects = <T>(destination: T, ...sources: Partial<T>[]): T =>
    reduce(mergeReducer, destination, sources);
