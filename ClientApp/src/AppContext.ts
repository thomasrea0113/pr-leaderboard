import React from 'react';
import { User } from './types/dotnet-types';

export interface AppState {
    user?: User;
}

// This is empty for now, but we're gonna go ahead and wire-in a default state
export const DefaultAppState: AppState = {};

export const AppContext = React.createContext<AppState>(DefaultAppState);
