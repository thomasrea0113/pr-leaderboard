import React from 'react';
import { nameof, isInputValueType, InputValueType } from './reflection';

export const HiddenFor = <T extends {}, K extends keyof T>(
    key: K,
    obj?: T,
    name?: string
) => {
    const val = obj != null ? obj[key] : undefined;
    if (val !== undefined) {
        const value = isInputValueType(val)
            ? (val as InputValueType)
            : undefined;
        return (
            <input type="hidden" name={name ?? nameof<T>(key)} value={value} />
        );
    }
    return null;
};
