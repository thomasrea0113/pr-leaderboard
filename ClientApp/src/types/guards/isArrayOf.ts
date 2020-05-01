import { isArray } from 'util';

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
    if (isArray(obj)) {
        // eslint-disable-next-line no-plusplus
        for (let i = 0; i < obj.length; i++)
            if (!itemGuard(obj[i])) return false;
        return true;
    }

    return false;
};
