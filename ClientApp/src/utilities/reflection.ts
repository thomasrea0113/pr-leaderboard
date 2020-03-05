// eslint-disable-next-line @typescript-eslint/no-unused-vars
export const nameof = <T>(key: keyof T, _instance?: T) => key.toString();
export const valueof = <T>(instance: T, prop: keyof T) => instance[prop];

export type InputValueType = string | number | string[];

/**
 * defining a custom type gaurd to check that the object is a string, number, or string[]
 * @param obj the object to check
 */
export const isInputValueType = (obj: unknown): obj is InputValueType => {
    if (typeof obj === 'string') return true;
    if (typeof obj === 'number') return true;

    const objArray = obj as string[];
    if (
        objArray?.length !== undefined &&
        objArray.length !== 0 &&
        typeof objArray[0] === 'string'
    )
        return true;
    return false;
};
