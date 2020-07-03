import React, { useEffect, useState } from 'react';
import 'react-dom';

import { Score } from '../../types/dotnet-types';
import { isScore } from '../../types/guards/isScore';
import { isArrayOf } from '../../types/guards/higherOrderGuards';
import { useStatelessLoading } from '../../hooks/useStatelessLoading';

interface Props {
    initialUrl: string;
    top: number;
    refreshEvery: number;
}

interface State {
    /**
     * indiciates that the component has performed the initial load. Will be set after the first call to laodAsync
     */
    isLoaded: boolean;
    isLoading: boolean;
    data: Score[];
    lastQuery: string;
}

const guard = isArrayOf(isScore);

const RecentComponent: React.FC<Props> = ({
    initialUrl,
    top,
    refreshEvery,
}) => {
    const { loadAsync } = useStatelessLoading({
        guard,
    });

    const [{ data, lastQuery, isLoaded, isLoading }, setState] = useState<
        State
    >({
        data: [],
        lastQuery: '',
        isLoaded: false,
        isLoading: true,
    });

    const updateState = (newState: Partial<State>) =>
        setState(
            ({
                data: oldData,
                lastQuery: oldLastQuery,
                isLoaded: oldIsLoaded,
                isLoading: oldIsLoading,
            }) => {
                const {
                    data: newData,
                    lastQuery: newLastQuery,
                    isLoaded: newIsLoaded,
                    isLoading: newIsLoading,
                } = newState;

                const mergeData =
                    newData != null
                        ? [...newData, ...oldData].slice(0, top - 1)
                        : oldData;

                const mergedLastQuery = newLastQuery ?? oldLastQuery;
                const mergedIsLoaded = newIsLoaded ?? oldIsLoaded;
                const mergedIsLoading = newIsLoading ?? oldIsLoading;

                return {
                    data: mergeData,
                    lastQuery: mergedLastQuery,
                    isLoaded: mergedIsLoaded,
                    isLoading: mergedIsLoading,
                };
            }
        );

    const reloadAsync = () => {
        const now = new Date().toJSON();
        const promise = Promise.resolve(updateState({ isLoading: true }))
            .then(() =>
                loadAsync({
                    actionUrl: `${initialUrl}&ApprovedSince=${lastQuery}`,
                })
            )
            .then(response => {
                const newData =
                    response?.data != null && guard(response.data)
                        ? response.data
                        : [];

                const state: Partial<State> = {
                    lastQuery: now,
                    isLoaded: true,
                    isLoading: false,
                };

                if (newData.length > 0)
                    updateState({ data: newData, ...state });
                else updateState(state);
            });
        return promise;
    };

    // load data on component mount. Will still trigger an update to lastQuery
    useEffect(() => {
        reloadAsync();
    }, []);

    // because the above call triggers a change to lastQuery, we don't want to perform any updates
    // if we haven't completed the intial load. Once the initial load completes, lastQuery will
    // be updated, and the timeout will be set.
    useEffect(() => {
        if (isLoaded) {
            const timeout = setTimeout(reloadAsync, refreshEvery);
            return () => clearTimeout(timeout);
        }
        return undefined;
    }, [lastQuery]);

    return (
        <div>
            Recent Updates
            {isLoaded && !isLoading ? <>Loaded</> : null}
        </div>
    );
};

export default RecentComponent;
