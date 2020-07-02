import React, { useEffect, useMemo } from 'react';
import 'react-dom';

import { useLoading } from '../../hooks/useLoading';
import { Score } from '../../types/dotnet-types';
import { isScore } from '../../types/guards/isScore';
import { isIndexOf, isArrayOf } from '../../types/guards/higherOrderGuards';

interface Props {
    initialUrl: string;
}

type ServerData = Score[];

const guard = isArrayOf(isScore);

const RecentComponent: React.FC<Props> = ({ initialUrl }) => {
    const { loadAsync, isLoaded, isLoading, response } = useLoading({
        guard,
    });

    const reloadAsync = () =>
        loadAsync({
            actionUrl: initialUrl,
        });

    useEffect(() => {
        reloadAsync();
        const interval = setInterval(() => reloadAsync(), 5000);
        return () => clearInterval(interval);
    }, []);

    const data = (guard(response?.data) ? response?.data : []) ?? [];

    return (
        <div>
            Recent Updates
            {isLoaded && !isLoading ? <>Loaded</> : null}
        </div>
    );
};

export default RecentComponent;
