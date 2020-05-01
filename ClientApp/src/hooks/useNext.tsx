import { useRef, useEffect } from 'react';

// useState and useReducer update asyncronously, so when we perform an update,
// we can't always gaurantee that the update has completed. useNext allows us
// to wait for the state object to change, indicating that the updates have compeleted.
export const useNext = <T extends {}>(value: T) => {
    const valueRef = useRef(value);
    const resolvesRef = useRef<((value?: T) => void)[]>([]);
    useEffect(() => {
        if (valueRef.current !== value) {
            resolvesRef.current.forEach(resolve => resolve(value));
            resolvesRef.current = [];
            valueRef.current = value;
        }
    }, [value]);
    return () =>
        new Promise(resolve => {
            resolvesRef.current.push(resolve);
        });
};
