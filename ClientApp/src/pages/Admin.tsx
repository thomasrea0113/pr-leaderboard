import React, { useEffect, useState } from 'react';
import 'react-dom';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';

import { User } from '../types/dotnet-types';
import { AppContext, AppState, DefaultAppState } from '../AppContext';
import {
    BoardsNav,
    BoardsAdminComponent,
} from '../Components/Admin/Boards/Boards';
import {
    ScoresNav,
    ScoresAdminComponent,
} from '../Components/Admin/Scores/Scores';
import { UsersAdminComponent, UsersNav } from '../Components/Admin/Users/Users';

const AdminComponent: React.FC<{}> = () => {
    const [appState, setAppState] = useState<AppState>(DefaultAppState);

    useEffect(() => {
        fetch('/api/Users/Me')
            .then(resp => resp.json())
            .then((user: User) => setAppState(os => ({ ...os, user })));
    }, []);

    if (appState.user == null) {
        return <>Hang tight. Logging in....</>;
    }

    if (!appState.user.isAdmin) {
        return <>Sorry, you must be an adminstrator to access this page.</>;
    }

    const scoresPath = '/Scores';
    const boardsPath = '/Boards';
    const usersPath = '/Users';

    return (
        <AppContext.Provider value={appState}>
            <div className="pt-3">
                Welcome to the admin center! Below you&apos;ll find some useful
                admin functions
                {/* The app is deployed to and Admin area of the MVC app, so all
            routes should be under the Admin area */}
                <Router basename="/Admin">
                    <ul className="nav nav-tabs">
                        <ScoresNav path={scoresPath} />
                        <BoardsNav path={boardsPath} />
                        <UsersNav path={usersPath} />
                    </ul>
                    <Switch>
                        <Route
                            path={scoresPath}
                            component={ScoresAdminComponent}
                        />
                        <Route
                            path={boardsPath}
                            component={BoardsAdminComponent}
                        />
                        <Route
                            path={usersPath}
                            component={UsersAdminComponent}
                        />
                    </Switch>
                </Router>
            </div>
        </AppContext.Provider>
    );
};

export default AdminComponent;
