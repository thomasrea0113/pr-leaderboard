import React, { useEffect, useState, useContext } from 'react';

export interface WithLoadingProps<D> {
    initialUrl: string;
    useLoadingContext?: () => ContextProps<D>;
}

interface State<D> {
    isLoading: boolean;
    data?: D;
}

interface ContextProps<D> extends State<D> {
    reloadAsync: () => Promise<void>;
}

/**
 * adds initial data loading to the passed component. D is the type of the server
 * return data. P is the components properties
 * @param Component The child component to render
 */
export const withLoading = <D extends {}>(
    Component: React.FC<WithLoadingProps<D>>
): React.FC<WithLoadingProps<D>> => props => {
    const { initialUrl } = props;

    const [state, setState] = useState<State<D>>({
        isLoading: true,
    });

    const reloadAsync = async () => {
        const resp = await fetch(initialUrl);
        await resp.json().then(json => {
            setState(oldState => {
                return {
                    ...oldState,
                    data: json,
                    isLoading: false,
                };
            });
        });
    };

    // load initial data
    useEffect(() => {
        reloadAsync();
    }, []);

    const LoadingContext = React.createContext<ContextProps<D>>({
        isLoading: false,
        reloadAsync,
    });

    return (
        <LoadingContext.Provider
            value={{
                ...state,
                reloadAsync,
            }}
        >
            <Component
                {...{
                    ...props,
                    useLoadingContext: () => useContext(LoadingContext),
                }}
            />
        </LoadingContext.Provider>
    );
};
