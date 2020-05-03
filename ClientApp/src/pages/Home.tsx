import React, { useState, useEffect, useMemo } from 'react';
import 'react-dom';
import uniqueId from 'lodash/fp/uniqueId';
import { useLoading } from '../hooks/useLoading';
import { Featured } from '../types/dotnet-types';
import { FeaturedCard } from '../Components/FeaturedCard';
import { ensureDelay } from '../utilities/ensureDelay';
import { animationDelay } from '../utilities/animation';
import { isValidationErrorResponseData } from '../types/ValidationErrorResponse';
import { FontawesomeIconToIcon } from '../Components/StyleComponents';

interface ReactProps {
    initialUrl: string;
    backgroundImages: string[];
}

interface ServerData {
    featured: Featured[];
}

interface LocalState {
    delayed: boolean;
}

const HomeComponent: React.FC<ReactProps> = ({
    initialUrl,
    backgroundImages,
}) => {
    const backgroundImage = useMemo(
        () =>
            [
                ...backgroundImages.map(bg => `url(${bg})`),
                'radial-gradient(ellipse at bottom, #1b2735 0%, #090a0f 100%)',
            ].join(','),
        []
    );

    const { isLoading, response, loadAsync } = useLoading<ServerData>();

    const [{ delayed }, setState] = useState<LocalState>({
        delayed: false,
    });

    const mergeState = (newState: Partial<LocalState>) =>
        setState(os => ({ ...os, ...newState }));

    // We want to slow down the load for visual affect. If the load takes less than 1.5
    // seconds, we pause
    const reload = () =>
        ensureDelay(1500, loadAsync({ actionUrl: initialUrl })).then(() =>
            mergeState({ delayed: true })
        );

    // we want to load some initial data, and don't want to render anything until after
    // loading has begun
    const isLoaded = React.useRef(false);
    useEffect(() => {
        reload();
        isLoaded.current = true;
        return function dismount() {
            isLoaded.current = false;
        };
    }, []);
    if (!isLoaded.current) return null;

    const featured = !isValidationErrorResponseData(response?.data)
        ? response?.data?.featured ?? []
        : [];

    return (
        <div
            className="has-overlay center-background navbar-fixed-background-offset animate-parallax"
            style={{
                backgroundImage,
            }}
        >
            <div className="stars-sm" />
            <div className="stars-md" />
            <div className="stars-lg" />
            <div className="navbar-fixed-offset vh-background image-overlay">
                {/* TODO animate welcome text shift after featured load */}
                <div className="header">
                    <div className="title-text animate-fadeinup">
                        PR Leaderboard
                    </div>
                    {isLoading || !delayed ? null : (
                        <div className="animate-fadein">
                            <div className="row" style={{ margin: '0 1rem' }}>
                                <a
                                    href="/"
                                    className="btn btn-col btn-outline-primary"
                                >
                                    {FontawesomeIconToIcon('Go')}
                                    &nbsp;&nbsp;Recent PRs
                                </a>
                                <a
                                    href="/"
                                    className="btn btn-col btn-outline-warning"
                                >
                                    {FontawesomeIconToIcon('Go')}
                                    &nbsp;&nbsp;Browse Boards
                                </a>
                                <a
                                    href="/"
                                    className="btn btn-col btn-outline-danger"
                                >
                                    {FontawesomeIconToIcon('Go')}
                                    &nbsp;&nbsp;About
                                </a>
                            </div>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default HomeComponent;
