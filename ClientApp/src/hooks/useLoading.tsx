import { useState, useRef, useEffect } from 'react';

export interface LoadingState<D> {
    isLoading: boolean;
    isLoaded: boolean;
    data?: D;
}

export interface UseLoadingProps<D> extends LoadingState<D> {
    reloadAsync: () => Promise<void>;
}

export const useLoading = <D extends {}>(
    initialUrl: string,
    loadOnMount: boolean = false
): UseLoadingProps<D> => {
    const [loadingState, setState] = useState<LoadingState<D>>({
        isLoaded: false,
        isLoading: false,
    });
    const { isLoading } = loadingState;

    const mergeState = (newState: Partial<LoadingState<D>>) =>
        setState(os => ({ ...os, ...newState }));

    const isLoaded = useRef(false);

    // if the component is not currently loading, begin loading. The wrapped
    // promise will reject immediately if we are already loading. Otherwise,
    // it completes when the inner promise completes
    const reloadAsync = (): Promise<void> =>
        new Promise<void>((resolve, reject) => {
            if (!isLoading) {
                mergeState({ isLoading: true });
                fetch(initialUrl)
                    .then(resp => resp.json())
                    .then(j => j as D) // assume the returned json matches the typescript interface
                    .then(data => mergeState({ isLoading: false, data }))
                    .then(
                        fullfilled => {
                            isLoaded.current = true;
                            resolve(fullfilled);
                        },
                        error => reject(error)
                    );
            } else reject(new Error('This component is already loading'));
        });

    useEffect(() => {
        if (loadOnMount) reloadAsync();
        return function dismount() {
            isLoaded.current = false;
        };
    }, []);

    return { ...loadingState, isLoaded: isLoaded.current, reloadAsync };
};
