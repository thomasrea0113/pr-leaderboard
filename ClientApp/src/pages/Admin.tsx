import React, { useEffect, useState } from 'react';
import 'react-dom';
import {
    BrowserRouter as Router,
    Route,
    Switch,
    NavLink,
} from 'react-router-dom';

import { User } from '../types/dotnet-types';
import { AppContext, AppState, DefaultAppState } from '../AppContext';
import { ApproveScoreComponent } from '../Components/Admin/ApproveScore';

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

    return (
        <AppContext.Provider value={appState}>
            <div className="pt-3">
                {/* The app is deployed to and Admin area of the MVC app, so all
            routes should be under the Admin area */}
                <Router basename="/Admin">
                    <ul className="nav nav-tabs">
                        <li className="nav-item">
                            <NavLink
                                className="nav-link"
                                activeClassName="active"
                                to="/ApproveScore"
                            >
                                ApproveScore
                            </NavLink>
                        </li>
                    </ul>
                    <Switch>
                        <Route
                            path="/ApproveScore"
                            exact
                            component={ApproveScoreComponent}
                        />
                    </Switch>
                </Router>
            </div>
        </AppContext.Provider>
    );
};

export default AdminComponent;
