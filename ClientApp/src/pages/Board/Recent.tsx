import React, { useEffect } from 'react';
import 'react-dom';

import { useLoading } from '../../hooks/useLoading';
import { Score } from '../../types/dotnet-types';
import { isScore } from '../../types/guards/isScore';
import { isIndexOf, isArrayOf } from '../../types/guards/higherOrderGuards';

interface Props {
    initialUrl: string;
}

type ServerData = Score[];

const RecentComponent: React.FC<Props> = ({ initialUrl }) => {
    const { loadAsync, isLoaded, isLoading, response } = useLoading({
        guard: isIndexOf(isArrayOf(isScore)),
    });

    const reloadAsync = () =>
        loadAsync({
            actionUrl: initialUrl,
        });

    useEffect(() => {
        reloadAsync();
    }, []);

    return (
        <div>
            Recent Updates
            {isLoaded && !isLoading ? <>Loaded</> : null}
        </div>
    );
};

export default RecentComponent;
