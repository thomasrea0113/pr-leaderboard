import React, { useMemo } from 'react';
import 'react-dom';

import { Link } from 'react-router-dom';
import { FontawesomeIconToIcon } from '../Components/StyleComponents';

interface ReactProps {
    initialUrl: string;
    backgroundImages: string[];
}

const HomeComponent: React.FC<ReactProps> = ({ backgroundImages }) => {
    const backgroundImage = useMemo(
        () =>
            [
                ...backgroundImages.map(bg => `url(${bg})`),
                'linear-gradient(180deg, rgba(9,10,15,1) 5%, rgba(27,39,53,1) 93%)',
            ].join(','),
        []
    );

    return (
        <>
            <div className="navbar-height" />
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
                        <div className="animate-fadein">
                            <div className="row" style={{ margin: '0 1rem' }}>
                                <a
                                    href="/Recent"
                                    className="btn btn-col btn-outline-primary spaced-text"
                                >
                                    {FontawesomeIconToIcon('Go')}
                                    &nbsp;&nbsp;Recent PRs
                                </a>
                                <a
                                    href="/"
                                    className="btn btn-col btn-outline-warning spaced-text"
                                >
                                    {FontawesomeIconToIcon('Go')}
                                    &nbsp;&nbsp;Browse Boards
                                </a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
};

export default HomeComponent;
