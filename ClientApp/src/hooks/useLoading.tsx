import { useEffect, useState, useRef } from 'react';

export interface LoadingState {
    isLoading: boolean;
}

export interface UseLoadingProps<D> {
    isLoading: boolean;
    reloadAsync: () => Promise<D>;
}

export const useLoading = <D extends {}>(
    initialUrl: string
): UseLoadingProps<D> => {
    const [isLoading, setLoading] = useState(false);
    const mount = useRef(false);

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
    useEffect(() => {
        mount.current = true;
        return function unMount() {
            mount.current = false;
        };
    }, []);

    return { isLoading, reloadAsync };
};
