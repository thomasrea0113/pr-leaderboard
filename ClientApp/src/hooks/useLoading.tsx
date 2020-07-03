import { useState } from 'react';
import { attachRenderEvents } from '../utilities/attachRenderEvents';
import {
    TypedResponse,
    LoadProps,
    UseLoadingProps,
    useStatelessLoading,
} from './useStatelessLoading';

/**
 * props for internal loading state
 */
export interface UseLoadingState<D> {
    isLoading: boolean;
    isLoaded: boolean;
    response?: TypedResponse<D>;
}

/**
 * UseLoading instance returned by the useLoading hook. Note, it exposes
 * the internal loading state as well
 */
export interface UseLoading<D> extends UseLoadingState<D> {
    loadAsync: (props: LoadProps) => Promise<void>;
}

/**
 * a hook allowing for common loading functionality
 */
export const useLoading = <D extends {}>(
    props?: UseLoadingProps<D>
): UseLoading<D> => {
    const { loadAsync: statelessLoadAsync } = useStatelessLoading(props);

    const [loadingState, setState] = useState<UseLoadingState<D>>({
        isLoaded: false,
        isLoading: false,
    });
    const { isLoading } = loadingState;

    const mergeState = (newState: Partial<UseLoadingState<D>>) =>
        setState(os => ({ ...os, ...newState }));

    // if the component is not currently loading, begin loading. The wrapped
    // promise will reject immediately if we are already loading. Otherwise,
    // it completes when the inner promise completes
    const loadAsync = (loadProps: LoadProps): Promise<void> => {
        return new Promise<void>((resolve, reject) => {
            if (!isLoading) {
                mergeState({ isLoading: true, isLoaded: false });
                statelessLoadAsync(loadProps).then(
                    response => {
                        mergeState({
                            isLoading: false,
                            isLoaded: true,
                            response,
                        });
                        attachRenderEvents();
                        resolve();
                    },
                    error => {
                        mergeState({
                            response: undefined,
                            isLoading: false,
                        });
                        return reject(error);
                    }
                );
            } else reject(new Error('This component is already loading'));
        });
    };

    return {
        ...loadingState,
        loadAsync,
    };
};
