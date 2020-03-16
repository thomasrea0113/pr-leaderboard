import { useState } from 'react';

export interface LoadingState {
    isLoading: boolean;
    isLoaded: false;
}

export interface UseLoadingProps<D> extends LoadingState {
    reloadAsync: () => Promise<D>;
}

export const useLoading = <D extends {}>(
    initialUrl: string
): UseLoadingProps<D> => {
    const [loadingState, setState] = useState<LoadingState>({
        isLoaded: false,
        isLoading: false,
    });
    const { isLoading } = loadingState;

    const setLoading = (loading: boolean) =>
        setState(os => ({ ...os, isLoading: loading }));

    // if the component is not currently loading, begin loading. The wrapped
    // promise will reject immediately if we are already loading. Otherwise,
    // it completes when the inner promise completes
    const reloadAsync = (): Promise<D> =>
        new Promise<D>((resolve, reject) => {
            if (!isLoading) {
                setLoading(true);
                fetch(initialUrl)
                    .then(resp => resp.json())
                    .then(j => j as D)
                    .then(data => {
                        setLoading(false);
                        return data;
                    })
                    .then(
                        fullfilled => resolve(fullfilled),
                        error => reject(error)
                    );
            } else reject(new Error('This component is already loading'));
        });

    // mount the component and perform the initial load
    // const mount = useRef(false);
    // useEffect(() => {
    //     mount.current = true;
    //     return function unMount() {
    //         mount.current = false;
    //     };
    // }, []);

    return { ...loadingState, reloadAsync };
};
