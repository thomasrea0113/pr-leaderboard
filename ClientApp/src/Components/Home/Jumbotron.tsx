import React, { useMemo, useContext } from 'react';
import { Link } from 'react-router-dom';

import { FontawesomeIconToIcon } from '../StyleComponents';
import { HomeContext } from './HomeContext';

// eslint-disable-next-line @typescript-eslint/no-empty-interface
export interface HomeJumbotronProps {
    // stubbed
}

export const HomeJumbotronComponent: React.FC<HomeJumbotronProps> = () => {
    const { backgroundImages, isReady } = useContext(HomeContext);

    const backgroundImage = useMemo(
        () =>
            [
                ...backgroundImages.map(bg => `url(${bg})`),
                'linear-gradient(180deg, rgba(9,10,15,1) 5%, rgba(27,39,53,1) 93%)',
            ].join(','),
        []
    );

    return (
        <div
            className="has-overlay vh-75 center-background animate-parallax"
            style={{
                backgroundImage,
            }}
        >
            <div className="stars-sm" />
            <div className="stars-md" />
            <div className="stars-lg" />
            <div className="image-overlay">
                <div className="header">
                    <div className="header-text animate-fadeinup">
                        PR Leaderboard
                    </div>
                    {isReady ? (
                        <div className="animate-fadein">
                            <div className="row" style={{ margin: '0 1rem' }}>
                                <Link
                                    to="/Recent"
                                    className="btn btn-col btn-outline-primary spaced-text"
                                >
                                    {FontawesomeIconToIcon('Go')}
                                    &nbsp;&nbsp;Recent PRs
                                </Link>
                                <Link
                                    to="/"
                                    className="btn btn-col btn-outline-warning spaced-text"
                                >
                                    {FontawesomeIconToIcon('Go')}
                                    &nbsp;&nbsp;Browse Boards
                                </Link>
                            </div>
                        </div>
                    ) : null}
                </div>
            </div>
        </div>
    );
};
