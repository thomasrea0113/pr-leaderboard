import React, { useState, useEffect } from 'react';
import 'react-dom';

// import flow from 'lodash/fp/flow';
// import split from 'lodash/fp/split';
// import drop from 'lodash/fp/drop';
// import join from 'lodash/fp/join';

import { useLoading } from '../hooks/useLoading';
import { ensureDelay } from '../utilities/ensureDelay';
import { HomeContext, ServerData } from '../Components/Home/HomeContext';
import { HomeJumbotronComponent } from '../Components/Home/Jumbotron';
import { HomeAboutComponent } from '../Components/Home/About';

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

    // const location = flow(
    //     split('/'),
    //     drop(1),
    //     join('.')
    // )(window.location.pathname);

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
            <HomeJumbotronComponent />
            <HomeAboutComponent />
        </HomeContext.Provider>
    );
};

export default HomeComponent;
