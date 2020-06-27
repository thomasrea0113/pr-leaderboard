import React, { useState, useEffect } from 'react';
import 'react-dom';

import {
    BrowserRouter as Router,
    Route,
    Switch,
    Redirect,
} from 'react-router-dom';

import { useLoading } from '../hooks/useLoading';
import { ensureDelay } from '../utilities/ensureDelay';
import { HomeContext, ServerData } from '../Components/Home/HomeContext';
import { HomeJumbotronComponent } from '../Components/Home/Jumbotron';
import { HomeUpdatesComponent } from '../Components/Home/HomeUpdatesComponent';

interface ReactProps {
    initialUrl: string;
    backgroundImages: string[];
}

const HomeComponent: React.FC<ReactProps> = ({
    initialUrl,
    backgroundImages,
}) => {
    const { isLoading, isLoaded, response, loadAsync } = useLoading<
        ServerData
    >();

    const [delayed, setDelayed] = useState(false);

    const isServerReady = isLoaded && !isLoading;

    // We want to slow down the load for visual affect. If the load takes less than 1.5
    // seconds, we pause
    const refreshData = () =>
        ensureDelay(1500, loadAsync({ actionUrl: initialUrl })).then(() =>
            setDelayed(true)
        );

    // load on start
    useEffect(() => {
        refreshData();
    }, []);

    return (
        <HomeContext.Provider
            value={{
                isReady: isServerReady && delayed,
                data: response?.data,
                refreshData,
                backgroundImages,
            }}
        >
            <div className="navbar-height" />
            <Router>
                <Switch>
                    <Route path="/Recent" component={HomeUpdatesComponent} />
                    <Route path="/" exact component={HomeJumbotronComponent} />
                    <Route render={() => <Redirect to="/Error/404" />} />
                </Switch>
            </Router>
        </HomeContext.Provider>
    );
};

export default HomeComponent;
