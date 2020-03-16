import React, { useState, useEffect } from 'react';
import 'react-dom';
import uniqueId from 'lodash/fp/uniqueId';
import { useLoading } from '../hooks/useLoading';
import { Featured } from '../types/dotnet-types';
import { FeaturedCard } from '../Components/FeaturedCard';

interface ReactProps {
    initialUrl: string;
}

interface ServerData {
    featured: Featured[];
}

interface LocalState {
    delayed: boolean;
}

const HomeComponent: React.FC<ReactProps> = ({ initialUrl }) => {
    const { isLoading, data, reloadAsync } = useLoading<ServerData>(initialUrl);

    const [{ delayed }, setState] = useState<LocalState>({
        delayed: false,
    });

    const mergeState = (newState: Partial<LocalState>) =>
        setState(os => ({ ...os, ...newState }));

    // We want to slow down the load for visual affect. If the load takes less than 3
    // seconds, we pause
    const reload = () => {
        Promise.all([
            new Promise<void>(resolve => setTimeout(resolve, 3000)),
            reloadAsync(),
        ]).then(() => mergeState({ delayed: true }));
    };

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

    const featured = data?.featured ?? [];

    return (
        <>
            {/* TODO animate welcome text shift after featured load */}
            {isLoading || !delayed ? (
                <h1 className="animate-fadeinup">Welcome...</h1>
            ) : (
                <div className="animate-fadeinup">
                    <h1>check out what we&apos;ve got.</h1>
                    <div className="card-deck ml-1 mr-1 mr-lg-5 ml-lg-5 hide-sm">
                        {featured.map((f, i) => (
                            <FeaturedCard
                                key={uniqueId('featured')}
                                {...f}
                                divProps={{
                                    className: 'animate-fadeinleft-500ms',
                                    style: {
                                        animationDelay: `${(featured.length -
                                            i) *
                                            175}ms`,
                                    },
                                }}
                            />
                        ))}
                    </div>
                </div>
            )}
        </>
    );
};

export default HomeComponent;
