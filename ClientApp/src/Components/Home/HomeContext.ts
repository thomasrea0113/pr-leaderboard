import React from 'react';
import { Featured } from '../../types/dotnet-types';
import { ValidationErrorResponseData } from '../../types/ValidationErrorResponse';

export interface ServerData {
    featured: Featured[];
}

export interface HomeContextState {
    // will be set to true after the initial state is loaded
    isReady: boolean;
    data?: ServerData | ValidationErrorResponseData<ServerData>;
    refreshData?: () => Promise<void>;
    backgroundImages: string[];
}

export const defaultHomeState: HomeContextState = {
    isReady: false,
    backgroundImages: [],
};

export const HomeContext = React.createContext(defaultHomeState);
