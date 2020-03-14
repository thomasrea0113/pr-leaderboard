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
            {isLoading
                ? null
                : featured.map(f => (
                      <FeaturedCard key={uniqueId('featured')} {...f} />
                  ))}
        </div>
    );
};

export default HomeComponent;
