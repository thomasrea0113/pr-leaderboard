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

// eslint-disable-next-line react/prefer-stateless-function
const HomeComponent: React.FC<ReactProps> = ({ initialUrl }) => {
    const { isLoading, reloadAsync } = useLoading<ServerData>(initialUrl);

    const [state, setState] = useState<ServerData>();

    const featured = state?.featured ?? [];

    const reload = () => {
        reloadAsync().then(d => setState(d));
    };

    useEffect(reload, []);

    return (
        <div>
            <h1>Welcome... I&apos;m bad with words.</h1>
            {isLoading ? null : (
                <div className="card-deck ml-1 mr-1 mr-lg-5 ml-lg-5">
                    {featured.map(f => (
                        <FeaturedCard {...f} />
                    ))}
                </div>
            )}
        </div>
    );
};

export default HomeComponent;
