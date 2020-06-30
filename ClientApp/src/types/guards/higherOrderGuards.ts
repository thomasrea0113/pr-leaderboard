import { Featured } from '../types';
import { nameof } from '../../utilities/reflection';

/**
 * a higher-order function that returns a type guard capable
 * checking that the guarded object is an array the type specified
 * by the passed in type guard
 * @param itemGuard the guard for the elements in the array
 */
export const isArrayOf = <T>(
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    itemGuard: (obj: any) => obj is T
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
) => (obj: any): obj is T[] => {
    if (Array.isArray(obj)) {
        // eslint-disable-next-line no-plusplus
        for (let i = 0; i < obj.length; i++)
            if (!itemGuard(obj[i])) return false;
        return true;
    }

    return false;
};

/**
 * a higher-order function that returns a type guard capable
 * checking that the guarded object is an array the type specified
 * by the passed in type guard
 * @param itemGuard the guard for the elements in the array
 */
export const isIndexOf = <TValue>(
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    itemGuard: (obj: any) => obj is TValue
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
) => (obj: any): obj is { [key: string]: TValue } => {
    if (typeof obj !== 'object') return false;

    const entries = Object.entries(obj);
    // eslint-disable-next-line no-plusplus
    for (let i = 0; i < entries.length; i++) {
        const [key, value] = entries[i];

        if (typeof key !== 'string') return false;
        if (!itemGuard(value)) return false;
    }
    return true;
};

export const isFeatured = <T>(
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    guard: (obj: any) => obj is T
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
) => (obj: any): obj is Featured<T> =>
    guard(obj) && nameof<Featured<T>>('isFeatured') in obj;
